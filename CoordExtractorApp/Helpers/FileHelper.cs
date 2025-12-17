namespace CoordExtractorApp.Helpers
{
    public class FileHelper
    {

        public static async Task<string> SaveImageFromBytesAsync(byte[] imageBytes, string originalFileName, int projectId, IConfiguration configuration)

        {
            var storagePath = configuration["StoragePaths:Images"]; //από appsettings

            //έλεγψος αν υπάρχει το storage path
            if (storagePath == null)
            {
                throw new InvalidOperationException("Storage path for images is not configured.");
            }

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

        public static bool MoveImageToDeleted(string? croppedFileName, int projectId, IConfiguration configuration)
        {
                if (string.IsNullOrEmpty(croppedFileName))
                {
                    return false;
                }

                var storagePath = configuration["StoragePaths:Images"]; //από appsettings

                //έλεγψος αν υπάρχει το storage path
                if (storagePath == null)
                {
                    throw new InvalidOperationException("Storage path for images is not configured.");
                }

                var projectPath = Path.Combine(storagePath, $"Project_{projectId}");
                var deletedPath = Path.Combine(projectPath, "deleted");


                    if (!Directory.Exists(deletedPath))
                {
                    Directory.CreateDirectory(deletedPath);
                }

                var sourcePath = Path.Combine(storagePath, $"Project_{projectId}", croppedFileName);
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
