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

			// WebSockets
            routes.Add(new Route("monitor", new WsRouteHandler<MoniterHandler>()));
            routes.Add(new Route("play/{rule}/{id}", new WsRouteHandler<PlayerHandler>()));

			// Web Pages
            routes.MapRoute(
                name: "QuickPlay",
                url: "QuickPlay/{rule}/",
                defaults: new { controller = "Game", action = "Index" }
            );
            routes.MapRoute(
                name: "Entry",
                url: "Entry/{rule}/{id}",
                defaults: new { controller = "Game", action = "Entry" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}