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
using Daifugo.Observers;
using System.Text.RegularExpressions;
using WebDaifugo.AppClasses;
using WebDaifugo.Models;
using WebDaifugo.Basis;

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
                var key = this.WebSocketContext.CreatePlayerKey(playerName);

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

    }
}