using DMAssist.Badges;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Twitchs
{
    public class PrivateMessage : WrappedCommand
    {
        public List<TwitchBadge> Badges { get; }
        public string DisplayName { get; set; }
        public bool ColorChat { get; set; }
        public List<ChatComponent> Components { get; }
        public string Color { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }

        public override string Type => "PrivateMessage";

        public PrivateMessage()
        {
            this.Badges = new List<TwitchBadge>();
            this.Components = new List<ChatComponent>();
        }

        public override void Read(JToken token)
        {
            base.Read(token);
        }

        public override void Write(JToken token)
        {
            base.Write(token);

            token["Badges"] = new JArray(this.Badges.Select(c =>
            {
                var t = new JObject();
                c.Write(t);
                return t;
            }));
            token["DisplayName"] = this.DisplayName;
            token["ColorChat"] = this.ColorChat;
            token["Components"] = new JArray(this.Components.Select(c =>
            {
                var t = new JObject();
                c.Write(t);
                return t;
            }));

            token["Color"] = this.Color;
            token["UserName"] = this.UserName;
            token["Id"] = this.Id;
        }

    }

}
