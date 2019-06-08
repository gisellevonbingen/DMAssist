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
        private Button OpenDirectoryFileButton;

        public ThemesGroupBox()
        {
            this.SuspendLayout();

            this.Text = "테마";
            var themes = Program.Instance.ThemeManager.Values;

            var comboBox = this.ComboBox = new ComboBox();
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(comboBox);

            var openDirectoryFileButton = this.OpenDirectoryFileButton = new Button();
            openDirectoryFileButton.Text = "폴더 열기";
            openDirectoryFileButton.Click += this.OnOpenDirectoryFileButtonClick;
            this.Controls.Add(openDirectoryFileButton);

            if (themes.Length > 0)
            {
                foreach (var theme in themes)
                {
                    comboBox.Items.Add(new ThemeItem(theme));
                }

                comboBox.SelectedIndex = 0;
            }

            this.ResumeLayout(false);
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
            var openDirectoryFileButton = this.OpenDirectoryFileButton;

            var openDirectoryFileButtonSize = new Size(100, 29);
            var openDirectoryFileButtonLocation = new Point(layoutBounds.Right - openDirectoryFileButtonSize.Width, layoutBounds.Top);
            var openDirectoryFileButtonBounds = map[openDirectoryFileButton] = new Rectangle(openDirectoryFileButtonLocation, openDirectoryFileButtonSize);

            var comboBoxLocation = new Point(layoutBounds.Left, layoutBounds.Top);
            var comboBoxSize = new Size(openDirectoryFileButtonBounds.Left - 10 - comboBoxLocation.X, openDirectoryFileButtonSize.Height);
            var comboBoxBounds = map[comboBox] = new Rectangle(comboBoxLocation, comboBoxSize);

            return map;
        }

    }

}
