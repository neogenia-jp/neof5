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
            routes.Add(new Route("monitor/{id}", new WsRouteHandler<MoniterHandler>()));
            routes.Add(new Route("play/{rule}/{id}", new WsRouteHandler<PlayerHandler>()));
            routes.Add(new Route("test/connection", new WsRouteHandler<ConnectionTestHandler>()));
            routes.Add(new Route("test/testcase", new WsRouteHandler<TestcaseHandler>()));
            routes.Add(new Route("test/practice", new WsRouteHandler<PracticeHandler>()));

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
                name: "Watch",
                url: "Watch/{id}",
                defaults: new { controller = "Game", action = "Watch" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}