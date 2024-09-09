using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TrahvManage.Models;
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
            db.Fines.Remove(fineModel);
            db.SaveChanges();
            ViewBag.Amount = amount;
            return View();
        }
    }
}
