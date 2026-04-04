using CarDealershipManager.Models.Dtos;

namespace CarDealershipManager.Models
{
    public class FilterOptions
    {
        public IReadOnlyList<MakeDto> Makes { get; set; } = new List<MakeDto>().AsReadOnly();
        public IReadOnlyList<ModelDto> Models { get; set; } = new List<ModelDto>().AsReadOnly();
        public IReadOnlyList<GenerationDto> Generations { get; set; } = new List<GenerationDto>().AsReadOnly();
        public IReadOnlyList<FuelTypeDto> FuelTypes { get; set; } = new List<FuelTypeDto>().AsReadOnly();
        public IReadOnlyList<TransmissionTypeDto> TransmissionTypes { get; set; } = new List<TransmissionTypeDto>().AsReadOnly();
        public IReadOnlyList<BodyTypeDto> BodyTypes { get; set; } = new List<BodyTypeDto>().AsReadOnly();
        public IReadOnlyList<ColorDto> Colors { get; set; } = new List<ColorDto>().AsReadOnly();
        public IReadOnlyList<DrivetrainDto> Drivetrains { get; set; } = new List<DrivetrainDto>().AsReadOnly();
        public IReadOnlyList<EuroClassDto> EuroClasses { get; set; } = new List<EuroClassDto>().AsReadOnly();
        public IReadOnlyList<CarStatusDto> CarStatuses { get; set; } = new List<CarStatusDto>().AsReadOnly();
    }
}
