using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MantisSubmit2.Windows;

namespace MantisSubmit2.Controls
{
    public class AttachmentListBox : DynamicListBox
    {
        #region Constructors

        static AttachmentListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AttachmentListBox), new FrameworkPropertyMetadata(typeof(AttachmentListBox)));
        }

        public AttachmentListBox()
        {
            AddHandler(Control.MouseDoubleClickEvent, (RoutedEventHandler)OnMouseDoubleClicked);
        }

        #endregion

        #region Event handlings

        #region Drag & Drop

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (GetDraggingFileList(e.Data).Length > 0)
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            PasteFileList(GetDraggingFileList(e.Data));
        }

        #endregion

        protected override void OnAddButtonClicked()
        {
            PasteFromClipboard();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (object item in SelectedItems)
                {
                    DynamicListBoxItem listBoxItem = (DynamicListBoxItem)ItemContainerGenerator.ContainerFromItem(item);
                    listBoxItem.RemoveWithAnimation();
                }
            }
            else if (e.Key == Key.Enter)
            {
                DisplaySelectedAttachment();
            }
        }

        private void OnMouseDoubleClicked(object sender, RoutedEventArgs e)
        {
            FrameworkElement source = e.OriginalSource as FrameworkElement;
            if (source != null)
            {
                string tag = source.Tag as String;
                if (tag != null && tag == "Attachment")
                {
                    DisplaySelectedAttachment();
                }
            }
        }

        #endregion

        #region Public methods

        public void ClearWithAnimation()
        {
            foreach (object item in Items)
            {
                DynamicListBoxItem listBoxItem = (DynamicListBoxItem)ItemContainerGenerator.ContainerFromItem(item);
                listBoxItem.RemoveWithAnimation();
            }
        }

        public void PasteFromClipboard()
        {
            if (System.Windows.Forms.Clipboard.ContainsImage())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (System.Drawing.Image img = System.Windows.Forms.Clipboard.GetImage())
                    {
                        img.Save(ms, ImageFormat.Png);
                    }

                    AddImageFromMemory(ms.ToArray());
                }
            }
            else if (System.Windows.Forms.Clipboard.ContainsFileDropList())
            {
                PasteFileList(System.Windows.Forms.Clipboard.GetFileDropList().Cast<string>());
            }
            else
            {
                if (ParseFromClipboardFailed != null)
                    ParseFromClipboardFailed(this, EventArgs.Empty);
            }
        }

        public void AddImageFromMemory(byte[] data)
        {
            string fileName = Guid.NewGuid().ToString() + ".png";
            ImageAttachment attachment = new ImageAttachment(fileName, data);
            Items.Add(attachment);
        }

        #endregion

        #region Private methods

        private string[] GetDraggingFileList(IDataObject data)
        {
            if (data.GetDataPresent("FileDrop"))
            {
                return (string[])data.GetData("FileDrop");
            }
            else
            {
                return new string[0];
            }
        }

        private void DisplaySelectedAttachment()
        {
            ImageAttachment imageAttachment = SelectedItem as ImageAttachment;
            if (imageAttachment != null)
            {
                DisplayAttachmentWindow window = new DisplayAttachmentWindow(imageAttachment);
                window.ShowDialog();
            }
            else
            {
                Attachment attachment = SelectedItem as Attachment;
                if (attachment != null)
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.FileName = attachment.OriginalPath;
                        process.Start();
                    }
                    catch
                    {
                        // Not an executable file. Exception intentionally ignored...
                    }
                }
            }

        }

        private void PasteFileList(IEnumerable<string> fileNames)
        {
            foreach (string path in fileNames.Where(w => File.Exists(w)))
            {
                string fileName = GetFileName(path);
                Attachment attachment = null;
                try
                {
                    attachment = new ImageAttachment(fileName, path);
                }
                catch
                {
                    // Parse error, not an image. Exception intentionally ignored...
                }
                if (attachment == null)
                {
                    attachment = new Attachment(fileName, path);
                }
                Items.Add(attachment);
            }
        }

        private string GetFileName(string originalPath)
        {
            IEnumerable<string> currentFileNames = Items.OfType<Attachment>().Select(w => w.FileName);

            string fileNameWithoutExtensions = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);
            string fileName = fileNameWithoutExtensions + extension;

            for (int i = 2; currentFileNames.Contains(fileName); i++)
            {
                fileName = fileNameWithoutExtensions + i + extension;
            }

            return fileName;
        }

        #endregion

        #region Events

        public event EventHandler ParseFromClipboardFailed;

        #endregion
    }
}
