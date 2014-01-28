using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daifugo.Cards;
using Daifugo.Rules;
using Daifugo.Bases;

namespace Daifugo.GameImples
{

    internal class GameContext : BaseGameContext, IDisposable
    {
        // プレイヤー
        public List<IGamePlayer> _players = new List<IGamePlayer>();
        public override IEnumerable<IPlayerInfo> PlayerInfo { get { return from p in _players select _getPInfoFuncs[p](); } }

        // 手番
        private int _teban;
        public override int Teban { get { return _teban; } set { _teban = value < _players.Count ? value : 0; } }

        //// 親
        //private int _dealer;
        //public override int LastPutPlayerNum { get { return _dealer; } set { _dealer = value < _players.Count ? value : 0; } }

        // 場に出ているカード
        //[Obsolete("error")]
        //public readonly List<Card[]> _ba = new List<Card[]>();
        public override IEnumerable<IEnumerable<Card>> Ba { get { 
            int tricIdx = _history.FindLastIndex(e=>(e is HE_Nagare));
            tricIdx++;
            return _history.GetRange(tricIdx, _history.Count - tricIdx).Where(h=>h is HE_PutCards).Select(h=>((h as HE_PutCards).Cards));
        } }

        // カードヒストリ
        public readonly List<GameHistoryEntity> _history = new List<GameHistoryEntity>();
        public override IEnumerable<GameHistoryEntity> History { get { return _history; } }

        // 流れたカード
        //[Obsolete]
        //public readonly List<Card> _yama = new List<Card>();
        //[Obsolete]
        //public override IEnumerable<Card> Yama { get { return _yama; } }
        public override IEnumerable<Card> Yama
        {
            get
            {
                int tricIdx = _history.FindLastIndex(e => (e is HE_Nagare));
                if (tricIdx < 0) yield break;
                foreach (HE_PutCards ienum in (from r in _history.GetRange(0, tricIdx) where r is HE_PutCards select r))
                {
                    foreach (var c in ienum.Cards) yield return c;
                }
            }
        } 

        // 革命中かどうか
        public override bool IsKakumei { get; set; }

        // ルール
        public override IRule Rule { get; protected set; }
        internal void SetRule(IRule rule) { Rule = rule; }

        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        /// <param name="rule"></param>
        internal GameContext(IRule rule)
        {
            Rule = rule;
        }

        public override void AddPlayer(IGamePlayer p, Func<IPlayerInfo> f)
        {
            _players.Add(p);
            base.AddPlayer(p, f);
        }

        public void Reset()
        {
            //_ba.Clear();
            //_yama.Clear();
            _history.Clear();
            IsKakumei = false;
            Teban = 0;
        }

        public IGamePlayer GetCurrentPlayer()
        {
            return _players[Teban];
        }

        public void Dispose()
        {
            _players = null;
            _history.Clear();
        }
    }
}
