using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicord.WP7
{
    public class Utils
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

    }
}
