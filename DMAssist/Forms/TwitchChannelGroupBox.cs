using DMAssist.Twitchs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMAssist.Forms
{
    public class TwitchChannelGroupBox : OptimizedGroupBox
    {
        private TextBox ChannelNameTextBox;
        private Button ApplyButton;
        private Label StatusLabel;

        public TwitchChannelGroupBox()
        {
            this.SuspendLayout();

            this.Text = "트위치 채널";

            var channelNameTextBox = this.ChannelNameTextBox = new TextBox();
            channelNameTextBox.Text = Program.Instance.Settings.TwitchChannel;
            this.Controls.Add(channelNameTextBox);

            var applyButton = this.ApplyButton = new Button();
            applyButton.Text = "적용";
            applyButton.Click += this.OnApplyButtonClick;
            this.Controls.Add(applyButton);

            var statusLabel = this.StatusLabel = new Label();
            statusLabel.Text = "상태 :";
            this.Controls.Add(statusLabel);

            this.ResumeLayout(false);

            Program.Instance.UiUpdate += this.OnUiUpdate;
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
            map[JoinState.Joining] = "입장 중(혹은 존재하지 않는 채널)";
            map[JoinState.Joined] = "입장";
            map[JoinState.Parting] = "퇴장 중";

            return map.TryGetValue(state, out var text) ? text : null;
        }

        private void OnUiUpdate(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                var tcm = Program.Instance.TwitchChatManager;
                this.StatusLabel.Text = $"서버 : {this.ToStringState(tcm.State)}{Environment.NewLine}채널 : {this.ToStringState(tcm.JoinState)}";
            }

        }

        private void OnApplyButtonClick(object sender, EventArgs e)
        {
            var settings = Program.Instance.Settings;
            settings.TwitchChannel = this.ChannelNameTextBox.Text;
            settings.Save();

            var tcm = Program.Instance.TwitchChatManager;
            tcm.AddActivity(new ActivityChangeChannel(settings.TwitchChannel));
        }

        protected override Dictionary<Control, Rectangle> GetPreferredBounds(Rectangle layoutBounds)
        {
            var map = base.GetPreferredBounds(layoutBounds);

            var channelNameTextBox = this.ChannelNameTextBox;
            var applyButton = this.ApplyButton;
            var applyButtonSize = new Size(100, channelNameTextBox.Height);
            var applyButtonBounds = map[applyButton] = new Rectangle(new Point(layoutBounds.Right - applyButtonSize.Width, layoutBounds.Top), applyButtonSize);

            var channelNameTextBoxLocation = new Point(layoutBounds.Left, layoutBounds.Top);
            var channelNameTextBoxSize = new Size(applyButtonBounds.Left - channelNameTextBoxLocation.X - 10, channelNameTextBox.Height);
            map[channelNameTextBox] = new Rectangle(channelNameTextBoxLocation, channelNameTextBoxSize);

            var statusLabel = this.StatusLabel;
            var statusLabelLeft = map[channelNameTextBox].Left;
            var statusLabelWidth = applyButtonBounds.Right - statusLabelLeft;
            map[statusLabel] = new Rectangle(statusLabelLeft, applyButtonBounds.Bottom + 5, statusLabelWidth, 45);

            return map;
        }

    }

}
