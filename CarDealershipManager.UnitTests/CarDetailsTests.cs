using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarDealershipManager.Controllers;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CarDealershipManager.UnitTests
{
    public class HomeControllerDetailsTests
    {
        [Fact]
        public async Task Details_DlaIstniejacegoCar_ZwracaViewZDanymi()
        {
            // Arrange
            var mockCarSearchService = new Mock<ICarSearchService>();
            var mockFilterService = new Mock<IFilterService>();

            var carDto = new CarDto
            {
                CarId = 1,
                ProductionYear = 2020,
                HorsePower = 200,
                Mileage = 50000,
                Vin = "TESTVIN123456789",
                Price = 45000m,
                Description = "Testowy samochód",
                FuelType = new FuelTypeDto { FuelTypeId = 1, Name = "Benzyna" },
                CarStatus = new CarStatusDto { CarStatusId = 1, Name = "Dostępny" },
                Gallery = new List<GalleryDto>()
            };

            mockCarSearchService
                .Setup(s => s.GetCarByIdAsync(1))
                .ReturnsAsync(carDto);

            var controller = new HomeController(mockCarSearchService.Object, mockFilterService.Object);

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarDto>(viewResult.Model);
            Assert.Equal(1, model.CarId);
            Assert.Equal("TESTVIN123456789", model.Vin);
            Assert.Equal(45000m, model.Price);
            mockCarSearchService.Verify(s => s.GetCarByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task Details_DlaNieistniejacegoCar_ZwracaNotFound()
        {
            // Arrange
            var mockCarSearchService = new Mock<ICarSearchService>();
            var mockFilterService = new Mock<IFilterService>();

            mockCarSearchService
                .Setup(s => s.GetCarByIdAsync(999))
                .ReturnsAsync((CarDto)null);

            var controller = new HomeController(mockCarSearchService.Object, mockFilterService.Object);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockCarSearchService.Verify(s => s.GetCarByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task Details_LadujeCarDtoZPodanymId()
        {
            // Arrange
            var mockCarSearchService = new Mock<ICarSearchService>();
            var mockFilterService = new Mock<IFilterService>();

            var carDto = new CarDto
            {
                CarId = 5,
                ProductionYear = 2022,
                HorsePower = 250,
                Mileage = 15000,
                Vin = "BMW123XYZ789",
                Price = 80000m,
                FuelType = new FuelTypeDto { FuelTypeId = 2, Name = "Diesel" },
                TransmissionType = new TransmissionTypeDto { TransmissionTypeId = 1, Name = "Automatyczna" },
                BodyType = new BodyTypeDto { BodyTypeId = 1, Name = "Sedan" },
                Color = new ColorDto { ColorId = 1, Name = "Czarny" },
                CarStatus = new CarStatusDto { CarStatusId = 1, Name = "Dostępny" }
            };

            mockCarSearchService
                .Setup(s => s.GetCarByIdAsync(5))
                .ReturnsAsync(carDto);

            var controller = new HomeController(mockCarSearchService.Object, mockFilterService.Object);

            // Act
            var result = await controller.Details(5);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarDto>(viewResult.Model);

            Assert.Equal(5, model.CarId);
            Assert.Equal(2022, model.ProductionYear);
            Assert.Equal(250, model.HorsePower);
            Assert.Equal(15000, model.Mileage);
            Assert.Equal("BMW123XYZ789", model.Vin);
            Assert.Equal(80000m, model.Price);
            Assert.NotNull(model.FuelType);
            Assert.NotNull(model.TransmissionType);
            Assert.NotNull(model.BodyType);
            Assert.NotNull(model.Color);
            Assert.NotNull(model.CarStatus);
        }

        [Fact]
        public async Task Details_LadujeGalerieZdj()
        {
            // Arrange
            var mockCarSearchService = new Mock<ICarSearchService>();
            var mockFilterService = new Mock<IFilterService>();

            var carDto = new CarDto
            {
                CarId = 3,
                ProductionYear = 2021,
                HorsePower = 180,
                Mileage = 30000,
                Vin = "AUDI456XYZ789",
                Price = 55000m,
                Gallery = new List<GalleryDto>
                {
                    new GalleryDto { PhotoId = 1, FilePath = "/images/car-1.jpg" },
                    new GalleryDto { PhotoId = 2, FilePath = "/images/car-2.jpg" },
                    new GalleryDto { PhotoId = 3, FilePath = "/images/car-3.jpg" }
                },
                FuelType = new FuelTypeDto { FuelTypeId = 1, Name = "Benzyna" },
                CarStatus = new CarStatusDto { CarStatusId = 1, Name = "Dostępny" }
            };

            mockCarSearchService
                .Setup(s => s.GetCarByIdAsync(3))
                .ReturnsAsync(carDto);

            var controller = new HomeController(mockCarSearchService.Object, mockFilterService.Object);

            // Act
            var result = await controller.Details(3);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarDto>(viewResult.Model);

            Assert.NotNull(model.Gallery);
            Assert.Equal(3, model.Gallery.Count);
        }

        [Fact]
        public async Task Details_CallsCarSearchServiceWithCorrectId()
        {
            // Arrange
            var mockCarSearchService = new Mock<ICarSearchService>();
            var mockFilterService = new Mock<IFilterService>();

            var testId = 42;
            var carDto = new CarDto { CarId = testId, Vin = "TEST", Price = 10000m };

            mockCarSearchService
                .Setup(s => s.GetCarByIdAsync(testId))
                .ReturnsAsync(carDto);

            var controller = new HomeController(mockCarSearchService.Object, mockFilterService.Object);

            // Act
            await controller.Details(testId);

            // Assert - Verify the service was called with correct ID
            mockCarSearchService.Verify(s => s.GetCarByIdAsync(testId), Times.Once);
        }
    }
}
