using FamilyShowLib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FamilyShow
{
  /// <summary>
  /// Photo gallery window that shows thumbnails and allows viewing full-size images
  /// </summary>
  public partial class PhotoGalleryWindow : Window
  {
    private readonly Person person;
    private readonly List<Photo> photos;
    private int currentPhotoIndex = 0;

    public PhotoGalleryWindow(Person person)
    {
      InitializeComponent();

      this.person = person;
      this.photos = new List<Photo>(person.Photos);

      // Set the window title
      Title = $"Photos - {person.FullName}";
      HeaderText.Text = $"{person.FullName}'s Photos ({photos.Count})";

      LoadThumbnails();
    }

    /// <summary>
    /// Load thumbnails in a grid
    /// </summary>
    private void LoadThumbnails()
    {
      ThumbnailPanel.Children.Clear();

      for (int i = 0; i < photos.Count; i++)
      {
        Photo photo = photos[i];
        int photoIndex = i; // Capture for lambda

        try
        {
          // Check if file exists first
          bool fileExists = System.IO.File.Exists(photo.FullyQualifiedPath);
          string fileName = System.IO.Path.GetFileName(photo.RelativePath) ?? "Unknown";

          // Create container for thumbnail and label
          StackPanel thumbContainer = new StackPanel
          {
            Width = 150,
            Margin = new Thickness(5)
          };

          if (!fileExists)
          {
            // Show placeholder for missing file
            Border placeholderBorder = new Border
            {
              Width = 150,
              Height = 150,
              Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
              BorderBrush = Brushes.Gray,
              BorderThickness = new Thickness(1),
              Child = new TextBlock
              {
                Text = "Image\nNot Found",
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Gray,
                FontSize = 12
              }
            };

            Button placeholderButton = new Button
            {
              Style = (Style)FindResource("ThumbnailButtonStyle"),
              Content = placeholderBorder,
              ToolTip = $"File not found\nExpected path: {photo.FullyQualifiedPath}"
            };

            thumbContainer.Children.Add(placeholderButton);

            // Add filename label
            TextBlock fileLabel = new TextBlock
            {
              Text = fileName,
              Foreground = Brushes.Gray,
              FontSize = 10,
              TextAlignment = TextAlignment.Center,
              TextTrimming = TextTrimming.CharacterEllipsis,
              Margin = new Thickness(0, 3, 0, 0),
              ToolTip = photo.FullyQualifiedPath
            };
            thumbContainer.Children.Add(fileLabel);

            ThumbnailPanel.Children.Add(thumbContainer);
            continue;
          }

          // Create thumbnail button
          Button thumbButton = new Button
          {
            Style = (Style)FindResource("ThumbnailButtonStyle"),
            Width = 150,
            Height = 150,
            Tag = photoIndex
          };

          // Create image
          Image img = new Image
          {
            Width = 140,
            Height = 140,
            Stretch = Stretch.UniformToFill
          };

          // Load image
          BitmapImage bitmap = new BitmapImage();
          bitmap.BeginInit();
          bitmap.UriSource = new Uri(photo.FullyQualifiedPath, UriKind.Absolute);
          bitmap.DecodePixelWidth = 140; // Thumbnail size
          bitmap.CacheOption = BitmapCacheOption.OnLoad;
          bitmap.EndInit();
          img.Source = bitmap;

          thumbButton.Content = img;
          thumbButton.Click += (s, e) => ShowFullImage(photoIndex);
          thumbButton.ToolTip = $"File: {photo.FullyQualifiedPath}\nClick to view full size";

          // Add context menu for right-click
          ContextMenu contextMenu = new ContextMenu();

          MenuItem openFolderItem = new MenuItem
          {
            Header = "Open in Explorer",
            Tag = photo.FullyQualifiedPath
          };
          openFolderItem.Click += OpenFolderInExplorer_Click;
          contextMenu.Items.Add(openFolderItem);

          MenuItem removePhotoItem = new MenuItem
          {
            Header = "Remove Photo",
            Tag = photoIndex
          };
          removePhotoItem.Click += RemovePhoto_Click;
          contextMenu.Items.Add(removePhotoItem);

          thumbButton.ContextMenu = contextMenu;

          thumbContainer.Children.Add(thumbButton);

          // Add filename label below thumbnail
          TextBlock label = new TextBlock
          {
            Text = fileName,
            Foreground = (Brush)FindResource("FontColor"),
            FontSize = 10,
            TextAlignment = TextAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
            Margin = new Thickness(0, 3, 0, 0),
            ToolTip = photo.FullyQualifiedPath
          };
          thumbContainer.Children.Add(label);

          ThumbnailPanel.Children.Add(thumbContainer);
        }
        catch (Exception ex)
        {
          // If image can't be loaded, show placeholder
          string fileName = System.IO.Path.GetFileName(photo.RelativePath) ?? "Unknown";

          StackPanel errorContainer = new StackPanel
          {
            Width = 150,
            Margin = new Thickness(5)
          };

          Border errorBorder = new Border
          {
            Width = 150,
            Height = 150,
            Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
            BorderBrush = Brushes.Red,
            BorderThickness = new Thickness(2),
            Child = new TextBlock
            {
              Text = "Load\nError",
              TextAlignment = TextAlignment.Center,
              VerticalAlignment = VerticalAlignment.Center,
              Foreground = Brushes.Red,
              FontSize = 12
            }
          };

          Button errorButton = new Button
          {
            Style = (Style)FindResource("ThumbnailButtonStyle"),
            Content = errorBorder,
            ToolTip = $"Error loading image\nPath: {photo.FullyQualifiedPath}\nError: {ex.Message}"
          };

          errorContainer.Children.Add(errorButton);

          // Add filename label
          TextBlock errorLabel = new TextBlock
          {
            Text = fileName,
            Foreground = Brushes.Red,
            FontSize = 10,
            TextAlignment = TextAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
            Margin = new Thickness(0, 3, 0, 0),
            ToolTip = $"Path: {photo.FullyQualifiedPath}\nError: {ex.Message}"
          };
          errorContainer.Children.Add(errorLabel);

          ThumbnailPanel.Children.Add(errorContainer);
        }
      }

      FooterText.Text = $"{photos.Count} photo{(photos.Count != 1 ? "s" : "")} - Click a photo to view full size";

      // Add working folder info to help user locate files
      if (photos.Count > 0)
      {
        string workingFolder = System.IO.Path.GetDirectoryName(photos[0].FullyQualifiedPath);
        FooterText.ToolTip = $"Working folder: {workingFolder}";
      }
    }

    /// <summary>
    /// Show full size image
    /// </summary>
    private void ShowFullImage(int index)
    {
      if (index < 0 || index >= photos.Count)
        return;

      currentPhotoIndex = index;
      Photo photo = photos[index];

      try
      {
        // Load full size image
        BitmapImage bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(photo.FullyQualifiedPath, UriKind.Absolute);
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        FullImage.Source = bitmap;

        // Set caption (use file name from path)
        string fileName = System.IO.Path.GetFileName(photo.RelativePath);
        ImageCaption.Text = fileName ?? photo.RelativePath;

        // Set counter
        ImageCounter.Text = $"{currentPhotoIndex + 1} / {photos.Count}";

        // Update navigation button visibility
        PrevButton.IsEnabled = currentPhotoIndex > 0;
        NextButton.IsEnabled = currentPhotoIndex < photos.Count - 1;

        // Switch to full image view
        ThumbnailView.Visibility = Visibility.Collapsed;
        FullImageView.Visibility = Visibility.Visible;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Could not load image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void PrevButton_Click(object sender, RoutedEventArgs e)
    {
      if (currentPhotoIndex > 0)
      {
        ShowFullImage(currentPhotoIndex - 1);
      }
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
      if (currentPhotoIndex < photos.Count - 1)
      {
        ShowFullImage(currentPhotoIndex + 1);
      }
    }

    private void BackToThumbnails_Click(object sender, RoutedEventArgs e)
    {
      ThumbnailView.Visibility = Visibility.Visible;
      FullImageView.Visibility = Visibility.Collapsed;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    /// <summary>
    /// Handle keyboard navigation
    /// </summary>
    protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
    {
      base.OnKeyDown(e);

      if (FullImageView.Visibility == Visibility.Visible)
      {
        switch (e.Key)
        {
          case System.Windows.Input.Key.Left:
            if (currentPhotoIndex > 0)
              ShowFullImage(currentPhotoIndex - 1);
            break;
          case System.Windows.Input.Key.Right:
            if (currentPhotoIndex < photos.Count - 1)
              ShowFullImage(currentPhotoIndex + 1);
            break;
          case System.Windows.Input.Key.Escape:
            BackToThumbnails_Click(null, null);
            break;
        }
      }
      else if (e.Key == System.Windows.Input.Key.Escape)
      {
        Close();
      }
    }

    /// <summary>
    /// Drag over event for thumbnail panel - show copy cursor for image files
    /// </summary>
    private void ThumbnailView_DragOver(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (files != null && files.Length > 0)
        {
          foreach (string file in files)
          {
            if (IsFileSupported(file))
            {
              e.Effects = DragDropEffects.Copy;
              return;
            }
          }
        }
      }
      e.Effects = DragDropEffects.None;
    }

    /// <summary>
    /// Drop event for thumbnail panel - add photos to person
    /// </summary>
    private void ThumbnailView_Drop(object sender, DragEventArgs e)
    {
      // Retrieve the dropped files
      string[] fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];

      if (fileNames == null || fileNames.Length == 0)
        return;

      int addedCount = 0;

      // Get the files that are supported and add them to the photos for the person
      foreach (string fileName in fileNames)
      {
        if (IsFileSupported(fileName))
        {
          try
          {
            Photo photo = new Photo(fileName);

            // Make the first photo added the person's avatar if they don't have one
            if (person.Photos.Count == 0)
            {
              photo.IsAvatar = true;
            }

            // Associate the photo with the person
            person.Photos.Add(photo);

            // Add to local list
            photos.Add(photo);

            addedCount++;
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Could not add {System.IO.Path.GetFileName(fileName)}:\n{ex.Message}", 
              "Error Adding Photo", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
      }

      if (addedCount > 0)
      {
        // Setter for property change notification
        person.Avatar = string.Empty;

        // Reload thumbnails to show new photos
        LoadThumbnails();

        // Update header
        Title = $"Photos - {person.FullName}";
        HeaderText.Text = $"{person.FullName}'s Photos ({photos.Count})";

        MessageBox.Show($"{addedCount} photo{(addedCount != 1 ? "s" : "")} added successfully.", 
          "Photos Added", MessageBoxButton.OK, MessageBoxImage.Information);
      }

      // Mark the event as handled
      e.Handled = true;
    }

    /// <summary>
    /// Only allow the most common photo formats (JPEG, PNG, and GIF)
    /// </summary>
    private static bool IsFileSupported(string fileName)
    {
      string extension = System.IO.Path.GetExtension(fileName);

      if (string.Compare(extension, ".jpg", true, System.Globalization.CultureInfo.InvariantCulture) == 0 ||
          string.Compare(extension, ".jpeg", true, System.Globalization.CultureInfo.InvariantCulture) == 0 ||
          string.Compare(extension, ".png", true, System.Globalization.CultureInfo.InvariantCulture) == 0 ||
          string.Compare(extension, ".gif", true, System.Globalization.CultureInfo.InvariantCulture) == 0 ||
          string.Compare(extension, ".bmp", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Opens Windows Explorer to the folder containing the photo
    /// </summary>
    private void OpenFolderInExplorer_Click(object sender, RoutedEventArgs e)
    {
      if (sender is MenuItem menuItem && menuItem.Tag is string filePath)
      {
        try
        {
          if (System.IO.File.Exists(filePath))
          {
            // Open Explorer and select the file
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
          }
          else
          {
            // If file doesn't exist, try to open the folder
            string folder = System.IO.Path.GetDirectoryName(filePath);
            if (System.IO.Directory.Exists(folder))
            {
              System.Diagnostics.Process.Start("explorer.exe", $"\"{folder}\"");
            }
            else
            {
              MessageBox.Show($"Folder not found:\n{folder}", "Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Could not open Explorer:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
    }

    /// <summary>
    /// Remove photo from person and delete file
    /// </summary>
    private void RemovePhoto_Click(object sender, RoutedEventArgs e)
    {
      if (sender is MenuItem menuItem && menuItem.Tag is int photoIndex)
      {
        if (photoIndex < 0 || photoIndex >= photos.Count)
          return;

        Photo photo = photos[photoIndex];
        string fileName = System.IO.Path.GetFileName(photo.RelativePath);

        // Confirm deletion
        MessageBoxResult result = MessageBox.Show(
          $"Are you sure you want to remove this photo?\n\n{fileName}\n\nThe file will be deleted if it exists.",
          "Confirm Remove Photo",
          MessageBoxButton.YesNo,
          MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
          try
          {
            // Remove from person's collection
            person.Photos.Remove(photo);

            // Delete the physical file if it exists
            if (System.IO.File.Exists(photo.FullyQualifiedPath))
            {
              System.IO.File.Delete(photo.FullyQualifiedPath);
            }

            // Reload the gallery
            photos.RemoveAt(photoIndex);
            LoadThumbnails();

            // Update header
            Title = $"Photos - {person.FullName}";
            HeaderText.Text = $"{person.FullName}'s Photos ({photos.Count})";

            // If we were viewing this photo in full view, go back to thumbnails
            if (FullImageView.Visibility == Visibility.Visible)
            {
              BackToThumbnails_Click(null, null);
            }

            MessageBox.Show("Photo removed successfully.", "Photo Removed", MessageBoxButton.OK, MessageBoxImage.Information);
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Could not remove photo:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
      }
    }
  }
}
