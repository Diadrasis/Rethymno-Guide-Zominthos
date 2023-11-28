//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class MapAreaArgs 
	{
		public bool createNativeMarkers = false;
		[Header("[AREA native marker scale]")]
		public float nativeMarkerScale = 1.3f;
        [Header("[AREA GUI marker scale]")]
        public float guiMarkerScale = 0.7f;

    }

}
