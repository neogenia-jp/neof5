using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json.Linq;
using Daifugo;
using WebDaifugo.WsProtocols;
using Daifugo.Cards;
using Daifugo.Bases;
using Daifugo.GameImples;
using System.Diagnostics;
using Daifugo.Players;
using System.Text.RegularExpressions;
using WebDaifugo.AppClasses;
using WebDaifugo.Models;

namespace WebDaifugo.WsHandlers
{
    public class PlayerHandler : WebSocketHandler, IGamePlayer 
    {
        private static Object lockObj = new object();
        private static WebSocketCollection AllClients = new WebSocketCollection();
        private static IEnumerable<WebSocketHandler> ActiveClients { get { return AllClients.Where(c => c.WebSocketContext.IsClientConnected); } }

        private string sessionId = null;
        private string rule = null;

        private string playerName;
        private int playerNum = 0;
        private DaifugoPlayRoom room = null;


        public override void OnOpen()
        {
            lock(lockObj) {
                AllClients.Add(this);
            
                playerName = this.WebSocketContext.QueryString["name"];

                var mc = Regex.Matches(this.WebSocketContext.RequestUri.OriginalString, @"/play/(A|B)/([\-A-Z0-9]*)\?");
                if (mc.Count > 0)
                {
                    rule = mc[0].Groups[1].Success ? mc[0].Groups[1].Value.ToString() : "A";
                    sessionId = mc[0].Groups[2].Success ? mc[0].Groups[2].Value.ToString() : "1";
                }

                room = PlayRoomsManager.GetOrCreate(sessionId, rule);

				// すでにプレイ中なら切断する
                if (room==null || room.Master.IsPlaing) { this.Close(); return; }

                playerNum = room.Master.NumOfPlayers;
                room.AddPlayer(this);
            }
        }

        public override void OnClose()
        {
        }

        public override void OnError()
        {
            lock (lockObj)
            {
                base.OnError();
            }
        }

        public override void OnMessage(string jsonMsg)
        {
            JObject jsonObj = JObject.Parse(jsonMsg);

            Debug.WriteLine("OnMessage()" + jsonObj);

            try
            {
                var pd = ProcMessage(jsonObj);
                if (pd != null) Send(pd);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Send(new ProtocolData(e));
            }
        }

        private ProtocolData ProcMessage(JObject jsonObj)
        {
            string kind = null;

            try
            {
                kind = jsonObj["Kind"].ToString();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Kindがありません");
            }

            if (kind == "Tweet")
            {
                jsonObj["PlayerNum"] = playerNum;
                AllClients.Broadcast(jsonObj.ToString());
            }
            else if (kind == "Start")
            {
                // 自動的に不足人数を追加してゲーム開始する
                var limit = 5 - room.Master.NumOfPlayers;
                for (int i = 1; i <= limit; i++)
                {
                    room.AddPlayer(PoorPlayer.Create("COM" + i, room.Master));
                }
                room.Master.Start();
            }
            else if (kind == "Put")
            {
                var ret = room.Master.PutCards(this, DeckGenerator.FromCardsetString(jsonObj["Cards"].ToString()));
                if (!(ret is CheckOK)) return new ProtocolData(ret);
                myTimer.Enabled = false;
            }
            return null;
        }

        internal void Send(ProtocolData wsp)
        {
            string str = wsp.ToJson();
            Debug.WriteLine("Send: " + str);
            base.Send(str);
        }


        // =================================== IGamePlayer Impl ===================================

        public string Name { get { return playerName + (this.WebSocketContext.IsClientConnected ? "" : "(離脱)"); } }

        public void Start(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "Start", ctx));
        }

        public void Finish(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "Finish", ctx));
        }

        public void ProcessTurn(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "ProcessTurn", ctx));

			// タイマースタート
            _startTimer(30*1000);
        }

        private System.Timers.Timer myTimer;
        private void _startTimer(int time)
        {
            if (myTimer == null)
            {
                myTimer = new System.Timers.Timer();
                myTimer.AutoReset = false;
                myTimer.Interval = time;
                myTimer.Elapsed += OnTimer;
            }
            myTimer.Enabled = true;
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
			room.Master.PutCards(this, DeckGenerator.FromCardsetString(""));
        }

        void CardsArePut(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "CardsArePut", ctx));
        }

        public void Nagare(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "Nagare", ctx));
        }

        public void Agari(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "Agari", ctx));
        }

        public void Kakumei(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "Kakumei", ctx));
        }

        public void CardDistributed(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "CardDistributed", ctx));
        }

        public void CardSwapped(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "CardSwapped", ctx));
        }

        public void Thinking(IPlayerContext ctx)
        {
            Send(new WsProtocols.ProtocolData(playerNum, "Thinking", ctx));
        }


        public void Connect(GameEvents evt)
        {
            evt.agari += (arg)=>this.Agari(arg(this) as IPlayerContext);
            evt.cardDistributed+= (arg)=>this.CardDistributed(arg(this) as IPlayerContext);
            evt.cardsArePut += (arg)=>this.CardsArePut(arg(this) as IPlayerContext);
            evt.cardSwapped += (arg)=>this.CardSwapped(arg(this) as IPlayerContext);
            evt.finish += (arg)=>this.Finish(arg(this) as IPlayerContext);
            evt.kakumei += (arg)=>this.Kakumei(arg(this) as IPlayerContext);
            evt.nagare += (arg)=>this.Nagare(arg(this) as IPlayerContext);
            evt.start += (arg)=>this.Start(arg(this) as IPlayerContext);
            evt.thinking += (arg)=>this.Thinking(arg(this) as IPlayerContext);
        }
    }
}