//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Diadrasis.Rethymno
{
    [Serializable]
    public class MapInitialArgs
    {
        [Header("Greece View (long, lat)")]
        public Vector2 greeceMapPos = new Vector2(23.6729876618352f, 37.8665345360163f);
        [Range(7f, 14f)]
        public float zoomGreece = 7.8f;
        [Header("-----------------------------------")]

        [Space(10f)]
        [Header("[---AREAs CUSTOM CENTER ON START---]")]
        public bool useCustomAreasCenter = true;
        public Vector2 customAreasCenter = new Vector2(23.7193795746872f, 37.975656852048f);
        [Space]//at start - areas zoom
        [Range(7f, 20f)]
        [SerializeField]
        private float areasPreferredZoom = 17f;
        

        public float GetAreasPrefferedZoom()
        {
            float log = (float)Screen.width / (float)Screen.height * 1f;

            if (log < 0.525f && log >= 0.49f)
            {
                return areasPreferredZoom - 0.5f;
            }
            return areasPreferredZoom;
        }
        [Header("-----------------------------------")]

        [Header("[---MAP LIMITS---]")]

        /// <summary>
        /// Flag indicating that need to limit the zoom.
        /// </summary>
        public bool useZoomRange;

        /// <summary>
        /// The minimum zoom value.
        /// </summary>
        public float minZoom = 14f;

        /// <summary>
        /// The maximum zoom value. 
        /// </summary>
        public float maxZoom = OnlineMaps.MAXZOOM_EXT;

        /// <summary>
        /// Flag indicating that need to limit the position.
        /// </summary>
        public bool usePositionRange;

        /// <summary>
        /// The minimum latitude value.
        /// </summary>
        public float minLatitude = 37.73f;

        /// <summary>
        /// The maximum latitude value. 
        /// </summary>
        public float maxLatitude = 38.17f;

        /// <summary>
        /// The minimum longitude value.
        /// </summary>
        public float minLongitude = 23.6f;

        /// <summary>
        /// The maximum longitude value. 
        /// </summary>
        public float maxLongitude = 23.85f;

        /// <summary>
        /// Type of limitation position map.
        /// </summary>
        public OnlineMapsPositionRangeType positionRangeType = OnlineMapsPositionRangeType.center;

    }


}
