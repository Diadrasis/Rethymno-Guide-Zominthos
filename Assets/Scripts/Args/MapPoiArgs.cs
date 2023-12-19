//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Diadrasis.Rethymno
{
	[Serializable]
	public class MapPoiArgs
	{
		[Space]
		public bool deleteSavedVisitedPoisOnStart = true;

        [Space]
        [Header("-----------------------------------")]
        public bool createNativePoiMarkers = true;

		[Header("[POI native marker scale]")]
		[SerializeField]
		private float poiNativeMarkerScale = 1.2f;
        [Header("[POI native marker scale with camera pos Y algorithm]")]
        public bool useCameraAlgorithm;
        public float cameraLogY = 1400f;
        [Header("-----------------------------------")]

		[Header("if False then order is sorted from instatiate]", order = 2)]
		[Space(-10, order = 1)]
		[Header("[Order is a.Y > b.Y => a appears under b", order = 0)]
		public bool changeSortOrderOfPOIMarkers = true;//pois

		[Header("[POI MARKER]")]
		public bool zoomOnPoiClick;
		[Header("[In case of wrong json data]")]
		public bool dontCreateMarkerIfFarFromArea = true;
		public float maxKmDistFromArea = 5f;

		[Header("POIs consecutively creation")]
		public bool createPoisInRow = true;

        [Header("ON-SITE Options")]
        [Tooltip("[Manual] Allow poi to manually clicked by user?")]
        public bool allowPoiClick = true;
        [Tooltip("[Manual] Allow triggered poi to be clickable by user?")]
        public bool allowVisitedMarkerToBeSelectedAgain = true;
        [Tooltip("[Auto] Allow poi to be triggered while info panel is open?")]
        public bool allowPoiSelectionIfInfoPanelIsOpen = true;
        [Tooltip("[Auto] Allow auto triggered poi to be triggered again?")]
        public bool allowTriggeredPoiToBeTriggeredAgain = true;
		[Tooltip("[Auto] Auto trigger poi info (clicked) if user location is near")]
        public bool enablePoiInfoIfUserIsNear = true;//show info title
        [Tooltip("[Auto] Trigger poi info if narration is not playing")]
        public bool checkPoiInfoIfNarrationPlaying = true;//show info title
        [Header("[On-Site] Αν απομακρυνθεί ο χρήστης απο το poi να κλείσει η πληροφορία")]
        public bool HidePoiInfoIfUserIsFar;


        public float GetNativeMarkerScale()
        {
            if (useCameraAlgorithm)
            {
                Camera cam = Camera.main;
                if (cam)
                {
                    return cam.transform.position.y / cameraLogY;
                }
            }
            return poiNativeMarkerScale;
        }

    }

}
