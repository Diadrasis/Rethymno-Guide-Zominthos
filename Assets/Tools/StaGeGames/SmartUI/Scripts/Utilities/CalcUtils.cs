//StaGe Games ©2022
using StaGeGames.BestFit.Extens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaGeGames.BestFit.Utils
{
	public class CalcUtils : MonoBehaviour
	{
        /// <summary>
        /// Clamps from 1f to Mathf.Infinity
        /// <para>Avoid DivideByZeroException</para>
        /// </summary>
        /// <returns>1f - Mathf.Infinity</returns>
        public static float GetPositiveRealFloat(float val)
        {
            return Mathf.Clamp(Mathf.Abs(val), 1f, Mathf.Infinity);
        }

        /// <summary>
        /// Clamps from 1f to Mathf.Infinity
        /// <para>Avoid DivideByZeroException</para>
        /// </summary>
        /// <returns>foreach axis: 1f - Mathf.Infinity</returns>
        public static Vector2 GetPositiveRealVector2(Vector2 val)
        {
            return new Vector2(GetPositiveRealFloat(val.x), GetPositiveRealFloat(val.y));
        }

        /// <summary>
        /// Clamps from 0f to Mathf.Infinity
        /// </summary>
        /// <returns>0f -  Mathf.Infinity</returns>
        public static float GetPositiveFloat(float val)
        {
            return Mathf.Clamp(Mathf.Abs(val), 0f, Mathf.Infinity);
        }
        /// <summary>
        /// Clamps from 0f to Mathf.Infinity
        /// </summary>
        /// <returns>foreach axis: 0f -  Mathf.Infinity</returns>
        public static Vector2 GetPositiveVector2(Vector2 val)
        {
            return new Vector2(GetPositiveFloat(val.x), GetPositiveFloat(val.y));
        }

        public static Vector2 GetNoNanVector2Position(Vector2 val)
        {
            if (float.IsNaN(val.x) || float.IsPositiveInfinity(val.x) || float.IsNegativeInfinity(val.x)) val.x = 0f;
            if (float.IsNaN(val.y) || float.IsPositiveInfinity(val.y) || float.IsNegativeInfinity(val.y)) val.y = 0f;
            return val;
        }

        /// <summary>
        /// Substract two Vectors
        /// </summary>
        /// <returns>Vector2 subtraction result</returns>
        public static Vector2 SubtractRectSizes(RectTransform a, RectTransform b)
        {
            return a.RealSize().IsBiggerThan(b.RealSize()) ? a.RealSize() - b.RealSize() : b.RealSize() - a.RealSize();
        }

        public static bool IsEqualIntVectors(Vector2 a, Vector2 b)
        {
            return a.ToInt() == b.ToInt();
        }
        public static bool IsEqualIntVectors(Vector3 a, Vector3 b)
        {
            return a.ToInt() == b.ToInt();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="minMax"></param>
        /// <returns>new Vector2(Mathf.Clamp(v.x, minMax.x, minMax.y), Mathf.Clamp(v.y, minMax.z, minMax.w));</returns>
        public static Vector2 ClampVector2(Vector2 v, Vector4 minMax)
        {
            return new Vector2(Mathf.Clamp(v.x, minMax.x, minMax.y), Mathf.Clamp(v.y, minMax.z, minMax.w));
        }

    }

}
