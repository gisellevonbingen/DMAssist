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
    public class PacketCommand : PacketBase
    {
        public WrappedCommand Command { get; set; }

        public PacketCommand()
        {

        }

        public override void Read(JToken token)
        {

        }

        public override void Write(JToken token)
        {
            var command = this.Command;
            token["CommandType"] = command.Type;

            var commandToken = new JObject();
            command.Write(commandToken);

            token["Command"] = commandToken;
        }

    }

}
