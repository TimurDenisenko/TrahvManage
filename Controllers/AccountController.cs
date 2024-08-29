using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Mvc;
using TrahvManage.Models;
using TrahvManage.Models.Account;

namespace TrahvManage.Controllers
{
    public class AccountController : Controller
    {
        private TrahvContext db = new TrahvContext();
        public static int Id { get; set; }
        public static string Name { get; set; }
        public static bool Authorized { get; set; }
        public static string Role { get; set; }
        public ActionResult Index()
        {
            if (Authorized && Role == "Admin")
                return View(db.Accounts.ToList());
            else if (Authorized && Role != "Admin")
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("Register", "Account");
        }
        public ActionResult Create()
        {
            if (Authorized)
                return View();
            else
                return RedirectToAction("Register", "Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Gender,Email,PinCode,PersonalCode,Role")] AccountModel accountModel)
        {
            if (ModelState.IsValid && !db.Accounts.Select(x => x.PersonalCode).Contains(accountModel.PersonalCode))
            {
                db.Accounts.Add(accountModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accountModel);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Id,FirstName,LastName,Gender,Email,PinCode,PersonalCode,Role")] AccountModel accountModel)
        {
            if (!db.Accounts.Select(x => x.PersonalCode).Contains(accountModel.PersonalCode))
            {
                accountModel.Role = "User";
                db.Accounts.Add(accountModel);
                HttpContext.Cache.Insert("Authorized", true, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
                Authorized = true;
                Role = "User";
                Name = accountModel.FirstName + " " + accountModel.LastName;
                Id = accountModel.Id;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(accountModel);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "PersonalCode, PinCode")] AccountModel accountModel)
        {
            try
            {
                AccountModel reAcc = db.Accounts.Where(x => x.PersonalCode == accountModel.PersonalCode).ToList()[0];
                if (reAcc.PinCode == accountModel.PinCode)
                {
                    HttpContext.Cache.Insert("Authorized", true, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
                    Authorized = true;
                    Role = reAcc.Role;
                    Name = reAcc.FirstName + " " + reAcc.LastName;
                    Id = reAcc.Id;
                    return RedirectToAction("Index", "Home");
                }
            }
            catch {}
            return View(accountModel);
        }
        public ActionResult Logout()
        {
            HttpContext.Cache.Insert("Authorized", false, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
            Authorized = false;
            Role = string.Empty;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Recovery()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Recovery([Bind(Include = "PersonalCode")] AccountModel accountModel)
        {
            Email(accountModel);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountModel accountModel = db.Accounts.Find(id);
            if (accountModel == null)
            {
                return HttpNotFound();
            }
            return View(accountModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Gender,Email,PinCode,PersonalCode,Role")] AccountModel accountModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accountModel);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountModel accountModel = db.Accounts.Find(id);
            if (accountModel == null)
            {
                return HttpNotFound();
            }
            return View(accountModel);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountModel accountModel = db.Accounts.Find(id);
            db.Accounts.Remove(accountModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details()
        {
            return View(db.Accounts.Find(Id));
        }
        private void Email(AccountModel acc)
        {
            AccountModel fullAcc = db.Accounts.Where(x => x.PersonalCode == acc.PersonalCode).ToArray()[0];
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "timur.denisenko.work@gmail.com";
                WebMail.Password = "duto ahun xrzh hjsq";
                WebMail.From = "timur.denisenko.work@gmail.com";
                WebMail.Send($"{fullAcc.Email}", "Parooli taastamine", $"Teie PIN-kood: {fullAcc.PinCode}");
                ViewBag.Message = "Kiri on saatnud!";
            }
            catch (Exception)
            {
                ViewBag.Message = "Mul on kahju! Ei saa kirja saada!";
            }
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