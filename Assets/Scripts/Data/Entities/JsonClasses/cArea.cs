//Diadrasis ©2023
using System;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class cArea
	{
		public int area_id;
		public string area_icon, area_image;
		public cLocalizedText[] area_title, area_short_description, area_description;
		public string area_geo;//coordinates


        public Vector2 geoPosition;

		//convert geo coordinates string to vector2
		//for online maps
		public void ReadGeoPosition()
        {
			string[] splitArray = area_geo.Split('-'); 
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
