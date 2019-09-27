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
    public class ConfigGroupBox : OptimizedGroupBox
    {
        private LabeledTextBox ClientIdControl;
        private LabeledTextBox TwitchChannelNameControl;
        private LabeledTextBox DCConURLControl;
        private LabeledTextBox WebSocketServerControl;
        private LabeledTextBox ToonationWidgetControl;

        private Button ApplyButton;

        public ConfigGroupBox()
        {
            var value = Program.Instance.Configuration.Value;
            this.SuspendLayout();

            this.Text = "설정";

            var clientIdControl = this.ClientIdControl = new LabeledTextBox();
            clientIdControl.Label.Text = "Client-Id";
            clientIdControl.TextBox.Text = value.ClientId;
            this.Controls.Add(clientIdControl);

            var twitchChannelNameControl = this.TwitchChannelNameControl = new LabeledTextBox();
            twitchChannelNameControl.Label.Text = "트위치 채널";
            twitchChannelNameControl.TextBox.Text = value.TwitchChannelName;
            this.Controls.Add(twitchChannelNameControl);

            var dcConURLControl = this.DCConURLControl = new LabeledTextBox();
            dcConURLControl.Label.Text = "디씨콘 주소";
            dcConURLControl.TextBox.Text = value.DCConURL;
            this.Controls.Add(dcConURLControl);

            var webSocketServerControl = this.WebSocketServerControl = new LabeledTextBox();
            webSocketServerControl.Label.Text = "WebSocket 포트";
            webSocketServerControl.TextBox.Text = value.WebSocketPort.ToString();
            this.Controls.Add(webSocketServerControl);

            var toonationWidgetServerControl = this.ToonationWidgetControl = new LabeledTextBox();
            toonationWidgetServerControl.Label.Text = "투네이션 위젯 포트";
            toonationWidgetServerControl.TextBox.Text = value.ToonationWidgetPort.ToString();
            this.Controls.Add(toonationWidgetServerControl);

            var applyButton = this.ApplyButton = new Button();
            applyButton.Text = "적용";
            applyButton.Click += this.OnApplyButtonClick;
            this.Controls.Add(applyButton);

            this.ResumeLayout(false);
        }

        protected override Dictionary<Control, Rectangle> GetPreferredBounds(Rectangle layoutBounds)
        {
            var map = base.GetPreferredBounds(layoutBounds);

            var lastLocation = layoutBounds.Location;

            foreach (var control in this.Controls.OfType<LabeledTextBox>())
            {
                var location = lastLocation;
                var size = new Size(layoutBounds.Width, control.TextBox.Height);
                control.Label.Width = 140;
                var bounds = map[control] = new Rectangle(location, size);

                lastLocation = new Point(bounds.Left, bounds.Bottom + 5);
            }

            var applyButton = this.ApplyButton;
            var applyButtonSize = new Size(100, 30);
            var applyButtonBounds = map[applyButton] = new Rectangle(new Point(layoutBounds.Right - applyButtonSize.Width, lastLocation.Y), applyButtonSize);

            return map;
        }

        private void OnApplyButtonClick(object sender, EventArgs e)
        {
            var program = Program.Instance;

            var config = program.Configuration;
            config.Value.ClientId = this.ClientIdControl.TextBox.Text;
            config.Value.TwitchChannelName = this.TwitchChannelNameControl.TextBox.Text;
            config.Value.DCConURL = this.DCConURLControl.TextBox.Text;
            config.Value.WebSocketPort = NumberUtils.ToUShort(this.WebSocketServerControl.TextBox.Text);
            config.Value.ToonationWidgetPort = NumberUtils.ToUShort(this.ToonationWidgetControl.TextBox.Text);
            config.Save();

            program.DCConManager.Reload();
            program.BadgeManager.Reload();
            program.TwitchChatManager.AddActivity(new ActivityChangeChannel(config.Value.TwitchChannelName));
            program.WebServerManager.Start();
            program.ToonationManager.Start();
        }

    }

}
