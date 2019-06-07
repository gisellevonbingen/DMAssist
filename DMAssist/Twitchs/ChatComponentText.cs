using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using TwitchAPIs;

namespace DMAssist.Twitchs
{
    public class ChatComponentText : ChatComponent
    {
        public string Text { get; set; }

        public ChatComponentText()
        {
            this.Text = null;
        }

        public override void Read(JToken token)
        {
            base.Read(token);

            this.Text = token.Value<string>("Text");
        }

        public override void Write(JToken token)
        {
            base.Write(token);

            token["Text"] = this.Text;
        }

        public override string ToString()
        {
            return this.Text;
        }

    }

}
