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

        private TwitchChannelGroupBox TwitchGroupBox;
        private ThemesGroupBox ThemesGroupBox;
        private NetworkGroupBox NetworkGroupBox;
        private RecentChatView RecentChatView;

        public MainForm()
        {
            this.SuspendLayout();

            this.Text = "Daengmin Assist";
            this.Icon = this._Icon = Icon.FromHandle(Properties.Resources.Icon.GetHicon());
            this.MaximizeBox = false;

            var twitchGroupBox = this.TwitchGroupBox = new TwitchChannelGroupBox();
            this.Controls.Add(twitchGroupBox);

            var themesGroupBox = this.ThemesGroupBox = new ThemesGroupBox();
            this.Controls.Add(themesGroupBox);

            var networkGroupBox = this.NetworkGroupBox = new NetworkGroupBox();
            this.Controls.Add(networkGroupBox);

            var recentChatView = this.RecentChatView = new RecentChatView();
            this.Controls.Add(recentChatView);

            this.ResumeLayout(false);

            this.ClientSize = this.GetPreferredSize(new Size(800, 0));
            Program.Instance.TwitchChatManager.PrivateMessage += this.OnTwitchChatManagerPrivateMessage;
        }

        private void OnTwitchChatManagerPrivateMessage(object sender, PrivateMessageEventArgs _e)
        {
            this.BeginInvoke(new Action<PrivateMessageEventArgs>((e)=>
            {
                this.RecentChatView.Add(e);
            }), _e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            var clientSize = this.ClientSize;
            var packSize = this.GetPreferredSize(new Size(800, 0));

            if (clientSize.Height != packSize.Height)
            {
                this.ClientSize = new Size(clientSize.Width, packSize.Height);
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

            var proposedSize = new Size(330, 106);

            var twitchGroupBox = this.TwitchGroupBox;
            var twitchGroupBoxBounds = map[twitchGroupBox] = new Rectangle(layoutBounds.Location, twitchGroupBox.GetPreferredSize(proposedSize));

            var themesGroupBox = this.ThemesGroupBox;
            var themesGroupBoxLocation = new Point(twitchGroupBoxBounds.Left, twitchGroupBoxBounds.Bottom + 10);
            var themesGroupBoxBounds = map[themesGroupBox] = new Rectangle(themesGroupBoxLocation, themesGroupBox.GetPreferredSize(proposedSize));

            var networkGroupBox = this.NetworkGroupBox;
            var networkGroupBoxLocation = new Point(themesGroupBoxBounds.Left, themesGroupBoxBounds.Bottom + 10);
            var networkGroupBounds = map[networkGroupBox] = new Rectangle(networkGroupBoxLocation, networkGroupBox.GetPreferredSize(proposedSize));

            var recentChatView = this.RecentChatView;
            var recentChatViewLeft = twitchGroupBoxBounds.Right + 10;
            map[recentChatView] = Rectangle.FromLTRB(recentChatViewLeft, twitchGroupBoxBounds.Top, layoutBounds.Right, twitchGroupBoxBounds.Top + 347);

            return map;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            ObjectUtils.DisposeQuietly(this._Icon);
        }

    }

}
