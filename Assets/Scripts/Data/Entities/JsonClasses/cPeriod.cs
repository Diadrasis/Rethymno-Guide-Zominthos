//Diadrasis ©2023
using System;

namespace Diadrasis.Rethymno
{
    [Serializable]
	public class cPeriod 
	{
		public int period_id, period_order;
		public string period_color, period_icon, period_poi_icon, period_poi_icon_active, period_poi_icon_visited;

		public cLocalizedText[] period_title, period_short_description;
	}

}
