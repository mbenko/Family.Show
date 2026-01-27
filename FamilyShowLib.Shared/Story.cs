using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace FamilyShowLib.Shared
{
    [Serializable]
    public class Story : INotifyPropertyChanged
    {
        public static class Const
        {
            public const string StoriesFolderName = "Stories";
        }

        private string relativePath;

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

        [XmlIgnore]
        public string AbsolutePath
        {
            get
            {
                // For web, just return relativePath
                return relativePath ?? string.Empty;
            }
        }

        public Story() { }

        public void Delete()
        {
            // For web, no-op
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
