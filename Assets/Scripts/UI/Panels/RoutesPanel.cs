//Diadrasis ©2023
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class RoutesPanel : PanelBase
	{
        public bool HideRoutesWithoutPois = true;
        [Space]
        public List<RouteMarkerUI> routeMarkers = new List<RouteMarkerUI>();



		public void Init(AreaEntity areaEntity)
        {
			//if (Application.isEditor)
			//{
			//	Debug.Log("area routes are " + areaEntity.routes.Count);
			//	areaEntity.routes.ForEach(b => Debug.Log(b.routeType.route_type_id));
			//}

			routeMarkers.ForEach(b => b.gameObject.SetActive(false));

            bool hasRoutes = areaEntity.routes.Count > 0;

			guiController.infoPanelMotion.OnShowPercentageComplete.RemoveListener(EnableButtons);

			if (hasRoutes)
			{
				List<RouteEntity> routessOrdered = areaEntity.routes.OrderBy(w => w.route.route_id).ToList();//.routeType.route_type_order).ToList();
                //check if periods exceeds buttons
                int len = routessOrdered.Count > routeMarkers.Count ? routeMarkers.Count : routessOrdered.Count;
                for (int i = 0; i < len; i++)
                {
					if (HideRoutesWithoutPois)
					{
						if (routessOrdered[i].HasPeriods() || routessOrdered[i].HasPoiEntities())
						{
							routeMarkers[i].gameObject.SetActive(true);
							routeMarkers[i].Init(routessOrdered[i]);
						}
					}
					else
					{
                        routeMarkers[i].gameObject.SetActive(true);
                        routeMarkers[i].Init(routessOrdered[i]);
                    }


					//Debug.Log("route: " + routessOrdered[i].route.route_id);
					//Debug.Log("type: " + routessOrdered[i].route.route_type_id);
                }
				Show(true);
			}
			else
			{
				Show(false);
			}
        }

        public override void OnShow()
        {
			DisableButtons();
        }

        public override void AddEvents()
        {
			guiController.infoPanelMotion.OnShowPercentageComplete.AddListener(EnableButtons);
		}

        public override void RemoveEvents()
        {
			guiController.infoPanelMotion.OnShowPercentageComplete.RemoveListener(EnableButtons);
		}

		void EnableButtons()
		{
			routeMarkers.ForEach(b => b.btn.interactable = true);
		}
		void DisableButtons()
		{
			routeMarkers.ForEach(b => b.btn.interactable = false);
		}

	}

}
