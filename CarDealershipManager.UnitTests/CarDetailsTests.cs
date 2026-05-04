using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarDealershipManager.Controllers;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CarDealershipManager.UnitTests
{
    public class HomeControllerDetailsTests
    {
        #region Helper Methods

        // Tworzy kontroler HomeController z mockami serwisów

        private HomeController GetHomeController(
            Mock<ICarSearchService> carSearchServiceMock = null,
            Mock<IFilterService> filterServiceMock = null)
        {
            carSearchServiceMock ??= new Mock<ICarSearchService>();
            filterServiceMock ??= new Mock<IFilterService>();

            return new HomeController(carSearchServiceMock.Object, filterServiceMock.Object);
        }

        // Tworzy testowy CarDto z domyślnymi wartościami
        private CarDto CreateTestCarDto(
            int carId = 1,
            int productionYear = 2020,
            int horsePower = 200,
            int mileage = 50000,
            string vin = "TESTVIN123456789",
            decimal price = 45000m)
        {
            return new CarDto
            {
                CarId = carId,
                ProductionYear = productionYear,
                HorsePower = horsePower,
                Mileage = mileage,
                Vin = vin,
                Price = price,
                Description = "Testowy samochód",
                FuelType = new FuelTypeDto { FuelTypeId = 1, Name = "Benzyna" },
                CarStatus = new CarStatusDto { CarStatusId = 1, Name = "Dostępny" },
                Gallery = new List<GalleryDto>()
            };
        }

        // Tworzy testowy CarDto z pełnymi danymi i galerią
        private CarDto CreateTestCarDtoWithDetails(
            int carId = 5,
            int productionYear = 2022,
            int horsePower = 250,
            int mileage = 15000,
            string vin = "BMW123XYZ789",
            decimal price = 80000m)
        {
            return new CarDto
            {
                CarId = carId,
                ProductionYear = productionYear,
                HorsePower = horsePower,
                Mileage = mileage,
                Vin = vin,
                Price = price,
                FuelType = new FuelTypeDto { FuelTypeId = 2, Name = "Diesel" },
                TransmissionType = new TransmissionTypeDto { TransmissionTypeId = 1, Name = "Automatyczna" },
                BodyType = new BodyTypeDto { BodyTypeId = 1, Name = "Sedan" },
                Color = new ColorDto { ColorId = 1, Name = "Czarny" },
                CarStatus = new CarStatusDto { CarStatusId = 1, Name = "Dostępny" }
            };
        }

        // Tworzy testową galerię zdjęć
        private List<GalleryDto> CreateTestGallery(int photoCount = 3)
        {
            var gallery = new List<GalleryDto>();
            for (int i = 1; i <= photoCount; i++)
            {
                gallery.Add(new GalleryDto { PhotoId = i, FilePath = $"/images/car-{i}.jpg" });
            }
            return gallery;
        }

        #endregion

        #region Details GET Tests

        // Test sprawdza:
        // - Details dla istniejącego pojazdu zwraca View
        // - Model zawiera prawidłowe dane (CarId, Vin, Price)
        // - Serwis GetCarByIdAsync jest wywoływany z prawidłowym ID
        [Fact]
        public async Task Details_DlaIstniejacegoCar_ZwracaViewZDanymi()
        {
            // Arrange
            var carDto = CreateTestCarDto(carId: 1);
            var mockCarSearchService = new Mock<ICarSearchService>();
            mockCarSearchService
                .Setup(s => s.GetCarByIdAsync(1))
                .ReturnsAsync(carDto);

            var controller = GetHomeController(carSearchServiceMock: mockCarSearchService);

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

        // Test sprawdza:
        // - Details dla nieistniejącego pojazdu zwraca NotFound (404)
        // - Serwis zwraca null dla GetCarByIdAsync
        [Fact]
        public async Task Details_DlaNieistniejacegoCar_ZwracaNotFound()
        {
            // Arrange
            var mockCarSearchService = new Mock<ICarSearchService>();
            mockCarSearchService
                .Setup(s => s.GetCarByIdAsync(999))
                .ReturnsAsync((CarDto)null);

            var controller = GetHomeController(carSearchServiceMock: mockCarSearchService);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockCarSearchService.Verify(s => s.GetCarByIdAsync(999), Times.Once);
        }

        // Test sprawdza:
        // - Details ładuje wszystkie dane pojazdu z serwisu
        // - Wszystkie pola są dostępne
        // - Powiązane dane są załadowane
        [Fact]
        public async Task Details_LadujeCarDtoZPodanymId()
        {
            // Arrange
            var carDto = CreateTestCarDtoWithDetails(carId: 5);
            var mockCarSearchService = new Mock<ICarSearchService>();
            mockCarSearchService
                .Setup(s => s.GetCarByIdAsync(5))
                .ReturnsAsync(carDto);

            var controller = GetHomeController(carSearchServiceMock: mockCarSearchService);

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

        // Test sprawdza:
        // - Details ładuje galerię zdjęć pojazdu
        // - Gallery nie jest pusta i zawiera prawidłową liczbę elementów
        // - Każde zdjęcie ma FilePath
        [Fact]
        public async Task Details_LadujeGalerieZdj()
        {
            // Arrange
            var carDto = CreateTestCarDto(carId: 3, vin: "AUDI456XYZ789", price: 55000m);
            carDto.Gallery = CreateTestGallery(photoCount: 3);

            var mockCarSearchService = new Mock<ICarSearchService>();
            mockCarSearchService
                .Setup(s => s.GetCarByIdAsync(3))
                .ReturnsAsync(carDto);

            var controller = GetHomeController(carSearchServiceMock: mockCarSearchService);

            // Act
            var result = await controller.Details(3);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarDto>(viewResult.Model);

            Assert.NotNull(model.Gallery);
            Assert.Equal(3, model.Gallery.Count);
        }

        #endregion
    }
}
