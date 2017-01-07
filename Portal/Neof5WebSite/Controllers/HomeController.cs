using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

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

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var b = filterContext.RequestContext.HttpContext.Request.Browser;
            if (b.Browser.StartsWith("IE") && b.MajorVersion < 9 ||
				b.Browser.StartsWith("Chrome") && b.MajorVersion < 3 ||
				b.Browser.StartsWith("Firefox") && b.MajorVersion < 4 ||
				b.Browser.StartsWith("Safari") && b.MajorVersion < 4  && b.MobileDeviceModel!="IPhone" ||
				b.Browser.StartsWith("Opera") && b.MajorVersion < 10)
            {
                    var viewResult = filterContext.Result as ViewResult;
                    viewResult.ViewName = "unsupported";
            }

 			base.OnActionExecuted(filterContext);
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
