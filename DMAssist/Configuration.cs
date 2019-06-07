using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist
{
    public class Configuration
    {
        public string ClientId { get; set; }
        public string TwitchChannelName { get; set; }
        public string DCConURL { get; set; }
        public ushort WebSocketPort { get; set; }

        public Configuration()
        {
            this.ClientId = "";
            this.TwitchChannelName = "daengmin2";
            this.DCConURL = "https://raw.githubusercontent.com/yooya200/DaengMinDcCon/master/list.json";
            this.WebSocketPort = 6974;
        }

        public void Read(JToken token)
        {
            this.ClientId = token.Value<string>("ClientId");
            this.TwitchChannelName = token.Value<string>("TwitchChannelName");
            this.DCConURL = token.Value<string>("DCConURL");
            this.WebSocketPort = token.Value<ushort>("WebSocketPort");
        }

        public void Write(JToken token)
        {
            token["ClientId"] = this.ClientId;
            token["TwitchChannelName"] = this.TwitchChannelName;
            token["DCConURL"] = this.DCConURL;
            token["WebSocketPort"] = this.WebSocketPort;
        }

    }

}
