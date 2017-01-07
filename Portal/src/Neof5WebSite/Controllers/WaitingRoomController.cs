using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Neof5WebSite.Controllers
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

    }
}
