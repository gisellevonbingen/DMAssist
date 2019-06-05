using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Themes
{
    public class ThemeConfigChangedEventArgs : EventArgs
    {
        public ThemeConfigChangedEventArgs(Theme theme)
        {
            this.Theme = theme;
        }

        public Theme Theme { get; }
    }

}
