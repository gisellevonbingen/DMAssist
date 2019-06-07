using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Utils
{
    public static class ColorUtils
    {
        public const string HashPrefix = "#";

        public static Color Random()
        {
            return Random(new System.Random());
        }

        public static Color Random(Random random)
        {
            var buffer = new byte[3];
            random.NextBytes(buffer);

            var color = Color.FromArgb(buffer[0], buffer[1], buffer[2]);
            return color;
        }

        public static string ToRgbaHashString(this Color color)
        {
            return $"{HashPrefix}{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}{color.A.ToString("X2")}";
        }

    }

}
