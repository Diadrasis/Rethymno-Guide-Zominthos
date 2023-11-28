using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Diadrasis.Rethymno
{
    public static class StringExtensions
    {

        public static bool IsNull(this string str) {  return string.IsNullOrWhiteSpace(str); }

        public static void RemoveList(this List<string> list, List<string> listToRemove)
        {
            foreach (string item in listToRemove) { list.Remove(item); }
        }

        public static bool EndsWith(this string str, string[] vals)
        {
            foreach(string item in vals) { if (str.ToLower().EndsWith(item.ToLower())) return true; }
            return false;
        }

        public static string WithNoExtension(this string str) {  return Path.GetFileNameWithoutExtension(str); }
        
        public static void AddToList(this List<string> target, string val)
        {
            if (val.IsNull()) return;
            val = val.Trim();
            if (!val.IsNull())
            {
                if (!target.Contains(val))
                {
                    target.Add(val);
                }
            }
        }

        /// <summary>
        /// check strings ToLower
        /// </summary>
        /// <param name="val"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool ContainsWord(this string val, string word)
        {
            return val.ToLower().Contains(word.ToLower());
        } 

        public static Color HexToColor(this string val, Color def)
        {
            if (ColorUtility.TryParseHtmlString(val, out Color newCol))
            {
                return newCol;
            }
            return def;
        }

    }
}