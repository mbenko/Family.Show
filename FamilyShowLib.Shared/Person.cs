using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FamilyShowLib.Shared
{
    [Serializable]
    public class Person : INotifyPropertyChanged, IEquatable<Person>, IDataErrorInfo
    {
        private string _firstName;
        private string _lastName;
        private string _suffix;
        private string _birthPlace;
        private string _deathPlace;
        private DateTime? _birthDate;
        private DateTime? _deathDate;
        private bool _isLiving;
        private Gender _gender;
        private string _avatarRelativePath;
        private ObservableCollection<Relative> _relatives;

        public Person()
        {
            _relatives = new ObservableCollection<Relative>();
            _suffix = string.Empty;
            _birthPlace = string.Empty;
            _deathPlace = string.Empty;
            _isLiving = true;
            _gender = Gender.Male;
            _avatarRelativePath = string.Empty;
        }

        [XmlAttribute]
        public string Id { get; set; }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        public DateTime? BirthDate
        {
            get { return _birthDate; }
            set
            {
                if (_birthDate != value)
                {
                    _birthDate = value;
                    OnPropertyChanged(nameof(BirthDate));
                }
            }
        }

        public string Suffix
        {
            get { return _suffix; }
            set
            {
                if (_suffix != value)
                {
                    _suffix = value;
                    OnPropertyChanged(nameof(Suffix));
                }
            }
        }

        public string BirthPlace
        {
            get { return _birthPlace; }
            set
            {
                if (_birthPlace != value)
                {
                    _birthPlace = value;
                    OnPropertyChanged(nameof(BirthPlace));
                }
            }
        }

        public DateTime? DeathDate
        {
            get { return _deathDate; }
            set
            {
                if (_deathDate != value)
                {
                    _deathDate = value;
                    OnPropertyChanged(nameof(DeathDate));
                }
            }
        }

        public string DeathPlace
        {
            get { return _deathPlace; }
            set
            {
                if (_deathPlace != value)
                {
                    _deathPlace = value;
                    OnPropertyChanged(nameof(DeathPlace));
                }
            }
        }

        public bool IsLiving
        {
            get { return _isLiving; }
            set
            {
                if (_isLiving != value)
                {
                    _isLiving = value;
                    OnPropertyChanged(nameof(IsLiving));
                }
            }
        }

        public Gender Gender
        {
            get { return _gender; }
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    OnPropertyChanged(nameof(Gender));
                }
            }
        }

        public string AvatarRelativePath
        {
            get { return _avatarRelativePath; }
            set
            {
                if (_avatarRelativePath != value)
                {
                    _avatarRelativePath = value;
                    OnPropertyChanged(nameof(AvatarRelativePath));
                }
            }
        }

        public ObservableCollection<Relative> Relatives
        {
            get { return _relatives; }
            set
            {
                if (_relatives != value)
                {
                    _relatives = value;
                    OnPropertyChanged(nameof(Relatives));
                }
            }
        }

        public string FullName => $"{FirstName} {LastName}";

        // Computed relationship properties for web visualization
        [XmlIgnore]
        public IEnumerable<Person> Parents
        {
            get
            {
                return _relatives?.Where(r => r.RelationType == "Parent")
                    .Select(r => r.Person) ?? Enumerable.Empty<Person>();
            }
        }

        [XmlIgnore]
        public IEnumerable<Person> Children
        {
            get
            {
                return _relatives?.Where(r => r.RelationType == "Child")
                    .Select(r => r.Person) ?? Enumerable.Empty<Person>();
            }
        }

        [XmlIgnore]
        public IEnumerable<Person> Spouses
        {
            get
            {
                return _relatives?.Where(r => r.RelationType == "Spouse")
                    .Select(r => r.Person) ?? Enumerable.Empty<Person>();
            }
        }

        [XmlIgnore]
        public IEnumerable<Person> Siblings
        {
            get
            {
                return _relatives?.Where(r => r.RelationType == "Sibling")
                    .Select(r => r.Person) ?? Enumerable.Empty<Person>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(Person other)
        {
            if (other == null) return false;
            return FirstName == other.FirstName && LastName == other.LastName && BirthDate == other.BirthDate;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Person);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case nameof(FirstName):
                        if (string.IsNullOrWhiteSpace(FirstName))
                            result = "First name is required.";
                        break;
                    case nameof(LastName):
                        if (string.IsNullOrWhiteSpace(LastName))
                            result = "Last name is required.";
                        break;
                    case nameof(BirthDate):
                        if (BirthDate == null || BirthDate == default)
                            result = "Birth date is required.";
                        break;
                }
                return result;
            }
        }
    }

    [Serializable]
    public class Relative
    {
        public string RelationType { get; set; }
        public Person Person { get; set; }
    }
}
