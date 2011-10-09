using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MantisSubmit2.Controls
{
    public class ImageAttachment : Attachment
    {
        #region Properties

        public ImageSource Image { get; private set; }

        #endregion

        #region Constructors

        public ImageAttachment(string fileName, byte[] data)
            : base(fileName, data)
        {
            LoadImage();
        }

        public ImageAttachment(string fileName, string originalPath)
            : base(fileName, originalPath)
        {
            LoadImage();
        }

        #endregion

        #region Private methods

        private void LoadImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = new MemoryStream(Data);
            image.EndInit();
            Image = image;
        }

        #endregion
    }
}
