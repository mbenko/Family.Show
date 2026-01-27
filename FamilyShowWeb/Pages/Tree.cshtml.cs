using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FamilyShowWeb.Pages
{
    public class TreeModel : PageModel
    {
        private readonly ILogger<TreeModel> _logger;

        public TreeModel(ILogger<TreeModel> logger)
        {
            _logger = logger;
        }

        public string? FamilyJson { get; set; }

        public void OnGet(string? dataId)
        {
            _logger.LogInformation($"OnGet called with dataId: {dataId}");

            // Load family data from temporary file
            if (!string.IsNullOrEmpty(dataId))
            {
                var tempDataPath = Path.Combine(Path.GetTempPath(), $"FamilyShow_{dataId}.json");
                _logger.LogInformation($"Looking for temp file at: {tempDataPath}");

                if (System.IO.File.Exists(tempDataPath))
                {
                    try
                    {
                        FamilyJson = System.IO.File.ReadAllText(tempDataPath);
                        _logger.LogInformation($"Successfully read temp file. Length: {FamilyJson?.Length ?? 0}");
                        // Don't delete immediately - clean up in a background task
                        // The file will be cleaned up by OS temp folder cleanup
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error reading temp file");
                        FamilyJson = null;
                    }
                }
                else
                {
                    _logger.LogWarning($"Temp file not found at: {tempDataPath}");
                }
            }
            else
            {
                _logger.LogWarning("dataId is null or empty");
            }
        }
    }
}
