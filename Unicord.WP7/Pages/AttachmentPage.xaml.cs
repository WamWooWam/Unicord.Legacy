using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Unicord.WP7.Pages
{
    // 
    // taken from http://www.frenk.com/2011/03/windows-phone-7-correct-pinch-zoom-in-silverlight/
    //
    public partial class AttachmentPage : PhoneApplicationPage
    {
        private double TotalImageScale = 1d;
        private Point ImagePosition = new Point(0, 0);

        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;

        public AttachmentPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ZoomImage.Source = new BitmapImage(new Uri(NavigationContext.QueryString["url"]));
        }

        private void OnPinchStarted(object s, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(ZoomImage, 0);
            _oldFinger2 = e.GetPosition(ZoomImage, 1);
            _oldScaleFactor = 1;
        }

        private void OnPinchDelta(object s, PinchGestureEventArgs e)
        {
            var scaleFactor = e.DistanceRatio / _oldScaleFactor;

            var currentFinger1 = e.GetPosition(ZoomImage, 0);
            var currentFinger2 = e.GetPosition(ZoomImage, 1);

            var translationDelta = GetTranslationDelta(
                currentFinger1,
                currentFinger2,
                _oldFinger1,
                _oldFinger2,
                ImagePosition,
                scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            UpdateImage(scaleFactor, translationDelta);
        }

        private void UpdateImage(double scaleFactor, Point delta)
        {
            TotalImageScale *= scaleFactor;
            ImagePosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

            var transform = (CompositeTransform)ZoomImage.RenderTransform;
            transform.ScaleX = TotalImageScale;
            transform.ScaleY = TotalImageScale;
            transform.TranslateX = ImagePosition.X;
            transform.TranslateY = ImagePosition.Y;
        }

        private Point GetTranslationDelta(
            Point currentFinger1, Point currentFinger2,
            Point oldFinger1, Point oldFinger2,
            Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(
                currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
                currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(
                currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
                currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point(
                (newPos1.X + newPos2.X) / 2,
                (newPos1.Y + newPos2.Y) / 2);

            return new Point(
                newPos.X - currentPosition.X,
                newPos.Y - currentPosition.Y);
        }
    }
}