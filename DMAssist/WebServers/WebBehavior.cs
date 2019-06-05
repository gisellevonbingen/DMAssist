using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DMAssist.WebServers
{
    public class WebBehavior : WebSocketBehavior
    {
        public string ThemeName { get; set; }

        public event EventHandler<JToken> Message;

        public WebBehavior()
        {
            this.ThemeName = null;
        }

        public void Send(JToken token)
        {
            base.Send(token.ToString(Formatting.None));
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            var token = new JObject();
            token["Value"] = 12345;
            this.Send(token);

        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);

            var token = JObject.Parse(e.Data);
            this.Message?.Invoke(this, token);

            this.Send(token);
        }

    }

}
