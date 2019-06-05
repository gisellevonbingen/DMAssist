using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist
{
    public class ConfigurationManager
    {
        public string Path { get; }
        public Configuration Value { get; }

        public ConfigurationManager(string path)
        {
            this.Path = path;
            this.Value = new Configuration();
        }

        public void Load()
        {
            if (File.Exists(this.Path) == true)
            {
                var json = File.ReadAllText(this.Path);
                var token = JObject.Parse(json);

                var value = this.Value;
                value.Read(token);
            }

            this.Save();
        }

        public void Save()
        {
            var token = new JObject();
            this.Value.Write(token);

            File.WriteAllText(this.Path, token.ToString(Newtonsoft.Json.Formatting.Indented));
        }

    }

}
