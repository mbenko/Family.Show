using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyShowLib.Shared;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using FamilyShowWeb.Utilities;
using System.Xml;

namespace FamilyShowWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public IFormFile? FamilyFile { get; set; }

    public People? LoadedFamily { get; set; }
    public string? Message { get; set; }

    public void OnGet()
    {
        // Initial page load
    }

    public async Task<IActionResult> OnPostUpload()
    {
        _logger.LogInformation("OnPostUpload called. FamilyFile is null: {IsNull}, Length: {Length}", FamilyFile == null, FamilyFile?.Length ?? -1);
        _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);
        foreach (var kvp in ModelState)
        {
            foreach (var error in kvp.Value.Errors)
            {
                _logger.LogInformation("ModelState[{Key}]: {Error}", kvp.Key, error.ErrorMessage);
            }
        }
        if (FamilyFile == null || FamilyFile.Length == 0)
        {
            Message = "Please select a FamilyShow .familyx file to upload.";
            _logger.LogWarning("No file uploaded or file is empty.");
            return Page();
        }

        string? tempFilePath = null;
        string? extractFolder = null;
        try
        {
            // Save uploaded file to temp location
            tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(FamilyFile.FileName));
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                FamilyFile.CopyTo(fileStream);
            }

            // Only process .familyx files
            if (!Path.GetExtension(tempFilePath).Equals(".familyx", StringComparison.OrdinalIgnoreCase))
            {
                Message = "Only .familyx files are supported in the web app.";
                return Page();
            }

            // Extract the archive using FamilyArchiveUtility
            extractFolder = Path.Combine(Path.GetTempPath(), "FamilyShowWeb_" + Guid.NewGuid());
            Directory.CreateDirectory(extractFolder);
            FamilyArchiveUtility.ExtractFamilyArchive(tempFilePath, extractFolder);

            // Find content.xml
            var contentXmlPath = Path.Combine(extractFolder, "content.xml");
            if (!System.IO.File.Exists(contentXmlPath))
            {
                    Message = "content.xml not found in archive.";
                    return Page();
                }

                // Parse and convert content.xml directly
                Dictionary<string, int> photoCountMap = new Dictionary<string, int>();
                try
                {
                    var (family, photoCount) = ParseFamilyXml(contentXmlPath);
                    LoadedFamily = family;
                    photoCountMap = photoCount;
                    _logger.LogInformation($"Successfully parsed family data with {LoadedFamily.PeopleCollection.Count} people");
                }
                catch (Exception parseEx)
                {
                    _logger.LogError(parseEx, "Failed to parse family XML");
                    Message = $"Failed to parse family file: {parseEx.Message}";
                    return Page();
                }

                // Store family data in a temporary file instead of session/tempdata
                var tempDataId = Guid.NewGuid().ToString();
                var tempDataPath = Path.Combine(Path.GetTempPath(), $"FamilyShow_{tempDataId}.json");

            // Convert to DTO to avoid circular references
            var familyDto = new FamilyDto
            {
                PeopleCollection = LoadedFamily.PeopleCollection.Select(person => new PersonDto
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    FullName = person.FullName ?? $"{person.FirstName} {person.LastName}".Trim(),
                    BirthDate = person.BirthDate,
                    BirthPlace = person.BirthPlace ?? string.Empty,
                    DeathDate = person.DeathDate,
                    DeathPlace = person.DeathPlace ?? string.Empty,
                    Gender = person.Gender.ToString(),
                    PhotoCount = photoCountMap.ContainsKey(person.Id) ? photoCountMap[person.Id] : 0,
                    Relatives = person.Relatives.Select(rel => new RelativeDto
                    {
                        RelationType = rel.RelationType,
                        PersonId = rel.Person.Id,
                        PersonName = rel.Person.FullName ?? $"{rel.Person.FirstName} {rel.Person.LastName}".Trim()
                    }).ToList()
                }).ToList()
            };
            
            // Simple JSON serialization without circular references
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            await System.IO.File.WriteAllTextAsync(tempDataPath, JsonSerializer.Serialize(familyDto, jsonOptions));
            
            _logger.LogInformation($"Family data stored in temp file: {tempDataPath}");
            return RedirectToPage("Tree", new { dataId = tempDataId });
        }
        catch (Exception ex)
        {
            Message = $"Failed to load family file. Please check the format. Exception: {ex.Message}";
            _logger.LogError(ex, "Exception during file upload/deserialization.");
        }
        finally
        {
            // Clean up temp files
            try
            {
                if (tempFilePath != null && System.IO.File.Exists(tempFilePath))
                    System.IO.File.Delete(tempFilePath);
                if (extractFolder != null && Directory.Exists(extractFolder))
                    Directory.Delete(extractFolder, true);
            }
            catch { /* ignore cleanup errors */ }
        }
        return Page();
    }

    private (FamilyShowLib.Shared.People, Dictionary<string, int>) ParseFamilyXml(string contentXmlPath)
    {
        _logger.LogInformation($"Starting to parse XML file: {contentXmlPath}");
        var family = new FamilyShowLib.Shared.People();
        var personMap = new Dictionary<string, FamilyShowLib.Shared.Person>();
        var photoCountMap = new Dictionary<string, int>();
        
        try
        {
            // First pass: Read all persons
            _logger.LogInformation("Starting first pass: reading all persons");
            using (var xmlReader = System.Xml.XmlReader.Create(contentXmlPath))
            {
                int personCount = 0;
                while (xmlReader.ReadToFollowing("Person"))
                {
                    try
                    {
                        var personXml = xmlReader.ReadOuterXml();
                        var personDoc = new System.Xml.XmlDocument();
                        personDoc.LoadXml(personXml);
                        
                        var person = new FamilyShowLib.Shared.Person
                        {
                            Id = personDoc.DocumentElement?.GetAttribute("Id") ?? $"person_{personCount}",
                            FirstName = GetElementValue(personDoc, "FirstName"),
                            LastName = GetElementValue(personDoc, "LastName"),
                            BirthDate = ParseDateTime(GetElementValue(personDoc, "BirthDate")),
                            DeathDate = ParseDateTime(GetElementValue(personDoc, "DeathDate")),
                            BirthPlace = GetElementValue(personDoc, "BirthPlace"),
                            DeathPlace = GetElementValue(personDoc, "DeathPlace"),
                            Gender = ParseGender(GetElementValue(personDoc, "Gender"))
                        };

                        // Count photos
                        var photosNode = personDoc.SelectSingleNode("//Photos");
                        int photoCount = 0;
                        if (photosNode != null)
                        {
                            photoCount = photosNode.ChildNodes.Count;
                        }

                        // Store photo count temporarily using a dictionary
                        if (!photoCountMap.ContainsKey(person.Id))
                        {
                            photoCountMap[person.Id] = photoCount;
                        }

                        personMap[person.Id] = person;
                        family.PeopleCollection.Add(person);
                        personCount++;
                        
                        if (personCount % 10 == 0)
                            _logger.LogInformation($"Processed {personCount} persons so far");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to parse person {personCount}, skipping");
                    }
                }
                _logger.LogInformation($"First pass complete. Found {personCount} persons");
            }
        
            // Second pass: Read relationships
            _logger.LogInformation("Starting second pass: reading relationships");
            using (var xmlReader = System.Xml.XmlReader.Create(contentXmlPath))
            {
                int relationshipCount = 0;
                while (xmlReader.ReadToFollowing("Person"))
                {
                    try
                    {
                        var personXml = xmlReader.ReadOuterXml();
                        var personDoc = new System.Xml.XmlDocument();
                        personDoc.LoadXml(personXml);
                        
                        var personId = personDoc.DocumentElement?.GetAttribute("Id");
                        if (string.IsNullOrEmpty(personId) || !personMap.ContainsKey(personId))
                            continue;
                            
                        var person = personMap[personId];
                        
                        // Read relationships
                        var relationshipsNode = personDoc.SelectSingleNode("//Relationships");
                        if (relationshipsNode != null)
                        {
                            foreach (System.Xml.XmlNode relationNode in relationshipsNode.ChildNodes)
                            {
                                try
                                {
                                    // Read PersonId element, not attribute
                                    var relationToIdElement = relationNode.SelectSingleNode("PersonId");
                                    var relationToId = relationToIdElement?.InnerText;
                                    
                                    // Read RelationshipType element, not node name
                                    var relationshipTypeElement = relationNode.SelectSingleNode("RelationshipType");
                                    var relationshipType = relationshipTypeElement?.InnerText;
                                    
                                    if (!string.IsNullOrEmpty(relationToId) && !string.IsNullOrEmpty(relationshipType) && personMap.ContainsKey(relationToId))
                                    {
                                        var relatedPerson = personMap[relationToId];
                                        
                                        var relative = new FamilyShowLib.Shared.Relative
                                        {
                                            RelationType = relationshipType, // Use the actual relationship type from XML
                                            Person = relatedPerson
                                        };
                                        
                                        person.Relatives.Add(relative);
                                        relationshipCount++;
                                        
                                        if (relationshipCount % 10 == 0)
                                            _logger.LogInformation($"Processed {relationshipCount} relationships so far");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, $"Failed to parse relationship for person {personId}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to parse relationships for a person");
                    }
                }
                _logger.LogInformation($"Second pass complete. Found {relationshipCount} relationships");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed during XML parsing");
            throw;
        }

            _logger.LogInformation($"XML parsing complete. Family has {family.PeopleCollection.Count} people");
            return (family, photoCountMap);
        }
    
    private string GetElementValue(System.Xml.XmlDocument doc, string elementName)
    {
        return doc.SelectSingleNode($"//{elementName}")?.InnerText ?? "";
    }
    
        private DateTime? ParseDateTime(string dateString)
        {
            if (string.IsNullOrEmpty(dateString) || dateString == "0001-01-01T00:00:00")
                return null;

            if (DateTime.TryParse(dateString, out DateTime result))
                return result;

            return null;
        }

        private FamilyShowLib.Shared.Gender ParseGender(string genderString)
        {
            if (string.IsNullOrEmpty(genderString))
                return FamilyShowLib.Shared.Gender.Male; // Default

            if (Enum.TryParse<FamilyShowLib.Shared.Gender>(genderString, true, out var result))
                return result;

                        // Legacy support for "M" and "F"
                        if (genderString.Equals("F", StringComparison.OrdinalIgnoreCase) || 
                            genderString.Equals("Female", StringComparison.OrdinalIgnoreCase))
                            return FamilyShowLib.Shared.Gender.Female;

                        return FamilyShowLib.Shared.Gender.Male;
                    }
                }

            // DTOs for JSON serialization
            public class FamilyDto
            {
                public List<PersonDto> PeopleCollection { get; set; } = new List<PersonDto>();
            }

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
                public string Gender { get; set; } = "Male";
                public int PhotoCount { get; set; }
                public List<RelativeDto> Relatives { get; set; } = new List<RelativeDto>();
            }

            public class RelativeDto
            {
                public string RelationType { get; set; } = string.Empty;
                public string PersonId { get; set; } = string.Empty;
                public string PersonName { get; set; } = string.Empty;
            }
