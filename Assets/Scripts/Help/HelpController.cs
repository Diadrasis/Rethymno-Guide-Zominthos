//Diadrasis �2023 - Stathis Georgiou
using StaGeGames.BestFit;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Diadrasis.Rethymno
{

    public class HelpController : MonoBehaviour
    {
        [Header("[--- Auto scroll properties ---]")]
        public bool autoPlayOn = true;//enable auto play mode
        [Space]
        public float pageViewTime = 2f;//how much time the page will be on screen?
        public float pageTransitionSpeed = 10f;//change page speed
        public float autoPlayAfterIdleTime = 5f;//wait time to start auto scroll after last user's touch?
        public bool jumpToFirstPageOnAutoPlayRefocused;//jump to first page after idle time expires - if false continues from the current visible page
        public bool loopPlayPages = true;//when auto play mode is at last page, continue playing again from start
        [Tooltip("This is being ignored if loopPlayPages is true")]
        public bool closeHelpOnAutoScrollComplete;
        public bool disableCanvasOnHide = true;
        [Tooltip("After touching the screen for the first time, stop AutoPlay completely.")]
        public bool disableAutoPlayAfterTouch = true;

        [Header("[--- Fade effect ---]")]
        public CanvasGroup canvasGroup;
        public bool showWithFade;//if true enables fade in effect
        public bool hideWithFade;//if true enables fade out effect
        public float fadeTime = 1.2f;

        [Header("[--- The texture data for the page creation ---]")]
        public HelpDatabase helpDatabase;//the texture data for the page creation
        [Header("[--- The parent tranform of pages ---]")]
        public Transform pagesParent;//the horizontal container
        [Header("------------------------------")]
        [SerializeField]
        private HorizontalScrollSnap scrollSnap;
        
        public Button btnClose;
        public event System.Action HideHelpEvent; // an event for when this exits


        [Header("[--- Pages count text ---]")]
        public TextMeshProUGUI pageCounterText;

        //private variables
        private bool isTouchingScreen;
        private bool isAutoPlay;
        private int _pages;
        private float draftViewTime, draftAutoPlayAfterIdleTime;
        private bool fadeIn, fadeOut;
        private float timeElapsed;
        private float lerpDuration = 1.2f;
        private float startValue = 0;
        private float endValue = 10;
        private int isEnabledOnStart;
        private bool wasAutoPlayOn;

        [ReadOnly]
        public List<BestFitter> fitters = new List<BestFitter>();

        [Space]
        public bool setNonPowerOf2ToNone = true;
        [Space]
        public bool ShowHelpAtFirstTime;



        private void Awake()
        {
            isEnabledOnStart = canvasGroup.gameObject.activeSelf ? 1 : 0;
            wasAutoPlayOn = autoPlayOn;

            EventHolder.OnShowHelp += ShowPanel;

            if (ShowHelpAtFirstTime)
            {
                if (!PlayerPrefs.HasKey("HELP")) EventHolder.OnHelpFirstTime += ShowHelpPanel;
            }

            fitters = pagesParent.GetComponentsInChildren<BestFitter>().ToList();
        }

        void Start()
        {
            CreatePointerDownTrigger();//touch inputs
            btnClose.onClick.AddListener(HideHelp);
            UpdatePagesCounter();

            fitters.ForEach(b => b.Init());
        }

        void ShowHelpPanel()
        {
            if (PlayerPrefs.HasKey("HELP")) return;
            ShowPanel();
            PlayerPrefs.SetInt("HELP", 1);
        }

        [ContextMenu("Show Panel")]
        public void ShowPanel()
        {
            CheckAutoPlayMode();

            canvasGroup.gameObject.SetActive(true);
            fadeOut = false;
            fadeIn = false;

            fitters.ForEach(b => b.Init());

            if (!showWithFade)
            {
                canvasGroup.alpha = 1f;
                if (autoPlayOn)  PlayHelp();
            }
            else
            {
                canvasGroup.alpha = 0f;
                timeElapsed = 0f;
                startValue = 0f;
                endValue = 1f;
                lerpDuration = fadeTime;
                fadeIn = true;
                return;
            }
           
        }
        
        [ContextMenu("Hide Panel")]
        public void HideHelp()
        {
            if (!hideWithFade)
            {
                fadeOut = false;
                canvasGroup.gameObject.SetActive(false);
            }
            else 
            {
                timeElapsed = 0f;
                startValue = 1f;
                endValue = 0f;
                lerpDuration = fadeTime;
                fadeOut = true; 
            }
            HideHelpEvent?.Invoke();  // invoke this to connect with Chapter Selection manager
        }

        void CreatePointerDownTrigger()
        {
            EventTrigger trigger = scrollSnap.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Clear();
            // PointerDownEvent
            EventTrigger.Entry pointerDownEntry = new() { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
            trigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new() { eventID = EventTriggerType.PointerUp };
            pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
            trigger.triggers.Add(pointerUpEntry);
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            isTouchingScreen = true;
            StopPlay();            
            if (disableAutoPlayAfterTouch) { autoPlayOn = false; }// Stop Autoplay if disableAutoPlayAfterTouch is true
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            isTouchingScreen = false;            
        }

        void StopPlay()
        {
            draftAutoPlayAfterIdleTime = autoPlayAfterIdleTime;

            draftViewTime = pageViewTime;
            //enable buttons
            scrollSnap.buttonsNotVisible = false;
            isAutoPlay = false;
            scrollSnap.NextButton.SetActive(true);
            scrollSnap.PrevButton.SetActive(true);
        }

        private void Update()
        {
            if (!canvasGroup.gameObject.activeSelf || !canvasGroup.gameObject.activeInHierarchy) return;

            UpdatePagesCounter();  // Need to be placed within the segment which changes the pages
            if (fadeIn)
            {
                Fade(ref fadeIn);
                return;
            }
            if(fadeOut)
            {
                Fade(ref fadeOut);
                return;
            }

            if (!autoPlayOn) return;

            if (isAutoPlay)
            {
                draftViewTime -= Time.deltaTime;

                if(draftViewTime <= 0)
                {
                    _pages--;
                    TurnPage();
                    draftViewTime = pageViewTime;
                }

                if (_pages == 0 && !loopPlayPages) StopPlay();
            }
            else
            {
                if (!isTouchingScreen)
                {
                    draftAutoPlayAfterIdleTime -= Time.deltaTime;
                    if (draftAutoPlayAfterIdleTime <= 0)
                    {
                        PlayHelp();
                        if (!jumpToFirstPageOnAutoPlayRefocused && isEnabledOnStart == 0)
                        {
                            TurnPage();//change current page as we already looking at it
                        }
                    }
                }
            }
            
        }

        void Fade(ref bool val)
        {
            if (timeElapsed < lerpDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
            }
            else
            {
                canvasGroup.alpha = endValue;
                val = false;
                if(endValue == 0)
                {
                    if (disableCanvasOnHide)
                    {
                        scrollSnap.Init();
                        canvasGroup.gameObject.SetActive(false);
                        autoPlayOn = wasAutoPlayOn;
                        return;
                    }
                }
                if (autoPlayOn)
                {
                    PlayHelp();
                }
            }
        }

        [ContextMenu("Play Help")]
        void PlayHelp()
        {
            draftAutoPlayAfterIdleTime = autoPlayAfterIdleTime;
            draftViewTime = pageViewTime;
            scrollSnap.ButtonsHide();
            scrollSnap.transitionSpeed = pageTransitionSpeed;

            if(jumpToFirstPageOnAutoPlayRefocused) scrollSnap.Init();

            //hide buttons
            scrollSnap.buttonsNotVisible = true;
            _pages = helpDatabase.pages_textures.Count;
            isAutoPlay = true;

            //draftViewTime = pageViewTime;
            //for (int i=0; i<helpDatabase.pages_textures.Count; i++)
            //{
            //    Invoke(nameof(TurnPage), draftViewTime);
            //    draftViewTime += pageViewTime;
            //}
        }

        void TurnPage()
        {
            int currentPage = scrollSnap.CurrentPage + 1;
            _pages = scrollSnap.ChildObjects.Length - currentPage;
            if (loopPlayPages)
            {
                if (currentPage > scrollSnap.ChildObjects.Length - 1)//helpDatabase.pages_textures.Count - 1
                    currentPage = 0;
            }
            else
            {
                if (closeHelpOnAutoScrollComplete)
                {
                    if (currentPage > scrollSnap.ChildObjects.Length - 1)
                        btnClose.onClick.Invoke();
                }
            }            
            scrollSnap.ChangePage(currentPage);
        }

        void UpdatePagesCounter() 
        {
            // The current and total pages
            int totalPages = scrollSnap.ChildObjects.Length;// helpDatabase.pages_textures.Count; 
            int currentPage = scrollSnap.CurrentPage + 1;
            // Update the TextMeshProUGUI text with the pages information
            if(pageCounterText) pageCounterText.text = currentPage + " / " + totalPages;
        }

        void CheckAutoPlayMode()
        {
            if (!autoPlayOn)
            {
                //enable buttons
                scrollSnap.buttonsNotVisible = false;
                scrollSnap.NextButton.SetActive(true);
            }
            else
            {
                scrollSnap.buttonsNotVisible = true;
                scrollSnap.NextButton.SetActive(false);
                scrollSnap.PrevButton.SetActive(false);
            }
        }

        [ContextMenu("SETUP")]
        public void SetupHelp()
        {
            if (helpDatabase.pages_textures.Count <= 0 || helpDatabase.prefabPage == null)
            {
                Debug.LogWarning("ERROR database missing elements!");
                return;
            }

            for (int i = 0; i < helpDatabase.pages_textures.Count; i++)
            {
                Transform page = Instantiate(helpDatabase.prefabPage, pagesParent);
                int v = i + 1;
                page.name = "Page_" + v.ToString();
                if (!page.TryGetComponent<PageSetup>(out var pageSetup))
                {
                    Debug.LogError("Script [PageSetup] is missing from prefab page!");
                    #if UNITY_EDITOR
                        DestroyImmediate(page.gameObject);
                    #else
                        Destroy(page.gameObject); 
                    #endif
                    return;
                }
                pageSetup.Setup(helpDatabase.pages_textures[i]);

                // Set the "Non-Power of 2" property to "none" for the texture
                //if (setNonPowerOf2ToNone) SetNonPowerOf2Property(helpDatabase.pages_textures[i]);
            }
        }
        // Set the "Non-Power of 2" property to "none" for the texture
        //private void SetNonPowerOf2Property(Texture2D texture)
        //{
        //    string assetPath = UnityEditor.AssetDatabase.GetAssetPath(texture);
        //    UnityEditor.TextureImporter importer = (UnityEditor.TextureImporter)UnityEditor.TextureImporter.GetAtPath(assetPath);

        //    if (importer != null)
        //    {
        //        importer.npotScale = UnityEditor.TextureImporterNPOTScale.None;
        //        UnityEditor.AssetDatabase.ImportAsset(assetPath);
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Texture importer not found for: " + assetPath);
        //    }
        //}


        [ContextMenu("DELETE PAGES")]
        public void DeletePages()
        {
            PageSetup[] pages = pagesParent.GetComponentsInChildren<PageSetup>();
#if UNITY_EDITOR
            pages.ToList().ForEach(b => DestroyImmediate(b.gameObject));
#else
            pages.ToList().ForEach(b => Destroy(b.gameObject));
#endif
        }
    }

}
