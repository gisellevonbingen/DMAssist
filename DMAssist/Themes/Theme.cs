using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Themes
{
    public class Theme
    {
        public string ConfigFilePath { get; }

        public string Name { get; set; }

        public Theme(string configFilePath)
        {
            this.ConfigFilePath = configFilePath;
        }

        public void Read(JToken token)
        {
            this.Name = token.Value<string>("Name");
        }

    }

}
