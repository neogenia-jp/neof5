using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Utils
{
    public static class CollectionUtils
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string JoinString<T>(this IEnumerable<T> list, string delimiter=" ")
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in list) sb.Append(c.ToString()).Append(delimiter);
            if (sb.Length > 0) sb.Length -= 1;
            return sb.ToString();
        }
    }
}
