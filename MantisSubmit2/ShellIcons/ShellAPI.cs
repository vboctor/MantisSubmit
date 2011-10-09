using System;
using System.Runtime.InteropServices;

namespace MantisSubmit2.ShellIcons
{
    internal class ShellAPI
    {
        #region Constants

        public const uint Flag_FILE_ATTRIBUTE_DIRECTORY = 0x10;
        public const uint Flag_FILE_ATTRIBUTE_NORMAL = 0x80;
        public const uint Flag_SHGFI__SMALLICON = 1;
        public const uint Flag_SHGFI_ICON = 0x100;
        public const uint Flag_SHGFI_USEFILEATTRIBUTES = 0x10;

        #endregion

        #region Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DestroyIcon(IntPtr handle);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string path, uint fileAttributes, ref ShellFileInfo fileInfo, uint cbFileInfo, uint flags);

        #endregion
    }
}
