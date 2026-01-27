using System;

namespace FamilyShowLib.Shared
{
    [Serializable]
    public class Contact
    {
        // ...existing code from FamilyShowLib.Contact.cs...
    }

    [Serializable]
    public class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}
