using System;
using System.Runtime.InteropServices;

namespace MantisSubmit2.ShellIcons
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ShellFileInfo
    {
        #region Public fields

        public IntPtr handle;

        public int iIcon;

        public uint attributes;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] displayName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public char[] typeName;

        #endregion
    }
}
