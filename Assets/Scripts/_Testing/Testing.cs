//Diadrasis ©2023
using System;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class Testing : MonoBehaviour
	{
		public JsonDataDatabase jsonDatabase;
		public MarkerEngineManager markerEngineManager;

		void Start()
		{
			if (jsonDatabase == null) return;

			foreach(AreaEntity area in jsonDatabase.areaEntities)
            {
				foreach(RouteEntity route in area.routes)
                {
					
                }
            }

			//markerEngineManager.CreateGuiPoiMarkers(jsonDatabase.areaEntities[0].routes[0].periods[0].poiEntities);
		}

	}

}
