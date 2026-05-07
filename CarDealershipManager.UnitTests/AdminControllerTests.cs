using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarDealershipManager.Controllers;
using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.ViewModels;
using CarDealershipManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;

namespace CarDealershipManager.UnitTests
{
    public class AdminControllerTests
    {
        #region Helper Methods
        // Tworzy kontroler AdminController z mockami serwisów

        private AdminController GetAdminController(
            Mock<IAuthService> authServiceMock = null,
            Mock<ICarAdminService> carAdminServiceMock = null,
            Mock<IContractService> contractServiceMock = null)
        {
            authServiceMock ??= new Mock<IAuthService>();
            carAdminServiceMock ??= new Mock<ICarAdminService>();
            contractServiceMock ??= new Mock<IContractService>();

            return new AdminController(authServiceMock.Object, carAdminServiceMock.Object, contractServiceMock.Object);
        }
        // Tworzy testowe dane SelectItem
        private List<SelectItemDto> GetTestSelectItems()
        {
            return new List<SelectItemDto>
            {
                new SelectItemDto { Id = 1, Name = "Option1" },
                new SelectItemDto { Id = 2, Name = "Option2" }
            };
        }

        // Setup mock serwis ze wszystkimi listami SelectList
        private Mock<ICarAdminService> SetupCarAdminServiceWithAllLists()
        {
            var mockCarAdminService = new Mock<ICarAdminService>();
            var testItems = GetTestSelectItems();

            mockCarAdminService.Setup(s => s.GetMakesAsync()).ReturnsAsync(testItems);
            mockCarAdminService.Setup(s => s.GetModelsAsync()).ReturnsAsync(testItems);
            mockCarAdminService.Setup(s => s.GetGenerationsAsync()).ReturnsAsync(testItems);
            mockCarAdminService.Setup(s => s.GetTransmissionTypesAsync()).ReturnsAsync(testItems);
            mockCarAdminService.Setup(s => s.GetDrivetrainsAsync()).ReturnsAsync(testItems);
            mockCarAdminService.Setup(s => s.GetBodyTypesAsync()).ReturnsAsync(testItems);
            mockCarAdminService.Setup(s => s.GetEuroClassesAsync()).ReturnsAsync(testItems);
            mockCarAdminService.Setup(s => s.GetColorsAsync()).ReturnsAsync(testItems);
            mockCarAdminService.Setup(s => s.GetFuelTypesAsync()).ReturnsAsync(testItems);
            mockCarAdminService.Setup(s => s.GetCarStatusesAsync()).ReturnsAsync(testItems);

            return mockCarAdminService;
        }
        // Tworzy testowy CarDto z domyślnymi wartościami
        private CarDto CreateTestCarDto(int carId = 0, string vin = "TESTVIN123456789", decimal price = 45000m)
        {
            return new CarDto
            {
                CarId = carId,
                ProductionYear = 2020,
                HorsePower = 200,
                Mileage = 50000,
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

        // Tworzy testowy CarFormViewModel z domyślnymi wartościami
        private CarFormViewModel CreateTestCarFormViewModel(CarDto carDto = null)
        {
            return new CarFormViewModel
            {
                Car = carDto ?? CreateTestCarDto()
            };
        }

        #endregion

        #region Create Tests

        // Test sprawdza:
        // - GET Create zwraca View
        // - ViewModel jest zapatrywany danymi SelectList (Make, Model, Generation, etc.)
        // - Wszystkie 10 list jest załadowanych
        [Fact]
        public async Task Create_GET_ZwracaViewZPopulowanymViewModel()
        {
            // Arrange
            var mockCarAdminService = SetupCarAdminServiceWithAllLists();
            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CarFormViewModel>(viewResult.Model);
            Assert.NotNull(model.Makes);
            Assert.NotNull(model.Models);
            Assert.NotNull(model.Generations);

            mockCarAdminService.Verify(s => s.GetMakesAsync(), Times.Once);
            mockCarAdminService.Verify(s => s.GetFuelTypesAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_POST_ZPoprawnymiDanymi_PrzekierowujeNaHome()
        {
            // Test sprawdza:
            // - POST Create z poprawnymi danymi wywołuje CreateCarAsync
            // - Serwis otrzymuje prawidłowy CarDto
            // - Następuje przekierowanie na Home/Index
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.CreateCarAsync(It.IsAny<CarDto>()))
                .ReturnsAsync(1);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);
            var viewModel = CreateTestCarFormViewModel();

            // Act
            var result = await controller.Create(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

            mockCarAdminService.Verify(s => s.CreateCarAsync(It.IsAny<CarDto>()), Times.Once);
        }

        [Fact]
        public async Task Create_POST_ZNiepoprawnymModelem_ZwracaViewZRepopulowanymViewModel()
        {
            // Test sprawdza:
            // - POST Create z błędem walidacji zwraca View
            // - Nie wywołuje CreateCarAsync
            // - ViewModel jest repopulowany danymi SelectList
            // Arrange
            var mockCarAdminService = SetupCarAdminServiceWithAllLists();
            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);
            controller.ModelState.AddModelError("Car.Vin", "VIN jest wymagany");

            var viewModel = CreateTestCarFormViewModel();
            viewModel.Car.Vin = "";

            // Act
            var result = await controller.Create(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CarFormViewModel>(viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.NotNull(model.Makes);

            mockCarAdminService.Verify(s => s.CreateCarAsync(It.IsAny<CarDto>()), Times.Never);
        }

        #endregion

        #region Edit Tests

        [Fact]
        public async Task Edit_GET_DlaNieznajdującegoCar_ZwracaNotFound()
        {
            // Test sprawdza:
            // - GET Edit dla ID, które nie istnieje zwraca NotFound (404)
            // - Serwis zwraca null dla GetCarByIdAsync
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.GetCarByIdAsync(999))
                .ReturnsAsync((CarDto)null);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_POST_ZPoprawnymiDanymi_AktualizujeiPrzekierowuje()
        {
            // Test sprawdza:
            // - POST Edit z poprawnymi danymi wywołuje UpdateCarAsync
            // - Pojazd jest aktualizowany w bazie
            // - Następuje przekierowanie na Home/Index
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.CarExistsAsync(1))
                .ReturnsAsync(true);
            mockCarAdminService.Setup(s => s.UpdateCarAsync(It.IsAny<int>(), It.IsAny<CarDto>()))
                .Returns(Task.CompletedTask);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);
            var viewModel = CreateTestCarFormViewModel(CreateTestCarDto(carId: 1));

            // Act
            var result = await controller.Edit(1, viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
            Assert.Equal(1, redirectResult.RouteValues["id"]);

            mockCarAdminService.Verify(s => s.UpdateCarAsync(1, It.IsAny<CarDto>(), It.IsAny<List<Microsoft.AspNetCore.Http.IFormFile>>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Edit_POST_ZNiepropadającymId_ZwracaBadRequest()
        {
            // Test sprawdza:
            // - POST Edit z niezgodnym ID zwraca BadRequest (400)
            // - Nie wysyła UpdateCarAsync do serwisu
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            var viewModel = CreateTestCarFormViewModel(CreateTestCarDto(carId: 5));

            // Act
            var result = await controller.Edit(1, viewModel);

            // Assert
            Assert.IsType<BadRequestResult>(result);

            mockCarAdminService.Verify(s => s.UpdateCarAsync(It.IsAny<int>(), It.IsAny<CarDto>()), 
                Times.Never);
        }

        [Fact]
        public async Task Edit_POST_ZNiepoprawnymModelem_ZwracaViewZRepopulowanymViewModel()
        {
            // Test sprawdza:
            // - POST Edit z błędem walidacji zwraca View
            // - Nie wysyła UpdateCarAsync do serwisu
            // - ViewModel jest repopulowany danymi SelectList
            // Arrange
            var mockCarAdminService = SetupCarAdminServiceWithAllLists();
            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);
            controller.ModelState.AddModelError("Car.Vin", "VIN jest wymagany");

            var viewModel = CreateTestCarFormViewModel(CreateTestCarDto(carId: 1));
            viewModel.Car.Vin = "";

            // Act
            var result = await controller.Edit(1, viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CarFormViewModel>(viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.NotNull(model.Makes);
        }

        [Fact]
        public async Task Edit_POST_DlaNieznajdującegoCar_ZwracaNotFound()
        {
            // Test sprawdza:
            // - POST Edit dla samochodu, który nie istnieje zwraca NotFound (404)
            // - Serwis zwraca false dla CarExistsAsync
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.CarExistsAsync(999))
                .ReturnsAsync(false);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);
            var viewModel = CreateTestCarFormViewModel(CreateTestCarDto(carId: 999));

            // Act
            var result = await controller.Edit(999, viewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_POST_ZPoprawnymId_UsuwaSamochodIPrzekierowuje()
        {
            // Test sprawdza:
            // - POST Delete z poprawnymi danymi usuwa samochód
            // - Serwis wywoła DeleteCarAsync
            // - Następuje przekierowanie na Home/Index
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.DeleteCarAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.Delete(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

            mockCarAdminService.Verify(s => s.DeleteCarAsync(1), Times.Once);
        }

        [Fact]
        public async Task Delete_POST_ZIdNieznajdującegoCar_ZwracaNotFound()
        {
            // Test sprawdza:
            // - POST Delete dla ID, które nie istnieje zwraca NotFound (404)
            // - Serwis wyrzuca KeyNotFoundException
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.DeleteCarAsync(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Car not found"));

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            mockCarAdminService.Verify(s => s.DeleteCarAsync(999), Times.Once);
        }

        #endregion

        #region Helper Endpoints Tests

        [Fact]
        public async Task GetModelsByMake_ZPoprawnyMakeId_ZwracaJsonZModelami()
        {
            // Test sprawdza:
            // - GET GetModelsByMake zwraca JSON z modelami dla podanego MakeId
            // - Serwis wywoła GetModelsByMakeAsync
            // Arrange
            var models = GetTestSelectItems();
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.GetModelsByMakeAsync(1))
                .ReturnsAsync(models);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.GetModelsByMake(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);

            mockCarAdminService.Verify(s => s.GetModelsByMakeAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetGenerationsByModel_ZPoprawnyModelId_ZwracaJsonZGeneracjami()
        {
            // Test sprawdza:
            // - GET GetGenerationsByModel zwraca JSON z generacjami dla podanego ModelId
            // - Serwis wywoła GetGenerationsByModelAsync
            // Arrange
            var generations = GetTestSelectItems();
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.GetGenerationsByModelAsync(1))
                .ReturnsAsync(generations);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.GetGenerationsByModel(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);

            mockCarAdminService.Verify(s => s.GetGenerationsByModelAsync(1), Times.Once);
        }

        #endregion

        #region Photo Management Endpoints Tests

        [Fact]
        public async Task DeletePhoto_POST_ZPoprawnymId_ZwracaOk()
        {
            // Test sprawdza:
            // - POST DeletePhoto z poprawnym ID zwraca status OK
            // - Serwis zwraca true dla operacji usunięcia
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.DeletePhotoByIdAsync(1))
                .ReturnsAsync(true);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.DeletePhoto(1);

            // Assert
            Assert.IsType<OkResult>(result);
            mockCarAdminService.Verify(s => s.DeletePhotoByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeletePhoto_POST_ZBlednymId_ZwracaBadRequest()
        {
            // Test sprawdza:
            // - POST DeletePhoto z błędnym ID zwraca status BadRequest
            // - Serwis zwraca false dla operacji usunięcia
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.DeletePhotoByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.DeletePhoto(999);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            mockCarAdminService.Verify(s => s.DeletePhotoByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task SetMainPhoto_POST_ZPoprawnymId_ZwracaOk()
        {
            // Test sprawdza:
            // - POST SetMainPhoto z poprawnymi ID auta i zdjęcia zwraca status OK
            // - Serwis zwraca true dla operacji ustawienia zdjęcia głównego
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.SetMainPhotoAsync(1, 2))
                .ReturnsAsync(true);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.SetMainPhoto(1, 2);

            // Assert
            Assert.IsType<OkResult>(result);
            mockCarAdminService.Verify(s => s.SetMainPhotoAsync(1, 2), Times.Once);
        }

        [Fact]
        public async Task SetMainPhoto_POST_ZBlednymId_ZwracaBadRequest()
        {
            // Test sprawdza:
            // - POST SetMainPhoto z błędnym ID zdjęcia/auta zwraca status BadRequest
            // - Serwis zwraca false dla operacji ustawienia zdjęcia głównego
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.SetMainPhotoAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.SetMainPhoto(999, 999);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            mockCarAdminService.Verify(s => s.SetMainPhotoAsync(999, 999), Times.Once);
        }

        #endregion
    }
}
