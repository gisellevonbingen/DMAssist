using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.WebServers
{
    public class PacketCodec
    {
        private readonly List<PacketRegistration> List;

        public PacketCodec()
        {
            this.List = new List<PacketRegistration>();
        }

        public JToken Write(PacketBase packet)
        {
            var reg = this.From(packet.GetType());
            var token = new JObject();
            token["Type"] = reg.Name;
            packet.Write(token);

            return token;
        }

        public PacketBase Read(JToken token)
        {
            var type = token.Value<string>("Type");
            var reg = this.From(type);
            var packet = reg.Constructor();
            packet.Read(token);

            return packet;
        }

        public PacketRegistration From(string name)
        {
            lock (this.List)
            {
                return this.List.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

        }

        public PacketRegistration From(Type type)
        {
            lock (this.List)
            {
                return this.List.FirstOrDefault(r => r.Type.Equals(type));
            }

        }

        public void Register<T>(string name, Func<T> constructor) where T : PacketBase
        {
            lock (this.List)
            {
                var type = typeof(T);

                if (this.From(name) != null)
                {
                    throw new ArgumentException($"Name({name}) is already registered");
                }

                if (this.From(type) != null)
                {
                    throw new ArgumentException($"Type({type}) is already registered");
                }

                this.List.Add(new PacketRegistration(name, type, constructor));
            }

        }

    }

}
