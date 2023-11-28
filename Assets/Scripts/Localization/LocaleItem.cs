//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class LocaleItem : MonoBehaviour
	{
		public string key;
		public string key_2;
		private string key_now;

		private TMPro.TextMeshProUGUI txt;

		void Start()
		{
			key_now = key;
			if (!txt) txt = GetComponent<TMPro.TextMeshProUGUI>();
			if (!txt) return;
			EventHolder.OnLanguageChanged += ChangeLanguage;
			ChangeLanguage();
		}

		public void SetKeys(string k1, string k2)
        {
			key = k1; key_2 = k2;
			key_now = k1;
			ChangeLanguage();
        }

		public void SwapKeys()
        {
			key_now = key_now == key ? key_2 : key;
        }

		void ChangeLanguage()
        {
			if (!txt) return;
			txt.text = DataManager.Instance.GetTermText(key_now);
		}


#if UNITY_EDITOR
		//editor
		public void ApplyCurrentLanguage(EnumsHolder.Language lang)
        {
			if (!txt) txt = GetComponent<TMPro.TextMeshProUGUI>();
			if (!txt) return;
			DataManager dt = DataManager.Instance;
			if (dt == null)
            {
				dt = FindObjectOfType<DataManager>();
				if (dt == null) return;
            }
			//draft save
			EnumsHolder.Language lng = dt.SelectedLanguage;
			//change
			dt.SelectedLanguage = lang;
			dt.SetLangNow();
			//apply
			txt.text = dt.GetTermText(key);
			//restore
			dt.SelectedLanguage = lng;
			dt.SetLangNow();
		}
#endif
	}

}
