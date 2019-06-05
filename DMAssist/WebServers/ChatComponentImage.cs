using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.WebServers
{
    public class ChatComponentImage : ChatComponent
    {
        public string URL { get; set; }
        public string Type { get; set; }
        public string Alt { get; set; }

        public ChatComponentImage()
        {
            this.URL = null;
            this.Type = null;
            this.Alt = null;
        }

        public override string ToString()
        {
            return $"{this.Type}:{this.Alt}";
        }

    }

}
