using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MantisSubmit2.Properties;
using MantisSubmit2.Windows;

namespace MantisSubmit2
{
    public partial class App : Application
    {
        #region Static properties

        public static new App Current { get { return Application.Current as App; } }

        #endregion

        #region Properties

        public string ApplicationName { get { return "Mantis Submit v2.0"; } }
        public BackgroundWorkerService BackgroundWorkerService { get; private set; }
        public MantisService MantisService { get; private set; }

        #endregion

        #region Constructors

        static App()
        {
            DispatcherHelper.Initialize();
            Dispatcher.CurrentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(Dispatcher_UnhandledException);
        }

        public App()
        {
            BackgroundWorkerService = new BackgroundWorkerService();
            MantisService = new MantisService(BackgroundWorkerService);
        }

        #endregion

        #region Event handlings

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Settings settings = Settings.Default;

            MainWindow window = new MainWindow();
            window.Show();

            if (String.IsNullOrEmpty(settings.MantisConnectUrl) ||
                String.IsNullOrEmpty(settings.MantisUsername) ||
                String.IsNullOrEmpty(settings.MantisPassword))
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Owner = window;
                bool? loginDialogResult = loginWindow.ShowDialog();

                if (!loginDialogResult.Value)
                {
                    Shutdown();
                }
            }

            if (!MantisService.IsLoggedIn)
            {
                MantisService.Login(settings.MantisConnectUrl, settings.MantisUsername, settings.MantisPassword, settings.HttpAuthUsername, settings.HttpAuthPassword, null);
            }
        }

        private static void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            IEnumerable<MainWindow> mainWindows = App.Current.Windows.OfType<MainWindow>();
            if (mainWindows.Any(w => w.NeedBackup()))
            {
                foreach (MainWindow window in mainWindows)
                {
                    if (window.NeedBackup())
                    {
                        window.Backup();
                    }
                }
                MessageBox.Show("Fatal error, backup files created.\r\n" + e.Exception.ToString(), App.Current.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Fatal error, there were no need to create backup files.\r\n" + e.Exception.ToString(), App.Current.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
