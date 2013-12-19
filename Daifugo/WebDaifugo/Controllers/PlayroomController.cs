using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDaifugo.AppClasses;
using WebDaifugo.WsHandlers;

namespace WebDaifugo.Controllers
{
    public class PlayroomController : Controller
    {
        public class ViewModel
        {
            public readonly Dictionary<string, DaifugoPlayRoom> RoomsA = new Dictionary<string, DaifugoPlayRoom>();
            public readonly Dictionary<string, DaifugoPlayRoom> RoomsB = new Dictionary<string, DaifugoPlayRoom>();

            internal static ViewModel Create()
            {
                var vm = new ViewModel();
                for (int i = 1; i <= 8; i++)
                {
                    string id = string.Format("A-{0:00}", i);
                    vm.RoomsA[id] = PlayRoomsManager.Get(id);
                }
                for (int i = 1; i <= 8; i++)
                {
                    string id = string.Format("B-{0:00}", i);
                    vm.RoomsB[id] = PlayRoomsManager.Get(id);
                }
                return vm;
            } 
        }

        //
        // GET: /Playroom/

        public ActionResult Index()
        {
            var vm = ViewModel.Create();
            return View(vm);
        }

        public ActionResult Room(string id)
        {
            var playroom = PlayRoomsManager.Get(id);
            ViewBag.RoomID = id;
            ViewBag.Rule = id.StartsWith("A") ? "A" : "B";
            return View(playroom);
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
                id = PlayRoomsManager.MakeTempSession();
            }

            ViewBag.RoomId = id;
            ViewBag.Name = name;
            ViewBag.Rule = rule;

            return View();
        }
    }
}
