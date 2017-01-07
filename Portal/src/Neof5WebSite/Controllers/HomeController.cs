using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace Neof5WebSite.Controllers
{

    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var userAgent = filterContext.HttpContext.Request.Headers["User-Agent"].ToString();
            if (!_IsSupportedBrowser(userAgent))
            {
                var viewResult = filterContext.Result as ViewResult;
                viewResult.ViewName = "unsupported";
            }

 			base.OnActionExecuted(filterContext);
        }

        bool _IsSupportedBrowser(string ua)
        {
            var mr = Regex.Match(ua, @"MSIE (\d+).(\d+)");
            if (mr.Success && int.Parse(mr.Groups[1].Value) < 9) return false;
            
            mr = Regex.Match(ua, @"Chrome/(\d+).(\d+)");
            if (mr.Success && int.Parse(mr.Groups[1].Value) < 3) return false;

            mr = Regex.Match(ua, @"FireFox/(\d+).(\d+)");
            if (mr.Success && int.Parse(mr.Groups[1].Value) < 4) return false;

            mr = Regex.Match(ua, @"Safari/(\d+).(\d+)");
            if (mr.Success && int.Parse(mr.Groups[1].Value) < 528) return false;

            mr = Regex.Match(ua, @"Opera (\d+).(\d+)");
            if (mr.Success && int.Parse(mr.Groups[1].Value) < 10) return false;

            mr = Regex.Match(ua, @"Opera/(\d+).(\d+)");
            if (mr.Success && int.Parse(mr.Groups[1].Value) < 9 && int.Parse(mr.Groups[2].Value) < 80) return false;

            return true;
        }

        public ActionResult Viewer(string id)
        {
            if (id == "201401_A-01") // クラスＡ予選1組
            {
                ViewBag.Rounds = 9;
            }
            else if (id == "201401_A-03") // クラスＡ予選2組
            {
                ViewBag.Rounds = 9;
            }
            else if (id == "201401_A-04") // クラスＡ決勝
            {
                ViewBag.Rounds = 9;
            }
            else if (id == "201401_A-06") // クラスＡエキシビションマッチ
            {
                ViewBag.Rounds = 10;
            }
            else if (id == "201401_B-01") // クラスＢ
            {
                ViewBag.Rounds = 5;
            }
            ViewBag.RecordID = id;

            return View();
        }
    }
}
