﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMAssist.Utils
{
    public static class PathUtils
    {
        public static string Normalize(string path)
        {
            return path?.Replace('/', '\\');
        }

        public static string FilterInvalids(string fileName)
        {
            var builder = new StringBuilder();
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var c in fileName)
            {
                if (Array.IndexOf(invalidChars, c) == -1)
                {
                    builder.Append(c);
                }

            }

            return builder.ToString();
        }

    }

}
