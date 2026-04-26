using CarDealershipManager.Models.Entities;

namespace CarDealershipManager.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<GalleryModel> AddPhotoAsync(IFormFile file);
        Task<bool> DeletePhotoAsync(string publicId);
    }
}
