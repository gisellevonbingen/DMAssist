using DMAssist.Utils;
using Giselle.Commons;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist.Themes
{
    public class ThemeManager : IDisposable
    {
        public string Directory { get; }

        private List<Theme> Themes { get; }
        private FileSystemWatcher Watcher { get; }
        public bool Disposed { get; private set; }

        public event EventHandler<ThemeConfigChangedEventArgs> ConfigChanged;

        public ThemeManager(string directory)
        {
            this.Directory = directory;

            this.Themes = new List<Theme>();
            var watcher = this.Watcher = new FileSystemWatcher(directory);
            watcher.Created += this.OnConfigFileChanged;
            watcher.Changed += this.OnConfigFileChanged;
            watcher.EnableRaisingEvents = true;

            this.Disposed = false;
            Directory = directory;
        }

        public Theme[] Values
        {
            get
            {
                lock (this.Themes)
                {
                    return this.Themes.ToArray();
                }

            }

        }


        public void LoadAll()
        {
            this.EnsureDisposed();

            var configFiles = System.IO.Directory.GetFiles(this.Directory, "*.json");

            foreach (var configFile in configFiles)
            {
                this.Load(configFile);
            }

            this.Themes.Sort((o1, o2) => o1.Name.CompareTo(o2.Name));
        }

        public void Load(string configFile)
        {
            this.EnsureDisposed();

            var json = File.ReadAllText(configFile);
            var jobj = JObject.Parse(json);

            var theme = new Theme(configFile);
            theme.Read(jobj);

            lock (this.Themes)
            {
                this.Themes.Add(theme);
            }

        }

        public void EnsureDisposed()
        {
            if (this.Disposed == true)
            {
                throw new ObjectDisposedException(nameof(ThemeManager));
            }

        }

        private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            var filePath = PathUtils.Normalize(e.FullPath);
            var theme = this.Values.FirstOrDefault(t => t.ConfigFilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

            if (theme != null)
            {
                this.OnConfigChanged(new ThemeConfigChangedEventArgs(theme));
            }

        }

        protected virtual void OnConfigChanged(ThemeConfigChangedEventArgs e)
        {
            this.ConfigChanged?.Invoke(this, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Disposed = true;
            ObjectUtils.DisposeQuietly(this.Watcher);
        }

        ~ThemeManager()
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
