//Diadrasis ©2023
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class PeriodsPanel : PanelBase
	{
		public bool HidePeriodsWithoutPois = true;
		[Space]
		public List<PeriodMarkerUI> periodMarkers = new List<PeriodMarkerUI>();
		
		public void Init(RouteEntity routeEntity)
		{
			periodMarkers.ForEach(b => b.gameObject.SetActive(false));

			//bool hasPeriods = routeEntity.periods.Count > 0;

            if (routeEntity.HasPeriods())
			{

				//Debug.LogWarning("PERIODS ARE = " + routeEntity.periods.Count);
    //            Debug.LogWarning("PERIODS[0] order is = " + routeEntity.periods[0].period.period_order);
    //            Debug.LogWarning("PERIODS[0] pois are = " + routeEntity.periods[0].poiEntities.Count);

                List<PeriodEntity> periodsOrdered = routeEntity.periods.OrderBy(w => w.period.period_order).ToList();

                //check if periods exceeds buttons
                int len = periodsOrdered.Count > periodMarkers.Count ? periodMarkers.Count : periodsOrdered.Count;
				for (int i = 0; i < len; i++)
				{
					List<PeriodEntity> previousPeriods = new List<PeriodEntity>();
					if(i > 0)
                    {
						for(int p=0; p<i; p++)
                        {
							if (HidePeriodsWithoutPois)
							{
								if(periodsOrdered[p].HasPoiEntities()) previousPeriods.Add(periodsOrdered[p]);
							}
							else { previousPeriods.Add(periodsOrdered[p]); }
						}
                    }

                    bool hasPois = HidePeriodsWithoutPois ? periodsOrdered[i].HasPoiEntities() : true;
                    periodMarkers[i].gameObject.SetActive(hasPois);
					periodMarkers[i].Init(periodsOrdered[i], previousPeriods);

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
			periodMarkers.ForEach(b => b.btn.interactable = true);
		}

		void DisableButtons()
		{
			periodMarkers.ForEach(b => b.btn.interactable = false);
		}

	}

}
