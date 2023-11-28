//Diadrasis ©2023
using System;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class cLocalizedText
	{
		public string key;
		[TextAreaAttribute(2, 10)]
		public string text;
	}

}
