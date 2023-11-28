//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class PeriodMarkerUI : ButtonMarkerBase
	{
		public void Init(PeriodEntity selectedPeriod, List<PeriodEntity> previousPeriods = null)
		{
			btn.onClick.RemoveAllListeners();
			SetText(DataManager.Instance.GetTraslatedText(selectedPeriod.period.period_title));
			SetTexture(selectedPeriod.period.period_icon, true);
			btn.onClick.AddListener(() => EventHolder.OnPeriodsSelected?.Invoke(selectedPeriod, previousPeriods));
		}
	}

}
