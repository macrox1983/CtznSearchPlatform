using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Extensions
{
    public static class DictionaryExtensions
    {        
        /// <summary>
        /// Возвращает текст или если значения нет то ""
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetStringOrDefault<TKey>(this IDictionary<TKey, string> source, TKey key)
        {
            if (!source.TryGetValue(key, out string result)) return "";
            return result;
        }
        /// <summary>
        /// Возвращает текст или если значения нет то ""
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValueOrOther<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue other)
        {
            if (!source.TryGetValue(key, out TValue result)) return other;
            return result;
        }

        /// <summary>
        /// Возвращает значени или default
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            if (!source.TryGetValue(key, out TValue result)) return default;
            return result;
        }


        /// <summary>
        /// Возвращает значени или default
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<TValue> TakeFromKeyToPrev<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, int count)
        {           
            List<TKey> keys = source.Keys.ToList();
            int index = keys.IndexOf(key);
            int from = index - count;
            if (from < 0) from = 0;
            int to = from + count;
            if (to > keys.Count) to = keys.Count;
            List<TValue> result = new List<TValue>();
            for (int i=from;i<to;i++) result.Add(source[keys[i]]);
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Возвращает значени или default
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<TValue> TakeFromKeyToNext<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, int count)
        {
            List<TKey> keys = source.Keys.ToList();
            int index = keys.IndexOf(key);
            int to = index + count+1;
            if (to > keys.Count) to = keys.Count;
            List<TValue> result = new List<TValue>();
            for (int i = index+1; i < to; i++) result.Add(source[keys[i]]);
            //result.Reverse();
            return result;
        }
    }
}
