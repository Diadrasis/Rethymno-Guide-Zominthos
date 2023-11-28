//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

    public static class EnumExtension
    {
        public static T Parse<T>(this System.Enum aEnum, string aText)
        {
            return (T)System.Enum.Parse(typeof(T), aText);
        }
    }

}
