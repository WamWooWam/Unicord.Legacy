using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using System.IO.IsolatedStorage;

namespace Unicord.WP7
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LoginToken"))
            {
                App.Current.RootFrame.Navigate(new Uri("/Unicord.WP7;component/MainPanorama.xaml", UriKind.Relative));
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.SetImmediate(() => ShowLoginStoryboard.Begin());
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var token = PasswordTextBox.Password.Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(token))
            {
                MessageBox.Show("Your token cannot be empty!");
                return;
            }

            IsolatedStorageSettings.ApplicationSettings.Add("LoginToken", token);

            App.Current.Discord = new DiscordLib.DiscordClient(token);
            App.Current.RootFrame.Navigate(new Uri("/Unicord.WP7;component/MainPanorama.xaml", UriKind.Relative));
        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordWatermark.Visibility = Visibility.Collapsed;
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Password.Length == 0)
            {
                PasswordWatermark.Visibility = Visibility.Visible;
            }
        }
    }
}