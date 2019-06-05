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
        private List<Theme> Themes { get; }
        private List<FileSystemWatcher> Watchers { get; }
        public bool Disposed { get; private set; }

        public event EventHandler<ThemeConfigChangedEventArgs> ConfigChanged;

        public ThemeManager()
        {
            this.Themes = new List<Theme>();
            this.Watchers = new List<FileSystemWatcher>();
            this.Disposed = false;
        }

        public Theme[] Values
        {
            get
            {
                return this.Themes.ToArray();
            }

        }

        public void LoadDirectory(string baseDirectory)
        {
            this.EnsureDisposed();

            var directories = Directory.GetDirectories(baseDirectory);

            foreach (var directoryPath in directories)
            {
                var configFilePath = PathUtils.Normalize(Path.Combine(directoryPath, "dmaconfig.json"));
                this.Load(directoryPath, configFilePath);

            }

            this.Themes.Sort((o1, o2) => o1.Name.CompareTo(o2.Name));
        }

        public void Load(string directoryPath, string configFilePath)
        {
            this.EnsureDisposed();

            if (File.Exists(configFilePath) == false)
            {
                return;
            }

            FileSystemWatcher watcher = null;

            try
            {
                var json = File.ReadAllText(configFilePath);
                var jobj = JObject.Parse(json);

                var theme = new Theme(directoryPath, configFilePath);
                theme.Read(jobj);

                lock (this.Themes)
                {
                    this.Themes.Add(theme);
                }

                watcher = new FileSystemWatcher(directoryPath);
                //watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite;
                watcher.Created += this.OnConfigFileChanged;
                watcher.Changed += this.OnConfigFileChanged;
                watcher.EnableRaisingEvents = true;

                lock (this.Watchers)
                {
                    this.Watchers.Add(watcher);
                }

            }
            catch (Exception e)
            {
                ObjectUtils.DisposeQuietly(watcher);
                Console.WriteLine(e);
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

            lock (this.Themes)
            {
                var theme = this.Themes.FirstOrDefault(t => t.ConfigFilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

                if (theme != null)
                {
                    this.OnConfigChanged(new ThemeConfigChangedEventArgs(theme));
                }

            }

        }

        protected virtual void OnConfigChanged(ThemeConfigChangedEventArgs e)
        {
            this.ConfigChanged?.Invoke(this, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (this.Watchers)
            {
                this.Disposed = true;

                foreach (var watcher in this.Watchers)
                {
                    ObjectUtils.DisposeQuietly(watcher);
                }

            }

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
