//Diadrasis ©2023 - Stathis Georgiou
using StaGeGames.BestFit;
using StaGeGames.BestFit.Extens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Diadrasis.Rethymno 
{

	public class VideoItemsController : MonoBehaviour
	{
        public BestFitter bestFitter;

        public GameObject videoPanel;
        public RectTransform videoContainer;

        public RawImage rawImage;
        public VideoPlayer videoPlayer;
        public Button btnPlay, btnPause, btnReplay;
        public GameObject pauseIcon, videoSpaceUI, videoSpaceUIUp, blackPanel;

        string _copyright = "©";

        void InitButtons()
        {
            blackPanel.SetActive(true);
            btnPlay.gameObject.SetActive(true);
            btnPause.gameObject.SetActive(false);
            pauseIcon.SetActive(true);
            btnReplay.gameObject.SetActive(false);
        }

        [Space]
        public bool showTitle;
        public TMPro.TextMeshProUGUI txtTitleOut;
        public TMPro.TextMeshProUGUI txtLabelOut, txtSourceOut;
        [SerializeField]
        private RectTransform rectTitleOut, rectLabelOut, rectSourceOut;

        private void Awake()
        {
            rectTitleOut = txtTitleOut.transform.parent as RectTransform;
            rectLabelOut = txtLabelOut.transform.parent as RectTransform;
            rectSourceOut = txtSourceOut.transform.parent as RectTransform;

            EventHolder.OnGuiMarkerClick += OnMarkerClicked;
            EventHolder.OnARInfoTriggered += OnARInfoTriggered;

            EventHolder.OnNativePoiMarkerClick += OnNativePoiMarkerClick;

            EventHolder.OnRouteInfoShow += SetFromRoute;
            EventHolder.OnPeriodInfoShow += SetFromPeriod;
            EventHolder.OnAreaInfoShow += SetFromArea;

            btnPlay.onClick.AddListener(PlayVideo);
            btnPause.onClick.AddListener(PauseVideo);
            btnReplay.onClick.AddListener(ReplayVideo);

            videoPlayer.renderMode = VideoRenderMode.APIOnly;
            videoPlayer.prepareCompleted += PrepareVideo;
            videoPlayer.loopPointReached += LoopPoint;

            InitButtons();
        }

        void PlayVideo() 
        {
            blackPanel.SetActive(false);
            videoPlayer.Play();
            btnPlay.gameObject.SetActive(false);
            btnPause.gameObject.SetActive(true);
            pauseIcon.SetActive(true);
            btnReplay.gameObject.SetActive(false);
            CancelInvoke();
            Invoke(nameof(HidePauseButton), 1.1f);
        }

        void HidePauseButton()
        {
            pauseIcon.SetActive(false);
        }

        void PauseVideo() 
        {
            blackPanel.SetActive(true);
            videoPlayer.Pause();
            btnPlay.gameObject.SetActive(true);
            btnPause.gameObject.SetActive(false);
            btnReplay.gameObject.SetActive(true);
        }
        void ReplayVideo()
        {
            //StopVideo();
            videoPlayer.time = 1;
            PlayVideo();
        }

        void StopVideo()
        {
            videoPlayer.Stop();
        }

        void SetFromArea(AreaEntity areaEntity) { HidePanel(); }
        void SetFromRoute(RouteEntity routeEntity) { HidePanel(); }
        void SetFromPeriod(PeriodEntity periodEntity) { HidePanel(); }

        void HidePanel() { videoPanel.SetActive(false); videoSpaceUI.SetActive(false); videoSpaceUIUp.SetActive(false); HideExternalTexts(); }

        void OnNativePoiMarkerClick(PoiEntity poiEntity)
        {
            if (poiEntity == null) return;
            SetFromPoi(poiEntity);
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
                SetFromArea(marker.data.areaEntity);
            }
            else
            {
                if (marker.data.poiEntity == null) return;
                SetFromPoi(marker.data.poiEntity);
            }
        }

        void SetFromPoi(PoiEntity poiEntity)
        {
            if (poiEntity == null) return;
            if (poiEntity.videos.Count <= 0)
            {
                HidePanel();
                return;
            }
            string _videoFile = poiEntity.videos[0].video_file;

            if (_videoFile.IsNull())
            {
                HidePanel();
                return;
            }

            videoSpaceUIUp.SetActive(true);
            videoSpaceUI.SetActive(true);
            videoPanel.SetActive(true);
            InitButtons();

            SaveLoadManager.GetVideoClip(this, _videoFile, OnVideoClipLoaded);

            if (showTitle)
                SetText(txtTitleOut, DataManager.Instance.GetTraslatedText(poiEntity.videos[0].video_title));
            SetText(txtLabelOut, DataManager.Instance.GetTraslatedText(poiEntity.videos[0].video_label));
            SetText(txtSourceOut, _copyright + " " + DataManager.Instance.GetTraslatedText(poiEntity.videos[0].video_source));
        }

        void OnVideoClipLoaded(VideoClip clip, string _url)
        {
            if(clip != null)
            {
                videoPlayer.source = VideoSource.VideoClip;
                videoPlayer.clip = clip;
            }
            else
            {
                if (_url.IsNull())
                {
                    HidePanel();
                    return;
                }

                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = _url;
            }

            videoPlayer.Prepare();

        }

        void PrepareVideo(VideoPlayer vp)
        {
            float aspectLog = vp.width / (float)vp.height;
            if (DataManager.Instance.IsMobile())
            {
                bestFitter.mockRatioMode = StaGeGames.BestFit.Utils.EnumUtils.MockRatioMode.CustomRatio;
                bestFitter.fMockAspectRatio = aspectLog;
            }

            vp.time = 1;
            rawImage.texture = vp.texture;
            vp.Play();
            vp.Pause();
        }

        void LoopPoint(VideoPlayer vp)
        {
            //vp.Stop();
            InitButtons();
            vp.time = 1;
            vp.Pause();
        }

        void SetText(TMPro.TextMeshProUGUI txt, string val)
        {
            if (!val.IsNull() && val.Length > 3)
            {
                txt.transform.parent.gameObject.SetActive(videoPanel.activeSelf);
                txt.text = val;
                txt.rectTransform.ForceRebuildLayout();
            }
            else
            {
                txt.transform.parent.gameObject.SetActive(false);
            }
        }

        void HideExternalTexts()
        {
            rectTitleOut.gameObject.SetActive(false);
            rectLabelOut.gameObject.SetActive(false);
            rectSourceOut.gameObject.SetActive(false);
        }
    }

}
