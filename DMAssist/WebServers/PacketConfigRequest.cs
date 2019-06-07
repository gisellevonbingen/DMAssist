using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DMAssist.WebServers
{
    public class PacketConfigRequest : PacketBase
    {
        public string Name { get; set; }

        public PacketConfigRequest()
        {
            this.Name = null;
        }

        public override void Read(JToken token)
        {
            this.Name = token.Value<string>("Name");
        }

        public override void Write(JToken token)
        {
            token["Name"] = this.Name;
        }

    }

}
