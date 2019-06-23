using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Extensions
{
    public static class StringExtensions
    {        
        public static string ReplaceCharOnPosition(this string source, string oldChar, string newChar, int position)
        {
            if (position < 1) return source.Replace(oldChar, newChar);
            int index = source.IndexOf(oldChar);
            for (var i=1; i<position; i++) index = source.IndexOf(oldChar, index+1);
            if (index < 0) return "";
            return source.Substring(0, index) + newChar + source.Substring(index + 1);
        }

        /// <summary>
        /// Удаляет все повторы символа подряд кроме одного.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RemoveMoreOneCharInLine(this string source, char c)
        {
            var newString = "";
            var was = false;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == c)
                {
                    if (!was) was = true; else continue;
                }
                else was = false;
                newString += source[i];
            }
            return newString;
        }
        /// <summary>
        /// Удаляет из строки все вхождения символа
        /// </summary>
        /// <param name="source"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RemoveAllChar(this string source, char c)
        {
            var newString = "";
            for (int i = 0; i < source.Length; i++)
                if (source[i] != c) newString += source[i];
            return newString;
        }
        public static string ConvertToHexCode(this string source)
        {
            string result = "";
            for (var i = 1; i < source.Length; i++) result= result+ Convert.ToUInt16(source[i]).ToString("X2");
            return result;
        }
    }
}
