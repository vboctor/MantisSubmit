using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MantisSubmit2.Controls
{
    public partial class LoadingIndicator : UserControl
    {
        #region Private fields

        private Storyboard showStoryboard;
        private Storyboard hideStoryboard;
        private Storyboard loadingStoryboard;

        #endregion

        #region Dependency properties

        [Category("Common Properties")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingIndicator), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsLoadingChanged)));

        #endregion

        #region Event handling

        private static void OnIsLoadingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            LoadingIndicator loadingIndicator = (LoadingIndicator)sender;
            loadingIndicator.OnIsLoadingChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        private void OnIsLoadingChanged(bool oldValue, bool newValue)
        {
            if (newValue == true)
            {
                this.hideStoryboard.Completed -= new System.EventHandler(HideStoryboard_Completed);
                this.showStoryboard.Begin();
                this.loadingStoryboard.Begin();
            }
            else
            {
                this.hideStoryboard.Completed += new System.EventHandler(HideStoryboard_Completed);
                this.hideStoryboard.Begin();
            }
        }

        private void HideStoryboard_Completed(object sender, System.EventArgs e)
        {
            this.loadingStoryboard.Stop();
        }

        #endregion

        #region Constructors

        public LoadingIndicator()
        {
            InitializeComponent();

            LoadingIndicatorPath.Opacity = 0.0;
            this.showStoryboard = (Storyboard)Resources["ShowStoryboard"];
            this.hideStoryboard = (Storyboard)Resources["HideStoryboard"];
            this.loadingStoryboard = (Storyboard)Resources["LoadingStoryboard"];
        }

        #endregion
    }
}
