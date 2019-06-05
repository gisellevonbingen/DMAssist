using DMAssist.Themes;
using DMAssist.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMAssist.Forms
{
    public class ThemesGroupBox : OptimizedGroupBox
    {
        private ComboBox ComboBox;
        private TextBox WidgetURLTextBox;
        private Button OpenDirectoryFileButton;
        private Button CopyWidgetURLButton;

        public ThemesGroupBox()
        {
            this.SuspendLayout();

            this.Text = "테마";
            var themes = Program.Instance.ThemeManager.Values;

            var comboBox = this.ComboBox = new ComboBox();
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.SelectedIndexChanged += this.OnComboBoxSelectedIndexChanged;

            this.Controls.Add(comboBox);

            var widgetURLTextBox = this.WidgetURLTextBox = new TextBox();
            widgetURLTextBox.ReadOnly = true;
            this.Controls.Add(widgetURLTextBox);

            var openDirectoryFileButton = this.OpenDirectoryFileButton = new Button();
            openDirectoryFileButton.Text = "폴더 열기";
            openDirectoryFileButton.Click += this.OnOpenDirectoryFileButtonClick;
            this.Controls.Add(openDirectoryFileButton);

            var copyWidgetURLButton = this.CopyWidgetURLButton = new Button();
            copyWidgetURLButton.Click += this.OnCopyWidgeURLButtonClick;
            this.Controls.Add(copyWidgetURLButton);

            if (themes.Length > 0)
            {
                foreach (var theme in themes)
                {
                    comboBox.Items.Add(new ThemeItem(theme));
                }

                comboBox.SelectedIndex = 0;
            }

            this.ResetCopbyWidgeURLButtonText();

            this.ResumeLayout(false);
        }

        private void OnCopyWidgeURLButtonClick(object sender, EventArgs e)
        {
            var copyWidgetURLButton = this.CopyWidgetURLButton;
            copyWidgetURLButton.Text = "복사 됨";

            Clipboard.SetText(this.WidgetURLTextBox.Text, TextDataFormat.Text);
        }

        private void OnComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.ResetCopbyWidgeURLButtonText();

            var theme = this.GetSelectedTheme();

            if (theme != null)
            {
                this.WidgetURLTextBox.Text = Path.Combine(theme.Directory, theme.Page);
            }
            else
            {
                this.WidgetURLTextBox.Text = string.Empty;
            }

        }

        private void ResetCopbyWidgeURLButtonText()
        {
            this.CopyWidgetURLButton.Text = "복사";
        }

        private Theme GetSelectedTheme()
        {
            return (this.ComboBox.SelectedItem as ThemeItem)?.Value;
        }

        private void OnOpenDirectoryFileButtonClick(object sender, EventArgs e)
        {
            var theme = this.GetSelectedTheme();

            if (theme != null)
            {
                ExplorerUtils.Open(theme.ConfigFilePath);
            }

        }

        protected override Dictionary<Control, Rectangle> GetPreferredBounds(Rectangle layoutBounds)
        {
            var map = base.GetPreferredBounds(layoutBounds);

            var comboBox = this.ComboBox;
            var widgetURLTextBox = this.WidgetURLTextBox;
            var openDirectoryFileButton = this.OpenDirectoryFileButton;
            var copyWidgetURLButton = this.CopyWidgetURLButton;

            var openDirectoryFileButtonSize = new Size(100, widgetURLTextBox.Height);
            var openDirectoryFileButtonLocation = new Point(layoutBounds.Right - openDirectoryFileButtonSize.Width, layoutBounds.Top);
            var openDirectoryFileButtonBounds = map[openDirectoryFileButton] = new Rectangle(openDirectoryFileButtonLocation, openDirectoryFileButtonSize);

            var comboBoxLocation = new Point(layoutBounds.Left, layoutBounds.Top);
            var comboBoxSize = new Size(openDirectoryFileButtonBounds.Left - 10 - comboBoxLocation.X, widgetURLTextBox.Height);
            var comboBoxBounds = map[comboBox] = new Rectangle(comboBoxLocation, comboBoxSize);

            var widgetURLTextBoxLocation = new Point(comboBoxBounds.Left, comboBoxBounds.Bottom + 10);
            var widgetURLTextBoxSize = new Size(comboBoxBounds.Width, widgetURLTextBox.Height);
            var widgetURLTextBoxBounds = map[widgetURLTextBox] = new Rectangle(widgetURLTextBoxLocation, widgetURLTextBoxSize);

            var copyWidgetURLButtonSize = openDirectoryFileButtonSize;
            var copyWidgetURLButtonLocation = new Point(openDirectoryFileButtonBounds.Left, widgetURLTextBoxBounds.Top);
            var copyWidgetURLButtonBounds = map[copyWidgetURLButton] = new Rectangle(copyWidgetURLButtonLocation, copyWidgetURLButtonSize);

            return map;
        }

    }

}
