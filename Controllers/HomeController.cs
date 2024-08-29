using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrahvManage.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AccountController.Authorized = Convert.ToBoolean(HttpContext.Cache.Get("Authorized"));
            if (AccountController.Authorized)
                return View();
            else
                return RedirectToAction("Register", "Account");
        }
    }
}