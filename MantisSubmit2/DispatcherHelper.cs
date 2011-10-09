using System;
using System.Windows.Threading;

namespace MantisSubmit2
{
    /// <summary>
    /// Helper class for dispatcher operations on the UI thread.
    /// </summary>
    public static class DispatcherHelper
    {
        #region Public properties

        /// <summary>
        /// Gets a reference to the UI thread's dispatcher, after the
        /// <see cref="Initialize" /> method has been called on the UI thread.
        /// </summary>
        public static Dispatcher UIDispatcher { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Executes an action on the UI thread. If this method is called
        /// from the UI thread, the action is executed immendiately. If the
        /// method is called from another thread, the action will be enqueued
        /// on the UI thread's dispatcher and executed asynchronously.
        /// <para>For additional operations on the UI thread, you can get a
        /// reference to the UI thread's dispatcher thanks to the property
        /// <see cref="UIDispatcher" /></para>.
        /// </summary>
        /// <param name="action">The action that will be executed on the UI
        /// thread.</param>
        public static void CheckBeginInvokeOnUI(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (UIDispatcher == null)
                throw new InvalidOperationException("Helper is not initialized yet.");

            if (UIDispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                UIDispatcher.BeginInvoke(action);
            }
        }

        /// <summary>
        /// This method should be called once on the UI thread to ensure that
        /// the <see cref="UIDispatcher" /> property is initialized.
        /// <para>In a Silverlight application, call this method in the
        /// Application_Startup event handler, after the MainPage is constructed.</para>
        /// <para>In WPF, call this method on the static App() constructor.</para>
        /// </summary>
        public static void Initialize()
        {
            if (UIDispatcher != null)
                throw new InvalidOperationException("Helper is already initialized.");

#if SILVERLIGHT
            UIDispatcher = System.Windows.Deployment.Current.Dispatcher;
#else
            UIDispatcher = Dispatcher.CurrentDispatcher;
#endif
        }

        #endregion
    }
}
