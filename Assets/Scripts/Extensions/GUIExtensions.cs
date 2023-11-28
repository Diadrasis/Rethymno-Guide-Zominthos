using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno
{
    public static class GUIExtensions
    {
        public static T ChangeAlpha<T>(this T g, float newAlpha)
        where T : Graphic
        {
            var color = g.color;
            color.a = newAlpha;
            g.color = color;
            return g;
        }

        public static bool IsTransparent(this Image img)
        {
            return img.color.a < 1;
        }

        public static bool IsOpaque(this Image img)
        {
            return img.color.a == 1;
        }
    }
}
