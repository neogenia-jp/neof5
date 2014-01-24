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
    public class PracticeHandler : WebSocketHandler
    {
        private const int TIMEOUT_SEC = 60 * 5;
        private const int PLAYROOM_LIFETIME = 60 * 30;

        public const string ROOM_ID_PREFIX = "P";

        DaifugoPlayRoom room;
        string sessionId;

        GamePlayerAdapter pAdapter = null;

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

            var mc = Regex.Matches(this.WebSocketContext.RequestUri.OriginalString, @"/test/practice/([\-A-Z0-9]+)");
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

            var key = this.WebSocketContext.CreatePlayerKey(playerName);

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
            if (room.Master.NumOfPlayers == 1)
            {
                // 他のプレイヤーを追加
                room.DoComplementPlayers(4);

                // ゲーム開始
                room.Master.Start();
            }
        }

        public override void OnClose()
        {
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