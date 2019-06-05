using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChat.Commands;

namespace DMAssist.Twitchs
{
    public class PrivateMessageEventArgs : EventArgs
    {
        public CommandPrivateMessage Command { get; }

        public PrivateMessageEventArgs(CommandPrivateMessage command)
        {
            this.Command = command;
        }

    }

}
