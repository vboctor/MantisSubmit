using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace MantisSubmit2
{
    public class BackgroundWorkerService : INotifyPropertyChanged
    {
        #region Private fields

        private Queue<Task> queue;
        private Thread backgroundThread;

        #endregion

        #region Bindable properties

        private bool isBusy;
        public bool IsBusy
        {
            get { return this.isBusy; }
            set
            {
                if (this.isBusy == value)
                    return;

                this.isBusy = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsBusy"));
            }
        }

        private string currentTaskName;
        public string CurrentTaskName
        {
            get { return this.currentTaskName; }
            set
            {
                if (this.currentTaskName == value)
                    return;

                this.currentTaskName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentTaskName"));

            }
        }

        #endregion

        #region Constructors

        public BackgroundWorkerService()
        {
            this.queue = new Queue<Task>();
            IsBusy = false;
            CurrentTaskName = String.Empty;
        }

        #endregion

        #region Public methods

        public void EnqueueTask(string name, Action<object> action)
        {
            EnqueueTask(name, null, action);
        }

        public void EnqueueTask(string name, object parameter, Action<object> action)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (action == null)
                throw new ArgumentNullException("action");

            lock (this.queue)
            {
                this.queue.Enqueue(new Task() { Name = name, Action = action, Parameter = parameter });
            }
            if (!IsBusy)
            {
                this.backgroundThread = new Thread(BackgroundThreadMain);
                this.backgroundThread.Start();
                IsBusy = true;
            }
        }

        #endregion

        #region Private methods

        private void BackgroundThreadMain()
        {
            while (true)
            {
                Task task = null;
                lock (this.queue)
                {
                    if (this.queue.Count > 0)
                    {
                        task = this.queue.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }
                if (task != null)
                {
                    CurrentTaskName = task.Name;
                    task.Action(task.Parameter);
                }
                Thread.Sleep(1);
            }
            IsBusy = false;
            CurrentTaskName = String.Empty;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Task class

        private class Task
        {
            public string Name { get; set; }
            public Action<object> Action { get; set; }
            public object Parameter { get; set; }
        }

        #endregion
    }
}
