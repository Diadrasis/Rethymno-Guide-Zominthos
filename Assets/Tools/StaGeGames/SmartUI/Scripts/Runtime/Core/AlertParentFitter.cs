//StaGe Games ©2022
using StaGeGames.BestFit.Extens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StaGeGames.BestFit 
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
	public class AlertParentFitter : UIBehaviour
	{
        public BestFitter fitterParent;
        [Space]
        public bool resetPosOnParentChanged;

        private RectTransform rect;

        public Scrollbar scrollbar;
        public bool autoHideScrollBar;

        private TMPro.TMP_InputField inputFieldPro;
        private TMPro.TextMeshProUGUI textMesh;
        private InputField inputField;
        private Text text;
        private Image img; //img.type == Image.Type.Sliced || img.type == Image.Type.Tiled

        protected override void Start()
        {
            inputFieldPro = GetComponent<TMPro.TMP_InputField>();
            inputField = GetComponent<InputField>();
            rect = GetComponent<RectTransform>();
            GetParentFitter();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            Debug.LogWarning("###############  IMPROVE THIS! ######################################################################");

            rect = GetComponent<RectTransform>();
            if (fitterParent) fitterParent.Init();
            Invoke(nameof(CheckScrollBar), 0.2f);
        }

        void CheckScrollBar()
        {
            CancelInvoke();
            if (!scrollbar) return;

            if (autoHideScrollBar)
            {
                
                if ((int)scrollbar.direction < 2)//horizontal
                {
                    float preferredWidth = inputField != null ? inputField.preferredWidth : inputFieldPro != null ? inputFieldPro.preferredWidth : -1f;
                    scrollbar.gameObject.SetActive(preferredWidth > rect.RealSize().x + 0.01f);
                }
                else
                {
                    float preferredHeight = inputField != null ? inputField.preferredHeight : inputFieldPro != null ? inputFieldPro.preferredHeight : -1f;
                    scrollbar.gameObject.SetActive(preferredHeight > rect.RealSize().y + 0.01f);
                }
            }
            else
            {
                if (!scrollbar.gameObject.activeSelf) scrollbar.gameObject.SetActive(true);
            }
        }

        protected override void OnTransformParentChanged()
        {
            GetParentFitter();
            if (resetPosOnParentChanged)
            {
                if (rect) rect.anchoredPosition = Vector2.zero;
            }
        }

        void GetParentFitter()
        {
            fitterParent = transform.parent != null ? transform.parent.GetComponent<BestFitter>() : null;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            if (this.GetComponent<Canvas>() || this.transform.parent == null)
            {
                CancelInvoke();
                DestroyImmediate(this);
                return;
            }
            if (this.GetType().Name == Utils.CommonUtilities.baseclass)
            {
                CancelInvoke();
                DestroyImmediate(this); 
                return;
            }
            rect = GetComponent<RectTransform>();
            GetParentFitter();


        }
#endif
    }

}
