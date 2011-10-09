using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MantisConnect;
using System.Net;
using System.Windows.Forms;
using System.Windows;
using System.Threading;

namespace MantisSubmit2
{
    public class MantisService : INotifyPropertyChanged
    {
        #region Private fields

        private BackgroundWorkerService backgroundWorkerService;
        private bool dataLoaded = false;

        #endregion

        #region Public properties

        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return this.isLoggedIn; }
            private set
            {
                if (this.isLoggedIn == value)
                    return;

                this.isLoggedIn = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsLoggedIn"));
            }
        }

        private string currentUsername;
        public string CurrentUsername
        {
            get { return this.currentUsername; }
            private set
            {
                if (this.currentUsername == value)
                    return;

                this.currentUsername = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentUsername"));
            }
        }

        public Session MantisSession { get; private set; }

        public MantisData Data { get; private set; }

        #endregion

        #region Constructors

        public MantisService(BackgroundWorkerService backgroundWorkerService)
        {
            if (backgroundWorkerService == null)
                throw new ArgumentNullException("backgroundWorkerService");
            this.backgroundWorkerService = backgroundWorkerService;

            IsLoggedIn = false;
            CurrentUsername = String.Empty;
            MantisSession = null;
            Data = MantisData.LoadFromCache();
        }

        #endregion

        #region Public methods

        public void Login(string mantisConnectUrl, string mantisUsername, string mantisPassword, string httpAuthUsername, string httpAuthPassword, Action<string> onCompleted)
        {
            this.backgroundWorkerService.EnqueueTask("Opening session", (param) =>
                {
                    string errorMessage = null;
                    try
                    {
                        Session session = new Session(mantisConnectUrl, mantisUsername, mantisPassword, new NetworkCredential(httpAuthUsername, httpAuthPassword));
                        session.Connect();

                        IsLoggedIn = true;
                        CurrentUsername = mantisUsername;
                        MantisSession = session;
                    }
                    catch (WebException)
                    {
                        errorMessage = "Unable to reach the mantis connect. Check the url and the http authentication if required.";
                    }
                    catch (Exception)
                    {
                        errorMessage = "Unknown error.";
                    }

                    if (errorMessage != null || this.dataLoaded)
                    {
                        if (onCompleted != null)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() => onCompleted(errorMessage));
                        }
                    }
                    else
                    {
                        this.backgroundWorkerService.EnqueueTask("Downloading data", (param2) =>
                        {
                            MantisData data = new MantisData();
                            data.UpdateFromMantis(MantisSession);
                            data.SaveToCache();
                            Data = data;
                            this.dataLoaded = true;
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                if (MantisDataChanged != null)
                                    MantisDataChanged(this, EventArgs.Empty);
                            });
                            if (onCompleted != null)
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() => onCompleted(null));
                            }
                        });
                    }
                });
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler MantisDataChanged;

        #endregion
    }
}
