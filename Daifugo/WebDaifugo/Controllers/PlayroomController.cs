using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDaifugo.AppClasses;
using WebDaifugo.Basis;
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
                for (int i = 1; i <= 10; i++)
                {
                    string id = string.Format("A-{0:00}", i);
                    vm.RoomsA[id] = PlayRoomsManager.Get(id);
                }
                for (int i = 1; i <= 5; i++)
                {
                    string id = string.Format("B-{0:00}", i);
                    vm.RoomsB[id] = PlayRoomsManager.Get(id);
                }
                return vm;
            } 
        }

        //
        // GET: /Playroom/
		[NoCache]
        public ActionResult Index()
        {
            var vm = ViewModel.Create();
            return View(vm);
        }

		[NoCache]
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

        public ActionResult RuleTest()
        {
            var sid = PlayRoomsManager.MakeTempSession(RuleTestHandler.ROOM_ID_PREFIX);
            PlayRoomsManager.GetOrCreate(sid);
            ViewBag.RoomId = sid;
            return View();
        }

        public ActionResult Practice(string id)
        {
            var rule = id;
            var sid = PlayRoomsManager.MakeTempSession(PracticeHandler.ROOM_ID_PREFIX);
            PlayRoomsManager.GetOrCreate(sid, rule);
            ViewBag.RoomId = sid;
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

        public ActionResult Monitor(string id)
        {
            ViewBag.RoomId = id;
            return View();
        }

    }
}
