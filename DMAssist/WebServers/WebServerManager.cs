using DMAssist.Themes;
using DMAssist.Twitchs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace DMAssist.WebServers
{
    public class WebServerManager : IDisposable
    {
        private const string Path = "/TwitchChat";

        private object StateLock { get; }
        public WebServerState State { get; private set; }
        private MessageCodec Codec;

        private WebSocketServer Server;


        public WebServerManager()
        {
            this.StateLock = new object();
            this.State = WebServerState.Stopped;
            this.Codec = new MessageCodec();
            this.Codec.Register("config_req", () => new MessageConfigRequest());
            this.Codec.Register("config_ntf", () => new MessageConfigNotify());
            this.Codec.Register("chat", () => new MessageChat());

            this.Server = null;

        }

        private void OnTwitchChatManagerPrivateMessage(object sender, PrivateMessageEventArgs e)
        {
            var message = e.Message;
            var text = message.Message;
            var tags = message.Tags;

            var messageChat = new MessageChat();
            messageChat.DisplayName = tags.DisplayName;
            messageChat.Color = tags.Color;

            var writeToken = this.Codec.Write(messageChat);

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
                var settings = program.Settings;

                try
                {
                    var server = this.Server = new WebSocketServer(settings.WebSocketPort);
                    server.Start();
                    server.AddWebSocketService(Path, () =>
                    {
                        var behavior = new WebBehavior();
                        behavior.Message += this.OnWebBehaviorMessage;
                        return behavior;
                    });

                    program.TwitchChatManager.PrivateMessage += this.OnTwitchChatManagerPrivateMessage;
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
                var token = this.Codec.Write(new MessageConfigNotify());
                session.Send(token);
            }

        }

        private void OnWebBehaviorMessage(object sender, JToken token)
        {
            var behavior = sender as WebBehavior;
            var message = this.Codec.Read(token);

            if (message is MessageConfigRequest mcr)
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
                program.TwitchChatManager.PrivateMessage -= this.OnTwitchChatManagerPrivateMessage;
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
