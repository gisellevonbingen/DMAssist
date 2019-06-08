using DMAssist.Twitchs;
using DMAssist.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMAssist.Forms
{
    public class RecentChatView : DataGridView
    {
        private DataGridViewColumn NameColumn;
        private DataGridViewColumn TextColumn;

        public RecentChatView()
        {
            this.SuspendLayout();

            this.DoubleBuffered = true;
            this.ReadOnly = true;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToOrderColumns = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;
            this.RowHeadersVisible = false;

            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            this.Font = Program.Instance.FontManager[9.0F, FontStyle.Regular];

            var columns = this.Columns;

            var nameColumn = this.NameColumn = new DataGridViewTextBoxColumn();
            nameColumn.Name = "Name";
            nameColumn.HeaderText = "닉네임(아이디)";
            nameColumn.Width = 160;
            nameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            columns.Add(nameColumn);

            var textColumn = this.TextColumn = new DataGridViewTextBoxColumn();
            textColumn.Name = "Text";
            textColumn.HeaderText = "내용";
            textColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            columns.Add(textColumn);

            this.ResumeLayout(false);
        }

        public void Add(PrivateMessage message)
        {
            this.Add(message.DisplayName, string.Join("", message.Components.Select(c => c.ToSimpleString())));
        }

        public void Add(string displayName, string text)
        {
            var rows = this.Rows;

            if (rows.Count > 14)
            {
                rows.RemoveAt(0);
            }

            var rowIndex = rows.Add();
            var row = rows[rowIndex];
            var cells = row.Cells;
            cells[this.NameColumn.Name].Value = displayName;
            cells[this.TextColumn.Name].Value = text;
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            this.ClearSelection();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var size = this.ClientSize;
            this.TextColumn.Width = size.Width - 3 - this.NameColumn.Width;
        }

    }

}
