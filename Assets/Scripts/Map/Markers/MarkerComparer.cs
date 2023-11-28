using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Diadrasis.Rethymno
{
    public class MarkerComparer : IComparer<OnlineMapsMarker>
    {
        public int Compare(OnlineMapsMarker m1, OnlineMapsMarker m2)
        {
            if (m1.position.y > m2.position.y) return -1;
            if (m1.position.y < m2.position.y) return 1;
            return 0;
        }
    }

}