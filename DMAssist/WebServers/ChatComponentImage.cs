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
        public string Title { get; set; }

        public ChatComponentImage()
        {
            this.URL = null;
            this.Type = null;
            this.Title = null;
        }

        public override string ToString()
        {
            return $"{this.Type}:{this.Title}";
        }

    }

}
