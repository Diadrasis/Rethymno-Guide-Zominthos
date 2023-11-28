//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class InfoPanelArgs
	{
		[Range(0.1f, 2f)]
		public float timeToMoveTitleInfo = 0.5f;
		[Range(0.1f, 2f)]
		public float timeToMoveFullInfo = 0.5f;
		[Space]
		public bool autoShowInfoOnMapSmoothMoveStop = true;
		[Space]
		public bool hideInfoOnMapDrag;
		public bool showShortDescOnFullInfo;
	}

}
