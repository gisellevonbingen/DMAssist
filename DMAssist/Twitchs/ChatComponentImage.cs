using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DMAssist.Twitchs
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

        public override void Read(JToken token)
        {
            base.Read(token);

            this.URL = token.Value<string>("URL");
            this.Type = token.Value<string>("Type");
            this.Title = token.Value<string>("Title");
        }

        public override void Write(JToken token)
        {
            base.Write(token);

            token["URL"] = this.URL;
            token["Type"] = this.Type;
            token["Title"] = this.Title;
        }

        public override string ToString()
        {
            return $"{this.Type}:{this.Title}";
        }

    }

}
