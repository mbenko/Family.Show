using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FamilyShowLib.Shared
{
    /// <summary>
    /// Simplified person model for JSON serialization that avoids circular references
    /// </summary>
    public class PersonDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; } = string.Empty;
        public DateTime? DeathDate { get; set; }
        public string DeathPlace { get; set; } = string.Empty;
        
        public List<RelativeDto> Relatives { get; set; } = new List<RelativeDto>();
    }

    /// <summary>
    /// Simplified relative model that only stores ID and name to avoid cycles
    /// </summary>
    public class RelativeDto
    {
        public string RelationType { get; set; } = string.Empty; // Parent, Child, Spouse, Sibling
        public string PersonId { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Simple family collection for JSON serialization
    /// </summary>
    public class FamilyDto
    {
        public List<PersonDto> PeopleCollection { get; set; } = new List<PersonDto>();
    }
}
