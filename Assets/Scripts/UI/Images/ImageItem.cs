//Diadrasis ©2023
using StaGeGames.BestFit;
using StaGeGames.BestFit.Extens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{

	public class ImageItem : MonoBehaviour
	{
		public bool IsInWorldCanvas;
		[HideInInspector]
		public RawImage rawImage;
		public BestFitter bestFitterImage;
		public TMPro.TextMeshProUGUI txtTitle, txtLabel, txtSource;

		private RectTransform rectTitle, rectLabel, rectSource, rectBottomContainer;

		Button btnShowFullImage;

		private cImage currentImageData;
		GameObject imgParent;

		public void ImageInteractable(bool val) { rawImage.raycastTarget = val; }

		public bool HasTexture() { return rawImage.texture != null; }
		public Texture GetTexture() { return rawImage.texture; }
		public void ApplyTexture(Texture tex) { rawImage.texture = tex; RefreshTexture(); }

		public void ResetData()
        {
			title = label = source = string.Empty;
			txtTitle.text = txtLabel.text = txtSource.text = string.Empty;
			rawImage.texture = null;
        }

        public void ShowParent()
        {
            if (imgParent) imgParent.SetActive(true);
        }
        public void HideParent()
		{
			if(imgParent)imgParent.SetActive(false);
		}

		string title; public string GetTitle() { return title; }
		string label; public string GetLabel() { return label; }
		string source; public string GetSource() { return source; }

        private Texture2D tex2D;

        void Start()
		{
            imgParent = transform.parent.gameObject;
			rawImage = GetComponentInChildren<RawImage>();
			bestFitterImage = rawImage.GetComponent<BestFitter>();

			rectTitle = txtTitle.transform.parent as RectTransform;
			rectLabel = txtLabel.transform.parent as RectTransform;
			rectSource = txtSource.transform.parent as RectTransform;
			rectBottomContainer = rectLabel.transform.parent as RectTransform;

			btnShowFullImage = rawImage.GetComponent<Button>();
			btnShowFullImage.onClick.AddListener(ShowFullScreen);

        }

        private void OnEnable()
        {
            if(this.tex2D == null) this.tex2D = new Texture2D(2, 2);
        }

        void ShowFullScreen()
		{
			if (IsInWorldCanvas) 
			{
                EventHolder.OnTextureFullImage?.Invoke(rawImage.texture);
            }
			EventHolder.OnImageShowFull?.Invoke();
        }

		private bool IsAlreadyLoaded(cImage cImageData)
		{
			if (rawImage == null || rawImage.texture == null || currentImageData == null 
				|| currentImageData.IsNull || cImageData == null || cImageData.IsNull) return false;

			if (currentImageData.image_file == cImageData.image_file)
			{
				if (rawImage.texture.name == cImageData.image_file.WithNoExtension())
					return true;
			}

			return false;
		}

		public void SetElements(cImage cImageData, ImagesArgs args)
        {
			if (IsAlreadyLoaded(cImageData))
			{
				if (Application.isEditor) { Debug.Log("<color=yellow>IMAGE ALREADY LOADED! Aborting...</color>"); }
				return;
			}

			currentImageData = cImageData;

			HideAllTexts();

			SetTexture(cImageData.image_file);

			//if (rawImage.texture == null || rawImage.texture.name == GlobalUtils.textureEmpty) gameObject.SetActive(false);

			title = DataManager.Instance.GetTraslatedText(cImageData.image_title);
			label = DataManager.Instance.GetTraslatedText(cImageData.image_label);
			source = DataManager.Instance.GetTraslatedText(cImageData.image_source);

			if (args != null)
			{
				if (args.showTextsInline)
				{
					if (args.showTitle)
						SetText(txtTitle, title);

					SetText(txtLabel, label);
					SetText(txtSource, source);
					Invoke(nameof(RefreshRects), 0.25f);
				}
			}
		}

		void SetText(TMPro.TextMeshProUGUI txt, string val)
        {
			if (!val.IsNull())
			{
				txt.transform.parent.gameObject.SetActive(true);
				txt.text = val;
				txt.rectTransform.ForceRebuildLayout();
			}
		}

		void HideAllTexts()
        {
			txtTitle.transform.parent.gameObject.SetActive(false);
			txtLabel.transform.parent.gameObject.SetActive(false);
			txtSource.transform.parent.gameObject.SetActive(false);
		}

        void RefreshRects()
        {
            CancelInvoke();
            rectTitle.ForceRebuildLayout();
            rectLabel.ForceRebuildLayout();
            rectSource.ForceRebuildLayout();
            rectBottomContainer.ForceRebuildLayout();
        }


        public void SetTexture(string _name)
        {
			if(Application.isEditor) Debug.LogWarning(_name);

			if (_name.IsNull())
			{
				SetDefaultTexture();
                return;
			}
			//check resources first

            rawImage.texture = Resources.Load<Texture2D>(ResourcesPath(_name));

            if (rawImage.texture != null)
			{
                RefreshTexture();
				return;
            }

            string imgPath = SaveLoadManager.GetPath_ForImages() + _name.WithNoExtension();
			
            if (!SaveLoadManager.IsDiskImageExist(imgPath)) 
			{
				SetDefaultTexture();
                return; 
			}

            if (tex2D.LoadImage(System.IO.File.ReadAllBytes(imgPath)))
			{
				//rawImage.texture = tex2D;
				ApplyTexture(tex2D);
			}
			else
			{
				SetDefaultTexture();

            }
		}

		string ResourcesPath(string img) { return SaveLoadManager.GetPath_ForImages(true) + img.WithNoExtension(); }
		string DefaultImagePath() { return SaveLoadManager.GetPath_ForImages(true) + GlobalUtils.textureEmpty; }

        public void RefreshTexture()
		{
			bestFitterImage.Init();
		}

		public void DestroyTexture()
		{
            //Debug.LogWarning("DestroyTexture");
            rawImage.texture = null;
        }

		private void SetDefaultTexture()
		{
            rawImage.texture = Resources.Load<Texture2D>(DefaultImagePath());
            RefreshTexture();
        }

    }

}
