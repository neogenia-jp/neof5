using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDaifugo.WsHandlers;

namespace WebDaifugo.Controllers
{
    public class WaitingRoomController : Controller
    {
        //
        // GET: /Playroom/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QuickPlay()
        {
            return View();
        }

        public ActionResult TestPlay()
        {
            return View();
        }

        public ActionResult Start(string id, string name, string rule)
        {
            if (id == null)
            {
                // あいているところを自動で探す
                id = GameMasterManager.MakeTempSession();
            }

            ViewBag.RoomId = id;
            ViewBag.Name = name;
            ViewBag.Rule = rule;

            return View();
        }
    }
}
