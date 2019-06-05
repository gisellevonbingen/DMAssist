using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.WebServers
{
    public enum WebServerState : byte
    {
        Stopped = 0,
        Starting = 1,
        Started = 2,
        Stopping = 3,
    }

}
