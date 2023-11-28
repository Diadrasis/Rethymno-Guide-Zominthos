//Diadrasis ©2023
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class RouteEntity
	{
		public cRoute route;
		public cRouteType routeType;
		//only for route_type 9535 (Monuments)
		public List<PeriodEntity> periods = new List<PeriodEntity>();
		public List<PoiEntity> poiEntities = new List<PoiEntity>();
		public List<cImage> routeImages = new List<cImage>();

		public string GetRouteTitle()
		{
			if (routeType == null) { return DataManager.Instance.GetTraslatedText(route.route_title); }
			return DataManager.Instance.GetTraslatedText(routeType.route_type_title);
		}

		public string GetRouteIcon()
		{
			if(!string.IsNullOrEmpty(route.route_icon)) { return route.route_icon; }
            if (routeType == null) { return string.Empty; }
			return routeType.route_type_poi_icon;
        }

		public bool IsNull() { return route.area_id == 0; }
		public bool HasPeriods() { return periods.Count > 0; }
		public bool HasPoiEntities() {  return poiEntities.Count > 0; }
	}

}
