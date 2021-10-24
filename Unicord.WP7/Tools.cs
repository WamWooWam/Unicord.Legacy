using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Unicord.WP7
{
    public static class Utils
    {
        public static void Scale(ref float currentWidth, ref float currentHeight, float maxWidth, float maxHeight)
        {
            if (currentWidth <= maxWidth && currentHeight <= maxHeight)
            {
                return;
            }
            else
            {
                var ratioX = maxWidth / currentWidth;
                var ratioY = maxHeight / currentHeight;
                var ratio = Math.Min(ratioX, ratioY);

                currentWidth = (currentWidth * ratio);
                currentHeight = (currentHeight * ratio);
            }
        }

        public static void SetImmediate(Action func)
        {
            var timer = new DispatcherTimer();
            timer.Tick += (o, ev) =>
            {
                timer.Stop();
                func();
            };

            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Start();
        }

        public static void SetTimeout(double ms, Action func)
        {
            var timer = new DispatcherTimer();
            timer.Tick += (o, ev) =>
            {
                (o as DispatcherTimer).Stop();
                func();
            };

            timer.Interval = TimeSpan.FromMilliseconds(ms);
            timer.Start();
        }
        
        public static T FindChild<T>(this DependencyObject parent, string controlName = null) where T : FrameworkElement
        {
            for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); ++index)
            {
                var child = VisualTreeHelper.GetChild(parent, index);
                if (child is T && (controlName == null || ((T)child).Name == controlName))
                {
                    return ((T)child);
                }
                else if ((child = FindChild<T>(child, controlName)) != null)
                {
                    return child as T;
                }
            }

            return default(T);
        }
    }
}
