using DMAssist.DCCons;
using DMAssist.Themes;
using DMAssist.Twitchs;
using DMAssist.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChat.Commands;
using WebSocketSharp.Server;

namespace DMAssist.WebServers
{
    public class WebServerManager : IDisposable
    {
        private const string Path = "/TwitchChat";

        private object StateLock { get; }
        public WebServerState State { get; private set; }
        private PacketCodec Codec;

        private WebSocketServer Server;

        public WebServerManager()
        {
            this.StateLock = new object();
            this.State = WebServerState.Stopped;
            this.Codec = new PacketCodec();
            this.Codec.Register("config_req", () => new PacketConfigRequest());
            this.Codec.Register("config_ntf", () => new PacketConfigNotify());
            this.Codec.Register("chat", () => new PacketChat());

            this.Server = null;
        }

        private void OnTwitchChatManagerPrivateMessage(object sender, PrivateMessage message)
        {
            var packet = new PacketChat();
            packet.Message = message;

            var writeToken = this.Codec.Write(packet);

            foreach (var session in this.GetSessions())
            {
                session.Send(writeToken);
            }

        }

        public void Start()
        {
            lock (this.StateLock)
            {
                this.Stop();

                this.State = WebServerState.Starting;
                var program = Program.Instance;
                var settings = program.Configuration;

                try
                {
                    var server = this.Server = new WebSocketServer(settings.Value.WebSocketPort);
                    server.Start();
                    server.AddWebSocketService(Path, () =>
                    {
                        var behavior = new WebBehavior();
                        behavior.Message += this.OnWebBehaviorMessage;
                        return behavior;
                    });

                    program.TwitchChatHandler.HandlePrivateMessage += this.OnTwitchChatManagerPrivateMessage;
                    program.ThemeManager.ConfigChanged += this.OnThemeManagerConfigChanged;

                    this.State = WebServerState.Started;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    this.Stop();
                }

            }

        }

        private void OnThemeManagerConfigChanged(object sender, ThemeConfigChangedEventArgs e)
        {
            var theme = e.Theme;
            var session = this.GetSessions().FirstOrDefault(s => string.Equals(theme.Name, s.ThemeName, StringComparison.OrdinalIgnoreCase));

            if (session != null)
            {
                var token = this.Codec.Write(new PacketConfigNotify());
                session.Send(token);
            }

        }

        private void OnWebBehaviorMessage(object sender, JToken token)
        {
            var behavior = sender as WebBehavior;
            var packet = this.Codec.Read(token);

            if (packet is PacketConfigRequest mcr)
            {
                behavior.ThemeName = mcr.Name;
            }

        }

        private WebBehavior[] GetSessions()
        {
            var server = this.Server;
            return server.WebSocketServices[Path].Sessions.Sessions.OfType<WebBehavior>().ToArray();
        }

        public void Stop()
        {
            lock (this.StateLock)
            {
                this.State = WebServerState.Stopping;

                var server = this.Server;

                if (server != null)
                {
                    server.Stop();
                }

                var program = Program.Instance;
                program.ThemeManager.ConfigChanged -= this.OnThemeManagerConfigChanged;
                program.TwitchChatHandler.HandlePrivateMessage -= this.OnTwitchChatManagerPrivateMessage;
                this.State = WebServerState.Stopped;
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            this.Stop();
        }

        ~WebServerManager()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

    }

}
