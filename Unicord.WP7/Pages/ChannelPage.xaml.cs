using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Unicord.WP7.ViewModels;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using Microsoft;
using DiscordLib.Net.Http;
using System.IO;

namespace Unicord.WP7.Pages
{
    public partial class ChannelPage : UnicordPage
    {
        private ChannelPageViewModel _viewModel;

        public ChannelPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var id = ulong.Parse(NavigationContext.QueryString["id"]);
            var channel = App.Current.Discord.GetCachedChannel(id);

            DataContext = _viewModel = new ChannelPageViewModel(channel);
            _viewModel.Messages.CollectionChanged += Messages_CollectionChanged;
            await _viewModel.LoadAsync().ConfigureAwait(false);
        }

        void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                MessagesList.ScrollIntoView(_viewModel.Messages.Last());
            }
        }

        private async void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var text = MessageTextBox.Text;
                MessageTextBox.Text = "";

                if (!string.IsNullOrWhiteSpace(text))
                    await App.Current.Discord.Rest.CreateMessageAsync(_viewModel.Id, text, false);
            }
        }

        private void MessagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                MessagesList.SelectedItem = null;
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            var media = new PhotoChooserTask();
            media.ShowCamera = true;
            media.Completed += OnPhotoTaskCompleted;
            media.Show();
        }

        private async void OnPhotoTaskCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult != TaskResult.OK)
                return;

            var fileName = Path.GetFileName(e.OriginalFileName);
            var progressIndicator = new ProgressIndicator();
            progressIndicator.Text = string.Format("Uploading '{0}'...", fileName);
            progressIndicator.IsIndeterminate = true;
            progressIndicator.IsVisible = true;

            var progress = new Progress<HttpProgress>((p) =>
            {
                if (p.BytesSent != null)
                {
                    progressIndicator.IsIndeterminate = false;
                    progressIndicator.Value = (double)p.BytesSent.Value / p.BytesToSend;
                }
            });

            var oldProgressIndicator = SystemTray.GetProgressIndicator(this);
            SystemTray.SetProgressIndicator(this, progressIndicator);

            await App.Current.Discord.Rest.CreateMessageAsync(_viewModel.Id, fileName, e.ChosenPhoto, progress).ConfigureAwait(false);
            await Dispatcher.InvokeAsync(() => SystemTray.SetProgressIndicator(this, oldProgressIndicator));
        }
    }
}