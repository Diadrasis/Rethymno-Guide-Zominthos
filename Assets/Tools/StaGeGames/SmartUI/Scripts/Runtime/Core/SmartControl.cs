//StaGe Games ©2022
using UnityEngine;

namespace StaGeGames.BestFit 
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	public class SmartControl : MonoBehaviour
	{
		[HideInInspector]
		public BestResize smartResize;
		[HideInInspector]
		public SmartMotion smartMotion;
	}

}
