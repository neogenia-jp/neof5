using Microsoft.Web.WebSockets;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Daifugo.Bases;
using Daifugo.Cards;
using Daifugo.Utils;
using Daifugo.Observers;
using Daifugo.GameImples;
using WebDaifugo.Neof5Protocols;
using WebDaifugo.AppClasses;
using WebDaifugo.Basis;
using System.Text.RegularExpressions;

namespace WebDaifugo.WsHandlers
{
    using PlayerInfoType = IPlayerInfo;

	/// <summary>
	/// テストのためのカード配置を行うクラス
	/// </summary>
    internal class TestCardDistributer : ICardDistributer
    {
        IList<Card>[] ret;

        internal TestCardDistributer(string p0, string p1, string p2)
        {
            ret = new[]{
				DeckGenerator.FromCardsetString(p0),
				DeckGenerator.FromCardsetString(p1),
				DeckGenerator.FromCardsetString(p2),
			};
        }
        public IList<Card>[] Distribute(IEnumerable<PlayerInfoType> players, IEnumerable<Card> cardset)
        {
            return ret;
        }
    }

	/// <summary>
	/// テストシナリオ通りの手を打つプレイヤー
	/// </summary>
    internal class TestcasePlayer : BasePlayer
    {
        string[] hands;
        int p = 0;

		internal TestcasePlayer(string name, Action<IEnumerable<Card>> putCardCallbac, string[] hands) : base(name, putCardCallbac) {this.hands=hands;}

        protected override IEnumerable<Card> _TurnCame(IPlayerContext ctx)
        {
			var cards = DeckGenerator.FromCardsetString(hands[p]);
            if (cards.Count==0 || ctx.GameContext.Rule.CheckPutCards(ctx.GameContext, cards) is CheckOK)
            {
                p++;
                return cards;
            }
            return null;
        }

        internal string GetInitialDeck() { return hands.JoinString(); }
    }

	/// <summary>
	/// テスト設定パラメータ
	/// </summary>
    internal static class TestSettings
    {
		// プレイヤーの初期手札
        internal static string Player0InitialDeck = "C3 C4 H5 S5 D6 S7 C7 D8 CJ DQ CK";

		// 対戦相手1の行動順
        internal static string[] Player1Hands = new[] {
			"S3", "",   "",          "CA SA", "C5",    "", "DA", "H0 D0 C0 S0", "SQ", "S2"
        };
		// 対戦相手2の行動順
        internal static string[] Player2Hands = new[] {
			"H6", "HA",     "H4 D4",  "",         "HK"
        };
		// 答え
        internal static string[] Ansers = new[] {
			/* H6 */ "S7 C7 D8 CJ DQ CK",
			/* HA */ "",
			/* H4 D4 */ "H5 S5",
			/* CA SA */ "",
			/* HK */ "",
			/*    */ "C3 C4 H5 S5 D6 S7 C7 D8 CJ DQ CK",
			/* DA */ "",
			/* H0 D0 C0 S0 */ "",
			/* SQ */ "C3 C4 H5 S5 D6 S7 C7 D8 CJ"
        };
    }

    public class RuleTestHandler: WebSocketHandler
    {
        private const int TIMEOUT_SEC = 60*5;
        private const int PLAYROOM_LIFETIME = 60*30;

        public const string ROOM_ID_PREFIX = "RT";

        DaifugoPlayRoom room;
        string sessionId;

        GamePlayerAdapter pAdapter = null;

		TestcasePlayer p1 {get;set;}
		TestcasePlayer p2 {get;set;}

        private System.Timers.Timer myTimer = new System.Timers.Timer();
        private void _startTimer(int time)
        {
            myTimer.AutoReset = false;
            myTimer.Interval = time;
            myTimer.Elapsed += OnTimer;
            myTimer.Enabled = true;
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            myTimer.Enabled = false;
            room.Dispose();
            PlayRoomsManager.Remove(sessionId);
            SendMessageAsJson("Timeout.");
            base.Close();
        }

        public override void OnOpen()
        {
            // nameパラメータの取り出し
            var playerName = this.WebSocketContext.QueryString["name"];

            if (string.IsNullOrWhiteSpace(playerName))
            {
                SendMessageAsJson("URL Parameter 'name' is not specified.");
                Close();
            }

            var mc = Regex.Matches(this.WebSocketContext.RequestUri.OriginalString, @"/test/ruletest/([\-A-Z0-9]+)");
            if (mc.Count > 0)
            {
                sessionId = mc[0].Groups[1].Success ? mc[0].Groups[1].Value.ToString() : "";

                if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    room = PlayRoomsManager.Get(sessionId);
                }
            }

            if (room == null || !sessionId.StartsWith(ROOM_ID_PREFIX))
            {
                SendMessageAsJson("Invalid session ID");
                base.Close();
                return;
            }

            _startTimer(PLAYROOM_LIFETIME * 1000);

            var key = this.WebSocketContext.CreatePlayerKey();

            pAdapter = room.FindRemotePlayer(key) as GamePlayerAdapter;
            if (pAdapter != null && !pAdapter.IsConnected)
            {
                // 再接続とみなす
                pAdapter.Reconnect(playerName, (str) => Send(str));
            }
            else
            {
                // プレイヤー通信ハンドラを作成
                pAdapter = new GamePlayerAdapter(room, playerName, (str) => Send(str));
                pAdapter.TurnTimeoutSec = TIMEOUT_SEC;
                room.AddPlayer(key, pAdapter);
            }
			if (room.Master.NumOfPlayers == 1){
                // 他のプレイヤーを追加
                p1 = new TestcasePlayer("革命マシン", (c) => room.Master.PutCards(p1, c), TestSettings.Player1Hands);
                p2 = new TestcasePlayer("早あがり太郎", (c) => room.Master.PutCards(p2, c), TestSettings.Player2Hands);
                room.AddPlayer("1", p1);
                room.AddPlayer("2", p2);

                room.Master.CardDistributer = new TestCardDistributer(TestSettings.Player0InitialDeck, p1.GetInitialDeck(), p2.GetInitialDeck());

                // ゲーム開始
                room.Master.Start();
            }
        }

        public override void OnClose() { 
            if (pAdapter != null) pAdapter.Disconnect();
        }

        public override void OnError() { }

        public override void OnMessage(string jsonMsg)
        {
            JObject jsonObj = JObject.Parse(jsonMsg);

            try
            {
                var pd = pAdapter.ProcMessage(jsonObj);
                if (pd != null) Send(pd.ToJson());
            }
            catch (Exception e)
            {
                Send(new ProtocolData(e).ToJson());
            }
        }

        private void SendMessageAsJson(string msg)
        {
            JObject jsonObj = new JObject();

            jsonObj["Kind"] = "Rule Test";
            jsonObj["Message"] = msg;

            Send(jsonObj.ToString());
        } 
    }
}