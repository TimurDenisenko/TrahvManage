using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using TrahvManage.Models;
using TrahvManage.Models.Account;

namespace TrahvManage.Controllers
{
    public class ChatController : Controller
    {
        private TrahvContext db = new TrahvContext();
        public ActionResult Index()
        {
            string ps = db.Accounts.Find(UserState.Id).PersonalCode;
            if (UserState.Role == "Admin")
                return View(db.Chats);
            return View(db.Chats.Where(x => x.FirstPersonalCode == ps));
        }
        [UserState("User")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserState("User")]
        public ActionResult Create([Bind(Include = "Id,History")] ChatModel chatModel)
        {
            chatModel.FirstPersonalCode = db.Accounts.Find(UserState.Id).PersonalCode;
            chatModel.History = HistoryConverter.ConvertToString(db, chatModel.History);
            db.Chats.Add(chatModel);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = db.Chats.Count()});
        }
        public ActionResult Edit(int? id)
        {
            ChatModel chat = db.Chats.Find(id);
            ViewBag.FirstPerson = db.Accounts.Where(x => x.PersonalCode == chat.FirstPersonalCode).ToArray()[0].FirstName;
            try
            {
                ViewBag.SecondPerson = db.Accounts.Where(x => x.PersonalCode == chat.SecondPersonalCode).ToArray()[0].FirstName;
            }
            catch (Exception)
            {
                AccountModel acc = db.Accounts.Find(UserState.Id);
                if (acc.PersonalCode == chat.FirstPersonalCode)
                    ViewBag.SecondPerson = "";
                else
                {
                    chat.SecondPersonalCode = acc.PersonalCode;
                    ViewBag.SecondPerson = acc.FirstName;
                    db.Entry(chat).State = EntityState.Modified;
                    db.SaveChanges();
                }    
            }
            return View(chat);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,History")] ChatModel chatModel)
        {
            if (ModelState.IsValid)
            {
                string newHistory = HistoryConverter.Concat(db, chatModel);
                ChatModel chat = db.Chats.Find(chatModel.Id);
                chat.History = newHistory;
                db.Entry(chat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new {id = chat.Id});
            }

            return View(chatModel);
        }
    }
}