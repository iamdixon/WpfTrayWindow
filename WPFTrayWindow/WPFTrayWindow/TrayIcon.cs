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

        #region Constructors
        public TrayIcon()
        {
            // create a new instance of a NotifyIcon to deal with the tray icon interactions
            trayicon = new System.Windows.Forms.NotifyIcon();

            

            // TODO: use a transparent icon file, so that we don't see the default icon during init.
            trayicon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/WPFTrayWindow;component/Resources/blank.ico")).Stream,new System.Drawing.Size(16,16));
            // set the default state of the trayicon to visible
            trayicon.Visible = true;

            // create a click handler to deal with left click actions
            trayicon.MouseClick += Trayicon_Click;
        }
        #endregion

        #region Properties
        // expose the icon property so that it can be updated by the window properties
        /// <summary>
        /// Gets or sets the Icon used for the TrayIcon
        /// </summary>
        public System.Drawing.Icon Icon
        {
            get { return trayicon.Icon; }
            set
            {
                trayicon.Icon = value;
            }
        }

        // expose the tray icon visibility property so that it can be controlled by the window
        /// <summary>
        /// Gets or sets the visibility of the TrayIcon
        /// </summary>
        public bool Visible
        {
            get { return trayicon.Visible; }
            set { trayicon.Visible = value; }
        }


        #endregion

        #region Events
        // public delegate to allow setting of Click event handlers from the window
        /// <summary>
        /// Event action triggered on the click event of the TrayIcon
        /// </summary>
        public event Action Invoked = delegate { };
        // trigger the Invoked delegate from the notify icon click event
        private void Trayicon_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            
            // only if its a left click - everything else we go to default except context (right click)
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Invoked.Invoke();
            }
        }


        public System.Drawing.Rectangle GetNotificationIconRectangle()
        {
            return TaskBar.GetNotificationIconRectangle(trayicon);
        }


        // make disposable so that tray icon disposition can be triggered
        /// <summary>
        /// Properly dispose of the TrayIcon
        /// </summary>
        public void Dispose()
        {
            // hide the trayicon before disposal or we end up with ghost icons in the tray.
            trayicon.Visible = false;
            trayicon.Dispose();
        }


        #endregion
    }
}
