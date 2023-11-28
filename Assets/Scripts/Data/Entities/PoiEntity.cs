//Diadrasis ©2023
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class PoiEntity
	{
		public cPoi poi;
		public int ID { get { return poi.poi_id; } }
		public List<cImage> images = new List<cImage>();
		public List<cVideo> videos = new List<cVideo>();

		public int OrderID() { return poi.poi_order; }

		[Space]
		//[HideInInspector]
		public Texture2D poiIcon;
		//[HideInInspector]
		public Texture2D poiIconActive, poiIconVisited;

		public void SetPoiIcons(Texture2D _poiIcon, Texture2D _poiIconActive, Texture2D _poiIconVisited)
		{
			poiIcon = _poiIcon;
			poiIconActive = _poiIconActive;
			poiIconVisited = _poiIconVisited;
		}

		public bool IsNull { get { return ID <= 0; } }
	}

}
