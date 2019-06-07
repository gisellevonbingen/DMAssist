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
        public event EventHandler<PrivateMessage> HandlePrivateMessage;
        public const char DCConPrefix = '~';

        public TwitchChatHandler(TwitchChatManager tcm)
        {
            tcm.PrivateMessage += this.OnPrivateMessage;
        }

        protected virtual void OnHandlePrivateMessage(PrivateMessage message)
        {
            this.HandlePrivateMessage?.Invoke(this, message);
        }

        private IEnumerable<Badges.Badge> SelectBadges(Badge[] badges)
        {
            var bm = Program.Instance.BadgeManager;
            return badges.Select(b => bm.Get(b.Name, b.Version)).Where(b => b != null);
        }

        private void OnPrivateMessage(object sender, PrivateMessageEventArgs e)
        {
            var command = e.Command;
            var tags = command.Tags;
            var components = this.ParseMessage(e.Command);

            var color = tags.Color;

            if (string.IsNullOrWhiteSpace(color) == true)
            {
                color = ColorUtils.Random().ToRgbaHashString();
            }

            var message = new PrivateMessage();
            message.Badges.AddRange(this.SelectBadges(tags.Badeges));
            message.DisplayName = tags.DisplayName;
            message.Components.AddRange(components);
            message.Color = color;

            this.OnHandlePrivateMessage(message);
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
                        lastMatched = null;
                        lastMatchedText = null;
                        matching = false;
                        builder.Clear();
                    }
                    else if (builder.Length > 0)
                    {
                        var str = builder.ToString();

                        if (matching == true)
                        {
                            str = DCConPrefix + str;
                        }

                        values.Add(new ChatComponentText() { Text = str });
                        builder.Clear();
                    }

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

        private List<ChatComponent> ParseMessage(CommandPrivateMessage command)
        {
            var emotes = this.SplitTwitchEmotes(command);
            var dccons = this.SplitDCCon(emotes);

            return dccons;
        }

        protected virtual void Dispose(bool disposing)
        {
            Program.Instance.TwitchChatManager.PrivateMessage -= this.OnPrivateMessage;
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
