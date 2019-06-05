using DMAssist.Themes;
using DMAssist.Twitchs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        private List<Emote> ParseTwitchEmotes(TwitchChat.Commands.Emote[] tagEmotes)
        {
            var emotes = new List<Emote>();

            foreach (var emote in tagEmotes)
            {
                foreach (var index in emote.Indices)
                {
                    var startIndex = index.StartIndex;
                    var count = index.LastIndex - index.StartIndex + 1;

                    emotes.Add(new Emote(startIndex, count, emote.EmoteId));
                }

            }

            emotes.Sort((o1, o2) => o1.StartIndex.CompareTo(o2.StartIndex));

            return emotes;
        }

        private string GetEmoteURL(string id, string size)
        {
            return $"http://static-cdn.jtvnw.net/emoticons/v1/{id}/{size}";
        }

        private List<ChatComponent> SplitTwitchEmotes(CommandPrivateMessage command)
        {
            var message = command.Message;
            var components = new List<ChatComponent>();

            var endIndex = 0;
            var emotes = this.ParseTwitchEmotes(command.Tags.Emotes);

            foreach (var emote in emotes)
            {
                var leftCount = emote.StartIndex - endIndex;
                var left = message.Substring(endIndex, leftCount);

                if (string.IsNullOrWhiteSpace(left) == false)
                {
                    components.Add(new ChatComponentText() { Text = left });
                }

                var emoteText = message.Substring(emote.StartIndex, emote.Count);
                components.Add(new ChatComponentImage() { Type = "Twitch", URL = this.GetEmoteURL(emote.Id, "1.0"), Alt = emoteText });

                endIndex = emote.StartIndex + emote.Count;
            }

            {
                var right = message.Substring(endIndex);

                if (string.IsNullOrWhiteSpace(right) == false)
                {
                    components.Add(new ChatComponentText() { Text = right });
                }

            }

            return components;
        }

        private List<ChatComponent> ParseMessage(CommandPrivateMessage command)
        {
            var components = this.SplitTwitchEmotes(command);

            return components;
        }


        private void OnTwitchChatManagerPrivateMessage(object sender, PrivateMessageEventArgs e)
        {
            var command = e.Command;
            var tags = command.Tags;
            var components = this.ParseMessage(e.Command);

            var message = new MessageChat();
            message.Badges.AddRange(tags.Badeges);
            message.DisplayName = tags.DisplayName;
            message.Color = tags.Color;
            message.Components.AddRange(components);

            var writeToken = this.Codec.Write(message);

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
