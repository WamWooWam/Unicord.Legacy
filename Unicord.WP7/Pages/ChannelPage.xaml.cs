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
using System.Windows.Data;
using System.Windows.Threading;
using System.Net.Http;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;
using Unicord.WP7.ViewModels.Cache;
using System.Threading.Tasks;

namespace Unicord.WP7.Pages
{
    public partial class ChannelPage : UnicordPage
    {
        private ChannelPageViewModel _viewModel;
        private ScrollViewer _scrollView;

        private DispatcherTimer _autoScrollTimer;
        private DispatcherTimer _autoSaveTimer;

        private bool _previousShouldAutoScroll;
        private bool _didStartLoadingMessages;
        private bool _isLoadingMessages;
        private double _scrollPosition;
        private object _lastScrollMessage;

        public bool IgnoreAutoScroll { get { return _scrollPosition < _scrollView.ScrollableHeight - 100; } }

        public double ScrollViewVerticalOffset
        {
            get { return (double)GetValue(ScrollViewVerticalOffsetProperty); }
            set { SetValue(ScrollViewVerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty ScrollViewVerticalOffsetProperty =
            DependencyProperty.Register("ScrollViewVerticalOffset", typeof(double), typeof(ChannelPage), new PropertyMetadata(0.0, OnScrollVerticalOffsetChanged));

        private static void OnScrollVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = (ChannelPage)d;
            page._scrollPosition = page._scrollView.VerticalOffset;
            page.FetchMoreMessages();
        }

        public ChannelPage()
        {
            InitializeComponent();

            _autoScrollTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(50) };
            _autoScrollTimer.Tick += OnAutoScrollTimerTick;

            _autoSaveTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
            _autoSaveTimer.Tick += OnAutoSaveTimerTick;
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var discord = App.Current.EnsureDiscordClient();
            if (!discord.Socket.IsConnected)
            {
                var id = ulong.Parse(NavigationContext.QueryString["id"]);
                DataContext = await TaskEx.Run(() => LoadCacheViewModel(id));
                AutoScroll();
            }

            base.OnNavigatedTo(e);
        }

        private async void UnicordPage_Loaded(object sender, RoutedEventArgs e)
        {
            var id = ulong.Parse(NavigationContext.QueryString["id"]);

            using (var context = this.GetProgressContext())
            {
                context.ProgressIndicator.IsIndeterminate = true;
                context.ProgressIndicator.IsVisible = true;
                context.ProgressIndicator.Text = "Loading...";

                await App.Current.Discord.EnsureConnectedAsync();
                var channel = App.Current.Discord.GetCachedChannel(id);

                _viewModel = new ChannelPageViewModel(channel);
                _viewModel.Messages.CollectionChanged += Messages_CollectionChanged;

                if (DataContext == null) // assign the datacontext now if there's no cache visible
                    DataContext = _viewModel;

                await _viewModel.LoadAsync();

                DataContext = _viewModel; // or now if there is a cache 
            }

            _autoSaveTimer.Start();
        }

        private ChannelPageViewModelCache LoadCacheViewModel(ulong id)
        {
            var fileName = string.Format("channel-cache_{0}.json", id);
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.FileExists(fileName))
            {
                var serializer = new JsonSerializer();
                using (var stream = store.OpenFile(fileName, FileMode.Open))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    return serializer.Deserialize<ChannelPageViewModelCache>(jsonReader);
                }
            }

            return null;
        }

