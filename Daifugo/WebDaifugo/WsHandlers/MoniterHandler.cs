using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json.Linq;

namespace WebDaifugo.WsHandlers
{
    public class MoniterHandler : WebSocketHandler
    {
        private static WebSocketCollection AllClients = new WebSocketCollection();


        public override void OnOpen()
        {
            AllClients.Add(this);

            // FIXME
        }

        public override void OnClose()
        {
            AllClients.Remove(this);

            // FIXME
        }

        public override void OnError()
        {
            base.OnError();

            // FIXME
        }

        public override void OnMessage(string jsonMsg)
        {
            // FIXME
        }

    }
}