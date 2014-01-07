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
    internal class GamePlayerAdapter : IRemoteGamePlayer, ITweetListener
    {
        internal int TurnTimeoutSec = 30;
        private Action<string> _sendFunc;
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

		/// <summary>
		/// 再接続
		/// </summary>
		/// <param name="playerName"></param>
		/// <param name="sendFunc"></param>
        public void Reconnect(string playerName, Action<string> sendFunc) { 
            _sendFunc = sendFunc;
            this.playerName = playerName;
            
			// 最後に送ったデータを再送
            if (lastSendData != null)
            {
                sendFunc(lastSendData);
            }
        }

		/// <summary>
		/// 切断
		/// </summary>
        public void Disconnect() { _sendFunc = null; }

		/// <summary>
		/// 接続されているかどうか
		/// </summary>
        public bool IsConnected { get { return _sendFunc != null; } }

		/// <summary>
		/// クライアントメッセージの処理
		/// </summary>
		/// <param name="jsonObj"></param>
		/// <returns></returns>
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
                _startTimer(TurnTimeoutSec * 1000);
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
            room.Master.PutCards(this, null);
        }

        public void CardsArePut(IPlayerContext ctx)
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


        public void bindEvents(GameEvents evt) { ((IGamePlayer)this).BindEvents(evt); }
        public void unbindEvents(GameEvents evt) { ((IGamePlayer)this).UnbindEvents(evt); }

        public void Tweet(string message)
        {
            if (_sendFunc != null)
            {
                JObject json = new JObject();
                json["Kind"] = "Tweet";
                json["Message"] = message;
                _sendFunc(json.ToString());
            }
        }
    }
}