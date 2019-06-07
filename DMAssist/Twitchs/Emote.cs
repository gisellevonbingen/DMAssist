using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Twitchs
{
    public struct Emote
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public string Id { get; set; }

        public Emote(int startIndex, int count, string id)
            :this()
        {
            this.StartIndex = startIndex;
            this.Count = count;
            this.Id = id;
        }

    }

}
