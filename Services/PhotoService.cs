using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using CarDealershipManager.Models.Entities;
using CarDealershipManager.Models.Settings;
using CarDealershipManager.Services.Interfaces;

namespace CarDealershipManager.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<GalleryModel> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "CarDealershipManager",
                    Transformation = new Transformation()
                        .Width(1600)
                        .Crop("limit")
                        .Quality("auto:good")
                        .FetchFormat("auto")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return new GalleryModel
            {
                FilePath = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId,
                Description = file.FileName,
                Car = null! // Będzie powiązane w logice CarAdminService
            };
        }

        public async Task<bool> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }
    }
}
