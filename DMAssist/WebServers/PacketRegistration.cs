using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.WebServers
{
    public class PacketRegistration
    {
        public string Name { get; }
        public Type Type { get; }
        public Func<PacketBase> Constructor { get; }

        public PacketRegistration(string name, Type type, Func<PacketBase> constructor)
        {
            this.Name = name;
            this.Type = type;
            this.Constructor = constructor;
        }

    }

}
