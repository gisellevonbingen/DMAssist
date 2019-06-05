using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TwitchChat.Commands;

namespace DMAssist.WebServers
{
    public class MessageChat : MessageBase
    {
        public List<Badge> Badges { get; }
        public string DisplayName { get; set; }
        public List<ChatComponent> Components { get; }
        public string Color { get; set; }

        public MessageChat()
        {
            this.Badges = new List<Badge>();
            this.Components = new List<ChatComponent>();
        }

        public override void Read(JToken token)
        {

        }

        public override void Write(JToken token)
        {
            token["Bages"] = new JArray(this.Badges.Select(b => b.ToString()));
            token["DisplayName"] = this.DisplayName;
            token["Color"] = this.Color;
        }

    }

}
