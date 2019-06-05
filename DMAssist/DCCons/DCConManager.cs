using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchAPIs;
using TwitchAPIs.Web;

namespace DMAssist.DCCons
{
    public class DCConManager
    {
        private List<DCCon> Cons { get; }

        public DCConManager()
        {
            this.Cons = new List<DCCon>();
        }

        public DCCon[] Values
        {
            get
            {
                lock (this.Cons)
                {
                    return this.Cons.ToArray();
                }

            }

        }

        public void Reload()
        {
            new Thread(() =>
            {
                var request = new RequestParameter();
                request.Method = "GET";
                request.URL = Program.Instance.Configuration.Value.DCConURL;

                var we = new WebExplorer();

                using (var response = we.Request(request))
                {
                    var token = response.ReadAsJSON();
                    var cons = token.ReadArray("dccons", t => { var con = new DCCon(); con.Read(t); return con; });

                    lock (this.Cons)
                    {
                        this.Cons.Clear();
                        this.Cons.AddRange(cons);
                    }

                }

            }).Start();
        }

    }

}
