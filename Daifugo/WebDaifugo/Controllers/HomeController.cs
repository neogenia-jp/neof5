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
            ViewData["show_fork_me_ribon"] = true;
            return View();
        }

    }
}
