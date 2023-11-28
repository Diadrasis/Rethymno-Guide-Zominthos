//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class AreaMarkerUI : MarkerUIBase
	{
        public override void Init()
        {
			base.Init();
			IsArea = true;
			SetPosition(areaEntity.area.geoPosition);
			SetText(FixedTitle(areaEntity));
			SetTexture(areaEntity.area.area_icon);
		}

		private string FixedTitle(AreaEntity area)
        {
			string s = DataManager.Instance.GetTraslatedText(areaEntity.area.area_title);

			s= s.Replace("Ά","Α")
				.Replace("ά", "α")
				.Replace("Ί", "Ι")
				.Replace("ί", "ι")
				.Replace("Ή", "Η")
				.Replace("ή", "η")
				.Replace("Ύ", "υ")
				.Replace("ύ", "υ")
				.Replace("Έ", "Ε")
				.Replace("έ", "ε")
				.Replace("Ό", "ο")
				.Replace("ό", "ο")
				.Replace("Ώ", "Ω")
				.Replace("ώ", "ω");

			return s.ToUpper();

		}
    }

}
