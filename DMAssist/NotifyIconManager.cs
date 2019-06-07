using Giselle.Commons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMAssist
{
    public class NotifyIconManager : IDisposable
    {
        public NotifyIcon Impl { get; }
        public Icon Icon { get; }

        public MenuItem MenuItemMainFormShow { get; }
        public MenuItem MenuItemMainFormHide { get; }
        public MenuItem MenuItemDispose { get; }

        public NotifyIconManager(Program program)
        {
            var impl = this.Impl = new NotifyIcon();
            impl.Icon = Icon.FromHandle(Properties.Resources.NotifyIcon.GetHicon());
            impl.DoubleClick += this.OnImplDoubleClick;

            var menu = impl.ContextMenu = new ContextMenu();

            var itemShow = this.MenuItemMainFormShow = menu.MenuItems.Add("창 표시");
            itemShow.Click += this.OnMenuItemMainFormShowClick;

            var itemHide = this.MenuItemMainFormHide = menu.MenuItems.Add("창 숨기기");
            itemHide.Click += this.OnMenuItemMainFormHideClick;

            var itemDispose = this.MenuItemDispose = menu.MenuItems.Add("프로그램 종료");
            itemDispose.Click += this.OnMenuItemDisposeClick;

            impl.Visible = true;

            program.MainFormVisibleChanged += this.OnMainFormVisibleChanged;
            this.UpdateItemState(false);
        }

        public void UpdateItemState(bool visible)
        {
            this.MenuItemMainFormShow.Enabled = visible == false;
            this.MenuItemMainFormHide.Enabled = visible == true;
        }

        private void OnMainFormVisibleChanged(object sender, EventArgs e)
        {
            var visible = Program.Instance?.MainForm?.Visible ?? false;
            this.UpdateItemState(visible);

            if (visible == false)
            {
                var impl = this.Impl;
                impl.ShowBalloonTip(1000, "DMAssist - " + this.MenuItemMainFormHide.Text, "프로그램은 계속 실행 중 입니다", ToolTipIcon.Info);
            }

        }

        private void OnMenuItemMainFormShowClick(object sender, EventArgs e)
        {
            Program.Instance.MainForm.Visible = true;
        }

        private void OnMenuItemMainFormHideClick(object sender, EventArgs e)
        {
            Program.Instance.MainForm.Visible = false;
        }

        private void OnMenuItemDisposeClick(object sender, EventArgs e)
        {
            Program.Instance.QueryQuit();
        }

        private void OnImplDoubleClick(object sender, EventArgs e)
        {
            Program.Instance.MainForm.Visible = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Impl.Visible = false;
            ObjectUtils.DisposeQuietly(this.Impl);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        ~NotifyIconManager()
        {
            this.Dispose(false);
        }

    }

}
