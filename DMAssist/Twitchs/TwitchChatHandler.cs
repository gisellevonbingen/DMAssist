using DMAssist.DCCons;
using DMAssist.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChat.Commands;

namespace DMAssist.Twitchs
{
    public class TwitchChatHandler : IDisposable
    {
        public static char DCConPrefix { get; } = '~';
        public static string ColorChatSuffix { get; } = ((char)1).ToString();
        public static string ColorChatPrefix { get; } = $"{ColorChatSuffix}ACTION ";

        public event EventHandler<WrappedCommand> HandleCommand;

        private Dictionary<string, string> UserChatColors;

        public TwitchChatHandler(TwitchChatManager tcm)
        {
            tcm.CommandRecieve += this.OnCommandRecieve;

            this.UserChatColors = new Dictionary<string, string>();
        }

        protected virtual void OnHandleCommand(WrappedCommand command)
        {
            this.HandleCommand?.Invoke(this, command);
        }

        private IEnumerable<TwitchBadge> SelectBadges(Badge[] badges)
        {
            var bm = Program.Instance.BadgeManager;
            return badges.Select(b =>
            {
                var lb = bm.Get(b.Name, b.Version);
                return lb != null ? new TwitchBadge() { Name = b.Name, Version = b.Version, Path = lb.Path } : null;
            }).Where(b => b != null);
        }

        private string PeekColor(string id, string tagColor)
        {
            if (string.IsNullOrWhiteSpace(tagColor) == true)
            {
                lock (this.UserChatColors)
                {
                    if (this.UserChatColors.TryGetValue(id, out var color) == false)
                    {
                        color = ColorUtils.Random().ToRgbaHashString();
                        this.UserChatColors[id] = color;
                    }

                    return color;
                }

            }

            return tagColor;
        }

        private void OnCommandRecieve(object sender, CommandEventArgs e)
        {
            var command = e.Command;

            if (command is CommandPrivateMessage cpm)
            {
                var tags = cpm.Tags;
                var tuple = this.ParseMessage(cpm);

                var message = new PrivateMessage();
                message.Badges.AddRange(this.SelectBadges(tags.Badeges));
                message.DisplayName = tags.DisplayName;
                message.ColorChat = tuple.ColorChat;
                message.Components.AddRange(tuple.Components);
                message.Color = this.PeekColor(tags.UserId, tags.Color);
                message.UserName = cpm.Sender.User;
                message.Id = tags.Id;
                this.OnHandleCommand(message);
            }
            else if (command is CommandClearChat clearChat)
            {
                var wc = new ClearChat();
                wc.User = clearChat.User;
                wc.BanDuration = clearChat.Tags.BanDuration;
                this.OnHandleCommand(wc);
            }
            else if (command is CommandClearMessage clearMessage)
            {
                var tags = clearMessage.Tags;
                var wc = new ClearMessage();
                wc.Login = tags.Login;
                wc.Message = tags.Message;
                wc.TargetMessageid = tags.TargetMessageid;
                this.OnHandleCommand(wc);
            }

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

        private string GetEmoteURL(string id)
        {
            return $"http://static-cdn.jtvnw.net/emoticons/v1/{id}/";
        }

        private List<ChatComponent> SplitTwitchEmotes(string message, TwitchChat.Commands.Emote[] tagEmotes)
        {
            var components = new List<ChatComponent>();

            var endIndex = 0;
            var emotes = this.ParseTwitchEmotes(tagEmotes);

            foreach (var emote in emotes)
            {
                var leftCount = emote.StartIndex - endIndex;
                var left = message.Substring(endIndex, leftCount);

                if (string.IsNullOrWhiteSpace(left) == false)
                {
                    components.Add(new ChatComponentText() { Text = left });
                }

                var emoteText = message.Substring(emote.StartIndex, emote.Count);
                components.Add(new ChatComponentImage() { Type = "TwitchEmote", URL = this.GetEmoteURL(emote.Id), Title = emoteText });

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

        private DCCon Filter(DCCon[] cons, string text)
        {
            var list = new List<DCCon>();

            foreach (var con in cons)
            {
                if (con.Keywords.Any(keyword => keyword.Equals(text)) == true)
                {
                    return con;
                }

            }

            return null;
        }

        private List<ChatComponent> SplitDCCon(DCCon[] cons, string text)
        {
            var values = new List<ChatComponent>();
            var builder = new StringBuilder();
            DCCon lastMatched = null;
            string lastMatchedText = null;
            bool matching = false;

            for (int i = 0; i < text.Length + 1; i++)
            {
                var c = i < text.Length ? text[i] : '\0';

                if (i == text.Length || c == DCConPrefix)
                {
                    if (lastMatched != null)
                    {
                        values.Add(new ChatComponentImage() { URL = lastMatched.Path, Title = DCConPrefix + lastMatchedText, Type = "DCCon" });
                    }
                    else
                    {
                        var str = builder.ToString();

                        if (matching == true)
                        {
                            str = DCConPrefix + str;
                        }

                        if (string.IsNullOrWhiteSpace(str) == false)
                        {
                            values.Add(new ChatComponentText() { Text = str });
                        }

                    }

                    lastMatched = null;
                    lastMatchedText = null;
                    builder.Clear();
                    matching = true;
                }
                else
                {
                    if (matching == true)
                    {
                        var str = builder.ToString() + c;
                        var matched = this.Filter(cons, str);

                        if (matched != null)
                        {
                            lastMatched = matched;
                            lastMatchedText = str;
                        }
                        else if (lastMatched != null)
                        {
                            values.Add(new ChatComponentImage() { URL = lastMatched.Path, Title = DCConPrefix + lastMatchedText, Type = "DCCon" });
                            lastMatched = null;
                            lastMatchedText = null;
                            matching = false;
                            builder.Clear();
                        }

                    }

                    builder.Append(c);
                }

            }

            return values;
        }

        private List<ChatComponent> SplitDCCon(List<ChatComponent> components)
        {
            var dcCons = Program.Instance.DCConManager.Values;
            var values = new List<ChatComponent>();

            foreach (var component in components)
            {
                if (component is ChatComponentText cct)
                {
                    values.AddRange(this.SplitDCCon(dcCons, cct.Text));
                }
                else
                {
                    values.Add(component);
                }

            }

            return values;
        }

        private (bool ColorChat, List<ChatComponent> Components) ParseMessage(CommandPrivateMessage command)
        {
            var message = command.Message;
            var colorChat = false;

            if (message.StartsWith(ColorChatPrefix) && message.EndsWith(ColorChatSuffix))
            {
                var count = message.Length - ColorChatPrefix.Length - ColorChatSuffix.Length;
                message = message.Substring(ColorChatPrefix.Length, count);

                colorChat = true;
            }

            var emotes = this.SplitTwitchEmotes(message, command.Tags.Emotes);
            var results = this.SplitDCCon(emotes);

            return (colorChat, results);
        }

        protected virtual void Dispose(bool disposing)
        {
            Program.Instance.TwitchChatManager.CommandRecieve -= this.OnCommandRecieve;
        }

        ~TwitchChatHandler()
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
