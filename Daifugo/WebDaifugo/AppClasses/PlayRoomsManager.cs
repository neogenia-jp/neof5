using Daifugo.GameImples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDaifugo.Models;

namespace WebDaifugo.AppClasses
{
	/// <summary>
	/// プレイルームのオブジェクト集合を一括管理するクラス
	/// </summary>
    public class PlayRoomsManager
    {
		// 未使用チェックのタイマー間隔
        private const int CHECK_UNUSED_INTERVAL_MSEC = 5 * 60 * 1000;

        private static Dictionary<string, DaifugoPlayRoom> pool = new Dictionary<string, DaifugoPlayRoom>();
        private static Random rand = new Random();

		// 自動消去するためのタイマー
        private static System.Timers.Timer myTimer;
        private static void _startTimer()
        {
            if (myTimer == null)
            {
                myTimer = new System.Timers.Timer();
                myTimer.AutoReset = false;
                myTimer.Interval = CHECK_UNUSED_INTERVAL_MSEC;
                myTimer.Elapsed += OnTimer;
            }
            myTimer.Enabled = true;
        }

		/// <summary>
		/// タイマーイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        public static void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (pool)
            {
                foreach (var p in pool.ToArray()) { if (p.Value.IsUnused()) pool.Remove(p.Key); }
            }
        }

		/// <summary>
		/// プレイルームの取り出し（インスタンスがなければ生成する）
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="rule"></param>
		/// <returns></returns>
        public static DaifugoPlayRoom GetOrCreate(string sessionId, string rule="A")
        {
            lock (pool)
            {
                sessionId = sessionId ?? "0";
                if (!pool.ContainsKey(sessionId))
                {
                    _startTimer();
					var pr = new DaifugoPlayRoom(sessionId, rule);
                    pool[sessionId] = pr;
                    pr.Master.wait_msec = 800;
                    return pr;
                }
                return pool[sessionId];
            }
        }

		/// <summary>
		/// プレイルームの取り出し。インスタンスがなければnullを返す。
		/// </summary>
		/// <param name="sessionid"></param>
		/// <returns></returns>
        public static DaifugoPlayRoom Get(string sessionid)
        {
            return pool.ContainsKey(sessionid) ? pool[sessionid] : null;
        }

		/// <summary>
		/// プレイルームの削除
		/// </summary>
		/// <param name="sessionId"></param>
        public static void Remove(string sessionId)
        {
            lock (pool)
            {
                sessionId = sessionId ?? "0";
                if (pool.ContainsKey(sessionId))
                {
                    pool.Remove(sessionId);
                }
            }
        }

		/// <summary>
		/// 一時的なプレイルームの作成（IDを作るだけ）
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
        public static string MakeTempSession(string prefix="")
        {
            var sessionId = (prefix + rand.Next(10000)).ToString();
            return pool.Keys.Contains(sessionId) ? MakeTempSession() : sessionId;
        }

    }
}