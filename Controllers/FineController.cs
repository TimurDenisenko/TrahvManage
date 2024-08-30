using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TrahvManage.Models;

namespace TrahvManage.Controllers
{
    public class FineController : Controller
    {
        private TrahvContext db = new TrahvContext();
        public ActionResult Index()
        {
            if (!AccountController.Authorized)
                return RedirectToAction("Register", "Account");
            return View(db.Fines.ToList());
        }
        public ActionResult Details(int? id)
        {
            if (!AccountController.Authorized)
                return RedirectToAction("Register", "Account");
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
        public ActionResult Create()
        {
            if (!AccountController.Authorized)
                return RedirectToAction("Register", "Account");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AutoNumber,Incident,IncidentPlace,IncidentDate,FineAmount")] FineModel fineModel)
        {
            if (!AccountController.Authorized)
                return RedirectToAction("Register", "Account");
            if (ModelState.IsValid)
            {
                db.Fines.Add(fineModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fineModel);
        }
        public ActionResult Edit(int? id)
        {
            if (!AccountController.Authorized)
                return RedirectToAction("Register", "Account");
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
        public ActionResult Edit([Bind(Include = "Id,AutoNumber,Incident,IncidentPlace,IncidentDate,FineAmount")] FineModel fineModel)
        {
            if (!AccountController.Authorized)
                return RedirectToAction("Register", "Account");
            if (ModelState.IsValid)
            {
                db.Entry(fineModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fineModel);
        }
        public ActionResult Delete(int? id)
        {
            if (!AccountController.Authorized)
                return RedirectToAction("Register", "Account");
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
        public ActionResult DeleteConfirmed(int id)
        {
            if (!AccountController.Authorized)
                return RedirectToAction("Register", "Account");
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
    }
}
