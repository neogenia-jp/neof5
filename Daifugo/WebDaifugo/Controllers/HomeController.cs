using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDaifugo.AppClasses;
using WebDaifugo.WsHandlers;

namespace WebDaifugo.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult QuickPlay(string rule, string name)
        {
            // あいているところを自動で探す
            var id = PlayRoomsManager.MakeTempSession();

            ViewBag.RoomId = id;
            ViewBag.Name = string.IsNullOrWhiteSpace(name) ? "あなた" : name;
            ViewBag.Rule = rule;
            ViewBag.AutoStart = true;
            return View("Entry");
        }

        public ActionResult Entry(string rule, string id, string name)
        {
            ViewBag.RoomId = id;
            ViewBag.Name = name;
            ViewBag.Rule = rule;
            ViewBag.AutoStart = false;
            return View();
        }
    }
}
