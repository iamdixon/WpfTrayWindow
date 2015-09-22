using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avalonprojects.wpf.tray
{
    public class TrayIcon : IDisposable
    {
        #region Private Fields
        private System.Windows.Forms.NotifyIcon trayicon;
        #endregion

        public TrayIcon()
        {
            trayicon = new System.Windows.Forms.NotifyIcon();
            trayicon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/WPFTrayWindow;component/Resources/default.ico")).Stream,new System.Drawing.Size(16,16));
            trayicon.Visible = true;
            trayicon.MouseClick += Trayicon_Click;
        }

        private void Trayicon_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Invoked.Invoke();
            }
        }


        public bool Visible
        {
            get { return trayicon.Visible; }
            set { trayicon.Visible = value; }
        }

        public void Dispose()
        {
            trayicon.Visible = false;
            trayicon.Dispose();
        }

        public event Action Invoked = delegate { }; 

        
    }
}
