using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace MantisSubmit2.ShellIcons
{
    public static class ShellIcon
    {
        #region Public methods

        public static IconContainer GetIconForFile(string path, bool useExtension, bool largeIcon)
        {
            string str = useExtension ? Path.GetExtension(path) : path;
            return GetIconFromShell(str, useExtension, largeIcon);
        }

        #endregion

        #region Private methods

        private static IconContainer GetIconFromShell(string path, bool useExtension, bool largeIcon)
        {
            IconContainer container;
            ShellFileInfo fileInfo = new ShellFileInfo();
            IconCriticalHandle critHandle = null;
            uint fileAttributes = 0x80;
            uint flags = 0x100;
            if (!largeIcon)
            {
                flags |= 1;
            }
            if (useExtension)
            {
                flags |= 0x10;
                if (string.IsNullOrEmpty(path))
                {
                    fileAttributes = 0x10;
                }
            }
            try
            {
                if (ShellAPI.SHGetFileInfo(path, fileAttributes, ref fileInfo, (uint)Marshal.SizeOf(fileInfo), flags) == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                critHandle = new IconCriticalHandle(fileInfo.handle);
                container = new IconContainer(critHandle);
            }
            finally
            {
                if ((critHandle == null) && (fileInfo.handle != IntPtr.Zero))
                {
                    ShellAPI.DestroyIcon(fileInfo.handle);
                }
            }
            return container;
        }

        #endregion
    }
}
