using DMAssist.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Forms
{
    public class ThemeItem
    {
        public Theme Value { get; }

        public ThemeItem(Theme value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Value.Name;
        }

    }

}
