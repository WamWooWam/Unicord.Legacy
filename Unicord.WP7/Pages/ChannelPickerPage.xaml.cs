using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Unicord.WP7.ViewModels;
using DiscordLib;

namespace Unicord.WP7.Pages
{
    public partial class ChannelPickerPage : UnicordPage
    {
        private ChannelPickerViewModel _pickerModel;
        public ChannelPickerPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var id = ulong.Parse(NavigationContext.QueryString["id"]);
            DataContext = _pickerModel = new ChannelPickerViewModel(id);
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.OfType<Channel>().FirstOrDefault();
            if (item != null && item.Type.IsText())
            {
                App.Current.RootFrame.Navigate(new Uri("/Unicord.WP7;component/Pages/ChannelPage.xaml?id=" + item.Id, UriKind.Relative));
            }
        }
    }
}