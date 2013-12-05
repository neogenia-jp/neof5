using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Cards
{
    /// <summary>
    /// スート（トランプのマークを表す）
    /// </summary>
    public enum Suit : byte
    {
        SPD = 0x50,  /// spades スペード
        HRT = 0x40,  /// hearts ハート
        DIA = 0x30,  /// diamonds ダイヤ
        CLB = 0x20,  /// clubs クローバー
        JKR = 0x10   /// joker ジョーカー
    }
}
