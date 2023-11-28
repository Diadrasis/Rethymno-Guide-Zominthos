//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class MathUtils : MonoBehaviour
	{

		public static bool V3Equal(Vector3 a, Vector3 b)
		{
			return Vector3.SqrMagnitude(a - b) < 0.0001;
		}

		public static bool V2Equal(Vector2 a, Vector2 b)
		{
			// Calculate the distance in km
			float distance = OnlineMapsUtils.DistanceBetweenPoints(a, b).magnitude;
			return distance < 0.01f;
			//return Vector2.SqrMagnitude(a - b) < 0.00001;
		}

		/// <summary>
		///   <para>Compares two floating point values and returns true if they are similar.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static bool Approximately(float a, float b)
		{
			return (double)Mathf.Abs(b - a) < (double)Mathf.Max(1E-06f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8f);
		}

		public static bool SimilarFloats(float a, float b)
        {
			return Mathf.Abs(a - b) < 0.01;
        }

	}

}
