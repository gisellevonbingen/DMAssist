using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChat;

namespace DMAssist.Twitchs
{
    public abstract class Activity
    {
        public abstract void Act(TwitchChatManager tcm, TwitchChatClient tcc);
    }

}
