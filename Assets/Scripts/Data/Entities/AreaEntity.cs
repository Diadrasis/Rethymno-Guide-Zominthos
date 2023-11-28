//Diadrasis ©2023
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class AreaEntity
	{
		public cArea area;
		public List<RouteEntity> routes = new List<RouteEntity>();
		public List<cImage> areaImages = new List<cImage>();

		public int ID { get { return area.area_id; } }	
		
		public void CreateImages()
		{
			areaImages.Clear();
			areaImages.Add(new cImage() { image_file = area.area_image });
		}

        public bool IsNull() { return area.area_geo.IsNull(); }
	}

}
