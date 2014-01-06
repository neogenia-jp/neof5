using Daifugo.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Bases
{
	using PlayerInfoType = IPlayerInfo;

    public interface ICardDistributer
    {
        IList<Card>[] Distribute(IEnumerable<PlayerInfoType> players, IEnumerable<Card> cardset);
    }
}
