using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Twitchs
{
    public abstract class WrappedCommand
    {
        public WrappedCommand()
        {

        }

        public virtual void Read(JToken token)
        {

        }

        public virtual void Write(JToken token)
        {

        }

        public virtual string Type { get; }

    }

}
