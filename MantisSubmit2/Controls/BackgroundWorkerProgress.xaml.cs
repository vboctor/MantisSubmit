using System.Windows.Controls;

namespace MantisSubmit2.Controls
{
    public partial class BackgroundWorkerProgress : UserControl
    {
        #region Properties

        public BackgroundWorkerService BackgroundWorkerService { get { return App.Current.BackgroundWorkerService; } }

        #endregion

        #region Constructors

        public BackgroundWorkerProgress()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        #endregion
    }
}
