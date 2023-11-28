//Diadrasis ©2023
using StaGeGames.BestFit;
using StaGeGames.BestFit.Extens;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Diadrasis.Rethymno 
{

	public class ImagesController : MonoBehaviour
	{
        public EnumsHolder.InfoPanelState infoPanelState = EnumsHolder.InfoPanelState.Closed;
        [ReadOnly]
        public bool infoPanelFlagOpen;
        [Space]
        public bool HideNotExistingImages;
        [Space]
        public Color colBackground;
        public Image[] backgroundItems;

        [Space]
		public HorizontalScrollSnap scrollSnap;
        public GameObject imagesPanel, spaceImages;
		public Transform imagesContainer;

        [Space]
        public bool IsDragEnabled;

        [Space]
        public TMPro.TextMeshProUGUI txtTitleOut;
        public TMPro.TextMeshProUGUI txtLabelOut, txtSourceOut;
        private RectTransform rectTitleOut, rectLabelOut, rectSourceOut;

        [Space]
		public Transform prefabImageItem;

        [Header("[---FULL IMAGE---")]
        public GameObject fullPanel;
        public RawImage fullImageContainer;
        public BestFitter bestFitterImage;
        public ImagePanZoom panZoom;

        public bool IsFullImagePanelOpened() { return fullPanel.activeSelf; }

        void ShowFullImagePanel()
        {
            if (fullPanel.activeSelf) return;
            fullPanel.SetActive(true);
        }
        public void HideFullImagePanel()
        {
            fullPanel.SetActive(false);
            panZoom.ResetZoom();
        }

        [Header("[Object Pooling for images]")]
        public List<ImageItem> previousImageItems = new List<ImageItem>();
        public List<ImageItem> currentImageItems = new List<ImageItem>();

        public void Touchable(bool val)
        {
            previousImageItems.ForEach(item => item.ImageInteractable(val));
            currentImageItems.ForEach(item => item.ImageInteractable(val));
        }

        private int currentPoiId = -100;
        public bool IsSamePoiSelected(int id)
        {
            if (currentPoiId == id) return true;
            currentPoiId = id;
            return false;
        }

        UIController uiControl;

        ImagesArgs imgArgs;

        string _copyright = "©";

        private void Awake()
        {
            uiControl = FindObjectOfType<UIController>();
            imgArgs = uiControl.settings.imagesArgs;

            rectTitleOut = txtTitleOut.transform.parent as RectTransform;
            rectLabelOut = txtLabelOut.transform.parent as RectTransform;
            rectSourceOut = txtSourceOut.transform.parent as RectTransform;

            rectTitleOut.gameObject.SetActive(imgArgs.showTitle);

            EventHolder.OnNativePoiAreaClick += OnNativePoiAreaClick;
            EventHolder.OnGuiMarkerClick += OnMarkerClicked;
            EventHolder.OnARInfoTriggered += OnARInfoTriggered;

            EventHolder.OnNativePoiMarkerClick += OnNativePoiMarkerClick;

            EventHolder.OnRouteInfoShow += SetFromRoute;
            EventHolder.OnPeriodInfoShow += SetFromPeriod;
            EventHolder.OnAreaInfoShow += SetFromArea;

            EventHolder.OnPoiInfoClosed += SetPoiSelectedToNull;

            EventHolder.OnImageShowFull += ShowFullImagePanel;
            EventHolder.OnTextureFullImage += SetFullImage;

            EventHolder.OnInfoState += OnInfoState;
            //show images info in main container - out of images
            if (!imgArgs.showTextsInline)
            {
                scrollSnap.OnSelectionPageChangedEvent.AddListener(OnSelectionPageChangedEvent);
            }

            foreach (Image img in backgroundItems) img.color = colBackground;

            //Application.GarbageCollectUnusedAssets();
        }

        
        void OnInfoState(EnumsHolder.InfoPanelState state)
        {
            if(infoPanelState == state) return;
            infoPanelState = state;

            if(infoPanelState == EnumsHolder.InfoPanelState.Closed) 
            {
                //its time to free up memory by destroying textures
                if (infoPanelFlagOpen)
                {
                    //DestroyTextures();
                    infoPanelFlagOpen = false;
                }
            }
            else
            {
                infoPanelFlagOpen = true;
            }
        }

        void DestroyTextures()
        {
            previousImageItems.ForEach(b=>b.DestroyTexture());
            currentImageItems.ForEach(b => b.DestroyTexture());
        }

        void OnSelectionPageChangedEvent(int val)
        {
            if (previousImageItems.Count <= 0 || val >= previousImageItems.Count) { SetFullImage(null); return; }
            if (previousImageItems[val] == null) { SetFullImage(null); return; };
            if (previousImageItems[val].rawImage.texture == null) { SetFullImage(null); return; };

            if(imgArgs.showTitle) SetText(txtTitleOut, currentImageItems[val].GetTitle());
            SetText(txtLabelOut, previousImageItems[val].GetLabel());
            SetText(txtSourceOut, _copyright +" "+ previousImageItems[val].GetSource());
            RefreshInfoPanel();
            //Invoke(nameof(RefreshInfoPanel), 0.2f);
            SetFullImage(previousImageItems[val].rawImage.texture);
        }

        void SetFullImage(Texture tex)
        {
            fullImageContainer.texture = tex;
            bestFitterImage.Init();
        }

        void HideExternalTexts()
        {
            rectTitleOut.gameObject.SetActive(false);
            rectLabelOut.gameObject.SetActive(false);
            rectSourceOut.gameObject.SetActive(false);
        }

        void SetText(TMPro.TextMeshProUGUI txt, string val)
        {
            if (!val.IsNull() && val.Length > 3)
            {
                txt.transform.parent.gameObject.SetActive(true);
                txt.text = val;
                txt.rectTransform.ForceRebuildLayout();
            }
            else
            {
                txt.transform.parent.gameObject.SetActive(false);
            }
        }

        void SetFromArea(AreaEntity areaEntity) 
        {
            CreateImages(areaEntity.areaImages); /*HidePanel();*/ 
        }
        void SetFromRoute(RouteEntity routeEntity) 
        {
            CreateImages(routeEntity.routeImages); /*HidePanel();*/ 
        }
        void SetFromPeriod(PeriodEntity periodEntity) 
        { 
            //Debug.Log("CreateImages FromPeriod to Area Images");

            AreaEntity areaEntity = DataManager.Instance.GetAreaEntityFromPeriod(periodEntity);
            if (areaEntity == null)
            {
                HidePanel();
            }
            else
            {
                Debug.Log(areaEntity.areaImages.Count);
                CreateImages(areaEntity.areaImages);
            }
        }

        void HidePanel() { imagesPanel.SetActive(false); HideExternalTexts(); spaceImages.SetActive(false); }

        void OnNativePoiMarkerClick(PoiEntity poiEntity)
        {
            if (poiEntity == null) return;

            //if (IsSamePoiSelected(poiEntity.poi.poi_id)) return;
            CreateImages(poiEntity.images);
        }

        void OnNativePoiAreaClick(AreaEntity areaEntity)
        {
            HidePanel();
        }

        void OnARInfoTriggered(MarkerInstance marker)
        {
            OnMarkerClicked(marker);
        }

        void OnMarkerClicked(MarkerInstance marker)
        {
            if (marker == null || marker.data == null) return;
            if (marker.data.IsArea)
            {
                CreateImages(marker.data.areaEntity.areaImages);
                //HidePanel();
            }
            else
            {
                if (marker.data.poiEntity == null) return;
                //if (IsSamePoiSelected(marker.data.poiEntity.poi.poi_id)) return;
                CreateImages(marker.data.poiEntity.images);
            }
        }

        void CreateImages(List<cImage> images)
        {
            StartCoroutine(SmoothCreateImages(images));
        }

        [ReadOnly]
        public bool isCreatingImages;
        IEnumerator SmoothCreateImages(List<cImage> images)
        {
            while (isCreatingImages) yield return null;

            isCreatingImages = true;

            currentImageItems.Clear();
            if (images == null || images.Count <= 0)
            {
                previousImageItems.ForEach(_ => _.ResetData());
                //hide images panel
                HidePanel();
                isCreatingImages = false;
                yield break;
            }
            spaceImages.SetActive(true);
            imagesPanel.SetActive(true);

            imagesContainer.GetComponent<Image>().raycastTarget = IsDragEnabled ? images.Count > 1 : false;

            images = images.OrderBy(w => w.image_order).ToList();

            if (HideNotExistingImages)
            {
                List<cImage> existingImages = new List<cImage>();
                foreach (cImage img in images)
                {
                    if (SaveLoadManager.IsFileImageExist(img.image_file))
                    {
                        existingImages.Add(img);
                    }
                }
                images = existingImages;

                if(images.Count <= 0)
                {
                    HidePanel();
                    isCreatingImages = false;
                    yield break;
                }
            }

            //POOL LOGIC (avoid destroy-create)
            scrollSnap.RemoveAllChildren(out GameObject[] gbs);//remove from scroll snap - also remove from canvas

            foreach (GameObject gb in gbs)
            {
                ImageItem item = gb.GetComponent<ImageItem>();//get class
                if(item && !previousImageItems.Contains(item))
                    previousImageItems.Add(gb.GetComponent<ImageItem>());//add to previous list
            }

            //hide all in advance
            previousImageItems.ForEach(b => b.gameObject.SetActive(false));

            for (int i=0; i<images.Count; i++)
            {
                if(i < previousImageItems.Count)
                {
                    previousImageItems[i].gameObject.SetActive(true);//show/enable
                    previousImageItems[i].SetElements(images[i], imgArgs);

                    scrollSnap.AddChild(previousImageItems[i].gameObject);//add to scroll again
                   // currentImageItems.Add(previousImageItems[i]);
                }
                else
                {
                    Transform item = Instantiate(prefabImageItem, imagesContainer);
                    item.localScale = Vector3.one;
                    ImageItem imageItem = item.GetComponent<ImageItem>();
                    imageItem.SetElements(images[i], imgArgs);
                    currentImageItems.Add(imageItem);
                }

                yield return new WaitForEndOfFrame();// (0.1f);
            }

            List<ImageItem> items = previousImageItems;
            items.AddRange(currentImageItems);

           // Debug.Log("Total Images are " + items.Count);

            yield return new WaitForEndOfFrame();

            Invoke(nameof(DelayInitSnapLayout), 0.5f);

            isCreatingImages = false;
        }

        void DelayInitSnapLayout()
        {
            scrollSnap.Init();
            RefreshInfoPanel();
        }

        void RefreshInfoPanel()
        {
            rectTitleOut.ForceRebuildLayout();
            rectLabelOut.ForceRebuildLayout();
            rectSourceOut.ForceRebuildLayout();
            EventHolder.OnInfoRectRefresh?.Invoke();
        }

        void DestroyItems()
        {
            Transform[] itemsOld = imagesContainer.GetComponentsInChildren<Transform>();
            foreach (Transform gb in itemsOld)
            {
                if (gb != imagesContainer) Destroy(gb.gameObject);
            }
        }

        void SetPoiSelectedToNull() { currentPoiId = -100; }

    }

}
