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

namespace WebDaifugo.WsHandlers
{
    public class PlayerHandler : WebSocketHandler, IGamePlayer
    {
        private static Object lockObj = new object();
        private static WebSocketCollection AllClients = new WebSocketCollection();

        private string sessionId = null;
        private string rule = null;

        private string playerName;
        private int playerNum = 0;
        private GameMaster gm = null;

        public override void OnOpen()
        {
            lock(lockObj) {
                AllClients.Add(this);
            
                playerName = this.WebSocketContext.QueryString["name"];
                sessionId = this.WebSocketContext.QueryString["sessionId"];
                rule = this.WebSocketContext.QueryString["rule"];
                gm = GameMasterManager.GetOrCreate(sessionId, rule);
                if (gm.IsPlaing)
                {
					// すでにプレイ中なら切断する
                    this.Close();
                    return;
                }

                playerNum = gm.NumOfPlayers;
                gm.AddPlayer(this);

            }
        }

        public override void OnClose()
        {
            lock(lockObj) {
                AllClients.Remove(this);
                
                // TODO
                GameMasterManager.Remove(sessionId);
            }
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
                var kind = jsonObj["kind"].ToString();
                if (kind == "Chat")
                {
                    AllClients.Broadcast(jsonObj.ToString());
                }
                else if (kind == "Start")
                {
                        // 自動的に不足人数を追加してゲーム開始する
                        var limit = 5 - gm.NumOfPlayers;
                        for (int i = 1; i <= limit; i++)
                        {
                            gm.AddPlayer(PoorPlayer.Create("COM" + i, gm));
                        }
                        gm.Start();
                }
                else if (kind == "Put")
                {
                    var ret = gm.PutCards(this, DeckGenerator.FromCardsetString(jsonObj["cards"].ToString()));
                    if (!(ret is CheckOK)) Send(new ProtocolData(ret));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Send(new ProtocolData(e));
            }
        }

        internal void Send(ProtocolData wsp)
        {
            string str = wsp.ToJson();
            Debug.WriteLine("Send: " + str);
            base.Send(str);
        }


        // =================================== IGamePlayer Impl ===================================

        public string Name { get { return playerName; } }

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
        }

        public void CardsArePut(IPlayerContext ctx)
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