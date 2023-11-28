//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class FakeGPS : MonoBehaviour
	{
		public AreaEntity area;
        private void Awake()
        {
#if !UNITY_EDITOR
			Destroy(this);
			return;
#endif
			EventHolder.OnAreaInfoShow += OnDatabaseReaded;

		}
        void OnDatabaseReaded(AreaEntity _area)
		{
			area = _area;
		}

	}

}
