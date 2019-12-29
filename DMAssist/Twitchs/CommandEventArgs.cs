using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChat.Commands;

namespace DMAssist.Twitchs
{
    public class CommandEventArgs : EventArgs
    {
        public Command Command { get; }

        public CommandEventArgs(Command command)
        {
            this.Command = command;
        }

    }

}
