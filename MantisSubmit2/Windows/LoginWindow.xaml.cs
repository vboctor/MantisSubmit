using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MantisSubmit2.Properties;
using System.Threading;
using MantisConnect;
using System.Net;

namespace MantisSubmit2.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public BackgroundWorkerService BackgroundWorkerService { get { return App.Current.BackgroundWorkerService; } }

        public LoginWindow()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
            Title = "Login - " + App.Current.ApplicationName;

            Loaded += new RoutedEventHandler(LoginWindow_Loaded);

            Settings settings = Settings.Default;
            MantisConnectTextBox.Text = settings.MantisConnectUrl;
            MantisUsernameTextBox.Text = settings.MantisUsername;
            MantisPasswordTextBox.Password = settings.MantisPassword;
            HttpAuthUsernameTextBox.Text = settings.HttpAuthUsername;
            HttpAuthPasswordTextBox.Password = settings.HttpAuthPassword;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartLogin();
            }
            else if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MantisConnectTextBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            StartLogin();
        }

        private void StartLogin()
        {
            /*string mantisConnect = MantisConnectTextBox.Text;
            string mantisUsername = MantisUsernameTextBox.Text;
            string mantisPassword = MantisPasswordTextBox.Password;
            string httpAuthUsername = HttpAuthUsernameTextBox.Text;
            string httpAuthPassword = HttpAuthPasswordTextBox.Password;*/

            App.Current.MantisService.Login(MantisConnectTextBox.Text,
                MantisUsernameTextBox.Text,
                MantisPasswordTextBox.Password,
                HttpAuthUsernameTextBox.Text,
                HttpAuthPasswordTextBox.Password,
                (errorMessage) =>
                {
                    if (errorMessage == null)
                    {
                        Settings settings = Settings.Default;
                        settings.MantisConnectUrl = MantisConnectTextBox.Text;
                        settings.MantisUsername = MantisUsernameTextBox.Text;
                        settings.MantisPassword = MantisPasswordTextBox.Password;
                        settings.HttpAuthUsername = HttpAuthUsernameTextBox.Text;
                        settings.HttpAuthPassword = HttpAuthPasswordTextBox.Password;
                        settings.Save();

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(this, errorMessage, App.Current.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
        }
    }
}
