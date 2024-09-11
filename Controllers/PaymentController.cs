using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System;
using System.Web.Mvc;
using TrahvManage.Models;
using TrahvManage.Models.Account;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf.draw;

namespace TrahvManage.Controllers
{
    public class PaymentController : Controller
    {
        public static float _amount = 0f;
        public static int _id = 0;
        private TrahvContext db = new TrahvContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PaymentView(int id, float amount)
        {
            _id = id;
            _amount = amount;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentView([Bind(Include = "Id,CreditCardNumber,ExpirationDate,CVV")] CardModel cardModel)
        {
            try
            {
                if (!CardValidate(cardModel))
                {
                    return View(cardModel);
                }
            }
            catch (Exception)
            {
                return View(cardModel);
            }
            return View("PaymentResult");
        }
        private bool CardValidate(CardModel cardModel) =>
            IsValidCreditCardNumber(cardModel.CreditCardNumber) && !IsCardExpired(cardModel.ExpirationDate) && IsValidCVV(cardModel.CVV);
        private bool IsValidCreditCardNumber(string creditCardNumber)
        {
            Regex visaRegex = new Regex(@"^4[0-9]{12}(?:[0-9]{3})?$");
            Regex masterCardRegex = new Regex(@"^5[1-5][0-9]{14}$");
            Regex maestroRegex = new Regex(@"^(50|5[6-9]|6[0-9])[0-9]{10,17}$");
            return visaRegex.IsMatch(creditCardNumber) || masterCardRegex.IsMatch(creditCardNumber) || maestroRegex.IsMatch(creditCardNumber);
        }
        private bool IsCardExpired(string expirationDate)
        {
            string[] dateParts = expirationDate.Split('-');
            int month = 0;
            int year = 0;
            int day = 0;
            if (dateParts.Length != 3)
                return true;
            if (!int.TryParse(dateParts[0], out year) || !int.TryParse(dateParts[1], out month) || !int.TryParse(dateParts[2], out day))
                return true;
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            int currentDay = DateTime.Now.Day;
            if ((year < currentYear) || (year == currentYear && month < currentMonth) || (year == currentYear && month == currentMonth && day < currentDay))
                return true;
            else
                return false;
        }
        private bool IsValidCVV(string cvv) =>
            new Regex(@"^[0-9]{3,4}$").IsMatch(cvv);
        public ActionResult PaymentResult()
        {
            AccountModel acc = db.Accounts.Find(UserState.Id);
            FineModel fine = db.Fines.Find(_id);
            db.Fines.Remove(fine);
            db.SaveChanges();
            string message = $"Lugupeetud {acc.FirstName} {acc.LastName},<br><br>" +
        $"Teavitame teid, et teie poolt määratud trahv summas {_amount} eurot on edukalt tasutud. Täname teid õigeaegse makse eest.<br><br>" +
        "Kui teil on küsimusi, kirjutage meie veebisaidil olevale tehnilisele toele.<br><br>" +
        "Lugupidamisega,<br>Politsei- ja Piirivalveamet";
            AccountController.Email(db.Accounts.Find(UserState.Id).Email, "Trahv maksti ära", message);
            return View();
        }
        private FileStreamResult CreatePDF(FineModel fineModel)
        {
            MemoryStream stream = GeneratePDF(fineModel);
            return File(stream, "application/pdf", $"Receipt_{fineModel.Id}.pdf");
        }

        private MemoryStream GeneratePDF(FineModel fineModel)
        {
            AccountModel acc = db.Accounts.Find(UserState.Id);
            MemoryStream stream = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, 40, 40, 50, 50);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
            writer.CloseStream = false;
            pdfDoc.Open();
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
            Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            Font regularFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10, BaseColor.GRAY);
            Paragraph title = new Paragraph("Makse kviitung", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20;
            pdfDoc.Add(title);
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1, 2 });
            AddCellToTable(table, "Nimi:", boldFont, regularFont, $"{acc.FirstName} {acc.LastName}");
            AddCellToTable(table, "Isikukood:", boldFont, regularFont, acc.PersonalCode);
            AddCellToTable(table, "Auto number:", boldFont, regularFont, fineModel.AutoNumber);
            AddCellToTable(table, "Juhtum:", boldFont, regularFont, fineModel.Incident);
            AddCellToTable(table, "Trahv summa:", boldFont, regularFont, $"${fineModel.FineAmount}");
            AddCellToTable(table, "Maksekuupäev:", boldFont, regularFont, DateTime.Now.ToString("dd/MM/yyyy"));
            pdfDoc.Add(table);
            LineSeparator separator = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -2);
            pdfDoc.Add(new Chunk(separator));
            pdfDoc.Add(new Paragraph("\n"));
            Paragraph info = new Paragraph("Täname makse eest. Palun hoidke see kviitung enda jaoks alles.", regularFont);
            info.Alignment = Element.ALIGN_CENTER;
            info.SpacingAfter = 30;
            pdfDoc.Add(info);
            Paragraph footer = new Paragraph("Politsei- ja Piirivalveamet", footerFont);
            footer.Alignment = Element.ALIGN_CENTER;
            footer.SpacingBefore = 40;
            pdfDoc.Add(footer);
            pdfDoc.Close();
            stream.Position = 0;
            return stream;
        }

        private void AddCellToTable(PdfPTable table, string label, Font labelFont, Font valueFont, string value)
        {
            PdfPCell cell = new PdfPCell(new Phrase(label, labelFont));
            cell.Border = Rectangle.NO_BORDER;
            cell.PaddingBottom = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(value, valueFont));
            cell.Border = Rectangle.NO_BORDER;
            cell.PaddingBottom = 5;
            table.AddCell(cell);
        }
        public ActionResult DownloadReceipt()
        {
            FineModel fineModel = db.Fines.Find(_id);
            return CreatePDF(fineModel);
        }
    }
}