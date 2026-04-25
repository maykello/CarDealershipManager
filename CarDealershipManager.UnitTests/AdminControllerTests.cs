using System.Collections.Generic;
using System.Threading.Tasks;
using CarDealershipManager.Controllers;
using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.ViewModels;
using CarDealershipManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;

namespace CarDealershipManager.UnitTests
{
    public class AdminControllerTests
    {
        private AdminController GetAdminController(
            Mock<IAuthService> authServiceMock = null,
            Mock<ICarAdminService> carAdminServiceMock = null)
        {
            authServiceMock ??= new Mock<IAuthService>();
            carAdminServiceMock ??= new Mock<ICarAdminService>();

            return new AdminController(authServiceMock.Object, carAdminServiceMock.Object);
        }

        private List<SelectItemDto> GetTestSelectItems()
        {
            return new List<SelectItemDto>
            {
                new SelectItemDto { Id = 1, Name = "Option1" },
                new SelectItemDto { Id = 2, Name = "Option2" }
            };
        }

        [Fact]
        public async Task Create_GET_ZwracaViewZPopulowanymViewModel()
        {
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();

            mockCarAdminService.Setup(s => s.GetMakesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetModelsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetGenerationsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetTransmissionTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetDrivetrainsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetBodyTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetEuroClassesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetColorsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetFuelTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetCarStatusesAsync())
                .ReturnsAsync(GetTestSelectItems());

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
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.CreateCarAsync(It.IsAny<CarDto>()))
                .ReturnsAsync(1);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            var viewModel = new CarFormViewModel
            {
                Car = new CarDto
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
                }
            };

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
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.GetMakesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetModelsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetGenerationsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetTransmissionTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetDrivetrainsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetBodyTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetEuroClassesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetColorsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetFuelTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetCarStatusesAsync())
                .ReturnsAsync(GetTestSelectItems());

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);
            controller.ModelState.AddModelError("Car.Vin", "VIN jest wymagany");

            var viewModel = new CarFormViewModel
            {
                Car = new CarDto
                {
                    ProductionYear = 2020,
                    HorsePower = 200,
                    Mileage = 50000,
                    Vin = "", // Empty VIN - invalid
                    Price = 45000m
                }
            };

            // Act
            var result = await controller.Create(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CarFormViewModel>(viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.NotNull(model.Makes);

            mockCarAdminService.Verify(s => s.CreateCarAsync(It.IsAny<CarDto>()), Times.Never);
        }

        [Fact]
        public async Task Edit_GET_DlaIstniejącegoCar_ZwracaViewZDanymi()
        {
            // Arrange
            var carDto = new CarDto
            {
                CarId = 1,
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

            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.GetCarByIdAsync(1))
                .ReturnsAsync(carDto);
            mockCarAdminService.Setup(s => s.GetMakesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetModelsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetGenerationsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetTransmissionTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetDrivetrainsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetBodyTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetEuroClassesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetColorsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetFuelTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetCarStatusesAsync())
                .ReturnsAsync(GetTestSelectItems());

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            // Act
            var result = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CarFormViewModel>(viewResult.Model);
            Assert.NotNull(model.Car);
            Assert.Equal(1, model.Car.CarId);
            Assert.Equal("TESTVIN123456789", model.Car.Vin);
        }

        [Fact]
        public async Task Edit_GET_DlaNieznajdującegoCar_ZwracaNotFound()
        {
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
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.CarExistsAsync(1))
                .ReturnsAsync(true);
            mockCarAdminService.Setup(s => s.UpdateCarAsync(It.IsAny<int>(), It.IsAny<CarDto>()))
                .Returns(Task.CompletedTask);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            var viewModel = new CarFormViewModel
            {
                Car = new CarDto
                {
                    CarId = 1,
                    ProductionYear = 2021,
                    HorsePower = 250,
                    Mileage = 55000,
                    Vin = "UPDATEDVIN",
                    Price = 50000m,
                    GenerationId = 1,
                    FuelTypeId = 1,
                    TransmissionTypeId = 1,
                    DrivetrainId = 1,
                    BodyTypeId = 1,
                    ColorId = 1,
                    EuroClassId = 1,
                    CarStatusId = 1
                }
            };

            // Act
            var result = await controller.Edit(1, viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

            mockCarAdminService.Verify(s => s.UpdateCarAsync(1, It.IsAny<CarDto>()), Times.Once);
        }

        [Fact]
        public async Task Edit_POST_ZNiepropadającymId_ZwracaBadRequest()
        {
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            var viewModel = new CarFormViewModel
            {
                Car = new CarDto
                {
                    CarId = 5, // Different ID than parameter
                    ProductionYear = 2020,
                    HorsePower = 200,
                    Mileage = 50000,
                    Vin = "TESTVIN",
                    Price = 45000m
                }
            };

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
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.GetMakesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetModelsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetGenerationsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetTransmissionTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetDrivetrainsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetBodyTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetEuroClassesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetColorsAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetFuelTypesAsync())
                .ReturnsAsync(GetTestSelectItems());
            mockCarAdminService.Setup(s => s.GetCarStatusesAsync())
                .ReturnsAsync(GetTestSelectItems());

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);
            controller.ModelState.AddModelError("Car.Vin", "VIN jest wymagany");

            var viewModel = new CarFormViewModel
            {
                Car = new CarDto
                {
                    CarId = 1,
                    ProductionYear = 2020,
                    HorsePower = 200,
                    Mileage = 50000,
                    Vin = "", // Invalid
                    Price = 45000m
                }
            };

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
            // Arrange
            var mockCarAdminService = new Mock<ICarAdminService>();
            mockCarAdminService.Setup(s => s.CarExistsAsync(999))
                .ReturnsAsync(false);

            var controller = GetAdminController(carAdminServiceMock: mockCarAdminService);

            var viewModel = new CarFormViewModel
            {
                Car = new CarDto
                {
                    CarId = 999,
                    ProductionYear = 2020,
                    HorsePower = 200,
                    Mileage = 50000,
                    Vin = "TESTVIN",
                    Price = 45000m
                }
            };

            // Act
            var result = await controller.Edit(999, viewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetModelsByMake_ZPoprawnyMakeId_ZwracaJsonZModelami()
        {
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
    }
}
