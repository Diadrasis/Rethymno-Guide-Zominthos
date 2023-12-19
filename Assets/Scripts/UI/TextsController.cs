//Diadrasis ©2023 - Stathis Georgiou
using StaGeGames.BestFit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{

	public class TextsController : MonoBehaviour
	{
		public EnumsHolder.FontSizeState fontSizeState;
		[Space]
		public Button btnFontSizer;
        public TMPro.TextMeshProUGUI txtLabel;

		List <TMPro.TextMeshProUGUI> texts = new List<TMPro.TextMeshProUGUI>();

        public BestFitter bestFitterMessages, bestFitterInfo;
		
		void Start()
		{

			texts = FindObjectsOfType<TMPro.TextMeshProUGUI>(true).ToList();

			foreach(TMPro.TextMeshProUGUI txt in texts)
            {
                if(txt.GetComponent<IgnoreTrigger>() == null)
                    txt.gameObject.AddComponent<TextFontTrigger>().InitializeTrigger();
            }

            btnFontSizer.onClick.AddListener(ChangeFontSize);

			EventHolder.OnFontSizeNormal += SetFontSizeNormal;
			EventHolder.OnFontSizeMedium += SetFontSizeMedium;
			EventHolder.OnFontSizeLarge += SetFontSizeBig;

            txtLabel.text = "";
        }

		void ChangeFontSize()
        {
            switch (fontSizeState)
            {
                case EnumsHolder.FontSizeState.NORMAL:
                    EventHolder.OnFontSizeMedium?.Invoke();
                    break;
                case EnumsHolder.FontSizeState.MEDIUM:
                    EventHolder.OnFontSizeLarge?.Invoke();
                    break;
                case EnumsHolder.FontSizeState.LARGE:
                    EventHolder.OnFontSizeNormal?.Invoke();
                    break;
                default:
                    EventHolder.OnFontSizeNormal?.Invoke();
                    break;
            }

            bestFitterMessages.Init();
            bestFitterInfo.Init();
        }


        void SetFontSizeNormal() { fontSizeState = EnumsHolder.FontSizeState.NORMAL;   txtLabel.text = ""; }
        void SetFontSizeMedium() { fontSizeState = EnumsHolder.FontSizeState.MEDIUM;  txtLabel.text = "<sup>+"; }
        void SetFontSizeBig() { fontSizeState = EnumsHolder.FontSizeState.LARGE;  txtLabel.text = "<sup>+<space=15>+"; }

        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1)) { EventHolder.OnFontSizeNormal?.Invoke(); }
        //    else if (Input.GetKeyDown(KeyCode.Alpha2)) { EventHolder.OnFontSizeMedium?.Invoke(); }
        //    else if (Input.GetKeyDown(KeyCode.Alpha3)) { EventHolder.OnFontSizeLarge?.Invoke(); }
        //}
    }

}

