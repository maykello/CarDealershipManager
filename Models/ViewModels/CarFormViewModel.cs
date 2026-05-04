using CarDealershipManager.Models.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealershipManager.Models.ViewModels
{
    public class CarFormViewModel
    {
        public CarDto Car { get; set; } = new CarDto();
        public List<IFormFile>? Photos { get; set; }
        public string? MainPhotoFilename { get; set; }

        // Select Lists for Dropdowns
        public SelectList? Makes { get; set; }
        public SelectList? Models { get; set; }
        public SelectList? Generations { get; set; }
        public SelectList? FuelTypes { get; set; }
        public SelectList? TransmissionTypes { get; set; }
        public SelectList? Drivetrains { get; set; }
        public SelectList? BodyTypes { get; set; }
        public SelectList? Colors { get; set; }
        public SelectList? EuroClasses { get; set; }
        public SelectList? CarStatuses { get; set; }
    }
}
