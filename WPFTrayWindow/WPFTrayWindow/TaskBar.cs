﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
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
        public static Rectangle TaskbarPositionRectangle
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
                var rectangle = TaskbarPositionRectangle;
                // return the screen that contains the taskbar
                return Screen.AllScreens.FirstOrDefault(x => x.Bounds.Contains(rectangle));
            }
        }

        public static TaskbarPosition Position
        {
            get
            {
                // work out which screen and where on that screen the taskbar is
                var rectangle = TaskbarPositionRectangle;
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

        public static Rectangle GetNotificationIconRectangle(NotifyIcon Icon)
        {
            FieldInfo idFieldInfo = Icon.GetType().GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
            int iconid = (int)idFieldInfo.GetValue(Icon);

            FieldInfo windowFieldInfo = Icon.GetType().GetField("window", BindingFlags.NonPublic | BindingFlags.Instance);
            System.Windows.Forms.NativeWindow nativeWindow = (System.Windows.Forms.NativeWindow)windowFieldInfo.GetValue(Icon);
            IntPtr iconHandle = nativeWindow.Handle;

            RECT rect = new RECT();
            NOTIFYICONIDENTIFIER nid = new NOTIFYICONIDENTIFIER()
            {
                hWnd = iconHandle,
                uID = (uint)iconid
            };
            nid.cbSize = (uint)Marshal.SizeOf(nid);
            int result = Shell32.Shell_NotifyIconGetRect(ref nid, out rect);

            Rectangle notifyiconrectangle = Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);

            return notifyiconrectangle;
        }
    }

    /// <summary>
    /// dll/method imports from user32.dll
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
    /// dll/method imports from shell32.dll
    /// </summary>
    static class Shell32
    {
        [DllImport("Shell32", SetLastError = true)]
        public static extern Int32 Shell_NotifyIconGetRect([In] ref NOTIFYICONIDENTIFIER identifier, [Out] out RECT iconLocation);
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

    public struct NOTIFYICONIDENTIFIER
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uID;
        public Guid guidItem;
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
