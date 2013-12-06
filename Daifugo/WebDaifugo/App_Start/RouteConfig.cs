using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebDaifugo.WsHandlers;

namespace WebDaifugo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add(new Route("monitor", new WsRouteHandler<MoniterHandler>()));
            routes.Add(new Route("play/{rule}/{id}", new WsRouteHandler<PlayerHandler>()));
            routes.MapRoute(
                name: "QuickPlay",
                url: "QuickPlay/{rule}/",
                defaults: new { controller = "Home", action = "QuickPlay" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}