using CarDealershipManager.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealershipManager.Models
{
    public class CarReferenceData
    {
        public SelectList Makes { get; set; }
        public SelectList Models { get; set; }
        public SelectList Generations { get; set; }
        public SelectList TransmissionTypes { get; set; }
        public SelectList Drivetrains { get; set; }
        public SelectList BodyTypes { get; set; }
        public SelectList EuroClasses { get; set; }
        public SelectList Colors { get; set; }
        public SelectList FuelTypes { get; set; }
        public SelectList CarStatuses { get; set; }

        public CarReferenceData(
            List<MakeModel> makes,
            List<ModelModel> models,
            List<GenerationModel> generations,
            List<TransmissionTypeModel> transmissionTypes,
            List<DrivetrainModel> drivetrains,
            List<BodyTypeModel> bodyTypes,
            List<EuroClassModel> euroClasses,
            List<ColorModel> colors,
            List<FuelTypeModel> fuelTypes,
            List<CarStatusModel> carStatuses)
        {
            Makes = new SelectList(makes, "MakeId", "Name");
            Models = new SelectList(models, "ModelId", "Name");
            Generations = new SelectList(generations, "GenerationId", "Name");
            TransmissionTypes = new SelectList(transmissionTypes, "TransmissionTypeId", "Name");
            Drivetrains = new SelectList(drivetrains, "DrivetrainId", "Name");
            BodyTypes = new SelectList(bodyTypes, "BodyTypeId", "Name");
            EuroClasses = new SelectList(euroClasses, "EuroClassId", "Name");
            Colors = new SelectList(colors, "ColorId", "Name");
            FuelTypes = new SelectList(fuelTypes, "FuelTypeId", "Name");
            CarStatuses = new SelectList(carStatuses, "CarStatusId", "Name");
        }
    }
}
