using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using FamilyShowLib;

namespace FamilyShow
{
    /// <summary>
    /// Interaction logic for StoryPreviewWindow.xaml
    /// </summary>
    public partial class StoryPreviewWindow : Window
    {
        private readonly Person person;

        public StoryPreviewWindow(Person person)
        {
            InitializeComponent();

            this.person = person;

            // Set window title
            TitleText.Text = $"Story - {person.FullName}";
            SubtitleText.Text = person.YearOfBirth != null ? $"Born {person.YearOfBirth}" : "";

            // Load the story
            LoadStory();
        }

        private void LoadStory()
        {
            if (person.Story == null || string.IsNullOrEmpty(person.Story.RelativePath))
            {
                ShowEmptyState();
                return;
            }

            try
            {
                string storyPath = person.Story.AbsolutePath;

                if (!File.Exists(storyPath))
                {
                    ShowEmptyState();
                    return;
                }

                // Load the RTF content into the RichTextBox
                TextRange textRange = new TextRange(
                    StoryContent.Document.ContentStart,
                    StoryContent.Document.ContentEnd);

                using (FileStream stream = new FileStream(storyPath, FileMode.Open, FileAccess.Read))
                {
                    textRange.Load(stream, DataFormats.Rtf);
                }

                // Override RTF formatting to ensure text is readable
                // RTF files often have embedded black text color that's not readable on dark backgrounds
                textRange.ApplyPropertyValue(System.Windows.Documents.TextElement.ForegroundProperty, Brushes.White);

                // Show the story content
                StoryContent.Visibility = Visibility.Visible;
                EmptyState.Visibility = Visibility.Collapsed;

                // Update status text with file info
                FileInfo fileInfo = new FileInfo(storyPath);
                StatusText.Text = $"Last modified: {fileInfo.LastWriteTime:g} • Press ESC to close";
            }
            catch (Exception ex)
            {
                // Show error state
                ShowEmptyState();
                StatusText.Text = $"Error loading story: {ex.Message}";
            }
        }

        private void ShowEmptyState()
        {
            StoryContent.Visibility = Visibility.Collapsed;
            EmptyState.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Set the person as current and navigate to details view
            App.Family.Current = person;

            // Close this window
            Close();

            // Raise the event to navigate to details
            // The parent window (MainWindow) should handle this
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Trigger navigation to person details
                // This will be handled by the existing navigation logic
                mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Simulate clicking the details view
                    // The MainWindow should already have logic to show details when Family.Current changes
                }));
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
