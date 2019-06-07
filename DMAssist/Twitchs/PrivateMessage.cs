using DMAssist.Badges;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Twitchs
{
    public class PrivateMessage
    {
        public List<Badge> Badges { get; }
        public string DisplayName { get; set; }
        public List<ChatComponent> Components { get; }
        public string Color { get; set; }

        public PrivateMessage()
        {
            this.Badges = new List<Badge>();
            this.Components = new List<ChatComponent>();
        }

        public void Read(JToken token)
        {

        }

        public void Write(JToken token)
        {
            token["Badges"] = new JArray(this.Badges.Select(b => b.Path));
            token["DisplayName"] = this.DisplayName;
            token["Color"] = this.Color;
            token["Components"] = new JArray(this.Components.Select(c =>
            {
                var t = new JObject();
                c.Write(t);
                return t;
            }));

        }

    }

}
