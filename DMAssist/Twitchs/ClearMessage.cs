using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DMAssist.Twitchs
{
    public class ClearMessage : WrappedCommand
    {
        public string Login { get; set; }
        public string Message { get; set; }
        public string TargetMessageid { get; set; }

        public override string Type => "ClearMessage";

        public override void Read(JToken token)
        {
            base.Read(token);
        }

        public override void Write(JToken token)
        {
            base.Write(token);

            token["Login"] = this.Login;
            token["Message"] = this.Message;
            token["TargetMessageid"] = this.TargetMessageid;
        }

    }

}
