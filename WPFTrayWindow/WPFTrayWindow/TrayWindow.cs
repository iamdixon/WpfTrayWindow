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
                traywindow.trayicon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/WPFTrayWindow;component/Resources/blank.ico")).Stream, new System.Drawing.Size(16, 16));
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


                if (this.PinToTray)
                {
                    AttachToTray();
                }
                else
                {
                    // Allow the window to display based on its normal position
                }
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

            try {
                System.Drawing.Rectangle trayiconrectangle = trayicon.GetNotificationIconRectangle();
                System.Drawing.Rectangle taskbarrectangle = TaskBar.TaskbarPositionRectangle;

                double trayiconwidth = trayiconrectangle.Right - trayiconrectangle.Left ;
                double trayiconheight = trayiconrectangle.Bottom - trayiconrectangle.Top;
                double trayiconhorizontalcenter = trayiconwidth / 2;
                double trayiconverticalcenter = trayiconheight / 2;

                double windowwidth = this.Width + this.Margin.Left + this.Margin.Right;
                double windowheight = this.Height + this.Margin.Top + this.Margin.Bottom;
                double windowhorizontalcenter = windowwidth / 2;
                double windowverticalcenter = windowheight / 2;

                // is the icon inside the notification tray?
                if (taskbarrectangle.Contains(trayiconrectangle))
                {
                    // inside the taskbar
                    switch (trayposition)
                    {
                        case TaskbarPosition.Bottom:
                            Top = TaskBar.Screen.WorkingArea.Bottom - Height - this.Margin.Bottom;
                            Left = (trayiconrectangle.Right - trayiconhorizontalcenter) - windowverticalcenter;
                            // does the window (including its bottom margin fall off the end of the screen?
                            if (Left + windowwidth > TaskBar.Screen.WorkingArea.Right)
                                Left = TaskBar.Screen.WorkingArea.Right - Width - Margin.Right;
                            break;
                        case TaskbarPosition.Left:
                            Top = (trayiconrectangle.Top + trayiconverticalcenter) - windowverticalcenter;
                            Left = TaskBar.Screen.WorkingArea.Left + this.Margin.Left;
                            // does the window (including its bottom margin fall off the end of the screen?
                            if (Top + windowheight > TaskBar.Screen.WorkingArea.Bottom)
                                Top = TaskBar.Screen.WorkingArea.Bottom - Height - Margin.Bottom;
                            break;
                        case TaskbarPosition.Right:
                            Top = (trayiconrectangle.Top + trayiconverticalcenter) - windowverticalcenter;
                            Left = TaskBar.Screen.WorkingArea.Right - Width - this.Margin.Right;
                            // does the window (including its bottom margin fall off the end of the screen?
                            if (Top + windowheight > TaskBar.Screen.WorkingArea.Bottom)
                                Top = TaskBar.Screen.WorkingArea.Bottom - Height - Margin.Bottom;
                            break;
                        case TaskbarPosition.Top:
                            Top = TaskBar.Screen.WorkingArea.Top + this.Margin.Top;
                            Left = (trayiconrectangle.Right - trayiconhorizontalcenter) - windowverticalcenter;
                            // does the window (including its bottom margin fall off the end of the screen?
                            if (Left + windowwidth > TaskBar.Screen.WorkingArea.Right)
                                Left = TaskBar.Screen.WorkingArea.Right - Width - Margin.Right;
                            break;
                    }
                }
                else
                {
                    // inside the notification pop-out
                    switch (trayposition)
                    {
                        case TaskbarPosition.Bottom:
                            Top = trayiconrectangle.Top - this.Margin.Bottom - this.Height;
                            Left = (trayiconrectangle.Right - trayiconhorizontalcenter) - windowverticalcenter;
                            if (Left + windowwidth > TaskBar.Screen.WorkingArea.Right)
                                Left = TaskBar.Screen.WorkingArea.Right - Width - Margin.Right;
                            break;
                        case TaskbarPosition.Left:
                            Top = trayiconrectangle.Top - this.Margin.Bottom - this.Height;
                            Left = (trayiconrectangle.Right - trayiconhorizontalcenter) - windowverticalcenter;
                            if (Left < TaskBar.Screen.WorkingArea.Left)
                                Left = TaskBar.Screen.WorkingArea.Left + Margin.Left;
                            break;
                        case TaskbarPosition.Right:
                            Top = trayiconrectangle.Top - this.Margin.Bottom - this.Height;
                            Left = (trayiconrectangle.Right - trayiconhorizontalcenter) - windowverticalcenter;
                            if (Left + windowwidth > TaskBar.Screen.WorkingArea.Right)
                                Left = TaskBar.Screen.WorkingArea.Right - Width - Margin.Right;
                            break;
                        case TaskbarPosition.Top:
                            Top = trayiconrectangle.Bottom + Margin.Top;
                            Left = (trayiconrectangle.Right - trayiconhorizontalcenter) - windowverticalcenter;
                            if (Left + windowwidth > TaskBar.Screen.WorkingArea.Right)
                                Left = TaskBar.Screen.WorkingArea.Right - Width - Margin.Right;
                            break;
                    }
                }

            }
            catch
            {
                // catchall pin it to the notification area instead of the icon
                Left = (trayposition == TaskbarPosition.Left) ? TaskBar.Screen.WorkingArea.Left + this.Margin.Left : TaskBar.Screen.WorkingArea.Right - Width - this.Margin.Right;
                Top = (trayposition == TaskbarPosition.Top) ? TaskBar.Screen.WorkingArea.Top + this.Margin.Top : TaskBar.Screen.WorkingArea.Bottom - Height - this.Margin.Bottom;
            }

            Topmost = true;

        }

        #endregion

    }
}
