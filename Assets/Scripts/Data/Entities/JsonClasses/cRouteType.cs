//Diadrasis ©2023
using System;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class cRouteType 
	{
		public int route_type_id, route_type_order;
		public string route_type_label, route_type_icon, route_type_poi_icon, route_type_poi_icon_active, route_type_poi_icon_visited;
		public cLocalizedText[] route_type_title, route_type_short_description;

        public bool HasPoiData()
        {
            return !route_type_poi_icon.IsNull() &&
                   !route_type_poi_icon_active.IsNull() &&
                   !route_type_poi_icon_visited.IsNull();

        }
    }

}
