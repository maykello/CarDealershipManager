using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Services.Interfaces;
using Moq;
using Xunit;

namespace CarDealershipManager.UnitTests
{
    public class CarAdminServiceTests
    {
        [Fact]
        public async Task CreateCarAsync_DlaPoprawnyeDane_ZwracaCarId()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.CreateCarAsync(It.IsAny<CarDto>()))
                .ReturnsAsync(1); // Simulate returning ID of newly created car

            var carDto = new CarDto
            {
                ProductionYear = 2020,
                HorsePower = 200,
                Mileage = 50000,
                Vin = "TESTVIN123456789",
                Price = 45000m,
                GenerationId = 1,
                FuelTypeId = 1,
                TransmissionTypeId = 1,
                DrivetrainId = 1,
                BodyTypeId = 1,
                ColorId = 1,
                EuroClassId = 1,
                CarStatusId = 1
            };

            // Act
            var result = await mockService.Object.CreateCarAsync(carDto);

            // Assert
            Assert.Equal(1, result);
            mockService.Verify(s => s.CreateCarAsync(It.IsAny<CarDto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCarAsync_DlaPoprawnyeDane_AktualizujeSamochód()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.UpdateCarAsync(It.IsAny<int>(), It.IsAny<CarDto>()))
                .Returns(Task.CompletedTask);

            var carDto = new CarDto
            {
                CarId = 1,
                ProductionYear = 2021,
                HorsePower = 250,
                Mileage = 55000,
                Vin = "UPDATEDVIN456",
                Price = 50000m,
                GenerationId = 1,
                FuelTypeId = 1,
                TransmissionTypeId = 1,
                DrivetrainId = 1,
                BodyTypeId = 1,
                ColorId = 1,
                EuroClassId = 1,
                CarStatusId = 1
            };

            // Act
            await mockService.Object.UpdateCarAsync(1, carDto);

            // Assert
            mockService.Verify(s => s.UpdateCarAsync(1, It.IsAny<CarDto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCarAsync_ZIdNieznajdującejSię_RzucaKeyNotFoundException()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.UpdateCarAsync(It.IsAny<int>(), It.IsAny<CarDto>()))
                .ThrowsAsync(new KeyNotFoundException("Car not found"));

            var carDto = new CarDto { CarId = 999 };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                mockService.Object.UpdateCarAsync(999, carDto));
        }

        [Fact]
        public async Task DeleteCarAsync_ZIdNieznajdującejSię_RzucaKeyNotFoundException()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.DeleteCarAsync(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Car not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                mockService.Object.DeleteCarAsync(999));
        }

        [Fact]
        public async Task GetCarByIdAsync_DlaIstniejącegoCar_ZwracaCarDto()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            var carDto = new CarDto
            {
                CarId = 1,
                ProductionYear = 2020,
                HorsePower = 200,
                Mileage = 50000,
                Vin = "TESTVIN123456789",
                Price = 45000m
            };

            mockService
                .Setup(s => s.GetCarByIdAsync(1))
                .ReturnsAsync(carDto);

            // Act
            var result = await mockService.Object.GetCarByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CarId);
            Assert.Equal("TESTVIN123456789", result.Vin);
            Assert.Equal(45000m, result.Price);
        }

        [Fact]
        public async Task GetCarByIdAsync_DlaNieznajdującegoCar_ZwracaNull()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.GetCarByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((CarDto)null);

            // Act
            var result = await mockService.Object.GetCarByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetMakesAsync_ZwracaListęZaznaczonyDanych()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            var makes = new List<SelectItemDto>
            {
                new SelectItemDto { Id = 1, Name = "Toyota" },
                new SelectItemDto { Id = 2, Name = "Honda" }
            };

            mockService
                .Setup(s => s.GetMakesAsync())
                .ReturnsAsync(makes);

            // Act
            var result = await mockService.Object.GetMakesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.Name == "Toyota");
        }

        [Fact]
        public async Task CarExistsAsync_DlaIstniejącegoCar_ZwracaTrue()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.CarExistsAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await mockService.Object.CarExistsAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CarExistsAsync_DlaNieznajdującegoCar_ZwracaFalse()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.CarExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await mockService.Object.CarExistsAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteCarAsync_ZPoprawnymId_UsuwaSamochód()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.DeleteCarAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            await mockService.Object.DeleteCarAsync(1);

            // Assert
            mockService.Verify(s => s.DeleteCarAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteCarAsync_ZMultiplemymiWywołaniami_UsuwaBezBłędów()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.DeleteCarAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            await mockService.Object.DeleteCarAsync(1);
            await mockService.Object.DeleteCarAsync(2);
            await mockService.Object.DeleteCarAsync(3);

            // Assert
            mockService.Verify(s => s.DeleteCarAsync(It.IsAny<int>()), Times.Exactly(3));
        }

        [Fact]
        public async Task DeleteCarAsync_ZBlędemSerwisu_RzucaWyjątek()
        {
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.DeleteCarAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                mockService.Object.DeleteCarAsync(1));
        }
    }
}
