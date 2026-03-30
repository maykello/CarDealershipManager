using CarDealershipManager.Models;
using CarDealershipManager.Models.Entities;

namespace CarDealershipManager.Services
{
    public class FilterService : IFilterService
    {
        private readonly CarDealershipDbContext _context;

        public FilterService(CarDealershipDbContext context)
        {
            _context = context;
        }

        public FilterOptions BuildFilterOptions(int? makeId, int? modelId)
        {
            var models = GetModelsForMake(makeId);
            var generations = GetGenerationsForModel(modelId);

            return new FilterOptions
            {
                Makes = _context.Makes.Cast<dynamic>().ToList().AsReadOnly(),
                Models = models.AsReadOnly(),
                Generations = generations.AsReadOnly(),
                FuelTypes = _context.FuelTypes.Cast<dynamic>().ToList().AsReadOnly(),
                TransmissionTypes = _context.TransmissionTypes.Cast<dynamic>().ToList().AsReadOnly(),
                BodyTypes = _context.BodyTypes.Cast<dynamic>().ToList().AsReadOnly(),
                Colors = _context.Colors.Cast<dynamic>().ToList().AsReadOnly(),
                Drivetrains = _context.Drivetrains.Cast<dynamic>().ToList().AsReadOnly(),
                EuroClasses = _context.EuroClasses.Cast<dynamic>().ToList().AsReadOnly(),
                CarStatuses = _context.CarStatus.Cast<dynamic>().ToList().AsReadOnly()
            };
        }
        public CarFilterCriteria BuildFilterCriteria(
            string searchTerm, int? makeId, int? modelId, int? generationId, decimal? minPrice, decimal? maxPrice,
            int? minYear, int? maxYear, int? fuelTypeId, int? transmissionId,
            int? bodyTypeId, int? colorId, int? drivetrainId, int? euroClassId, int? statusId)
        {
            return new CarFilterCriteria
            {
                SearchTerm = searchTerm,
                MakeId = makeId,
                ModelId = modelId,
                GenerationId = generationId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinYear = minYear,
                MaxYear = maxYear,
                FuelTypeId = fuelTypeId,
                TransmissionId = transmissionId,
                BodyTypeId = bodyTypeId,
                ColorId = colorId,
                DrivetrainId = drivetrainId,
                EuroClassId = euroClassId,
                StatusId = statusId
            };
        }


        // Pobiera listę modeli dla wybranej marki lub wszystkie modele jeśli marca nie jest wybrana.
        private List<dynamic> GetModelsForMake(int? makeId)
        {
            if (makeId.HasValue)
            {
                return _context.Models
                    .Where(m => m.Make.MakeId == makeId.Value)
                    .Cast<dynamic>()
                    .ToList();
            }

            return _context.Models.Cast<dynamic>().ToList();
        }

        // Pobiera listę generacji dla wybranego modelu lub wszystkie generacje jeśli model nie jest wybrany.
        private List<dynamic> GetGenerationsForModel(int? modelId)
        {
            if (modelId.HasValue)
            {
                return _context.Generations
                    .Where(g => g.Model.ModelId == modelId.Value)
                    .Cast<dynamic>()
                    .ToList();
            }

            return _context.Generations.Cast<dynamic>().ToList();
        }
    }
}
