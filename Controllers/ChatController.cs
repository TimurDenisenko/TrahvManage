using Microsoft.Ajax.Utilities;
using System;
using System.Web.Mvc;
using TrahvManage.Models;

namespace TrahvManage.Controllers
{
    public class ChatController : Controller
    {
        private TrahvContext db = new TrahvContext();
        public ActionResult Index()
        {
            return View(db.Chats);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,History")] ChatModel chatModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Title = Json(chatModel.History).Data.ToString();
                chatModel.History = Json(chatModel.History).Data.ToString();
                //db.Chats.Add(chatModel);
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }

            return View(chatModel);
        }
    }
}