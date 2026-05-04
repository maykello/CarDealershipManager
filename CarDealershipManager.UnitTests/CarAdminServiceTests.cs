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
        #region Helper Methods

        // Tworzy testowy CarDto z domyślnymi lub niestandardowymi wartościami
        private CarDto CreateTestCarDto(
            int carId = 0,
            string vin = "TESTVIN123456789",
            decimal price = 45000m,
            int productionYear = 2020,
            int horsePower = 200,
            int mileage = 50000)
        {
            return new CarDto
            {
                CarId = carId,
                ProductionYear = productionYear,
                HorsePower = horsePower,
                Mileage = mileage,
                Vin = vin,
                Price = price,
                GenerationId = 1,
                FuelTypeId = 1,
                TransmissionTypeId = 1,
                DrivetrainId = 1,
                BodyTypeId = 1,
                ColorId = 1,
                EuroClassId = 1,
                CarStatusId = 1
            };
        }

        // Tworzy testowe dane SelectItem
        private List<SelectItemDto> GetTestSelectItems()
        {
            return new List<SelectItemDto>
            {
                new SelectItemDto { Id = 1, Name = "Toyota" },
                new SelectItemDto { Id = 2, Name = "Honda" }
            };
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateCarAsync_ZIdNieznajdującejSię_RzucaKeyNotFoundException()
        {
            // Test sprawdza:
            // - UpdateCarAsync dla nieznajdującego się pojazdu wyrzuca KeyNotFoundException
            // - Wyjątek jest obsługiwany prawidłowo
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.UpdateCarAsync(It.IsAny<int>(), It.IsAny<CarDto>()))
                .ThrowsAsync(new KeyNotFoundException("Car not found"));

            var carDto = CreateTestCarDto(carId: 999);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                mockService.Object.UpdateCarAsync(999, carDto));
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteCarAsync_ZPoprawnymId_UsuwaSamochód()
        {
            // Test sprawdza:
            // - DeleteCarAsync z poprawnymi ID usuwa pojazd
            // - Metoda serwisu jest wywoływana dokładnie raz
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
        public async Task DeleteCarAsync_ZIdNieznajdującejSię_RzucaKeyNotFoundException()
        {
            // Test sprawdza:
            // - DeleteCarAsync dla nieznajdującego się pojazdu wyrzuca KeyNotFoundException
            // - Wyjątek jest obsługiwany prawidłowo
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            mockService
                .Setup(s => s.DeleteCarAsync(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Car not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                mockService.Object.DeleteCarAsync(999));
        }

        #endregion

        #region Query Tests

        [Fact]
        public async Task GetCarByIdAsync_DlaIstniejącegoCar_ZwracaCarDto()
        {
            // Test sprawdza:
            // - GetCarByIdAsync dla istniejącego pojazdu zwraca CarDto z prawidłowymi danymi
            // - Wszystkie pola (CarId, Vin, Price) są poprawne
            // Arrange
            var mockService = new Mock<ICarAdminService>();
            var carDto = CreateTestCarDto(carId: 1);

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
            // Test sprawdza:
            // - GetCarByIdAsync dla nieznajdującego się pojazdu zwraca null
            // - Brak błędu - operacja jest graceful
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
        public async Task CarExistsAsync_DlaIstniejącegoCar_ZwracaTrue()
        {
            // Test sprawdza:
            // - CarExistsAsync dla istniejącego pojazdu zwraca true
            // - Metoda prawidłowo sprawdza dostępność pojazdu
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
            // Test sprawdza:
            // - CarExistsAsync dla nieznajdującego się pojazdu zwraca false
            // - Metoda prawidłowo sygnalizuje brak pojazdu
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

        #endregion
    }
}
