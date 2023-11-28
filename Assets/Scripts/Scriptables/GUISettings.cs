//Diadrasis ©2023
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[CreateAssetMenu(fileName = "GUI_Settings", menuName = "Settings/New GUI Settings")]
	[Serializable]
	public class GUISettings : ScriptableObject
	{

		[Header("Bottom Info [Deprecated]")]
		[Range(0.1f, 2f)]
		public float waitTimeToShowBottomInfo = 1f;

		[Header("[---Map Overlay initial view---]")]
		public bool showOverlayAtStart;

        [Header("[---INFO PANEL---]")]
		public InfoPanelArgs infoArgs;

		[Header("[---IMAGES---]")]
		public ImagesArgs imagesArgs;
	}

}
