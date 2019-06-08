using DMAssist.Twitchs;
using Giselle.Commons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMAssist.Forms
{
    public class MainForm : OptimizedForm
    {
        private Icon _Icon;

        private StateGroupBox StateGroupBox;
        private ConfigGroupBox ConfigGroupBox;
        private ThemesGroupBox ThemesGroupBox;
        private RecentChatView RecentChatView;

        public MainForm()
        {
            this.SuspendLayout();

            this.Text = "Daengmin Assist";
            this.Icon = this._Icon = Icon.FromHandle(Properties.Resources.Icon.GetHicon());
            this.MaximizeBox = false;

            var stateGroupBox = this.StateGroupBox = new StateGroupBox();
            this.Controls.Add(stateGroupBox);

            var configGroupBox = this.ConfigGroupBox = new ConfigGroupBox();
            this.Controls.Add(configGroupBox);

            var themesGroupBox = this.ThemesGroupBox = new ThemesGroupBox();
            this.Controls.Add(themesGroupBox);

            var recentChatView = this.RecentChatView = new RecentChatView();
            this.Controls.Add(recentChatView);

            this.ResumeLayout(false);

            this.ClientSize = this.GetPreferredSize(new Size(800, 0));
            Program.Instance.TwitchChatHandler.HandlePrivateMessage += this.OnTwitchChatManagerPrivateMessage;
        }

        private void OnTwitchChatManagerPrivateMessage(object sender, PrivateMessage _e)
        {
            this.BeginInvoke(new Action<PrivateMessage>((e) =>
            {
                this.RecentChatView.Add(e);
            }), _e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (this.WindowState == FormWindowState.Normal)
            {
                var clientSize = this.ClientSize;
                var packSize = this.GetPreferredSize(new Size(800, 0));

                if (clientSize.Height != packSize.Height)
                {
                    this.ClientSize = new Size(clientSize.Width, packSize.Height);
                }

            }

        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            this.ShowInTaskbar = this.Visible;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            var reason = e.CloseReason;

            if (reason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }

        }

        protected override Dictionary<Control, Rectangle> GetPreferredBounds(Rectangle layoutBounds)
        {
            var map = base.GetPreferredBounds(layoutBounds);

            var proposedSize = new Size(348, 106);

            var configGroupBox = this.StateGroupBox;
            var configGroupBoxBounds = map[configGroupBox] = new Rectangle(layoutBounds.Location, configGroupBox.GetPreferredSize(proposedSize));

            var stateGroupBox = this.ConfigGroupBox;
            var stateGroupBoxLocation = new Point(configGroupBoxBounds.Left, configGroupBoxBounds.Bottom + 5);
            var stateGroupBoxBounds = map[stateGroupBox] = new Rectangle(stateGroupBoxLocation, stateGroupBox.GetPreferredSize(proposedSize));

            var themesGroupBox = this.ThemesGroupBox;
            var themesGroupBoxLocation = new Point(stateGroupBoxBounds.Left, stateGroupBoxBounds.Bottom + 5);
            var themesGroupBoxBounds = map[themesGroupBox] = new Rectangle(themesGroupBoxLocation, themesGroupBox.GetPreferredSize(proposedSize));

            var recentChatView = this.RecentChatView;
            var recentChatViewLeft = configGroupBoxBounds.Right + 10;
            map[recentChatView] = Rectangle.FromLTRB(recentChatViewLeft, configGroupBoxBounds.Top, layoutBounds.Right, configGroupBoxBounds.Top + 370);

            return map;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            ObjectUtils.DisposeQuietly(this._Icon);
        }

    }

}
