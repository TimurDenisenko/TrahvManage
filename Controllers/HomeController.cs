using System.Web.Mvc;

namespace TrahvManage.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (AccountController.Authorized)
                return View();
            else
                return RedirectToAction("Register", "Account");
        }
    }
}