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
        public string TwitchChannelName { get; set; }
        public ushort WebSocketPort { get; set; }
        public string TwitchEmoteSize { get; set; }
        public string DCConURL { get; set; }

        public Configuration()
        {
            this.TwitchChannelName = "daengmin2";
            this.WebSocketPort = 6974;
            this.TwitchEmoteSize = "1.0";
            this.DCConURL = "https://raw.githubusercontent.com/yooya200/DaengMinDcCon/master/list.json";
        }

        public void Read(JToken token)
        {
            this.TwitchChannelName = token.Value<string>("TwitchChannelName");
            this.WebSocketPort = token.Value<ushort>("WebSocketPort");
            this.TwitchEmoteSize = token.Value<string>("TwitchEmoteSize");
            this.DCConURL = token.Value<string>("DCConURL");
        }

        public void Write(JToken token)
        {
            token["TwitchChannelName"] = this.TwitchChannelName;
            token["WebSocketPort"] = this.WebSocketPort;
            token["TwitchEmoteSize"] = this.TwitchEmoteSize;
            token["DCConURL"] = this.DCConURL;
        }

    }

}
