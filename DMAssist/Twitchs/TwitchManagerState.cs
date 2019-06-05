using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Twitchs
{
    public enum TwitchManagerState : byte
    {
        Stopped = 0,
        Starting = 1,
        Connected = 2,
        Diconnected = 3,
        Stopping = 4,
    }

}
