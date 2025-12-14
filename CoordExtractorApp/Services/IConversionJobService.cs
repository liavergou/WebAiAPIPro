using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;

namespace CoordExtractorApp.Services
{
    public interface IConversionJobService
    {
        Task<ConversionJobReadOnlyDTO> CreateAndProcessJobAsync(ConversionJobInsertDTO dto, int userId);
        Task<ConversionJobReadOnlyDTO> UpdateConversionJobAsync(int id, ConversionJobUpdateDTO dto, int userId);
        //Task<bool>DeleteConversionJobAsync(int id, int userId);
        //Task<ConversionJobReadOnlyDTO> GetConversionJobByIdAsync(int id);
    }
}
