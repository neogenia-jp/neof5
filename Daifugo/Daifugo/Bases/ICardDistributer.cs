using Daifugo.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Bases
{
	using PlayerInfoType = IPlayerInfo;

	/// <summary>
	/// カードの配布ロジックを分離するためのインタフェースクラス
	/// </summary>
    public interface ICardDistributer
    {
		/// <summary>
		/// カードの配布
		/// </summary>
		/// <param name="players"></param>
		/// <param name="cardset"></param>
		/// <returns></returns>
        IList<Card>[] Distribute(IEnumerable<PlayerInfoType> players, IEnumerable<Card> cardset);

		/// <summary>
		/// カードの交換	
		/// </summary>
		/// <param name="players"></param>
        bool SwapCards(IDictionary<PlayerInfoType, IList<Card>> players);
    }
}
