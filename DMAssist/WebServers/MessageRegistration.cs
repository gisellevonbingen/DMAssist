using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.WebServers
{
    public class MessageRegistration
    {
        public string Name { get; }
        public Type Type { get; }
        public Func<MessageBase> Constructor { get; }

        public MessageRegistration(string name, Type type, Func<MessageBase> constructor)
        {
            this.Name = name;
            this.Type = type;
            this.Constructor = constructor;
        }

    }

}
