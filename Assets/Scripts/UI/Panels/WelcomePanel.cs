using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Diadrasis.Rethymno
{
    public class WelcomePanel : MonoBehaviour
    {
        public GameObject canvasPanel;
        public CanvasGroup cgroup;
        public Button btnGR, btnENG;
        [Space]
        public bool showSelectLanguagePanel = true;

        private void Awake()
        {
            if (!DataManager.Instance.HasSavedLanguage() && showSelectLanguagePanel)
            {
                canvasPanel.SetActive(true);
            }
            else
            {
                MessageManager.Instance.InitializeAtStart();
            }
        }

        void Start()
        {
            DOTween.Init();
            DOTween.SetTweensCapacity(500, 250);
            btnENG.onClick.AddListener(SetEnglish);
            btnGR.onClick.AddListener(SetGreek);
        }

        void SetGreek()
        {
            DataManager.Instance.SetManualLanguage(EnumsHolder.Language.GREEK);
            FadeOut();
        }

        void SetEnglish()
        {
            DataManager.Instance.SetManualLanguage(EnumsHolder.Language.ENGLISH);
            FadeOut();
        }

        void SetFrench()
        {
            DataManager.Instance.SetManualLanguage(EnumsHolder.Language.FRENCH);
            FadeOut();
        }

        void SetGerman()
        {
            DataManager.Instance.SetManualLanguage(EnumsHolder.Language.GERMAN);
            FadeOut();
        }

        void SetRussian()
        {
            DataManager.Instance.SetManualLanguage(EnumsHolder.Language.RUSSIAN);
            FadeOut();
        }

        void FadeOut()
        {
            MessageManager.Instance.InitializeAtStart();//.DelayCheckGPS();
                                                        // anim.SetBool("hide", true);
                                                        //Invoke(nameof(HidePanel), 0.75f);
            cgroup.DOFade(0f, 0.7f).OnComplete(HidePanel);
        }

        void HidePanel()
        {
            canvasPanel.SetActive(false);
        }
    }
}
