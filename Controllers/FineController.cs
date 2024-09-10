using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data.Entity;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml.Linq;
using TrahvManage.Models;
using TrahvManage.Models.Account;
using TrahvManage.Services;

namespace TrahvManage.Controllers
{
    public class FineController : Controller
    {
        private TrahvContext db = new TrahvContext();
        public ActionResult Index()
        {
            if (UserState.Role == "User")
                return View(db.Fines.ToList().Where(x => x.PersonalCode == db.Accounts.Find(UserState.Id).PersonalCode));
            else
                return View(db.Fines.ToList());
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FineModel fineModel = db.Fines.Find(id);
            if (fineModel == null)
            {
                return HttpNotFound();
            }
            return View(fineModel);
        }
        [UserState("Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FineModel fineModel = db.Fines.Find(id);
            if (fineModel == null)
            {
                return HttpNotFound();
            }
            return View(fineModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserState("Admin")]
        public ActionResult Edit([Bind(Include = "Id,AutoNumber,Incident,IncidentPlace,IncidentDate,FineAmount")] FineModel fineModel)
        {
            if (ModelState.IsValid)
            {
                fineModel.FineAmount = (float)Math.Round(fineModel.FineAmount, 2);
                db.Entry(fineModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fineModel);
        }
        [UserState("Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FineModel fineModel = db.Fines.Find(id);
            if (fineModel == null)
            {
                return HttpNotFound();
            }
            return View(fineModel);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [UserState("Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            FineModel fineModel = db.Fines.Find(id);
            db.Fines.Remove(fineModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult CreateRandom()
        {
            AccidentGenerator.LoadFine(db);
            return View();
        }
        public ActionResult PaymentView(float? amount, int? id)
        {
            ViewBag.Amount = amount;
            ViewBag.Id = id;
            return View();
        }
        public ActionResult PaymentResult(float? amount, int? id)
        {
            FineModel fineModel = db.Fines.Find(id);
            ViewBag.Amount = amount;
            ViewBag.Id = id;
            AccountModel acc = db.Accounts.Find(UserState.Id);
            string message = $"Lugupeetud {acc.FirstName} {acc.LastName},<br><br>" +
        $"Teavitame teid, et teie poolt määratud trahv summas {fineModel.FineAmount} eurot on edukalt tasutud. Täname teid õigeaegse makse eest.<br><br>" +
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

        public ActionResult DownloadReceipt(int id)
        {
            FineModel fineModel = db.Fines.Find(id);
            return CreatePDF(fineModel);
        }

    }
}
