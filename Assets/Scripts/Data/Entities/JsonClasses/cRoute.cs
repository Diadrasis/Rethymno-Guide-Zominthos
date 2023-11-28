//Diadrasis ©2023
using System;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class cRoute
	{
		public int route_id, area_id, route_type_id;
		public string route_bibliography, route_icon;
        public cLocalizedText[] route_title, route_short_description, route_description;

        public bool HasType() { return route_type_id > 0; }
    }

}
