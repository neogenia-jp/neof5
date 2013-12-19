using Daifugo.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDaifugo.Basis
{
    public static class Extentions
    {
        public static int PointOfRank(this PlayerRank rank)
        {
            switch (rank)
            {
                case PlayerRank.DAIFUGO: return 5;
                case PlayerRank.FUGO: return 4;
                case PlayerRank.HEIMIN: return 3;
                case PlayerRank.HINMIN: return 2;
            }
            return 0;
        }

    }
}