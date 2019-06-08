using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DMAssist.WebServers
{
    public class PacketConfigNotify : PacketBase
    {
        public JObject Config { get; set; }

        public PacketConfigNotify()
        {

        }

        public override void Read(JToken token)
        {
        }

        public override void Write(JToken token)
        {
            token["Config"] = this.Config;
        }

    }

}
