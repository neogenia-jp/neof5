using Microsoft.Web.WebSockets;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDaifugo.WsHandlers
{
    public class TestcaseHandler: WebSocketHandler
    {
        private string playerName;

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
            base.Close();
        }

        public override void OnOpen()
        {
            _startTimer(60 * 1000);

            // nameパラメータの取り出し
            playerName = this.WebSocketContext.QueryString["name"];

            if (string.IsNullOrWhiteSpace(playerName))
            {
                Send("URL Parameter 'name' is not specified.");
                Close();
            }
        }

        public override void OnClose() { myTimer.Enabled = false; }

        public override void OnError() { }

        public override void OnMessage(string jsonMsg)
        {
            try
            {
                JObject jsonObj = JObject.Parse(jsonMsg);

                Send(jsonObj.ToString());
            }
            catch (Exception e)
            {
                Send(e.Message);
            }
        }
    }
}