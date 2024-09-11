using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System;
using System.Web.Mvc;
using TrahvManage.Models;
using TrahvManage.Models.Account;
using System.Text.RegularExpressions;

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
            MemoryStream stream = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(pdfDoc, stream).CloseStream = false;
            pdfDoc.Open();
            pdfDoc.Add(new Paragraph("Payment Receipt"));
            pdfDoc.Add(new Paragraph($"Fine ID: {fineModel.Id}"));
            pdfDoc.Add(new Paragraph($"Amount Paid: ${fineModel.FineAmount}"));
            pdfDoc.Add(new Paragraph($"Date: {DateTime.Now.ToString("dd/MM/yyyy")}"));
            pdfDoc.Close();
            db.Fines.Remove(fineModel);
            db.SaveChanges();
            stream.Position = 0;
            return File(stream, "application/pdf", $"Receipt_{fineModel.Id}.pdf");
        }
        public ActionResult DownloadReceipt()
        {
            FineModel fineModel = db.Fines.Find(_id);
            return CreatePDF(fineModel);
        }
    }
}