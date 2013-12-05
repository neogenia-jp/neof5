using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daifugo.Cards;
using System.Diagnostics;
using Daifugo.Bases;

namespace Daifugo.Rules
{
    public static class RuleFactory
    {
        public static IRule GetDefaultRule()
        {
            return new KaidanRule();
        }
    }
}
