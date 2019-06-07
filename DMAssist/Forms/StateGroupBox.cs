using DMAssist.Twitchs;
using DMAssist.WebServers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMAssist.Forms
{
    public class StateGroupBox : OptimizedGroupBox
    {
        private Label StateLabel;

        public StateGroupBox()
        {
            this.SuspendLayout();

            this.Text = "상태";

            this.Controls.Add(this.StateLabel = new Label());

            this.ResumeLayout(false);

            Program.Instance.UiUpdate += this.OnUiUpdate;
        }

        protected override Dictionary<Control, Rectangle> GetPreferredBounds(Rectangle layoutBounds)
        {
            var map = base.GetPreferredBounds(layoutBounds);
            var lastLocation = layoutBounds.Location;

            foreach (var label in this.Controls.OfType<Label>())
            {
                var location = lastLocation;
                var size = new Size(layoutBounds.Width, 110);
                var bounds = map[label] = new Rectangle(location, size);

                lastLocation = new Point(bounds.Left, bounds.Bottom);
            }

            return map;
        }

        private string ToStringState(WebServerState state)
        {
            var map = new Dictionary<WebServerState, string>();
            map[WebServerState.Stopped] = "정지 됨";
            map[WebServerState.Starting] = "시작 중";
            map[WebServerState.Started] = "시작 됨";
            map[WebServerState.Stopping] = "정지 중";

            return map.TryGetValue(state, out var text) ? text : null;
        }

        private string ToStringState(TwitchManagerState state)
        {
            var map = new Dictionary<TwitchManagerState, string>();
            map[TwitchManagerState.Stopped] = "정지 됨";
            map[TwitchManagerState.Starting] = "시작 중";
            map[TwitchManagerState.Connected] = "연결 됨";
            map[TwitchManagerState.Diconnected] = "연결 끊어짐";
            map[TwitchManagerState.Stopping] = "정지 중";

            return map.TryGetValue(state, out var text) ? text : null;
        }

        private string ToStringState(JoinState state)
        {
            var map = new Dictionary<JoinState, string>();
            map[JoinState.Parted] = "퇴장";
            map[JoinState.Joining] = "입장 중(및 존재하지 않는 채널)";
            map[JoinState.Joined] = "입장";
            map[JoinState.Parting] = "퇴장 중";

            return map.TryGetValue(state, out var text) ? text : null;
        }

        private void OnUiUpdate(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                var program = Program.Instance;
                var lines = new List<string>();

                var dcm = program.DCConManager;
                lines.Add($"디씨콘 개수 : {dcm.Values.Length}");

                var tcm = program.TwitchChatManager;
                lines.Add($"채팅 서버 : {this.ToStringState(tcm.State)}");
                lines.Add($"채팅 채널 : {this.ToStringState(tcm.JoinState)}");

                var wsm = program.WebServerManager;
                var sessionCount = wsm.GetSessions()?.Length;
                lines.Add($"웹 서버 상태 : {this.ToStringState(wsm.State)}");

                if (sessionCount.HasValue == true)
                {
                    lines.Add($"웹 서버 세션 수 : {sessionCount.Value}");
                }

                this.StateLabel.Text = string.Join(Environment.NewLine, lines);
            }

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Program.Instance.UiUpdate -= this.OnUiUpdate;
        }

    }

}
