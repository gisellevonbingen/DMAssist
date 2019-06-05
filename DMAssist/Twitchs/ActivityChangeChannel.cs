using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChat;

namespace DMAssist.Twitchs
{
    public class ActivityChangeChannel : Activity
    {
        public string ChannelName { get; }

        public ActivityChangeChannel(string channelName)
        {
            this.ChannelName = channelName;
        }

        public override void Act(TwitchChatManager tcm, TwitchChatClient tcc)
        {
            tcm.ChangeChannel(tcc, this.ChannelName);
        }

    }

}
