using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MantisSubmit2.Controls
{
    [TemplatePart(Name = PartRemoveButtonName, Type = typeof(Button))]
    [TemplatePart(Name = PartLayoutRootName, Type = typeof(FrameworkElement))]
    public class DynamicListBoxItem : ListBoxItem
    {
        #region Constants

        private const string PartRemoveButtonName = "RemoveButton";
        private const string PartLayoutRootName = "LayoutRoot";

        #endregion

        #region Private fields

        private Button partRemoveButton;
        private FrameworkElement partLayoutRoot;
        private bool showStoryboardStarted = false;

        #endregion

        #region Dependency properties

        [Category("Common Properties")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Attachment Attachment
        {
            get { return (Attachment)GetValue(AttachmentProperty); }
            set { SetValue(AttachmentProperty, value); }
        }
        public static readonly DependencyProperty AttachmentProperty = DependencyProperty.Register("Attachment", typeof(Attachment), typeof(DynamicListBoxItem), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        static DynamicListBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicListBoxItem), new FrameworkPropertyMetadata(typeof(DynamicListBoxItem)));
        }

        public DynamicListBoxItem()
        {
            Loaded += new RoutedEventHandler(AttachmentsControlItem_Loaded);
        }

        #endregion

        #region Event handlings

        private void AttachmentsControlItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.partLayoutRoot != null && !this.showStoryboardStarted)
            {
                Storyboard showStoryboard = CreateShowStoryboard();
                showStoryboard.Begin();
                this.showStoryboardStarted = true;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.partRemoveButton = (Button)GetTemplateChild(PartRemoveButtonName);
            this.partLayoutRoot = (FrameworkElement)GetTemplateChild(PartLayoutRootName);

            if (this.partRemoveButton != null)
            {
                this.partRemoveButton.Click += new RoutedEventHandler(RemoveButton_Click);
            }

            if (!this.showStoryboardStarted)
            {
                Storyboard showStoryboard = CreateShowStoryboard();
                showStoryboard.Begin();
                this.showStoryboardStarted = true;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveWithAnimation();
        }

        private void HideStoryboard_Completed(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(RemoveRequestedEvent, this));
        }

        #endregion

        #region Public methods

        public void RemoveWithAnimation()
        {
            Storyboard hideStoryboard = CreateHideStoryboard();
            hideStoryboard.Completed += new EventHandler(HideStoryboard_Completed);
            hideStoryboard.Begin();

            IsHitTestVisible = false;
        }

        #endregion

        #region Private methods

        private Storyboard CreateShowStoryboard()
        {
            this.partLayoutRoot.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            DoubleAnimation widthAnimation = new DoubleAnimation();
            Storyboard.SetTarget(widthAnimation, this.partLayoutRoot);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(FrameworkElement.WidthProperty));
            widthAnimation.From = 0.0;
            widthAnimation.To = this.partLayoutRoot.DesiredSize.Width;
            widthAnimation.BeginTime = TimeSpan.FromSeconds(0.0);
            widthAnimation.Duration = TimeSpan.FromSeconds(0.3);
            widthAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseInOut };

            DoubleAnimationUsingKeyFrames opacityAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(opacityAnimation, this.partLayoutRoot);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(FrameworkElement.OpacityProperty));
            opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
            opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3))));
            opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.4))));

            Storyboard showStoryboard = new Storyboard();
            showStoryboard.Children.Add(widthAnimation);
            showStoryboard.Children.Add(opacityAnimation);

            return showStoryboard;
        }

        private Storyboard CreateHideStoryboard()
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            Storyboard.SetTarget(opacityAnimation, this.partLayoutRoot);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(FrameworkElement.OpacityProperty));
            opacityAnimation.From = 1.0;
            opacityAnimation.To = 0.0;
            opacityAnimation.BeginTime = TimeSpan.FromSeconds(0.0);
            opacityAnimation.Duration = TimeSpan.FromSeconds(0.1);

            DoubleAnimation widthAnimation = new DoubleAnimation();
            Storyboard.SetTarget(widthAnimation, this.partLayoutRoot);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(FrameworkElement.WidthProperty));
            widthAnimation.From = this.partLayoutRoot.ActualWidth;
            widthAnimation.To = 0.0;
            widthAnimation.BeginTime = TimeSpan.FromSeconds(0.1);
            widthAnimation.Duration = TimeSpan.FromSeconds(0.4);
            widthAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseInOut };

            Storyboard hideStoryboard = new Storyboard();
            hideStoryboard.Children.Add(opacityAnimation);
            hideStoryboard.Children.Add(widthAnimation);

            return hideStoryboard;
        }

        #endregion

        #region Events

        public static readonly RoutedEvent RemoveRequestedEvent = EventManager.RegisterRoutedEvent("RemoveRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DynamicListBoxItem));
        public event RoutedEventHandler RemoveRequested
        {
            add { AddHandler(RemoveRequestedEvent, value); }
            remove { RemoveHandler(RemoveRequestedEvent, value); }
        }

        #endregion
    }
}
