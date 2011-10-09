using System;
using System.Drawing;

namespace MantisSubmit2.ShellIcons
{
    public class IconContainer : IDisposable
    {
        #region Private fields

        private IconCriticalHandle handle;
        private Icon icon;

        #endregion

        #region Properties

        public Icon Icon { get { return this.icon; } }

        #endregion

        #region Constructors

        public IconContainer(IconCriticalHandle critHandle)
        {
            this.handle = critHandle;
            this.icon = Icon.FromHandle(critHandle.Handle);
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            this.icon = null;
            if (this.handle != null)
            {
                this.handle.Dispose();
                this.handle = null;
            }
        }

        #endregion
    }
}
