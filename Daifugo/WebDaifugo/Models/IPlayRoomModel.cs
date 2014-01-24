using Daifugo.GameImples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDaifugo.Models
{
	/// <summary>
	/// プレイルームを表すインタフェース
	/// </summary>
    public interface IPlayRoomModel
    {
		// ゲームマスタ
        GameMaster Master { get; }

		/// <summary>
		/// プレイルームID
		/// </summary>
        string RoomID { get; }

		/// <summary>
		/// プレイヤーの成績表のコレクション
		/// </summary>
		IEnumerable<IStandingsModel> Standings { get; }

		/// <summary>
		/// 現在のラウンド数
		/// </summary>
        int NumOfRounds { get; }
    }
}