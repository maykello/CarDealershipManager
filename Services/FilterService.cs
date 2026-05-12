using AutoMapper;
using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Services
{
    public class FilterService : IFilterService
    {
        private readonly CarDealershipDbContext _context;
        private readonly IMapper _mapper;

        public FilterService(CarDealershipDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FilterOptions> BuildFilterOptionsAsync(int? makeId, int? modelId)
        {
            var models = await GetModelsForMakeAsync(makeId);
            var generations = await GetGenerationsForModelAsync(modelId);

            return new FilterOptions
            {
                Makes = _mapper.Map<List<MakeDto>>(await _context.Makes.ToListAsync()).AsReadOnly(),
                Models = _mapper.Map<List<ModelDto>>(models).AsReadOnly(),
                Generations = _mapper.Map<List<GenerationDto>>(generations).AsReadOnly(),
                FuelTypes = _mapper.Map<List<FuelTypeDto>>(await _context.FuelTypes.ToListAsync()).AsReadOnly(),
                TransmissionTypes = _mapper.Map<List<TransmissionTypeDto>>(await _context.TransmissionTypes.ToListAsync()).AsReadOnly(),
                BodyTypes = _mapper.Map<List<BodyTypeDto>>(await _context.BodyTypes.ToListAsync()).AsReadOnly(),
                Colors = _mapper.Map<List<ColorDto>>(await _context.Colors.ToListAsync()).AsReadOnly(),
                Drivetrains = _mapper.Map<List<DrivetrainDto>>(await _context.Drivetrains.ToListAsync()).AsReadOnly(),
                EuroClasses = _mapper.Map<List<EuroClassDto>>(await _context.EuroClasses.ToListAsync()).AsReadOnly(),
                CarStatuses = _mapper.Map<List<CarStatusDto>>(await _context.CarStatus.ToListAsync()).AsReadOnly()
            };
        }

        private async Task<List<ModelModel>> GetModelsForMakeAsync(int? makeId)
        {
            if (makeId.HasValue)
            {
                return await _context.Models
                    .Where(m => m.Make.MakeId == makeId.Value)
                    .ToListAsync();
            }

            return await _context.Models.ToListAsync();
        }

        private async Task<List<GenerationModel>> GetGenerationsForModelAsync(int? modelId)
        {
            if (modelId.HasValue)
            {
                return await _context.Generations
                    .Where(g => g.Model.ModelId == modelId.Value)
                    .ToListAsync();
            }

            return await _context.Generations.ToListAsync();
        }
    }
}
