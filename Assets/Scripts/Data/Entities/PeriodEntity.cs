//Diadrasis ©2023
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno
{
    [Serializable]
	public class PeriodEntity
	{
		public cPeriod period;
		public List<PoiEntity> poiEntities = new List<PoiEntity>();
		public Texture2D periodIcon;
        public Texture2D periodIconVisited;
        public bool IsNull() { return period.period_id == 0; }

		public bool HasPoiEntities() { return poiEntities.Count > 0; }
	}

}
