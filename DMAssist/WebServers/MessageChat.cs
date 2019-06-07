using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMAssist.Twitchs;
using Newtonsoft.Json.Linq;
using TwitchChat.Commands;

namespace DMAssist.WebServers
{
    public class MessageChat : MessageBase
    {
        public PrivateMessage Message { get; set; }

        public MessageChat()
        {
            this.Message = new PrivateMessage();
        }

        public override void Read(JToken token)
        {

        }

        public override void Write(JToken token)
        {
            var messageToken = new JObject();
            this.Message.Write(messageToken);
            token["Message"] = messageToken;
        }

    }

}
