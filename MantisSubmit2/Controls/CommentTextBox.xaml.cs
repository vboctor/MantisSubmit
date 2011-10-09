using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MantisSubmit2.Controls
{
    public partial class CommentTextBox : UserControl
    {
        #region Private fields

        private Storyboard showCommentStoryboard;
        private Storyboard hideCommentStoryboard;

        #endregion

        #region Properties

        public bool IsOpened { get; private set; }

        public string Text
        {
            get { return CommentText.Text; }
            set
            {
                CommentText.Text = value;
                if (String.IsNullOrEmpty(value))
                    Hide();
                else
                    Show();
            }
        }

        #endregion

        #region Constructors

        public CommentTextBox()
        {
            InitializeComponent();

            CommentText.IsTabStop = false;
            this.showCommentStoryboard = (Storyboard)Resources["ShowCommentStoryboard"];
            this.hideCommentStoryboard = (Storyboard)Resources["HideCommentStoryboard"];
        }

        #endregion

        #region Event handlings

        private void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            Show();
        }

        #endregion

        #region Private methods

        private void Hide()
        {
            if (!IsOpened)
                return;

            CommentText.IsTabStop = false;
            AddCommentButton.IsEnabled = true;
            this.hideCommentStoryboard.Begin();
            IsOpened = false;
        }

        private void Show()
        {
            if (IsOpened)
                return;

            AddCommentButton.IsEnabled = false;
            CommentText.IsTabStop = true;
            CommentText.Focus();
            this.showCommentStoryboard.Begin();
            IsOpened = true;
        }

        #endregion
    }
}
