using System.Web.Mvc;
using TrahvManage.Models;
using TrahvManage.Models.Account;

namespace TrahvManage.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}