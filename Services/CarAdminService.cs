using AutoMapper;
using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Services
{
    public class CarAdminService : ICarAdminService
    {
        private readonly CarDealershipDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public CarAdminService(CarDealershipDbContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }
        public async Task<CarDto?> GetCarByIdAsync(int id)
        {
            var car = await BuildFullQuery()
                .FirstOrDefaultAsync(c => c.CarId == id);

            return car != null ? _mapper.Map<CarDto>(car) : null;
        }

        public async Task<int> CreateCarAsync(CarDto carDto, List<IFormFile>? photos = null)
        {
            var car = _mapper.Map<CarModel>(carDto);
            await SetNavigationPropertiesAsync(carDto, car);

            if (photos != null && photos.Count > 0)
            {
                car.Gallery ??= new List<GalleryModel>();
                var isFirst = !car.Gallery.Any(g => g.IsMain);
                foreach (var photo in photos)
                {
                    var galleryModel = await _photoService.AddPhotoAsync(photo);
                    if (isFirst)
                    {
                        galleryModel.IsMain = true;
                        isFirst = false;
                    }
                    car.Gallery.Add(galleryModel);
                }
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return car.CarId;
        }
        public async Task UpdateCarAsync(int id, CarDto carDto, List<IFormFile>? photos = null, string? mainPhotoFilename = null)
        {
            if (id != carDto.CarId)
                throw new InvalidOperationException("Car ID mismatch.");

            var car = await BuildFullQuery()
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
                throw new KeyNotFoundException($"Car with ID {id} not found.");

            _mapper.Map(carDto, car);

            await SetNavigationPropertiesAsync(carDto, car);

            if (photos != null && photos.Count > 0)
            {
                car.Gallery ??= new List<GalleryModel>();
                var isFirst = !car.Gallery.Any(g => g.IsMain);
                foreach (var photo in photos)
                {
                    var galleryModel = await _photoService.AddPhotoAsync(photo);
                    
                    bool shouldBeMain = false;
                    if (!string.IsNullOrEmpty(mainPhotoFilename) && photo.FileName == mainPhotoFilename)
                    {
                        shouldBeMain = true;
                    }
                    else if (isFirst && string.IsNullOrEmpty(mainPhotoFilename))
                    {
                        shouldBeMain = true;
                        isFirst = false;
                    }

                    if (shouldBeMain)
                    {
                        foreach (var existingPhoto in car.Gallery)
                        {
                            existingPhoto.IsMain = false;
                        }
                        galleryModel.IsMain = true;
                    }

                    car.Gallery.Add(galleryModel);
                }
            }

            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Gallery)
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
                throw new KeyNotFoundException($"Car with ID {id} not found.");

            if (car.Gallery != null && car.Gallery.Any())
            {
                foreach (var photo in car.Gallery.ToList())
                {
                    if (!string.IsNullOrEmpty(photo.PublicId))
                    {
                        await _photoService.DeletePhotoAsync(photo.PublicId);
                    }
                    _context.Galleries.Remove(photo);
                }
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePhotoByIdAsync(int photoId)
        {
            var photo = await _context.Galleries.FirstOrDefaultAsync(p => p.PhotoId == photoId);
            if (photo == null) return false;

            if (!string.IsNullOrEmpty(photo.PublicId))
            {
                await _photoService.DeletePhotoAsync(photo.PublicId);
            }

            _context.Galleries.Remove(photo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetMainPhotoAsync(int carId, int photoId)
        {
            var photos = await _context.Galleries.Where(p => p.CarId == carId).ToListAsync();
            if (!photos.Any(p => p.PhotoId == photoId)) return false;

            foreach (var photo in photos)
            {
                photo.IsMain = photo.PhotoId == photoId;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CarExistsAsync(int id)
        {
            return await _context.Cars.AnyAsync(e => e.CarId == id);
        }
        public async Task<List<SelectItemDto>> GetMakesAsync()
        {
            return await _context.Makes
                .Select(m => new SelectItemDto { Id = m.MakeId, Name = m.Name })
                .ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetModelsAsync()
        {
            return await _context.Models
                .Select(m => new SelectItemDto { Id = m.ModelId, Name = m.Name })
                .ToListAsync();
        }
        public async Task<List<SelectItemDto>> GetGenerationsAsync()
        {
            return await _context.Generations
                .Select(g => new SelectItemDto { Id = g.GenerationId, Name = g.Name })
                .ToListAsync();
        }
        public async Task<List<SelectItemDto>> GetTransmissionTypesAsync()
        {
            return await _context.TransmissionTypes
                .Select(t => new SelectItemDto { Id = t.TransmissionTypeId, Name = t.Name })
                .ToListAsync();
        }
        public async Task<List<SelectItemDto>> GetDrivetrainsAsync()
        {
            return await _context.Drivetrains
                .Select(d => new SelectItemDto { Id = d.DrivetrainId, Name = d.Name })
                .ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetBodyTypesAsync()
        {
            return await _context.BodyTypes
                .Select(b => new SelectItemDto { Id = b.BodyTypeId, Name = b.Name })
                .ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetEuroClassesAsync()
        {
            return await _context.EuroClasses
                .Select(e => new SelectItemDto { Id = e.EuroClassId, Name = e.Name })
                .ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetColorsAsync()
        {
            return await _context.Colors
                .Select(c => new SelectItemDto { Id = c.ColorId, Name = c.Name })
                .ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetFuelTypesAsync()
        {
            return await _context.FuelTypes
                .Select(f => new SelectItemDto { Id = f.FuelTypeId, Name = f.Name })
                .ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetCarStatusesAsync()
        {
            return await _context.CarStatus
                .Select(s => new SelectItemDto { Id = s.CarStatusId, Name = s.Name })
                .ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetModelsByMakeAsync(int makeId)
        {
            return await _context.Models
                .Where(m => m.MakeId == makeId)
                .Select(m => new SelectItemDto { Id = m.ModelId, Name = m.Name })
                .ToListAsync();
        }
        public async Task<List<SelectItemDto>> GetGenerationsByModelAsync(int modelId)
        {
            return await _context.Generations
                .Where(g => g.ModelId == modelId)
                .Select(g => new SelectItemDto { Id = g.GenerationId, Name = g.Name })
                .ToListAsync();
        }
        private IQueryable<CarModel> BuildFullQuery()
        {
            return _context.Cars
                .Include(c => c.Generation)
                    .ThenInclude(g => g.Model)
                        .ThenInclude(m => m.Make)
                .Include(c => c.FuelType)
                .Include(c => c.TransmissionType)
                .Include(c => c.Drivetrain)
                .Include(c => c.BodyType)
                .Include(c => c.Color)
                .Include(c => c.EuroClass)
                .Include(c => c.Gallery)
                .Include(c => c.CarStatus);
        }
        private async Task SetNavigationPropertiesAsync(CarDto carDto, CarModel car)
        {
            const int minValidId = 1;

            if (carDto.GenerationId.HasValue && carDto.GenerationId >= minValidId)
                car.Generation = await _context.Generations.FindAsync(carDto.GenerationId.Value);
            else
                car.Generation = null;

            if (carDto.FuelTypeId.HasValue && carDto.FuelTypeId >= minValidId)
                car.FuelType = await _context.FuelTypes.FindAsync(carDto.FuelTypeId.Value);
            else
                car.FuelType = null;

            if (carDto.CarStatusId.HasValue && carDto.CarStatusId >= minValidId)
                car.CarStatus = await _context.CarStatus.FindAsync(carDto.CarStatusId.Value);
            else
                car.CarStatus = null;

            if (carDto.TransmissionTypeId.HasValue && carDto.TransmissionTypeId >= minValidId)
                car.TransmissionType = await _context.TransmissionTypes.FindAsync(carDto.TransmissionTypeId.Value);
            else
                car.TransmissionType = null;

            if (carDto.DrivetrainId.HasValue && carDto.DrivetrainId >= minValidId)
                car.Drivetrain = await _context.Drivetrains.FindAsync(carDto.DrivetrainId.Value);
            else
                car.Drivetrain = null;

            if (carDto.BodyTypeId.HasValue && carDto.BodyTypeId >= minValidId)
                car.BodyType = await _context.BodyTypes.FindAsync(carDto.BodyTypeId.Value);
            else
                car.BodyType = null;

            if (carDto.ColorId.HasValue && carDto.ColorId >= minValidId)
                car.Color = await _context.Colors.FindAsync(carDto.ColorId.Value);
            else
                car.Color = null;

            if (carDto.EuroClassId.HasValue && carDto.EuroClassId >= minValidId)
                car.EuroClass = await _context.EuroClasses.FindAsync(carDto.EuroClassId.Value);
            else
                car.EuroClass = null;
        }
    }
}

