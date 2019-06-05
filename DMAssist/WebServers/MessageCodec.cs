using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.WebServers
{
    public class MessageCodec
    {
        private readonly List<MessageRegistration> List;

        public MessageCodec()
        {
            this.List = new List<MessageRegistration>();
        }

        public JToken Write(MessageBase message)
        {
            var reg = this.From(message.GetType());
            var token = new JObject();
            token["Type"] = reg.Name;
            message.Write(token);

            return token;
        }

        public MessageBase Read(JToken token)
        {
            var type = token.Value<string>("Type");
            var reg = this.From(type);
            var message = reg.Constructor();
            message.Read(token);

            return message;
        }

        public MessageRegistration From(string name)
        {
            lock (this.List)
            {
                return this.List.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

        }

        public MessageRegistration From(Type type)
        {
            lock (this.List)
            {
                return this.List.FirstOrDefault(r => r.Type.Equals(type));
            }

        }

        public void Register<T>(string name, Func<T> constructor) where T : MessageBase
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

                this.List.Add(new MessageRegistration(name, type, constructor));
            }

        }

    }

}
