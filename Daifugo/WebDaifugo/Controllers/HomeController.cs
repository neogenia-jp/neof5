using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            var id = GameMasterManager.MakeTempSession();

            ViewBag.RoomId = id;
            ViewBag.Name = string.IsNullOrWhiteSpace(name) ? "あなた" : name;
            ViewBag.Rule = rule;
            return View();
        }
    }
}
