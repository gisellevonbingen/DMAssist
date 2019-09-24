using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchAPIs;

namespace DMAssist.Badges
{
    public class BadgeManager
    {
        private TwitchAPI API;
        private Dictionary<string, Dictionary<string, Badge>> Badges;

        public BadgeManager()
        {
            this.API = new TwitchAPI();
            this.Badges = new Dictionary<string, Dictionary<string, Badge>>();
        }

        public Badge Get(string name, string version)
        {
            lock (this.Badges)
            {
                var pair1 = this.Badges.FirstOrDefault(p => p.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
                var pair2 = pair1.Value?.FirstOrDefault(p => p.Key.Equals(version, StringComparison.OrdinalIgnoreCase));

                return pair2?.Value;
            }

        }

        private void PutSet(Dictionary<string, Dictionary<string, Badge>> map, TwitchBadgeSet set)
        {
            foreach (var pair1 in set.Set)
            {
                var name = pair1.Key;
                var map2 = map[name] = new Dictionary<string, Badge>();

                foreach (var pair in pair1.Value)
                {
                    var version = pair.Key;
                    var tbadge = pair.Value;

                    var badge = map2[version] = new Badge();
                    badge.Path = this.SelectPath(tbadge);
                }

            }

        }

        private string SelectPath(TwitchBadge tbadge)
        {
            var url = tbadge.ImageUrl1x;
            var lastIndex = url.LastIndexOf('/');

            return url.Substring(0, lastIndex + 1);
        }

        public void Reload()
        {
            new Thread(() =>
            {
                try
                {
                    var config = Program.Instance.Configuration.Value;

                    var api = this.API;
                    api.ClientId = config.ClientId;
                    var user = api.Users.GetUser(new UserRequest(UserType.Login, config.TwitchChannelName));

                    if (user != null)
                    {
                        var globalSet = api.Badges.GetGlobalBadges();
                        var channelSet = api.Badges.GetChannelBadges(user.Id);

                        var badges = this.Badges;

                        lock (badges)
                        {
                            badges.Clear();
                            this.PutSet(badges, globalSet);
                            this.PutSet(badges, channelSet);
                        }

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }).Start();
        }

    }

}