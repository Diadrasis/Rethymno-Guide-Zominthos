//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{

	public class ButtonMarkerBase : MonoBehaviour
	{
		public Button btn;
		public RawImage img;
		public TMPro.TextMeshProUGUI txt;

		public void SetText(string val) { if (txt) txt.text = val; }

		Texture2D tex;
		public void SetTexture(string _texName, bool isPeriod)
		{
			if (img)
			{
                tex = SaveLoadManager.LoadTexture(_texName, GlobalUtils.iconMarkerButtonEmpty);

                //Texture2D copy = SaveLoadManager.DuplicateTexture(tex);
                //copy.alphaIsTransparency = true;
                //if (isPeriod) { AppRuntimeValues.colPeriodTitle = copy.GetPixel(60, 10);  }
                //else { AppRuntimeValues.colRouteTitle = copy.GetPixel(60, 10); }

                img.texture = tex;
			}
		}


		private void OnDestroy()
		{
			if (btn) btn.onClick.RemoveAllListeners();
		}
	}

}
