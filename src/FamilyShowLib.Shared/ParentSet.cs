using System;

namespace FamilyShowLib.Shared
{
    public class ParentSet : IEquatable<ParentSet>
    {
        private Person firstParent;
        private Person secondParent;

        public Person FirstParent
        {
            get { return firstParent; }
            set { firstParent = value; }
        }

        public Person SecondParent
        {
            get { return secondParent; }
            set { secondParent = value; }
        }

        public ParentSet(Person firstParent, Person secondParent)
        {
            this.firstParent = firstParent;
            this.secondParent = secondParent;
        }

        public string Name
        {
            get
            {
                string name = string.Empty;
                name += $"{firstParent?.FirstName ?? ""} {firstParent?.LastName ?? ""} + {secondParent?.FirstName ?? ""} {secondParent?.LastName ?? ""}";
                return name.Trim();
            }
        }

        // Parameterless constructor required for serialization
        public ParentSet() { }

        public bool Equals(ParentSet other)
        {
            if (other != null)
            {
                if (firstParent != null && secondParent != null && other.firstParent != null && other.secondParent != null)
                {
                    if (firstParent.Equals(other.firstParent) && secondParent.Equals(other.secondParent))
                    {
                        return true;
                    }

                    if (firstParent.Equals(other.secondParent) && secondParent.Equals(other.firstParent))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
