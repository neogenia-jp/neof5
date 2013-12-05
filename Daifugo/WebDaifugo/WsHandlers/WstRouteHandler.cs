using Microsoft.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebSockets;

namespace WebDaifugo.WsHandlers
{
    public class WsRouteHandler<TWS> : IRouteHandler, IHttpHandler where TWS : WebSocketHandler, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                var handler = new TWS();
                context.AcceptWebSocketRequest(handler);
            }
            else
            {
                context.Response.StatusCode = 400; // bad request
            }
        }

        public bool IsReusable { get { return false; } }

    }
}
