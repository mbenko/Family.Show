using System.IO.Compression;

namespace FamilyShowWeb.Utilities
{
    /// <summary>
    /// Cross-platform utility for reading FamilyShow .familyx files (ZIP archives)
    /// </summary>
    public static class FamilyArchiveUtility
    {
        /// <summary>
        /// Extracts a .familyx file (ZIP archive) to a temporary directory
        /// </summary>
        /// <param name="familyxFilePath">Path to the .familyx file</param>
        /// <param name="extractToDirectory">Directory to extract to</param>
        public static void ExtractFamilyArchive(string familyxFilePath, string extractToDirectory)
        {
            // Create extraction directory if it doesn't exist
            if (Directory.Exists(extractToDirectory))
            {
                Directory.Delete(extractToDirectory, recursive: true);
            }
            Directory.CreateDirectory(extractToDirectory);

            // Extract the ZIP archive
            ZipFile.ExtractToDirectory(familyxFilePath, extractToDirectory);
        }

        /// <summary>
        /// Extracts a .familyx file from a stream to a temporary directory
        /// </summary>
        /// <param name="familyxStream">Stream containing the .familyx file data</param>
        /// <param name="extractToDirectory">Directory to extract to</param>
        public static void ExtractFamilyArchive(Stream familyxStream, string extractToDirectory)
        {
            // Create extraction directory if it doesn't exist
            if (Directory.Exists(extractToDirectory))
            {
                Directory.Delete(extractToDirectory, recursive: true);
            }
            Directory.CreateDirectory(extractToDirectory);

            // Extract the ZIP archive from stream
            using (var archive = new ZipArchive(familyxStream, ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(extractToDirectory);
            }
        }

        /// <summary>
        /// Gets the path to the content.xml file after extraction
        /// </summary>
        /// <param name="extractedDirectory">Directory where files were extracted</param>
        /// <returns>Full path to content.xml</returns>
        public static string GetContentXmlPath(string extractedDirectory)
        {
            return Path.Combine(extractedDirectory, "content.xml");
        }

        /// <summary>
        /// Gets the path to the Photos directory after extraction
        /// </summary>
        /// <param name="extractedDirectory">Directory where files were extracted</param>
        /// <returns>Full path to Photos directory</returns>
        public static string GetPhotosDirectory(string extractedDirectory)
        {
            return Path.Combine(extractedDirectory, "Photos");
        }

        /// <summary>
        /// Gets the path to the Stories directory after extraction
        /// </summary>
        /// <param name="extractedDirectory">Directory where files were extracted</param>
        /// <returns>Full path to Stories directory</returns>
        public static string GetStoriesDirectory(string extractedDirectory)
        {
            return Path.Combine(extractedDirectory, "Stories");
        }
    }
}
