using DMAssist.DCCons;
using DMAssist.Themes;
using DMAssist.Twitchs;
using DMAssist.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            var config = this.ReadConfigFile(theme);

            if (config != null)
            {
                var message = new PacketConfigNotify();
                message.Config = config;
                var token = this.Codec.Write(message);

                var sessions = this.GetSessions().Where(s => string.Equals(theme.Name, s.ThemeName, StringComparison.OrdinalIgnoreCase));

                foreach (var session in sessions)
                {
                    session.Send(token);
                }

            }

        }

        private JObject ReadConfigFile(Theme theme)
        {
            try
            {
                var json = File.ReadAllText(theme.ConfigFilePath);
                return JObject.Parse(json);
            }
            catch (Exception)
            {
                return null;
            }

        }

        private void OnWebBehaviorMessage(object sender, JToken token)
        {
            var behavior = sender as WebBehavior;
            var packet = this.Codec.Read(token);

            if (packet is PacketConfigRequest mcr)
            {
                var theme = Program.Instance.ThemeManager.Values.FirstOrDefault(t => t.Name.Equals(mcr.Name));
                behavior.ThemeName = mcr.Name;

                if (theme != null)
                {
                    var message = new PacketConfigNotify();
                    message.Config = this.ReadConfigFile(theme);
                    behavior.Send(this.Codec.Write(message));
                }

            }

        }

        public WebBehavior[] GetSessions()
        {
            var server = this.Server;

            if (server != null && server.WebSocketServices.TryGetServiceHost(Path, out var host) == true)
            {
                return host.Sessions.Sessions.OfType<WebBehavior>().ToArray();
            }
            else
            {
                return null;
            }

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
