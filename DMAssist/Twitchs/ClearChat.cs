using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DMAssist.Twitchs
{
    public class ClearChat : WrappedCommand
    {
        public string User { get; set; }
        public int? BanDuration { get; set; }

        public override string Type => "ClearChat";

        public override void Read(JToken token)
        {
            base.Read(token);
        }

        public override void Write(JToken token)
        {
            base.Write(token);

            token["User"] = this.User;
            token["BanDuration"] = this.BanDuration;
        }

    }

}
