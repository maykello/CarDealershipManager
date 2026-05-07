using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace CarDealershipManager.Services
{
    public class ContractService : IContractService
    {
        private readonly CarDealershipDbContext _context;

        public ContractService(CarDealershipDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateContractNumberAsync()
        {
            var year = DateTime.Now.Year;
            var lastContractNumber = await Task.Run(() =>
                _context.Contracts
                    .Where(c => c.ContractNumber.StartsWith($"UMS-{year}"))
                    .OrderByDescending(c => c.ContractNumber)
                    .FirstOrDefault()?.ContractNumber
            );

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastContractNumber))
            {
                var parts = lastContractNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"UMS-{year}-{nextNumber:D6}";
        }

        public async Task<byte[]> GenerateContractPdfAsync(CarDto car, decimal? price = null)
        {
            return await Task.Run(() =>
            {
                using (var document = new PdfDocument())
                {
                    var page = document.AddPage();
                    using (var gfx = XGraphics.FromPdfPage(page))
                    {
                        var titleFont = new XFont("Arial", 18);
                        var headingFont = new XFont("Arial", 12);
                        var normalFont = new XFont("Arial", 10);
                        var smallFont = new XFont("Arial", 9);

                        int yPosition = 50;
                        const int lineHeight = 20;
                        const int margin = 50;
                        const int pageWidth = 595; // A4 width in points

                        // Title
                        gfx.DrawString("UMOWA KUPNA-SPRZEDAŻY POJAZDU",
                            titleFont,
                            XBrushes.Black,
                            new XRect(margin, yPosition, pageWidth - 2 * margin, lineHeight),
                            XStringFormats.TopCenter);
                        yPosition += lineHeight + 15;

                        // Contract Number and Date
                        var contractNum = $"Numer umowy: {DateTime.Now.Ticks}";
                        gfx.DrawString(contractNum, normalFont, XBrushes.Black, margin, yPosition);
                        yPosition += lineHeight;

                        var contractDate = $"Data zawarcia: {DateTime.Now:dd.MM.yyyy}";
                        gfx.DrawString(contractDate, normalFont, XBrushes.Black, margin, yPosition);
                        yPosition += lineHeight + 15;

                        // Vehicle Information Section
                        gfx.DrawString("DANE POJAZDU", headingFont, XBrushes.Black, margin, yPosition);
                        yPosition += lineHeight + 10;

                        // Table-like structure with two columns
                        var labelWidth = 200;
                        var valueXPosition = margin + labelWidth;

                        // VIN
                        gfx.DrawString("Numer VIN:", normalFont, XBrushes.Black, margin, yPosition);
                        gfx.DrawString(car.Vin ?? "N/A", normalFont, XBrushes.Black, valueXPosition, yPosition);
                        yPosition += lineHeight;

                        // Production Year
                        gfx.DrawString("Rok produkcji:", normalFont, XBrushes.Black, margin, yPosition);
                        gfx.DrawString(car.ProductionYear.ToString(), normalFont, XBrushes.Black, valueXPosition, yPosition);
                        yPosition += lineHeight;

                        // Mileage
                        gfx.DrawString("Przebieg:", normalFont, XBrushes.Black, margin, yPosition);
                        gfx.DrawString($"{car.Mileage:N0} km", normalFont, XBrushes.Black, valueXPosition, yPosition);
                        yPosition += lineHeight;

                        // Horse Power
                        gfx.DrawString("Moc silnika:", normalFont, XBrushes.Black, margin, yPosition);
                        gfx.DrawString($"{car.HorsePower} KM", normalFont, XBrushes.Black, valueXPosition, yPosition);
                        yPosition += lineHeight;

                        // Fuel Type
                        gfx.DrawString("Typ paliwa:", normalFont, XBrushes.Black, margin, yPosition);
                        gfx.DrawString(car.FuelType?.Name ?? "N/A", normalFont, XBrushes.Black, valueXPosition, yPosition);
                        yPosition += lineHeight;

                        // Transmission Type
                        gfx.DrawString("Typ transmisji:", normalFont, XBrushes.Black, margin, yPosition);
                        gfx.DrawString(car.TransmissionType?.Name ?? "N/A", normalFont, XBrushes.Black, valueXPosition, yPosition);
                        yPosition += lineHeight;

                        // Drivetrain
                        gfx.DrawString("Napęd:", normalFont, XBrushes.Black, margin, yPosition);
                        gfx.DrawString(car.Drivetrain?.Name ?? "N/A", normalFont, XBrushes.Black, valueXPosition, yPosition);
                        yPosition += lineHeight;

                        // Body Type
                        gfx.DrawString("Typ nadwozia:", normalFont, XBrushes.Black, margin, yPosition);
                        gfx.DrawString(car.BodyType?.Name ?? "N/A", normalFont, XBrushes.Black, valueXPosition, yPosition);
                        yPosition += lineHeight;

                        // Color
                        gfx.DrawString("Kolor:", normalFont, XBrushes.Black, margin, yPosition);
                        gfx.DrawString(car.Color?.Name ?? "N/A", normalFont, XBrushes.Black, valueXPosition, yPosition);
                        yPosition += lineHeight + 15;

                        // Price Section
                        var priceText = (price ?? car.Price).ToString("C");
                        var priceLabel = new XFont("Arial", 14);
                        gfx.DrawString($"CENA: {priceText}", priceLabel, XBrushes.Black, margin, yPosition);
                        yPosition += 30;

                        // Footer
                        var footerText = "Niniejsza umowa stanowi dokument potwierdzający sprzedaż pojazdu o podanych wyżej parametrach.";
                        var footerRect = new XRect(margin, yPosition, pageWidth - 2 * margin, 60);
                        gfx.DrawString(footerText, smallFont, XBrushes.Black, footerRect, XStringFormats.TopLeft);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        document.Save(memoryStream, false);
                        return memoryStream.ToArray();
                    }
                }
            });
        }

        public async Task<ContractDto?> GetContractByIdAsync(int id)
        {
            var contract = await Task.Run(() =>
                _context.Contracts.FirstOrDefault(c => c.Id == id)
            );

            if (contract == null)
                return null;

            return new ContractDto
            {
                ContractId = contract.Id,
                ContractNumber = contract.ContractNumber,
                TransactionDate = contract.TransactionDate,
                Price = contract.Price,
                TransactionType = contract.Type
            };
        }

        public async Task<int> CreateContractAsync(ContractDto contractDto)
        {
            //TO-DO
            return await Task.FromResult(0);
        }
    }
}
