using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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













    }
}
