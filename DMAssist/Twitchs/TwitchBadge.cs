using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Twitchs
{
    public class TwitchBadge
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }

        public TwitchBadge()
        {

        }

        public void Read(JToken token)
        {

        }

        public void Write(JToken token)
        {
            token["Name"] = this.Name;
            token["Version"] = this.Version;
            token["Path"] = this.Path;
        }

    }

}
