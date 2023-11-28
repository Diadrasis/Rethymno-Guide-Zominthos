//Diadrasis ©2023 - Stathis Georgiou
using UnityEngine;

namespace Diadrasis.Rethymno
{

	[DisallowMultipleComponent]
    public class TextFontTrigger : MonoBehaviour
	{

		TMPro.TextMeshProUGUI textMesh;
		[SerializeField]
		private float defaultFontSize;

		public void InitializeTrigger()
        {
			textMesh = GetComponent<TMPro.TextMeshProUGUI>();
			defaultFontSize = textMesh.fontSize;

			EventHolder.OnFontSizeNormal += SetFontSizeNormal;
			EventHolder.OnFontSizeMedium += SetFontSizeMedium;
			EventHolder.OnFontSizeLarge += SetFontSizeBig;
		}


		void SetFontSizeNormal() { textMesh.fontSize = defaultFontSize; }
		void SetFontSizeMedium() { textMesh.fontSize = defaultFontSize + 6f; }
		void SetFontSizeBig() { textMesh.fontSize = defaultFontSize + 12f; }

	}

}

