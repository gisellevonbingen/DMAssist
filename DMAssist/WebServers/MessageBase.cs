using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.WebServers
{
    public abstract class MessageBase
    {
        public abstract void Read(JToken token);

        public abstract void Write(JToken token);
    }

}
