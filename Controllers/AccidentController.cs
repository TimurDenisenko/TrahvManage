using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrahvManage.Services;

namespace TrahvManage.Controllers
{
    public class AccidentController : Controller
    {
        //private ApplicationDbContext db = new DATABASE;

        //public ActionResult GenerateAndSaveAccident()
        //{
        //    var accident = AccidentGenerator.GenerateRandomAccident();
        //    db.Accidents.Add(accident);
        //    db.SaveChanges();
        //}

        public ActionResult TestAccidentGenerator()
        {
            var accident = AccidentGenerator.GenerateRandomAccident();
            return Json(accident, JsonRequestBehavior.AllowGet);
        }
    }
}