using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Unicord.WP7
{
    public class UnicordPage : PhoneApplicationPage
    {
        private ProgressIndicator _connectingProgress;

        public UnicordPage()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            _connectingProgress = new ProgressIndicator();
            _connectingProgress.Text = "Connecting...";
            _connectingProgress.IsIndeterminate = true;

            SystemTray.SetProgressIndicator(this, _connectingProgress);
        }

        async void OnLoaded(object sender, RoutedEventArgs e)
        {
            string token;
            if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("LoginToken", out token))
                App.Current.RootFrame.Navigate(new Uri("/Unicord.WP7;component/LoginPage.xaml", UriKind.Relative));

            if (App.Current.Discord != null)
            {
                App.Current.EnsureDiscordClient();
            }

            if (App.Current.Discord != null)
            {
                App.Current.Discord.Connecting += OnConnecting;
                App.Current.Discord.Ready += OnReady;
                App.Current.Discord.Resumed += OnReady;

                if (App.Current.Discord.CurrentUser == null)
                {
                    await App.Current.Discord.ConnectAsync().ConfigureAwait(false);
                }
            }
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (App.Current.Discord != null)
            {
                App.Current.Discord.Connecting -= OnConnecting;
                App.Current.Discord.Ready -= OnReady;
            }
        }

        private async Task OnConnecting()
        {
            await Dispatcher.InvokeAsync(() => _connectingProgress.IsVisible = true);
        }

        private async Task OnReady()
        {
            await Dispatcher.InvokeAsync(() => _connectingProgress.IsVisible = false);
        }
    }
}
