//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public abstract class PanelBase : MonoBehaviour
	{
		public GameObject panel;
		public UIController guiController;

		public void Show(bool val) 
		{
			OnShow();
			if (val) { AddEvents(); } else { RemoveEvents(); }
			panel.SetActive(val);  
		}

		public virtual void Init() { }

		public abstract void AddEvents();
		public abstract void RemoveEvents();

		public abstract void OnShow();


	}

}
