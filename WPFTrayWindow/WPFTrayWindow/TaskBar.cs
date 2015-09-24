using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace avalonprojects.wpf.tray
{
    /// <summary>
    /// expose methods to determine information about the user's taskbar position
    /// </summary>
    public sealed class TaskBar
    {
        private const string taskbarclassname = "Shell_TrayWnd";

        /// <summary>
        /// Get a System.Drawing.Rectangle representation of the screen coordinates of the taskbar
        /// </summary>
        private static Rectangle taskbarPositionRectangle
        {
            get
            {
                // get the handle of the taskbar window
                var taskbarhandle = User32.FindWindow(taskbarclassname, null);

                // get a RECT representation of the coordinates of the taskbar window
                var taskbarRECT = new RECT();
                User32.GetWindowRect(taskbarhandle, ref taskbarRECT);

                // return the taskbar coordinates as a System.Drawing.Rectangle object
                return Rectangle.FromLTRB(taskbarRECT.left, taskbarRECT.top, taskbarRECT.right, taskbarRECT.bottom);
            }
        }

        /// <summary>
        /// Get a System.Windows.Forms.Screen object representing the screen on which the primary taskbar resides
        /// </summary>
        public static Screen Screen
        {
            get
            {
                // get a System.Drawing.Rectangle representation of the taskbar coordinates
                var rectangle = taskbarPositionRectangle;
                // return the screen that contains the taskbar
                return Screen.AllScreens.FirstOrDefault(x => x.Bounds.Contains(rectangle));
            }
        }

        public static TaskbarPosition Position
        {
            get
            {
                // work out which screen and where on that screen the taskbar is
                var rectangle = taskbarPositionRectangle;
                var screen = Screen;

                // the taskbar is touching the top and bottom of the screen, thereby the taskbar is either positioned left or right
                if(rectangle.Bottom == screen.Bounds.Bottom && rectangle.Top == screen.Bounds.Top)
                {
                    // if the taskbar is touching the left its on the left otherwise its on the right
                    return (rectangle.Left == screen.Bounds.Left) ? TaskbarPosition.Left : TaskbarPosition.Right;
                }

                // the taskbar is touching the left and right of the screen, thereby the taskbar is either positioned top or bottom
                if(rectangle.Left == screen.Bounds.Left && rectangle.Right == screen.Bounds.Right)
                {
                    // if the taskbar is touching the top its on the top, otherwise its on the bottom
                    return (rectangle.Top == screen.Bounds.Top) ? TaskbarPosition.Top : TaskbarPosition.Bottom;
                }

                // if all else fails return the taskbar at the bottom of the screen (it is the windows default afterall)
                return TaskbarPosition.Bottom;
            }
        }
    }

    /// <summary>
    /// dll/mehtod imports from user32.dll
    /// </summary>
    static class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
    }

    /// <summary>
    /// data structure to handle dimensional coordinates
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    /// <summary>
    /// Enum to define the possible positions of the taskbar
    /// </summary>
    public enum TaskbarPosition
    {
        Top,
        Left,
        Right,
        Bottom
    }
}
