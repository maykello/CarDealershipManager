using AutoMapper;
using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Services
{
    public class CarAdminService : ICarAdminService
    {
        private readonly CarDealershipDbContext _context;
        private readonly IMapper _mapper;

        public CarAdminService(CarDealershipDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CarDto?> GetCarByIdAsync(int id)
        {
            var car = await BuildFullQuery()
                .FirstOrDefaultAsync(c => c.CarId == id);

            return car != null ? _mapper.Map<CarDto>(car) : null;
        }

        public async Task<int> CreateCarAsync(CarDto carDto)
        {
            var car = _mapper.Map<CarModel>(carDto);
            await SetNavigationPropertiesAsync(carDto, car);

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return car.CarId;
        }
        public async Task UpdateCarAsync(int id, CarDto carDto)
        {
            if (id != carDto.CarId)
                throw new InvalidOperationException("Car ID mismatch.");

            var car = await BuildFullQuery()
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
                throw new KeyNotFoundException($"Car with ID {id} not found.");

            _mapper.Map(carDto, car);

            await SetNavigationPropertiesAsync(carDto, car);

            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
                throw new KeyNotFoundException($"Car with ID {id} not found.");

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
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

