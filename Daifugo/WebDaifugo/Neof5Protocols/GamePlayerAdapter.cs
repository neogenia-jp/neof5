using Daifugo.Bases;
using Daifugo.Cards;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using WebDaifugo.AppClasses;
using WebDaifugo.Basis;

namespace WebDaifugo.Neof5Protocols
{
    internal class GamePlayerAdapter : IRemoteGamePlayer
    {
        Action<string> _sendFunc;
        private DaifugoPlayRoom room = null;
        private int playerNum = 0;
        private string playerName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="room"></param>
        /// <param name="ph"></param>
        public GamePlayerAdapter(DaifugoPlayRoom room, string playerName, Action<string> ph)
        {
            this.room = room;
            _sendFunc = ph;
            this.playerName = playerName;
            playerNum = room.Master.NumOfPlayers;
        }

        public void Reconnect(string playerName, Action<string> sendFunc) { 
            _sendFunc = sendFunc;
            this.playerName = playerName;
            
			// 最後に送ったデータを再送
            if (lastSendData != null)
            {
                sendFunc(lastSendData);
            }
        }

        public void Disconnect() { _sendFunc = null; }

        public bool IsConnected { get { return _sendFunc != null; } }

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
                if (!(ret is CheckOK)) return new WebDaifugo.Neof5Protocols.ProtocolData(ret);
                myTimer.Enabled = false;
            }
            return null;
        }

        internal void Send(ProtocolData wsp)
        {
            if (_sendFunc != null)
            {
                string str = wsp.ToJson();
                Debug.WriteLine("Send: " + str);
                _sendFunc(str);
                lastSendData = str;
            }
        }

        private string lastSendData = null;

        // =================================== IGamePlayer Impl ===================================

        public string Name { get { return playerName + (_sendFunc != null ? "" : "(離脱)"); } }

        public void Start(IPlayerContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Start", ctx));
        }

        public void Finish(IPlayerContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Finish", ctx));
        }

        public void ProcessTurn(IPlayerContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "ProcessTurn", ctx));
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
            Send(new Neof5Protocols.ProtocolData(playerNum, "CardsArePut", ctx));
        }

        public void Nagare(IPlayerContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Nagare", ctx));
        }

        public void Agari(IPlayerContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Agari", ctx));
        }

        public void Kakumei(IPlayerContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Kakumei", ctx));
        }

        public void CardDistributed(IPlayerContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "CardDistributed", ctx));
        }

        public void CardSwapped(IPlayerContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "CardSwapped", ctx));
        }

        public void Thinking(IPlayerContext ctx)
        {
            Send(new Neof5Protocols.ProtocolData(playerNum, "Thinking", ctx));
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
}