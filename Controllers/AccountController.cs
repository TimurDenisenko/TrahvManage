using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using TrahvManage.Models;
using TrahvManage.Models.Account;

namespace TrahvManage.Controllers
{
    public class AccountController : Controller
    {
        private TrahvContext db = new TrahvContext();
        [UserState("Admin")]
        public ActionResult Index()
        {
             return View(db.Accounts.ToList());
        }
        [UserState("Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserState("Admin")]
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
            if (db.Accounts.Count() == 0)
            {
                db.Accounts.Add(new AccountModel
                {
                    Id = 0,
                    FirstName = "Timur",
                    LastName = "Denisenko",
                    Gender = "Male",
                    Email = "timur.denisenko.2109@gmail.com",
                    PersonalCode = "50609215714",
                    PinCode = 1234,
                    Role = "Admin"
                });
                db.SaveChanges();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Id,FirstName,LastName,Gender,Email,PinCode,PersonalCode,Role")] AccountModel accountModel)
        {
            try
            {
                if (!db.Accounts.Select(x => x.PersonalCode).Contains(accountModel.PersonalCode))
                {
                    accountModel.Role = UserState.Role = "User";
                    db.Accounts.Add(accountModel);
                    UserState.Name = accountModel.FirstName + " " + accountModel.LastName;
                    db.SaveChanges();
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception) { }
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
                    UserState.Authorized = true;
                    UserState.Role = reAcc.Role;
                    UserState.Name = reAcc.FirstName + " " + reAcc.LastName;
                    UserState.Id = reAcc.Id;
                    return RedirectToAction("Index", "Home");
                }
            }
            catch {}
            return View(accountModel);
        }
        public ActionResult Logout()
        {
            UserState.Authorized = false;
            UserState.Role = null;
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
            AccountModel fullAcc = db.Accounts.Where(x => x.PersonalCode == accountModel.PersonalCode).ToArray()[0];
            Email($"{fullAcc.Email}", "Parooli taastamine", $"Teie PIN-kood: {fullAcc.PinCode}");
            return RedirectToAction("Login", "Account");
        }
        [UserState("Admin")]
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
        [UserState("Admin")]
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
        [UserState("Admin")]
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
        [UserState("Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountModel accountModel = db.Accounts.Find(id);
            db.Accounts.Remove(accountModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details()
        {
            return View(db.Accounts.Find(UserState.Id));
        }
        public static void Email(string email, string teema, string message)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "timur.denisenko.work@gmail.com";
                WebMail.Password = "duto ahun xrzh hjsq";
                WebMail.From = "timur.denisenko.work@gmail.com";
                WebMail.Send(email, teema, message);
            }
            catch (Exception)
            {
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