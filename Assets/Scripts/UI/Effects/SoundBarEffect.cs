//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{

	public class SoundBarEffect : MonoBehaviour
	{

		public Image fillBar;
		public Color fillColor;

		void Start()
		{
			fillBar.color = fillColor;
			SetBar(0f);
		}

		public void SetBar(float val) { fillBar.fillAmount = Mathf.Clamp01(val); }
		
	}

}
