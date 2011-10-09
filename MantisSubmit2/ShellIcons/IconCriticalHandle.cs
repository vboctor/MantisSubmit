using System;
using System.Runtime.InteropServices;

namespace MantisSubmit2.ShellIcons
{
    public class IconCriticalHandle : CriticalHandle
    {
        #region Properties

        public IntPtr Handle { get { return base.handle; } }

        public override bool IsInvalid { get { return (base.handle == IntPtr.Zero); } }

        #endregion

        #region Constructors

        public IconCriticalHandle(IntPtr handle)
            : base(IntPtr.Zero)
        {
            base.SetHandle(handle);
        }

        #endregion

        #region CriticalHandle members

        protected override bool ReleaseHandle()
        {
            if (ShellAPI.DestroyIcon(base.handle))
            {
                base.SetHandleAsInvalid();
                return true;
            }
            return false;
        }

        #endregion
    }
}
