//Diadrasis ©2023
using System;
using UnityEngine;

namespace Diadrasis.Rethymno
{
	[Serializable]
	public class cIcon
	{
		public EnumsHolder.IconType iconType = EnumsHolder.IconType.NULL;
		public bool isPeriod;
		public int period_id;
		public int route_type_id;
		public string filename;
	}

}
