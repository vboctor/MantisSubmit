using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MantisConnect;
using MantisSubmit2.Properties;
using FeedbackManager.Model;
using System.Xml.Serialization;

namespace MantisSubmit2.Windows
{
    public partial class MainWindow : Window
    {
        #region Constants

        private const int TitleSummaryMaxLength = 20;
        private const string BackupDirectory = "Backup";

        #endregion

        #region Properties

        public BackgroundWorkerService BackgroundWorkerService { get { return App.Current.BackgroundWorkerService; } }
        public MantisService MantisService { get { return App.Current.MantisService; } }

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            Title = App.Current.ApplicationName;
            LayoutRoot.DataContext = this;
            PopulateComboBoxes();
            MantisService.MantisDataChanged += (sender, e) => PopulateComboBoxes();

            Width = Math.Max(MinWidth, Settings.Default.LastWindowWidth);
            Height = Math.Max(MinHeight, Settings.Default.LastWindowHeight);
        }

        #endregion

        #region Event handlings

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!String.IsNullOrEmpty(SummaryTextBox.Text) || !String.IsNullOrEmpty(DescriptionTextBox.Text) || !String.IsNullOrEmpty(CommentTextBox.Text))
            {
                if (MessageBox.Show("Are you sure you want to close this window? You will lose information you entered.", App.Current.ApplicationName, MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            if (!e.Cancel)
            {
                Settings.Default.LastWindowWidth = (int)Width;
                Settings.Default.LastWindowHeight = (int)Height;
                Settings.Default.Save();
            }
            base.OnClosing(e);
        }

        private void SummaryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string summary = SummaryTextBox.Text;
            if (summary.Length > TitleSummaryMaxLength)
            {
                summary = summary.Substring(0, TitleSummaryMaxLength).Trim() + "...";
            }
            Title = String.Format("{0} - {1}", summary, App.Current.ApplicationName);
        }

        private void NewInstanceButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Attachments.PasteFromClipboard();
            }
        }

        private void ChangeUserButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(App.Current.MainWindow, "If you log out, the application will close.", App.Current.ApplicationName, MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.OK)
            {
                Settings.Default.MantisPassword = String.Empty;
                Settings.Default.Save();
                App.Current.Shutdown();
            }
        }

        private void ProjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateProjectDependentComboBoxes();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = Settings.Default;
            MantisData data = MantisService.Data;
            Session session = MantisService.MantisSession;

            // Validating
            if (ProjectComboBox.SelectedItem == null ||
                CategoryComboBox.SelectedItem == null ||
                PriorityComboBox.SelectedItem == null ||
                EtaComboBox.SelectedItem == null ||
                SeverityComboBox.SelectedItem == null ||
                ReproducibilityComboBox.SelectedItem == null ||
                String.IsNullOrEmpty(SummaryTextBox.Text) ||
                String.IsNullOrEmpty(DescriptionTextBox.Text))
            {
                MessageBox.Show("You must fill every required fields.", App.Current.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            // Get issue properties
            int? issueId = null;
            string summary = SummaryTextBox.Text;
            string project = (string)ProjectComboBox.SelectedItem;
            string severity = (string)SeverityComboBox.SelectedItem;
            string priority = (string)PriorityComboBox.SelectedItem;
            string reproducibility = (string)ReproducibilityComboBox.SelectedItem;
            string eta = (string)EtaComboBox.SelectedItem;
            string category = (string)CategoryComboBox.SelectedItem;
            string productVersion = (string)ProductVersionComboBox.SelectedItem;
            string targetVersion = (string)TargetVersionComboBox.SelectedItem;
            string description = DescriptionTextBox.Text;
            string comment = CommentTextBox.Text;
            string assignedToString = (string)AssignedToComboBox.SelectedItem;
            User assignedTo;
            if (String.IsNullOrEmpty(assignedToString))
                assignedTo = new User { Id = 0, Name = "", Email = "", RealName = "" };
            else
                assignedTo = data.Users[project].Single(w => w.Name == (string)AssignedToComboBox.SelectedItem);
            Controls.Attachment[] attachments = Attachments.Items.OfType<Controls.Attachment>().ToArray();

            // Set last properties
            settings.LastProject = project;
            settings.LastCategory = category;
            settings.LastProductVersion = productVersion;
            settings.LastTargetVersion = targetVersion;
            settings.LastSeverity = severity;
            settings.LastReproducibility = reproducibility;
            settings.Save();

            // Clear form fields
            SummaryTextBox.Text = "";
            DescriptionTextBox.Text = "";
            CommentTextBox.Text = "";
            Attachments.ClearWithAnimation();

            BackgroundWorkerService.EnqueueTask("Submitting issue", (param) =>
            {
                Issue issue = new Issue();

                issue.Summary = summary;
                issue.Project = new ObjectRef(project);
                issue.Priority = new ObjectRef(priority);
                issue.Severity = new ObjectRef(severity);
                issue.Reproducibility = new ObjectRef(reproducibility);
                issue.Eta = new ObjectRef(eta);
                issue.Category = new ObjectRef(category);
                issue.ProductVersion = productVersion;
                issue.TargetVersion = targetVersion;
                issue.Description = description;
                issue.AssignedTo = assignedTo;
                issue.ReportedBy = new User { Name = session.Username, };

                issueId = session.Request.IssueAdd(issue);
            });
            BackgroundWorkerService.EnqueueTask("Submitting issue note", (param) =>
            {
                if (!String.IsNullOrEmpty(comment) && issueId.HasValue)
                {
                    IssueNote issueNote = new IssueNote();
                    issueNote.Author = new User { Name = session.Username, };
                    issueNote.Text = comment;

                    session.Request.IssueNoteAdd(issueId.Value, issueNote);
                }
            });
            foreach (Controls.Attachment attachment in attachments)
            {
                BackgroundWorkerService.EnqueueTask("Submitting " + attachment.FileName, attachment, (param) =>
                {
                    Controls.Attachment currentAttachment = (Controls.Attachment)param;
                    if (issueId.HasValue)
                    {
                        session.Request.IssueAddFile(issueId.Value, currentAttachment.FileName, "application/octet-stream", currentAttachment.Data);
                    }
                });
            }
        }

        private void Attachments_ParseFromClipboardFailed(object sender, EventArgs e)
        {
            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                FeedbackClipboardInfo info = null;
                try
                {
                    StringReader sr = new StringReader(Clipboard.GetText());
                    XmlSerializer serializer = new XmlSerializer(typeof(FeedbackClipboardInfo));
                    info = (FeedbackClipboardInfo)serializer.Deserialize(sr);
                }
                catch
                {
                    // Invalid format, clipboard doesn't contains a valid, xml serialized FeedbackClipboardInfo object.
                    // Intentionally ignored...
                }


                if (info != null)
                {
                    DescriptionTextBox.Text = info.Description;
                    CommentTextBox.Text = info.Comment;

                    if (ProductVersionComboBox.Items != null)
                    {
                        IEnumerable<string> productVersions = ProductVersionComboBox.Items.OfType<string>();
                        if (productVersions.Any())
                        {
                            string selectedProductVersion = productVersions.OrderByDescending(w => w.CalculateSimilarity(info.ClientVersion)).FirstOrDefault();
                            if (selectedProductVersion != null)
                                ProductVersionComboBox.SelectedItem = selectedProductVersion;
                        }
                    }

                    if (info.Screenshot != null && info.Screenshot.Length > 0)
                    {
                        Attachments.AddImageFromMemory(info.Screenshot);
                    }
                }
            }
        }

        #endregion

        #region Public methods

        public bool NeedBackup()
        {
            return !String.IsNullOrEmpty(SummaryTextBox.Text) || !String.IsNullOrEmpty(DescriptionTextBox.Text) || !String.IsNullOrEmpty(CommentTextBox.Text);
        }

        public void Backup()
        {
            try
            {
                if (!Directory.Exists(BackupDirectory))
                {
                    Directory.CreateDirectory(BackupDirectory);
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Summary");
                sb.AppendLine("-------");
                sb.AppendLine(SummaryTextBox.Text);
                sb.AppendLine();
                sb.AppendLine("Description");
                sb.AppendLine("-----------");
                sb.AppendLine(DescriptionTextBox.Text);

                if (!String.IsNullOrEmpty(CommentTextBox.Text))
                {
                    sb.AppendLine();
                    sb.AppendLine("Note");
                    sb.AppendLine("----");
                    sb.AppendLine(CommentTextBox.Text);
                }

                string fileName = String.Format("{0}.txt", Guid.NewGuid());
                File.WriteAllText(Path.Combine(BackupDirectory, fileName), sb.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to create backup." + Environment.NewLine + e.ToString(), App.Current.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Private methods

        private void PopulateComboBoxes()
        {
            Settings settings = Settings.Default;
            MantisData data = MantisService.Data;

            UpdateReminiscentComboBox(ProjectComboBox, settings.LastProject, data.Projects);
            UpdateComboBox(PriorityComboBox, data.Priorities);
            UpdateComboBox(EtaComboBox, data.Etas);
            UpdateReminiscentComboBox(SeverityComboBox, settings.LastSeverity, data.Severities);
            UpdateReminiscentComboBox(ReproducibilityComboBox, settings.LastReproducibility, data.Reproducibilities);
            PopulateProjectDependentComboBoxes();
        }

        private void PopulateProjectDependentComboBoxes()
        {
            Settings settings = Settings.Default;
            MantisData data = MantisService.Data;

            string selectedProject = (string)ProjectComboBox.SelectedItem;
            if (selectedProject == null || !data.Projects.Contains(selectedProject))
            {
                CategoryComboBox.ItemsSource = null;
                ProductVersionComboBox.ItemsSource = null;
                TargetVersionComboBox.ItemsSource = null;
                AssignedToComboBox.ItemsSource = null;
            }
            else
            {
                UpdateReminiscentComboBox(CategoryComboBox, settings.LastCategory, data.Categories[selectedProject]);
                UpdateReminiscentComboBox(ProductVersionComboBox, settings.LastProductVersion, data.Versions[selectedProject]);
                UpdateReminiscentComboBox(TargetVersionComboBox, settings.LastTargetVersion, data.Versions[selectedProject]);
                UpdateComboBox(AssignedToComboBox, new string[] { "" }.Concat(data.Users[selectedProject].Select(w => w.Name)));
            }
        }

        private void UpdateReminiscentComboBox(ComboBox target, string memory, IEnumerable<string> items)
        {
            string currentItem = (string)target.SelectedItem;
            target.ItemsSource = items;
            if (currentItem != null)
            {
                if (items.Contains(currentItem))
                    target.SelectedItem = currentItem;
                else
                    target.SelectedIndex = 0;
            }
            else
            {
                if (items.Contains(memory))
                    target.SelectedItem = memory;
                else
                    target.SelectedIndex = 0;
            }
        }

        private void UpdateComboBox(ComboBox target, IEnumerable<string> items)
        {
            string currentItem = (string)target.SelectedItem;
            target.ItemsSource = items;
            if (currentItem != null)
            {
                if (items.Contains(currentItem))
                    target.SelectedItem = currentItem;
                else
                    target.SelectedIndex = 0;
            }
            else
                target.SelectedIndex = 0;
        }

        #endregion
    }
}
