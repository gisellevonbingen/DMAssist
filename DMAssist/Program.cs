using DMAssist.DCCons;
using DMAssist.Forms;
using DMAssist.Properties;
using DMAssist.Resources;
using DMAssist.Themes;
using DMAssist.Twitchs;
using DMAssist.WebServers;
using Giselle.Commons;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMAssist
{
    public class Program : IDisposable
    {
        public static Program Instance { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            using (var instance = Instance = new Program())
            {
                instance.Run();
            }

        }

        public ConfigurationManager Configuration { get; }
        public ThemeManager ThemeManager { get; }
        public DCConManager DCConManager { get; }
        public FontManager FontManager { get; }
        public NotifyIconManager NotifyIconManager { get; }
        public TwitchChatManager TwitchChatManager { get; }
        public WebServerManager WebServerManager { get; }
        public MainForm MainForm { get; private set; }

        public bool Disposed { get; private set; }

        private System.Windows.Forms.Timer Timer;

        public event EventHandler MainFormVisibleChanged;
        public event EventHandler UiUpdate;

        public Program()
        {
            Console.CancelKeyPress += this.OnCancelKeyPress;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            this.Configuration = new ConfigurationManager(Path.Combine(Application.StartupPath, "Config.json"));
            this.ThemeManager = new ThemeManager();
            this.DCConManager = new DCConManager();
            this.FontManager = new FontManager();
            this.NotifyIconManager = new NotifyIconManager(this);
            this.TwitchChatManager = new TwitchChatManager();
            this.WebServerManager = new WebServerManager();
            this.MainForm = null;

            this.Disposed = false;
        }

        private void Run()
        {
            try
            {
                this.Configuration.Load();
                this.ThemeManager.LoadDirectory(Application.StartupPath + "/themes");

                this.DCConManager.Reload();

                this.TwitchChatManager.Start();
                this.TwitchChatManager.AddActivity(new ActivityChangeChannel(this.Configuration.Value.TwitchChannelName));

                this.WebServerManager.Start();

                var context = new ApplicationContext();

                using (var mainForm = new MainForm())
                {
                    var timer = this.Timer = new System.Windows.Forms.Timer();
                    timer.Interval = 50;
                    timer.Tick += this.OnTimerTick;
                    timer.Start();

                    context.MainForm = mainForm;
                    this.MainForm = mainForm;

                    mainForm.VisibleChanged += (sender, e) => this.OnMainFormVisibleChanged(e);
                    Application.Run(context);
                }

            }
            catch (Exception e)
            {
                var file = this.DumpCrashMessage(e);

                using (var form = new CrashReportForm(new CrashReport(file, e)))
                {
                    form.ShowDialog();
                }

            }

        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            this.UiUpdate?.Invoke(this, e);
        }

        protected virtual void OnMainFormVisibleChanged(EventArgs e)
        {
            this.MainFormVisibleChanged?.Invoke(this, e);
        }

        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            this.Dispose();
        }

        public void ShowCrashMessageBox(Exception exception)
        {
            var file = this.DumpCrashMessage(exception);
            var report = new CrashReport(file, exception);

            this.MainForm.Invoke(new Action<CrashReport>((CrashReport arg) =>
            {
                using (var form = new CrashReportForm(arg))
                {
                    form.ShowDialog(this.MainForm);
                }

            }), report);

        }

        public string GetCrashReportsDirectory()
        {
            var directory = Path.Combine(Application.StartupPath, "CrashReports");
            Directory.CreateDirectory(directory);

            return directory;
        }

        private string DumpCrashMessage(Exception exception)
        {
            try
            {
                Console.WriteLine(exception);
                var directory = this.GetCrashReportsDirectory();

                var dateTime = DateTime.Now;
                var file = Path.Combine(directory, dateTime.ToString("yyyy_MM_dd HH_mm_ss_fff") + ".log");

                File.WriteAllText(file, string.Concat(exception));

                return file;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            ObjectUtils.DisposeQuietly(this.WebServerManager);
            ObjectUtils.DisposeQuietly(this.TwitchChatManager);
            ObjectUtils.DisposeQuietly(this.ThemeManager);
            ObjectUtils.DisposeQuietly(this.MainForm);
            ObjectUtils.DisposeQuietly(this.NotifyIconManager);
            ObjectUtils.DisposeQuietly(this.FontManager);

            this.Disposed = true;
        }

        ~Program()
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
