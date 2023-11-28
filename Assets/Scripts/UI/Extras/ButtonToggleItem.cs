//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{

	public class ButtonToggleItem : MonoBehaviour
	{
        [Header("[---Use A as enable action or starting state---]")]
        public Sprite icon_A;
        public Sprite icon_B;
        public string key_A, key_B;

        [Space]
        public bool applyColors;
        public Color colA, colB;

		public Image img;
        public LocaleItem localeItem;

        private void Awake()
        {
            if (IsText())
            {
                localeItem.SetKeys(key_A, key_B);
            }
            if (IsAble())
            {
                img.sprite = icon_A;
                if (applyColors) img.color = colA;
            }
        }

        public void SetSprite(bool isEnabled)
        {
            img.sprite = isEnabled ? icon_A : icon_B;
            if (applyColors) img.color = isEnabled ? colA : colB;
        }

        public void SetPeriodEnabled(bool isEnabled)
        {
            img.sprite = !isEnabled ? icon_A : icon_B;
            if (applyColors) img.color = !isEnabled ? colA : colB;
        }

        public void SwapAction()
        {
            if (IsText())
            {
                localeItem.SwapKeys();
            }

            if (IsAble())
            {
                img.sprite = img.sprite == icon_A ? icon_B : icon_A;
                if (applyColors) img.color = img.sprite == icon_A ? colA : colB;
            }
        }

        bool IsText() { return localeItem != null && !key_A.IsNull() && !key_B.IsNull(); }

		bool IsAble() { return img != null && icon_A != null && icon_B != null; }

        public bool IsEnabled()
        {
            if(!IsAble()) return false;
            return img.sprite == icon_B;
        }
	}

}
