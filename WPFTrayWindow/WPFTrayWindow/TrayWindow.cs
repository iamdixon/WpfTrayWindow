using System;
using System.Windows;


namespace avalonprojects.wpf.tray
{
    public class TrayWindow : Window
    {

        #region Private Fields
        private TrayIcon trayicon;
        #endregion

        #region Constructors
        public TrayWindow()
        {
            trayicon = new TrayIcon();
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
            Application.Current.Exit += ApplicationShutdown;
        }


        #endregion

        #region Events
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            trayicon.Visible = false;
            trayicon.Dispose();
        }

        private void ApplicationShutdown(object sender, ExitEventArgs e)
        {
            base.OnClosed(e);
            trayicon.Visible = false;
            trayicon.Dispose();
        }
        #endregion
    }
}
