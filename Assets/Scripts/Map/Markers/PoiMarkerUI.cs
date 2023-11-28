//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class PoiMarkerUI : MarkerUIBase
	{
        public override void Init()
        {
			base.Init();
			IsArea = false;

			SetPosition(poiEntity.poi.geoPosition);
			SetText(DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_title));

			if (!IsGuiMarker) SetTexture(iconName);
		}

	}

}
