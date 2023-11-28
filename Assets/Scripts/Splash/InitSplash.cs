using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Diadrasis.Rethymno
{
    public class InitSplash : Singleton<InitSplash>
    {
        protected InitSplash(){}

        public Animator animPanel, animAppLogo, animInfoPanel;
        public List<Animator> infoAnimators = new List<Animator>();
        public GameObject canvasSpalsh, panelSplash, panelInfo;

        public bool lerpMainPanel;
        public float waitFirstTime = 0.7f;

        public GameObject[] pages;
        public float pageViewTime = 3f;
        public bool showLogosConsecutively;

        private int isStarted;

        [Space]
        public Button btnHideSplash;

        WaitForSeconds waitForAnimation = new WaitForSeconds(0.7f);

        private void Awake()
        {
            if (canvasSpalsh == null) return;

            DontDestroyOnLoad(this);
            //Screen.orientation = ScreenOrientation.LandscapeLeft;
            if (isStarted == 0) Init();

            if (btnHideSplash) btnHideSplash.onClick.AddListener(HideAsInfo);

            panelInfo.SetActive(false);
        }

        private void Start()
        {
            
        }

        public bool TryInit(bool isQuitting)
        {
            if (canvasSpalsh == null) return false;
            Init(isQuitting);
            return true;
        }

        public void ShowAsInfo()
        {
            if (canvasSpalsh == null) return;
            canvasSpalsh.gameObject.SetActive(true);
            panelSplash.SetActive(false);
            panelInfo.SetActive(true);
            animInfoPanel.SetBool("show", true);
            if (infoAnimators.Count > 0)
                StartCoroutine(DelayShowInfoLogos());
        }

        IEnumerator DelayShowInfoLogos()
        {
            yield return new WaitForSeconds(0.7f);
            infoAnimators.ForEach(b => b.SetBool("show", true));
            yield return new WaitForSeconds(0.7f);
            btnHideSplash.interactable = true;
            yield break;
        }

        public void HideAsInfo()
        {
            btnHideSplash.interactable = false;
            infoAnimators.ForEach(b => b.SetBool("show", false));
            StartCoroutine(CloseCanvas());
        }

        IEnumerator CloseCanvas() 
        {
            yield return new WaitForSeconds(0.7f);
            animInfoPanel.SetBool("show", false);
            yield return new WaitForSeconds(0.7f);
            canvasSpalsh.gameObject.SetActive(false);
            yield break;
        }

        public void Init(bool isQuitting = false)
        {
            if (canvasSpalsh == null) return;
            isStarted++;
            panelSplash.SetActive(true);
            panelInfo.SetActive(false);
            canvasSpalsh.gameObject.SetActive(true);
            animPanel.gameObject.SetActive(true);
            btnHideSplash.interactable = false;
            StartCoroutine(StartSplash(isQuitting));
        }

        IEnumerator StartSplash(bool isQuitting)
        {
            if (animPanel && lerpMainPanel)
            {
                animPanel.SetBool("show", true);
                yield return waitForAnimation;
            }
           // yield return new WaitForSeconds(waitFirstTime);
            animAppLogo.SetBool("show", true);

            yield return new WaitForSeconds(waitFirstTime);
            //yield return new WaitForSeconds(1.3f);

            foreach(GameObject page in pages)
            {
                yield return waitForAnimation;
                List<Animator> anims = page.GetComponentsInChildren<Animator>().ToList();
                if (showLogosConsecutively)
                {
                    int lastone = anims.Count - 1;
                    for(int i=0; i<anims.Count; i++)
                    {
                        anims[i].SetBool("show", true);
                       // if(i<lastone)
                            yield return waitForAnimation;
                    }
                }
                else
                {
                    anims.ForEach(b => b.SetBool("show", true));
                }
                yield return new WaitForSeconds(pageViewTime);
                anims.ForEach(b => b.SetBool("show", false));
            }

            yield return waitForAnimation;
            if (isQuitting)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                yield break;
            }
            yield return SceneManager.LoadSceneAsync(1);// new WaitForSeconds(1f);
            //animAppLogo.SetBool("show", false);
            yield return waitForAnimation;
            animPanel.SetBool("show", false);
            yield return waitForAnimation;
            yield return waitForAnimation;
            yield return waitForAnimation;
            yield return waitForAnimation;
            canvasSpalsh.gameObject.SetActive(false);
            yield return new WaitForSeconds(waitFirstTime);
            EventHolder.OnSplashFinished?.Invoke();
            yield break;
        }

    }

}