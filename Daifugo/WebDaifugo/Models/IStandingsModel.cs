using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDaifugo.Models
{
	/// <summary>
	/// プレイヤーの成績表を表すインタフェース
	/// </summary>
    public interface IStandingsModel
    {
		// プレイヤー名
        string PlayerName { get; }

		// ランクごとの成った回数の合計
        int NumTimesDaifugo { get; }
        int NumTimesFugo { get; }
        int NumTimesHeimin { get; }
        int NumTimesHinmin { get; }
        int NumTimesDaihinmin { get; }

		// 直前ラウンドでの得点
        int RoundPoint { get; }

		// 第1ラウンドからの累計得点
        int TotalPoint { get; }
    }
}
