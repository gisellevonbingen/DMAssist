using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchAPIs;

namespace DMAssist.DCCons
{
    public class DCCon
    {
        public string[] Keywords { get; set; }
        public string Path { get; set; }

        public DCCon()
        {
            this.Keywords = null;
            this.Path = null;
        }

        public void Read(JToken token)
        {
            this.Keywords = token.ReadArray("keywords", t => t.Value<string>());
            this.Path = token.Value<string>("path");
        }

    }

}
