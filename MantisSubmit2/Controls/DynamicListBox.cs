using System;
using System.Windows;
using System.Windows.Controls;

namespace MantisSubmit2.Controls
{
    [TemplatePart(Name = PartAddButtonName, Type = typeof(Button))]
    public class DynamicListBox : ListBox
    {
        #region Constants

        private const string PartAddButtonName = "AddButton";

        #endregion

        #region Private fields

        private Button partAddButton;

        #endregion

        #region Constructors

        static DynamicListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicListBox), new FrameworkPropertyMetadata(typeof(DynamicListBox)));
        }

        public DynamicListBox()
        {
            AddHandler(DynamicListBoxItem.RemoveRequestedEvent, (RoutedEventHandler)DynamicListBoxItem_RemoveRequested);
        }

        #endregion

        #region ListBox members

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.partAddButton = (Button)GetTemplateChild(PartAddButtonName);
            if (this.partAddButton != null)
            {
                this.partAddButton.Click += new RoutedEventHandler(AddButton_Click);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            DynamicListBoxItem item = new DynamicListBoxItem();
            if (ItemContainerStyle != null)
            {
                item.Style = ItemContainerStyle;
            }
            return item;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DynamicListBoxItem;
        }

        #endregion

        #region Event handlings

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            OnAddButtonClicked();
            if (AddButtonClicked != null)
                AddButtonClicked(this, EventArgs.Empty);
        }

        private void DynamicListBoxItem_RemoveRequested(object sender, RoutedEventArgs e)
        {
            DynamicListBoxItem listBoxItem = (DynamicListBoxItem)e.OriginalSource;
            object item = ItemContainerGenerator.ItemFromContainer(listBoxItem);
            Items.Remove(item);
        }

        protected virtual void OnAddButtonClicked()
        {
        }

        #endregion

        #region Events

        public event EventHandler AddButtonClicked;

        #endregion
    }
}
