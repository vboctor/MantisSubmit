
using System.IO;
using System;
namespace MantisSubmit2.Controls
{
    public class Attachment
    {
        #region Public properties

        public string FileName { get; private set; }
        public string OriginalPath { get; private set; }
        public byte[] Data { get; private set; }

        #endregion

        #region Constructors

        public Attachment(string fileName, byte[] data)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            if (data == null)
                throw new ArgumentNullException("data");

            FileName = fileName;
            OriginalPath = null;
            Data = data;
        }

        public Attachment(string fileName, string originalPath)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            if (originalPath == null)
                throw new ArgumentNullException("originalPath");

            FileName = fileName;
            OriginalPath = originalPath;
            Data = File.ReadAllBytes(originalPath);
        }

        #endregion
    }
}
