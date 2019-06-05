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
        public string Directory { get; }
        public string ConfigFilePath { get; }

        public string Name { get; set; }
        public string Page { get; set; }

        public Theme(string directory, string configFilePath)
        {
            this.Directory = directory;
            this.ConfigFilePath = configFilePath;
        }

        public void Read(JToken token)
        {
            this.Name = token.Value<string>("Name");
            this.Page = token.Value<string>("Page");
        }

    }

}
