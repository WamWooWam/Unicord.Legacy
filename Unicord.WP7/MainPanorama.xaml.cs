using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
//using SysWinGestureEventArgs = System.Windows.Input.GestureEventArgs;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DiscordLib;
using Microsoft.Phone.Shell;
using Unicord.WP7.ViewModels;

namespace Unicord.WP7
{
    public partial class MainPanorama : UnicordPage
    {
        private MainViewModel _viewModel;

        public MainPanorama()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            if (_viewModel == null)
            {
                DataContext = _viewModel = new MainViewModel();
            }
        }

        private void GuildHubTile_Tap(object sender, GestureEventArgs e)
        {
            var guildModel = (sender as FrameworkElement).DataContext as GuildViewModel;
            var guild = App.Current.Discord.Guilds[guildModel.Id];

            App.Current.RootFrame.Navigate(new Uri("/Unicord.WP7;component/Pages/ChannelPickerPage.xaml?id=" + guild.Id, UriKind.Relative));
        }

        private void DMsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.OfType<ChannelViewModel>().FirstOrDefault();
            if (item != null)
            {
                DMsList.SelectedItem = null;
                App.Current.RootFrame.Navigate(new Uri("/Unicord.WP7;component/Pages/ChannelPage.xaml?id=" + item.Id, UriKind.Relative));
            }
        }
    }
}