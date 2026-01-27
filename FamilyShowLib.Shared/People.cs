using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FamilyShowLib.Shared
{
    [XmlRoot("Family")]
    public class People
    {
        private ObservableCollection<Person> peopleCollection = new ObservableCollection<Person>();

        public ObservableCollection<Person> PeopleCollection
        {
            get { return peopleCollection; }
            set { peopleCollection = value; }
        }

        public People() { }
    }
}
