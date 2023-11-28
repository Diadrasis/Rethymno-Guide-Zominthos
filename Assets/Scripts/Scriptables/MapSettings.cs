//Diadrasis ©2023
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Diadrasis.Rethymno.EnumsHolder;

namespace Diadrasis.Rethymno 
{
	[CreateAssetMenu(fileName = "Map_Settings", menuName = "Settings/New Map Settings")]
	[Serializable]
	public class MapSettings : ScriptableObject
	{
		[Header("[EDITOR ONLY]--------------------")]
		public bool drawCircleAtCenterOfAreasZoom;
		public bool EditorDebug = true;
		[Header("-----------------------------------")]

		[Header("[---Initial Settings---]")]
		public MapInitialArgs initialArgs;
		[Header("-----------------------------------")]

		[Header("[---Visual Settings---]")]
		public MapVisualArgs visual;
		[Header("-----------------------------------")]

		[Header("[---USER Marker Settings---]")]
		public Texture2D userMarkerIcon;
		public float userMarkerScale = 1f;

		[Header("[---Area Marker Settings---]")]
		public MapAreaArgs areaMarker;
		[Header("-----------------------------------")]

		[Header("[---POI Marker Settings---]")]
		public MapPoiArgs poisOptions;
		[Header("-----------------------------------")]

		[Header("[CUSTOM AREA, ROUTE, PERIOD ZOOM]")]
		public bool useCustomAreaZoom;
		[Range(7f, 20f)]
		public float zoomAreaManual = 18f;
		[Header("-----------------------------------")]

		[Header("[SMOOTH MOVE MAP]")]
		public bool useSmoothnessForAllAreasView = true;
		public bool useSmoothnessForArea = true;
		public bool useSmoothnessForAllPoisView = true;
		/// <summary>
		/// Move duration (sec)
		/// </summary>
		[Range(1f, 3f)]
		public float smoothTime = 1f;
		public SmoothType smoothType = SmoothType.MoveAndZoom;


	
	}

}
