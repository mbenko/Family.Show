using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace FamilyShowLib.Shared
{
    [Serializable]
    public class Photo : INotifyPropertyChanged
    {
        public static class Const
        {
            public const string PhotosFolderName = "Images";
        }

        private string relativePath;
        private bool isAvatar;

        public string RelativePath
        {
            get { return relativePath; }
            set
            {
                if (relativePath != value)
                {
                    relativePath = value;
                    OnPropertyChanged(nameof(RelativePath));
                }
            }
        }

        public string FullyQualifiedPath
        {
            get
            {
                // For web, just return relativePath
                return relativePath;
            }
            set { }
        }

        public bool IsAvatar
        {
            get { return isAvatar; }
            set
            {
                if (isAvatar != value)
                {
                    isAvatar = value;
                    OnPropertyChanged(nameof(IsAvatar));
                }
            }
        }

        public Photo() { }
        public Photo(string photoPath)
        {
            if (!string.IsNullOrEmpty(photoPath))
            {
                relativePath = photoPath;
            }
        }

        public override string ToString()
        {
            return FullyQualifiedPath;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class PhotoCollection : ObservableCollection<Photo>
    {
    }
}
