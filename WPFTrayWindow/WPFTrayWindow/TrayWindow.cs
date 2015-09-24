using System;
using System.IO;
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

            trayicon.Invoked += RaiseTrayIconClick;

            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
            Application.Current.Exit += ApplicationShutdown;
            this.Visibility = Visibility.Hidden;
        }



        private static readonly DependencyProperty PinToTrayProperty = DependencyProperty.Register("PinToTray", typeof(bool), typeof(TrayWindow), new UIPropertyMetadata(false));
        public bool PinToTray
        {
            get { return (bool)this.GetValue(PinToTrayProperty); }
            set
            {
                this.SetValue(PinToTrayProperty, value);
            }
        }

        private static readonly DependencyProperty TrayIconProperty = DependencyProperty.Register("TrayIcon", typeof(string), typeof(TrayWindow), new UIPropertyMetadata("",TrayIconPropertyChanged));
        /// <summary>
        /// Set the path to the resource to be used as the TrayIcon
        /// </summary>
        public string TrayIcon
        {
            get { return (string)this.GetValue(TrayIconProperty); }
            set
            {
                this.SetValue(TrayIconProperty, value);
            }
        }
        private static void TrayIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrayWindow traywindow = d as TrayWindow;
            try {
                // try and set the icon based on the provided string
                traywindow.trayicon.Icon = new System.Drawing.Icon(TryFindResource(e.NewValue as string), new System.Drawing.Size(16,16));
            }
            catch
            {
                // if loading the icon fails for any reason - set it back to WPFTrayWindow default
                traywindow.trayicon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/WPFTrayWindow;component/Resources/default.ico")).Stream, new System.Drawing.Size(16, 16));
            }
        }
        /// <summary>
        /// Attempt to create a stream for a specified resource path.
        /// </summary>
        /// <param name="path"></param>
        private static Stream TryFindResource(String path)
        {
            // we need to try and find the resource by multiple paths as from the xaml we'll just be retrieving a path string.
            // Probably need to expand this a bit, works with localcopy content or embedded resources.
            try
            {
                // try and find an embedded resource.
                return Application.GetResourceStream(new Uri(path)).Stream;
            }
            catch
            {
                // couldn't find an embedded resource, so lets try and find a copylocal file.
                return System.IO.File.Open(path, FileMode.Open);
            }
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

        private void RaiseTrayIconClick()
        {
            if (this.Visibility == Visibility.Visible)
            {
                this.Hide();
            }
            else
            {
                if(this.PinToTray)
                    AttachToTray();
                this.Show();
                this.Activate();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Position the window adjacent to the natification tray
        /// </summary>
        public void AttachToTray()
        {
            TaskbarPosition trayposition = TaskBar.Position;

            Left = (trayposition == TaskbarPosition.Left) ? TaskBar.Screen.WorkingArea.Left + this.Margin.Left : TaskBar.Screen.WorkingArea.Right - Width - this.Margin.Right;
            Top = (trayposition == TaskbarPosition.Top) ? TaskBar.Screen.WorkingArea.Top + this.Margin.Top : TaskBar.Screen.WorkingArea.Bottom - Height - this.Margin.Bottom;
        }

        #endregion

    }
}
