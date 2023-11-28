//Diadrasis ©2023
using System;
using System.Globalization;
using UnityEngine;

namespace Diadrasis.Rethymno
{
	[Serializable]
	public class cPoi
	{
		public int poi_id, route_id, period_id, poi_order;

		public string poi_geo, poi_icon, poi_bibliography;

		public cLocalizedText[] poi_title, poi_short_description, poi_description, poi_narration, poi_testimony;

		public Vector2 geoPosition;

		//convert geo coordinates string to vector2
		//for online maps
		public void ReadGeoPosition()
		{
			string[] splitArray = poi_geo.Split('-');
			if (splitArray.Length == 2)
			{
				if (float.TryParse(splitArray[0], out float lon) && float.TryParse(splitArray[1], out float lat))
				{
					//var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
					//culture.NumberFormat.NumberDecimalSeparator = ".";
					lon = float.Parse(splitArray[0], System.Globalization.CultureInfo.CreateSpecificCulture("en-GB"));// System.Globalization.CultureInfo.InvariantCulture);
					lat = float.Parse(splitArray[1], System.Globalization.CultureInfo.CreateSpecificCulture("en-GB"));// System.Globalization.CultureInfo.InvariantCulture);
					geoPosition = new Vector2(lon, lat);
				}
			}
		}

	}

}
