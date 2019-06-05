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
    public class NetworkGroupBox : OptimizedGroupBox
    {
        private LabeledTextBox WebSocketServerControl;
        private Button ApplyButton;
        private Label StatusLabel;

        public NetworkGroupBox()
        {
            this.SuspendLayout();

            this.Text = "네트워크";

            var webSocketServerControl = this.WebSocketServerControl = new LabeledTextBox();
            webSocketServerControl.Label.Text = "WebSocket 포트";
            webSocketServerControl.TextBox.Text = Program.Instance.Settings.WebSocketPort.ToString();
            this.Controls.Add(webSocketServerControl);

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

        private void OnUiUpdate(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                var statusLabel = this.StatusLabel;
                statusLabel.Text = $"상태 : {Program.Instance.WebServerManager.State}";
            }

        }

        private void OnApplyButtonClick(object sender, EventArgs e)
        {
            var settings = Program.Instance.Settings;
            settings.WebSocketPort = NumberUtils.ToUShort(this.WebSocketServerControl.TextBox.Text);
            settings.Save();
        }

        protected override Dictionary<Control, Rectangle> GetPreferredBounds(Rectangle layoutBounds)
        {
            var map = base.GetPreferredBounds(layoutBounds);

            var webSocketServerControl = this.WebSocketServerControl;
            var applyButton = this.ApplyButton;
            var applyButtonSize = new Size(100, webSocketServerControl.TextBox.Height);
            var applyButtonBounds = map[applyButton] = new Rectangle(new Point(layoutBounds.Right - applyButtonSize.Width, layoutBounds.Top), applyButtonSize);

            var webSocketServerLocation = new Point(layoutBounds.Left, layoutBounds.Top);
            var webSocketServerSize = new Size(applyButtonBounds.Left - webSocketServerLocation.X - 10, webSocketServerControl.TextBox.Height);
            webSocketServerControl.Label.Width = 140;
            map[webSocketServerControl] = new Rectangle(webSocketServerLocation, webSocketServerSize);

            var statusLabel = this.StatusLabel;
            var statusLabelLeft = map[webSocketServerControl].Left;
            var statusLabelWidth = applyButtonBounds.Right - statusLabelLeft;
            map[statusLabel] = new Rectangle(statusLabelLeft, applyButtonBounds.Bottom + 10, statusLabelWidth, 30);

            return map;
        }

    }

}
