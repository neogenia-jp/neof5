using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json.Linq;
using Daifugo;
using WebDaifugo.Neof5Protocols;
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
    public class PlayerHandler : WebSocketHandler
    {
        private static Object lockObj = new object();
        private static WebSocketCollection AllClients = new WebSocketCollection();
        private static IEnumerable<WebSocketHandler> ActiveClients { get { return AllClients.Where(c => c.WebSocketContext.IsClientConnected); } }

        private string sessionId = null;
        private string rule = null;

        public string playerName { get; private set; }
        private DaifugoPlayRoom room = null;

        private GamePlayerAdapter pAdapter = null;

        public override void OnOpen()
        {
            Debug.WriteLine(WebSocketContext.Headers);
            lock (lockObj)
            {
                AllClients.Add(this);

                // クエリストリングを自力で解析
                playerName = this.WebSocketContext.QueryString["name"];

                var mc = Regex.Matches(this.WebSocketContext.RequestUri.OriginalString, @"/play/(A|B)/([\-A-Z0-9]*)\?");
                if (mc.Count > 0)
                {
                    rule = mc[0].Groups[1].Success ? mc[0].Groups[1].Value.ToString() : "A";
                    sessionId = mc[0].Groups[2].Success ? mc[0].Groups[2].Value.ToString() : "1";
                }

                room = PlayRoomsManager.GetOrCreate(sessionId, rule);
                var key = CreatePlayerKey();

                pAdapter = room.FindRemotePlayer(key) as GamePlayerAdapter;
                if (pAdapter != null && !pAdapter.IsConnected)
                {
                    // 再接続とみなす
                    pAdapter.Reconnect(playerName, (str)=>Send(str));
                }
                else
                {
                    // すでにプレイ中なら切断する
                    if (room == null || room.Master.IsPlaing) { this.Close(); return; }

                    pAdapter = new GamePlayerAdapter(room, playerName, (str)=>Send(str));
                    room.AddPlayer(key, pAdapter);
                }
            }
        }

        public override void OnClose()
        {
            AllClients.Remove(this);
            if (pAdapter != null) pAdapter.Disconnect();
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
                var pd = pAdapter.ProcMessage(jsonObj);
                if (pd != null) Send(pd.ToJson());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Send(new ProtocolData(e).ToJson());
            }
        }

		private string CreatePlayerKey() {
            return WebSocketContext.UserAgent + "\t" + WebSocketContext.UserHostAddress;
        }
/*
        class PlayerAdapter : IGamePlayer
        {
            PlayerHandler playerHandler;
            private DaifugoPlayRoom room = null;
            private int playerNum = 0;
            private string playerName;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="room"></param>
			/// <param name="ph"></param>
            public PlayerAdapter(DaifugoPlayRoom room, PlayerHandler ph)
            {
                this.room = room;
                playerHandler = ph;
                playerName = ph.playerName;
                playerNum = room.Master.NumOfPlayers;
            }

            public void Reconnect(PlayerHandler ph) { playerHandler = ph; playerName = ph.playerName; }

            public void Disconnect() { playerHandler = null; }

            public bool IsConnected { get { return playerHandler != null; } }

            public ProtocolData ProcMessage(JObject jsonObj)
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
                    //AllClients.Broadcast(jsonObj.ToString());
                    // todo
                }
                else if (kind == "Start")
                {
                    // 自動的に不足人数を追加してゲーム開始する
                    room.DoComplementPlayers(5);
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
                if (playerHandler != null)
                {
                    string str = wsp.ToJson();
                    Debug.WriteLine("Send: " + str);
                    playerHandler.Send(str);
                }
            }

            // =================================== IGamePlayer Impl ===================================

            public string Name { get { return playerName + (playerHandler != null ? "" : "(離脱)"); } }

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
                if (IsConnected)
                {
                    // タイマースタート
                    _startTimer(30 * 1000);
                }
                else
                {
					room.Master.PutCards(this, DeckGenerator.FromCardsetString(""));
                }
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


            public void BindEvents(GameEvents evt)
            {
                evt.agari += (arg) => this.Agari(arg(this) as IPlayerContext);
                evt.cardDistributed += (arg) => this.CardDistributed(arg(this) as IPlayerContext);
                evt.cardsArePut += (arg) => this.CardsArePut(arg(this) as IPlayerContext);
                evt.cardSwapped += (arg) => this.CardSwapped(arg(this) as IPlayerContext);
                evt.finish += (arg) => this.Finish(arg(this) as IPlayerContext);
                evt.kakumei += (arg) => this.Kakumei(arg(this) as IPlayerContext);
                evt.nagare += (arg) => this.Nagare(arg(this) as IPlayerContext);
                evt.start += (arg) => this.Start(arg(this) as IPlayerContext);
                evt.thinking += (arg) => this.Thinking(arg(this) as IPlayerContext);
            }
        }
		*/
    }
}