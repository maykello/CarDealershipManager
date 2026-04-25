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
            CreateMap<GenerationDto, GenerationModel>()
                .ForMember(dest => dest.Cars, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Model, opt => opt.Ignore());

            CreateMap<FuelTypeModel, FuelTypeDto>();

            CreateMap<TransmissionTypeModel, TransmissionTypeDto>();

            CreateMap<BodyTypeModel, BodyTypeDto>();

            CreateMap<ColorModel, ColorDto>();

            CreateMap<DrivetrainModel, DrivetrainDto>();

            CreateMap<EuroClassModel, EuroClassDto>();

            CreateMap<CarStatusModel, CarStatusDto>();

            CreateMap<GalleryModel, GalleryDto>();
            CreateMap<GalleryDto, GalleryModel>();

            CreateMap<CarModel, CarDto>()
                .ForMember(dest => dest.GenerationId, opt => opt.MapFrom(src => src.Generation != null ? src.Generation.GenerationId : (int?)null))
                .ForMember(dest => dest.FuelTypeId, opt => opt.MapFrom(src => src.FuelType != null ? src.FuelType.FuelTypeId : (int?)null))
                .ForMember(dest => dest.TransmissionTypeId, opt => opt.MapFrom(src => src.TransmissionType != null ? src.TransmissionType.TransmissionTypeId : (int?)null))
                .ForMember(dest => dest.DrivetrainId, opt => opt.MapFrom(src => src.Drivetrain != null ? src.Drivetrain.DrivetrainId : (int?)null))
                .ForMember(dest => dest.BodyTypeId, opt => opt.MapFrom(src => src.BodyType != null ? src.BodyType.BodyTypeId : (int?)null))
                .ForMember(dest => dest.ColorId, opt => opt.MapFrom(src => src.Color != null ? src.Color.ColorId : (int?)null))
                .ForMember(dest => dest.EuroClassId, opt => opt.MapFrom(src => src.EuroClass != null ? src.EuroClass.EuroClassId : (int?)null))
                .ForMember(dest => dest.CarStatusId, opt => opt.MapFrom(src => src.CarStatus != null ? src.CarStatus.CarStatusId : (int?)null))
                .ForMember(dest => dest.MakeId, opt => opt.MapFrom(src => src.Generation != null && src.Generation.Model != null && src.Generation.Model.Make != null ? src.Generation.Model.Make.MakeId : (int?)null))
                .ForMember(dest => dest.ModelId, opt => opt.MapFrom(src => src.Generation != null && src.Generation.Model != null ? src.Generation.Model.ModelId : (int?)null));

            CreateMap<CarDto, CarModel>()
                .ForMember(dest => dest.Generation, opt => opt.Ignore())
                .ForMember(dest => dest.TransmissionType, opt => opt.Ignore())
                .ForMember(dest => dest.FuelType, opt => opt.Ignore())
                .ForMember(dest => dest.CarStatus, opt => opt.Ignore())
                .ForMember(dest => dest.Drivetrain, opt => opt.Ignore())
                .ForMember(dest => dest.BodyType, opt => opt.Ignore())
                .ForMember(dest => dest.Color, opt => opt.Ignore())
                .ForMember(dest => dest.EuroClass, opt => opt.Ignore())
                .ForMember(dest => dest.Gallery, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore());
        }
    }
}
