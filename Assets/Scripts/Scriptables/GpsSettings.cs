//Diadrasis ©2023 - Stathis Georgiou
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[CreateAssetMenu(fileName = "GPS_Settings", menuName = "Settings/New GPS Settings")]
	[Serializable]
	public class GpsSettings : ScriptableObject
	{
		[Header("[EDITOR ONLY]--------------------")]
		public bool EditorDebug = true;
		[Header("-----------------------------------")]

		[Header("Time Interval to check gps is enabled")]
		public float gpsCheckTimeInterval = 5f;
		[Header("GPS Update distance (meters)")]
		public float gpsUpdateDistance = 5f;
		[Header("Filter - GPS Update distance (meters)")]
		public bool useFilterOnGpsUpdateDistance;
		[Header("Area minimum distance (meters)")]
		public float minDistanceForOnSiteMode = 1000f;
		[Header("Poi max trigger distance (meters)")]
		public float triggerMaxDistanceForPois = 30f;
        [Header("Poi start trigger distance (meters)")]
        public float triggerStartDistanceForPois = 12f;
        [Header("Auto calculate pois trigger distance")]
		public bool autoCalculateTriggerDistForPois;

	}

}
