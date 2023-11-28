//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class SplashController : MonoBehaviour
	{
		[Header("[Editor Only]")]
		public bool isActive;
		[Space]
		public float viewTime = 2f;
		[Space]
		public GameObject splashCanvas;
		public float animSpeed = 1f;
		

		Animator anim;

		private void Awake()
        {
			anim = splashCanvas.GetComponentInChildren<Animator>();


		}

        void Start()
		{
			anim.speed = animSpeed;

#if UNITY_EDITOR
			if (!isActive)
			{
				splashCanvas.SetActive(false);
				this.enabled = false;
				return;
			}
#endif
			splashCanvas.SetActive(true);

			//FadeOut();
			Invoke(nameof(FadeOut), viewTime);
		}

		

		void FadeOut()
        {
			anim.Play("splash-fade-out");
			Invoke(nameof(HideSplash), anim.GetCurrentAnimatorStateInfo(0).length);
		}

		void HideSplash() { splashCanvas.SetActive(false); }
	}

}
