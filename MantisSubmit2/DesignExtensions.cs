using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MantisSubmit2
{
    public class DesignExtensions
    {
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [Category("Common Properties")]
        [DisplayName("Title")]
        public static string GetTitle(DependencyObject obj)
        {
            return (string)obj.GetValue(TitleProperty);
        }
        public static void SetTitle(DependencyObject obj, string value)
        {
            obj.SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(string), typeof(DesignExtensions), new UIPropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [Category("Common Properties")]
        [DisplayName("IsRequired")]
        public static bool GetIsRequired(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRequiredProperty);
        }
        public static void SetIsRequired(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRequiredProperty, value);
        }
        public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.RegisterAttached("IsRequired", typeof(bool), typeof(DesignExtensions), new UIPropertyMetadata(false));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        [Category("Common Properties")]
        [DisplayName("Icon")]
        public static object GetIcon(DependencyObject obj)
        {
            return obj.GetValue(IconProperty);
        }
        public static void SetIcon(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(object), typeof(DesignExtensions), new UIPropertyMetadata(null));
    }
}
