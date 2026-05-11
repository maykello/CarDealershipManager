using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;
using CarDealershipManager.Models.ViewModels;
using CarDealershipManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var now = DateTime.Now;
            var prefix = $"FV/{now.Month:D2}/{now.Year}";
            var lastNumber = await Task.Run(() =>
                _context.Contracts
                    .Where(c => c.ContractNumber.StartsWith(prefix))
                    .OrderByDescending(c => c.ContractNumber)
                    .FirstOrDefault()?.ContractNumber
            );

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastNumber))
            {
                var parts = lastNumber.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int last))
                {
                    nextNumber = last + 1;
                }
            }

            return $"{prefix}/{nextNumber:D4}";
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(string invoiceNumber, CarDto car, CustomerModel buyer, CustomerModel seller, decimal price, PaymentMethod paymentMethod, string? bankAccountNumber)
        {
            return await Task.Run(() =>
            {
                using var document = new PdfDocument();
                document.Info.Title = "Faktura VAT";
                var page = document.AddPage();
                page.Width = XUnit.FromMillimeter(210);
                page.Height = XUnit.FromMillimeter(297);

                using var gfx = XGraphics.FromPdfPage(page);

                // ─── CZCIONKI ───
                var fontTitle = new XFont("Arial", 14, XFontStyleEx.Bold);
                var fontSection = new XFont("Arial", 10, XFontStyleEx.Bold);
                var fontNormal = new XFont("Arial", 8.5, XFontStyleEx.Regular);
                var fontBold = new XFont("Arial", 8.5, XFontStyleEx.Bold);
                var fontSmall = new XFont("Arial", 7, XFontStyleEx.Regular);
                var fontBig = new XFont("Arial", 12, XFontStyleEx.Bold);

                // ─── KOLORY ───
                var navyBlue = XColor.FromArgb(25, 25, 112);
                var lightGray = XColor.FromArgb(240, 240, 240);
                var mediumGray = XColor.FromArgb(200, 200, 200);

                double margin = 40;
                double pageW = page.Width.Point;
                double contentW = pageW - 2 * margin;
                double y = margin;
                double col2X = margin + contentW / 2 + 15;
                double halfW = contentW / 2 - 15;

                // ═══════════════════════════════════════════════════
                // NAGŁÓWEK: Tytuł + Data sprzedaży
                // ═══════════════════════════════════════════════════
                gfx.DrawString($"FAKTURA VAT nr {invoiceNumber}", fontTitle, XBrushes.Black, margin, y + 14);
                
                var today = DateTime.Now.ToString("dd.MM.yyyy");
                gfx.DrawString($"Data sprzedaży: {today}", fontBold, XBrushes.Black, pageW - margin - 130, y + 14);
                y += 22;

                gfx.DrawString($"Data wystawienia: {today}", fontNormal, XBrushes.Black, margin, y + 10);
                y += 14;
                gfx.DrawString($"Miejsce wystawienia: {seller.City ?? ""}", fontNormal, XBrushes.Black, margin, y + 10);
                y += 22;

                // Linia oddzielająca
                gfx.DrawLine(new XPen(XColors.Black, 0.5), margin, y, pageW - margin, y);
                y += 14;

                // ═══════════════════════════════════════════════════
                // SPRZEDAWCA / NABYWCA (dwie kolumny)
                // ═══════════════════════════════════════════════════
                gfx.DrawString("SPRZEDAWCA", fontSection, new XSolidBrush(navyBlue), margin, y);
                gfx.DrawString("NABYWCA", fontSection, new XSolidBrush(navyBlue), col2X, y);
                y += 16;

                var sellerName = seller.Type == CustomerType.Company ? seller.CompanyName : $"{seller.FirstName} {seller.LastName}";
                var buyerName = buyer.Type == CustomerType.Company ? buyer.CompanyName : $"{buyer.FirstName} {buyer.LastName}";

                gfx.DrawString(sellerName ?? "", fontBold, XBrushes.Black, margin, y);
                gfx.DrawString(buyerName ?? "", fontBold, XBrushes.Black, col2X, y);
                y += 13;

                gfx.DrawString(seller.AddressLine1 ?? "", fontNormal, XBrushes.Black, margin, y);
                gfx.DrawString(buyer.AddressLine1 ?? "", fontNormal, XBrushes.Black, col2X, y);
                y += 12;

                gfx.DrawString($"{seller.PostalCode} {seller.City}", fontNormal, XBrushes.Black, margin, y);
                gfx.DrawString($"{buyer.PostalCode} {buyer.City}", fontNormal, XBrushes.Black, col2X, y);
                y += 12;

                var sellerTax = seller.Type == CustomerType.Company ? $"NIP: {seller.TaxId}" : $"PESEL: {seller.NationalIdNumber}";
                var buyerTax = buyer.Type == CustomerType.Company ? $"NIP: {buyer.TaxId}" : $"";
                gfx.DrawString(sellerTax, fontBold, XBrushes.Black, margin, y);
                gfx.DrawString(buyerTax, fontBold, XBrushes.Black, col2X, y);
                y += 22;

                // ═══════════════════════════════════════════════════
                // SZCZEGÓŁY POJAZDU (szara ramka)
                // ═══════════════════════════════════════════════════
                var make = car.Generation?.Model?.Make?.Name ?? "";
                var model = car.Generation?.Model?.Name ?? "";
                var gen = car.Generation?.Name ?? "";

                double vehicleBoxH = 60;
                gfx.DrawRectangle(new XPen(mediumGray, 0.5), new XSolidBrush(lightGray), margin, y, contentW, vehicleBoxH);
                y += 4;

                gfx.DrawString("SZCZEGÓŁY POJAZDU", fontSection, new XSolidBrush(navyBlue), margin + 8, y + 10);
                y += 18;

                double vCol1 = margin + 8;
                double vCol2 = margin + contentW / 3;
                double vCol3 = margin + 2 * contentW / 3;

                gfx.DrawString($"Marka i model: {make} {model} {gen}", fontBold, XBrushes.Black, vCol1, y + 10);
                gfx.DrawString($"Rok produkcji: {car.ProductionYear}", fontNormal, XBrushes.Black, vCol2, y + 10);
                gfx.DrawString($"VIN: {car.Vin}", fontNormal, XBrushes.Black, vCol3, y + 10);
                y += 14;

                gfx.DrawString($"Przebieg: {car.Mileage:N0} km", fontNormal, XBrushes.Black, vCol1, y + 10);
                gfx.DrawString($"Paliwo: {car.FuelType?.Name ?? ""}", fontNormal, XBrushes.Black, vCol2, y + 10);
                gfx.DrawString($"Kolor: {car.Color?.Name ?? ""}", fontNormal, XBrushes.Black, vCol3, y + 10);

                y += vehicleBoxH - 32 + 14; // Jump past box

                // ═══════════════════════════════════════════════════
                // TABELA POZYCJI
                // ═══════════════════════════════════════════════════
                y += 10;
                double tableX = margin;
                double rowH = 18;

                // Kolumny: Lp | Nazwa | Ilość | Cena netto | Stawka VAT | Kwota VAT | Wartość brutto
                double[] colW = { 25, 175, 35, 70, 55, 60, 70 };
                string[] headers = { "Lp.", "Nazwa towaru / usługi", "Ilość", "Cena netto", "Stawka VAT", "Kwota VAT", "Wartość brutto" };

                // Nagłówek tabeli — granatowe tło
                gfx.DrawRectangle(new XSolidBrush(navyBlue), tableX, y, contentW, rowH);

                double hx = tableX;
                var whiteFont = new XFont("Arial", 7.5, XFontStyleEx.Bold);
                for (int i = 0; i < headers.Length; i++)
                {
                    gfx.DrawString(headers[i], whiteFont, XBrushes.White, new XRect(hx + 3, y + 3, colW[i] - 6, rowH - 6), XStringFormats.CenterLeft);
                    hx += colW[i];
                }
                y += rowH;

                // Linia pod nagłówkiem
                gfx.DrawLine(new XPen(navyBlue, 0.5), tableX, y, tableX + contentW, y);

                // Wiersz danych
                decimal vatRate = 23m;
                decimal netto = Math.Round(price / (1 + vatRate / 100), 2);
                decimal vatAmount = price - netto;

                var itemName = $"Samochód osobowy: {make} {model}";

                string[] rowVals = {
                    "1",
                    itemName,
                    "1 szt.",
                    $"{netto:N2}",
                    $"{vatRate:N0}%",
                    $"{vatAmount:N2}",
                    $"{price:N2}"
                };

                // Jasne tło wiersza
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(250, 250, 250)), tableX, y, contentW, rowH);

                hx = tableX;
                for (int i = 0; i < rowVals.Length; i++)
                {
                    var fmt = i <= 2 ? XStringFormats.CenterLeft : XStringFormats.CenterRight;
                    gfx.DrawString(rowVals[i], fontNormal, XBrushes.Black, new XRect(hx + 3, y + 2, colW[i] - 6, rowH - 4), fmt);
                    hx += colW[i];
                }
                y += rowH;

                // Dolna linia tabeli
                gfx.DrawLine(new XPen(navyBlue, 0.5), tableX, y, tableX + contentW, y);
                y += 20;

                // ═══════════════════════════════════════════════════
                // PODSUMOWANIE (wyrównane do prawej)
                // ═══════════════════════════════════════════════════
                double sumLabelX = pageW - margin - 200;
                double sumValueX = pageW - margin - 80;

                gfx.DrawString("Razem netto:", fontNormal, XBrushes.Black, sumLabelX, y);
                gfx.DrawString($"{netto:N2} PLN", fontNormal, XBrushes.Black, sumValueX, y);
                y += 14;

                gfx.DrawString($"VAT ({vatRate:N0}%):", fontNormal, XBrushes.Black, sumLabelX, y);
                gfx.DrawString($"{vatAmount:N2} PLN", fontNormal, XBrushes.Black, sumValueX, y);
                y += 14;

                gfx.DrawLine(new XPen(XColors.Black, 0.5), sumLabelX, y, pageW - margin, y);
                y += 15;

                gfx.DrawString("DO ZAPŁATY:", fontBig, XBrushes.Black, sumLabelX, y + 3);
                gfx.DrawString($"{price:N2} PLN", fontBig, XBrushes.Black, sumValueX, y + 3);
                y += 30;

                // ═══════════════════════════════════════════════════
                // PŁATNOŚĆ
                // ═══════════════════════════════════════════════════
                gfx.DrawLine(new XPen(XColors.Black, 0.5), margin, y, pageW - margin, y);
                y += 12;

                var paymentLabel = paymentMethod == PaymentMethod.Gotowka ? "Gotówka" : "Przelew bankowy";
                gfx.DrawString($"Sposób płatności:  {paymentLabel}", fontBold, XBrushes.Black, margin, y);
                gfx.DrawString($"Termin płatności:  {today}", fontNormal, XBrushes.Black, col2X, y);
                y += 14;

                if (paymentMethod == PaymentMethod.Przelew && !string.IsNullOrEmpty(bankAccountNumber))
                {
                    gfx.DrawString($"Nr konta bankowego:  {bankAccountNumber}", fontBold, XBrushes.Black, margin, y);
                    y += 14;
                }

                gfx.DrawString($"Kwota do zapłaty:  {price:N2} PLN", fontSection, XBrushes.Black, margin, y);
                y += 50;

                // ═══════════════════════════════════════════════════
                // PODPISY
                // ═══════════════════════════════════════════════════
                double sigLineLen = 160;
                gfx.DrawLine(new XPen(XColors.DarkGray, 0.5), margin, y, margin + sigLineLen, y);
                gfx.DrawLine(new XPen(XColors.DarkGray, 0.5), pageW - margin - sigLineLen, y, pageW - margin, y);

                gfx.DrawString("Osoba upoważniona do wystawienia", fontSmall, XBrushes.Gray,
                    new XRect(margin, y + 4, sigLineLen, 12), XStringFormats.TopCenter);
                gfx.DrawString("Osoba upoważniona do odbioru", fontSmall, XBrushes.Gray,
                    new XRect(pageW - margin - sigLineLen, y + 4, sigLineLen, 12), XStringFormats.TopCenter);

                using var memoryStream = new MemoryStream();
                document.Save(memoryStream, false);
                return memoryStream.ToArray();
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
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.UserId == contractDto.AdminId);
            if (admin == null) throw new InvalidOperationException($"Nie znaleziono administratora o ID {contractDto.AdminId}.");

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == contractDto.CarId);
            if (car == null) throw new InvalidOperationException($"Nie znaleziono samochodu o ID {contractDto.CarId}.");

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == contractDto.CustomerId);
            if (customer == null) throw new InvalidOperationException($"Nie znaleziono klienta o ID {contractDto.CustomerId}.");

            var contract = new ContractModel
            {
                ContractNumber = contractDto.ContractNumber,
                TransactionDate = contractDto.TransactionDate,
                Price = contractDto.Price,
                Type = contractDto.TransactionType,
                Car = car,
                Customer = customer,
                Admin = admin
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return contract.Id;
        }
    }
}
