using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using WebDaifugo.AppClasses;
using Daifugo.Bases;
using WebDaifugo.Neof5Protocols;
using WebDaifugo.Basis;

namespace WebDaifugo.WsHandlers
{
    public class MoniterHandler : WebSocketHandler, IGameMonitor, ITweetListener
    {
        private string sessionId = null;
        private DaifugoPlayRoom room = null;
        private int playerNum = 0;

        public override void OnOpen()
        {
            var mc = Regex.Matches(this.WebSocketContext.RequestUri.OriginalString, @"/monitor/([\-A-Z0-9]+)");
            if (mc.Count > 0)
            {
                sessionId = mc[0].Groups[1].Success ? mc[0].Groups[1].Value.ToString() : "";

                if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    room = PlayRoomsManager.Get(sessionId);
                }
            }

            if (room == null)
            {
                base.Close();
                return;
            }
            room.AddObserver(this);
        }

        public override void OnClose()
        {
            if(room!=null) room.RemoveObserver(this);
        }

        public override void OnError()
        {
        }

        public override void OnMessage(string jsonMsg)
        {
        }

        internal void Send(ProtocolData wsp)
        {
            Send(wsp.ToJson());
        }


        // =================================== Game Events ===================================

        public void Start(IMonitorContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Start", ctx));
        }

        public void Finish(IMonitorContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Finish", ctx));
        }

        public void CardsArePut(IMonitorContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "CardsArePut", ctx));
        }

        public void Nagare(IMonitorContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Nagare", ctx));
        }

        public void Agari(IMonitorContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Agari", ctx));
        }

        public void Kakumei(IMonitorContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Kakumei", ctx));
        }

        public void CardDistributed(IMonitorContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "CardDistributed", ctx));
        }

        public void CardSwapped(IMonitorContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "CardSwapped", ctx));
        }

        public void Thinking(IMonitorContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Thinking", ctx));
        }

        public void Tweet(int twetedPlayerNum, string message)
        {
            JObject json = new JObject();
            json["Kind"] = "Tweet";
            json["Message"] = message;
            json["PlayerNum"] = twetedPlayerNum;
            Send(json.ToString());
        }

        public void bindEvents(GameEvents evt) { ((IGameMonitor)this).BindEvents(evt); }
        public void unbindEvents(GameEvents evt) { ((IGameMonitor)this).UnbindEvents(evt); }
    }
}