        private void UpdateViewModelCache()
        {
            if (_viewModel == null) return;

            var serializer = new JsonSerializer();
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            var fileName = string.Format("channel-cache_{0}.json", _viewModel.Id);
            using (var stream = store.OpenFile(fileName, FileMode.OpenOrCreate))
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamWriter = new StreamWriter(stream))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(streamWriter, new ChannelPageViewModelCache(_viewModel));
                }
            }
        }

        private void OnAutoSaveTimerTick(object sender, EventArgs e)
        {
            var background = TaskEx.Run(() => UpdateViewModelCache());
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            // if (_scrollView != null) return;

            _scrollView = (ScrollViewer)sender;
            _scrollView.ManipulationCompleted += OnScrollManipulationCompleted;

            var binding = new Binding() { Source = _scrollView, Path = new PropertyPath("VerticalOffset"), Mode = BindingMode.OneWay };
            this.SetBinding(ScrollViewVerticalOffsetProperty, binding);
        }

        private void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (e.NewSize.Height < e.PreviousSize.Height)
            //    this._scrollPosition += e.PreviousSize.Height - e.NewSize.Height;
            //this.AutoScroll();


            if (_autoScrollTimer.IsEnabled || !_previousShouldAutoScroll || _didStartLoadingMessages || _isLoadingMessages)
                return;
            _scrollPosition = _scrollView.ScrollableHeight;
            AutoScroll();
        }

        private void OnScrollManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            FetchMoreMessages();
        }

        private void OnAutoScrollTimerTick(object sender, EventArgs e)
        {
            _autoScrollTimer.Stop();

            if (MessagesList.Items.Count <= 0)
                return;

            _scrollView.ScrollToVerticalOffset((double)int.MaxValue);
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            if (_didStartLoadingMessages && !_isLoadingMessages)
            {
                _didStartLoadingMessages = false;
                _isLoadingMessages = true;

                var previousScrollHeight = _scrollView.ScrollableHeight;
                Utils.SetTimeout(330, () =>
                {
                    if (this._scrollPosition < 100)
                    {
                        ScrollIntoView(_lastScrollMessage);
                    }
                    else
                    {
                        _scrollView.ScrollToVerticalOffset(_scrollView.ScrollableHeight - (previousScrollHeight - _scrollView.VerticalOffset));
                    }

                    Utils.SetTimeout(100, () =>
                        _isLoadingMessages = false);
                });
            }

            if (_isLoadingMessages)
                return;

            AutoScroll();
        }

        private void ScrollIntoView(object item)
        {
            var itemContainer = this.MessagesList.ItemContainerGenerator.ContainerFromItem(item);
            if (!(itemContainer is FrameworkElement))
                return;

            var frameworkElement = itemContainer as FrameworkElement;
            Point point = frameworkElement.TransformToVisual(_scrollView)
                                          .Transform(new Point(0.0, 0.0));
            if (point.Y >= 0.0 && point.Y <= _scrollView.ViewportHeight)
                return;

            _scrollView.ScrollToVerticalOffset(_scrollView.VerticalOffset + point.Y - -30.0);
        }

        private void AutoScroll()
        {
            if (_scrollView != null && !this.IgnoreAutoScroll)
            {
                _previousShouldAutoScroll = true;
                _autoScrollTimer.Start();
            }
            else
                _previousShouldAutoScroll = false;
        }

        private async void FetchMoreMessages()
        {
            if (_viewModel == null || _scrollPosition > 0.0 || _didStartLoadingMessages || _isLoadingMessages)
                return;

            if (this.MessagesList.Items.Count > 0)
            {
                _lastScrollMessage = this.MessagesList.Items.First<object>();
                _didStartLoadingMessages = true;
            }

            using (var context = this.GetProgressContext())
            {
                context.ProgressIndicator.IsIndeterminate = true;
                context.ProgressIndicator.IsVisible = true;

                await _viewModel.LoadAsync().ConfigureAwait(false);
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
            using (var context = this.GetProgressContext())
            {
                var progressIndicator = context.ProgressIndicator;
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

                await App.Current.Discord.Rest.CreateMessageAsync(_viewModel.Id, fileName, e.ChosenPhoto, progress)
                                              .ConfigureAwait(false);
            }
        }

        private async void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Format("Shared\\ShellContent\\{0}.png", _viewModel.IconHash);

            var store = IsolatedStorageFile.GetUserStoreForApplication();
            if (!store.DirectoryExists("Shared\\ShellContent"))
                store.CreateDirectory("Shared\\ShellContent");
            if (!store.FileExists(filePath))
            {
                using (var client = new HttpClient())
                {
                    var stream = await client.GetStreamAsync(_viewModel.IconUrl);
                    var fileStream = store.CreateFile(filePath);

                    await stream.CopyToAsync(fileStream);
                }
            }

            var data = new StandardTileData();
            data.Title = string.Format("#{0}", _viewModel.Title);
            data.BackTitle = _viewModel.SubTitle;
            data.BackgroundImage = new Uri(("isostore:\\" + filePath).Replace('\\', '/'));

            ShellTile.Create(new Uri("/Pages/ChannelPage.xaml?id=" + _viewModel.Id, UriKind.Relative), data);
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var messageViewModel = (sender as FrameworkElement).DataContext as MessageViewModel;
            var message = messageViewModel.Message;
            var attachment = message.Attachments.FirstOrDefault();
            if (attachment != null)
            {
                App.Current.RootFrame.Navigate(new Uri("/Unicord.WP7;component/Pages/AttachmentPage.xaml?url=" + HttpUtility.UrlEncode(attachment.Url), UriKind.Relative));
            }
        }
    }
}