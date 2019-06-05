using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Twitchs
{
    public enum JoinState : byte
    {
        Parted = 0,
        Joining = 1,
        Joined = 2,
        Parting = 3,
    }

}
