using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.WebServers
{
    public class ChatComponentText : ChatComponent
    {
        public string Text { get; set; }

        public ChatComponentText()
        {
            this.Text = null;
        }

        public override string ToString()
        {
            return this.Text;
        }

    }

}
