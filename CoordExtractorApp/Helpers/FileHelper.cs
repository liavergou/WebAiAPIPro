namespace CoordExtractorApp.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// Saves an image file to the disk within a project-specific subdirectory.
        /// </summary>
        /// <param name="imageBytes">The raw bytes of the image file.</param>
        /// <param name="originalFileName">The original filename (used to determine extension).</param>
        /// <param name="projectId">The ID of the project (used for folder organization).</param>
        /// <param name="configuration">The application configuration (for storage paths).</param>
        /// <returns>The unique filename of the saved image.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the storage path is not configured.</exception>

        public static async Task<string> SaveImageFromBytesAsync(byte[] imageBytes, string originalFileName, int projectId, IConfiguration configuration)

        {
            var storageRelativePath = configuration["StoragePaths:Images"]; //από appsettings

            //έλεγψος αν υπάρχει το storage path
            if (storageRelativePath == null)
            {
                throw new InvalidOperationException("Storage path for images is not configured.");
            }
            // αν ειναι relative path μετατροπή σε absolute path
            var storagePath = Path.IsPathRooted(storageRelativePath) ? storageRelativePath : Path.Combine(Directory.GetCurrentDirectory(), storageRelativePath); //Path.IsPathRooted https://learn.microsoft.com/en-us/dotnet/api/system.io.path.ispathrooted?view=net-8.0

            //δημιουργία subfolder για κάθε project πχ. Project_1 κλπ
            var projectPath = Path.Combine(storagePath, $"Project_{projectId}");
            Directory.CreateDirectory(projectPath);

            //είδα και τα ticks αλλα λεει είναι not thread safe. Προσοχή ή θα επιλέξω {extension} ή παω απευθειας για png
            var extension = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            //Full Path
            var fullPath = Path.Combine(projectPath, uniqueFileName);

            //save στον δίσκο
            await File.WriteAllBytesAsync(fullPath, imageBytes);

            //για αποθηκευση στη βαση
            return uniqueFileName;
        }

        /// <summary>
        /// Moves an image file to a "deleted" subdirectory instead of permanently deleting it (Soft Delete).
        /// </summary>
        /// <param name="croppedFileName">The name of the file to move.</param>
        /// <param name="projectId">The id of the project.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>True if the file was successfully moved; otherwise, false.</returns>
        public static bool MoveImageToDeleted(string? croppedFileName, int projectId, IConfiguration configuration)
        {

            var storageRelativePath = configuration["StoragePaths:Images"]; //από appsettings

            //έλεγψος αν υπάρχει το storage path
            if (storageRelativePath == null)
            {
                throw new InvalidOperationException("Storage path for images is not configured.");
            }

            // αν ειναι relative path μετατροπή σε absolute path
            var storagePath = Path.IsPathRooted(storageRelativePath) //Path.IsPathRooted https://learn.microsoft.com/en-us/dotnet/api/system.io.path.ispathrooted?view=net-8.0
                ? storageRelativePath
                : Path.Combine(Directory.GetCurrentDirectory(), storageRelativePath);

            var projectPath = Path.Combine(storagePath, $"Project_{projectId}");
            var deletedPath = Path.Combine(projectPath, "deleted");


                    if (!Directory.Exists(deletedPath))
                {
                    Directory.CreateDirectory(deletedPath);
                }

                var sourcePath = Path.Combine(storagePath, $"Project_{projectId}", croppedFileName!);
                var targetPath = Path.Combine(deletedPath, croppedFileName);

                
                    if (!File.Exists(sourcePath))
                    {
                        return false;
                    }

                    File.Move(sourcePath, targetPath);
                    return true;

            }

        }
    }
