using AutoMapper;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;

namespace CarDealershipManager.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MakeModel, MakeDto>();
            CreateMap<ModelModel, ModelDto>();
            CreateMap<GenerationModel, GenerationDto>();
            CreateMap<FuelTypeModel, FuelTypeDto>();
            CreateMap<TransmissionTypeModel, TransmissionTypeDto>();
            CreateMap<BodyTypeModel, BodyTypeDto>();
            CreateMap<ColorModel, ColorDto>();
            CreateMap<DrivetrainModel, DrivetrainDto>();
            CreateMap<EuroClassModel, EuroClassDto>();
            CreateMap<CarStatusModel, CarStatusDto>();
        }
    }
}
