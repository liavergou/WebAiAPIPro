using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;

namespace CoordExtractorApp.Services
{
    public interface IConversionJobService
    {
            Task<ConversionJobReadOnlyDTO> CreateAndProcessJobAsync(ConversionJobInsertDTO dto, int userId);
    }
}
