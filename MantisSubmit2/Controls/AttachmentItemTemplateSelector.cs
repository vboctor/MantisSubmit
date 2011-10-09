using System;
using System.Windows;
using System.Windows.Controls;

namespace MantisSubmit2.Controls
{
    public class AttachmentItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GenericTemmplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ImageAttachment)
            {
                return ImageTemplate;
            }
            else if (item is Attachment)
            {
                return GenericTemmplate;
            }
            else
            {
                throw new ArgumentException("Invalid item type.", "item");
            }
        }
    }
}
