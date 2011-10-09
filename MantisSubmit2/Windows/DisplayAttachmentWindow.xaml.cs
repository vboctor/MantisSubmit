using System;
using System.Windows;
using System.Windows.Input;
using MantisSubmit2.Controls;

namespace MantisSubmit2.Windows
{
    public partial class DisplayAttachmentWindow : Window
    {
        #region Constructors

        public DisplayAttachmentWindow(ImageAttachment attachment)
        {
            InitializeComponent();

            Title = String.Format("{0} - {1}", attachment.FileName, App.Current.ApplicationName);
            ImageHost.Source = attachment.Image;
            ImageWrapper.MouseDown += new MouseButtonEventHandler(ImageWrapper_MouseDown);

            if (String.IsNullOrEmpty(attachment.OriginalPath))
            {
                OriginalPathTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                OriginalPathTextBlock.Text = attachment.OriginalPath;
                OriginalPathTextBlock.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Event handlings

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                Close();
            }
            base.OnKeyDown(e);
        }

        private void ImageWrapper_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
