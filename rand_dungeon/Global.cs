using System;
using System.Collections.Generic;
using System.Linq;

namespace rand_dungeon
{
    static class Global
    {
        public static Random random = new Random();
    }

    public static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            TValue rtn;
            dic.TryGetValue(key, out rtn);
            return rtn;
        }

        public static void AddOrReplace<TKey, TValue>
        (
            this Dictionary<TKey, TValue> self,
            TKey key,
            TValue value
        )
        {
            if (self.ContainsKey(key))
            {
                self[key] = value;
            }
            else
            {
                self.Add(key, value);
            }
        }
    }
}
