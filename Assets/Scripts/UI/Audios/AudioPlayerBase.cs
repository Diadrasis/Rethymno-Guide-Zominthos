//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioPlayerBase : MonoBehaviour
	{
		public Button btnPlay, btnPause;
		[Space]
		public SoundBarEffect soundBar;
		[Space]
		public Slider slider;


		AudioClip clip;
		AudioSource source;
		bool isPlaying;

		public bool IsPlaying { get { return isPlaying; } }

		public bool HasAudioClip() { return gameObject.activeSelf && source.clip != null; }

		public List<AudioPlayerBase> allPlayers = new List<AudioPlayerBase>();

		bool isInitialized;

		private string clipName;

		//test
		[ContextMenu("TestSetClip")]
		void TestSetClip()
        {
			SetClip("clip_1");
        }

		void Start()
		{
			if (!isInitialized) Init();
		}

		void Init()
        {
			source = GetComponent<AudioSource>();

			btnPlay.gameObject.SetActive(true);
			btnPause.gameObject.SetActive(false);

			btnPause.onClick.AddListener(PauseAudio);
			btnPlay.onClick.AddListener(PlayAudio);

			if (slider) slider.onValueChanged.AddListener((b) => OnSliderValueChanged());

			//if(Application.isEditor) TestSetClip();

			isInitialized = true;
		}

        private void OnEnable()
        {
			EventHolder.OnAudioStop += StopAudio;
			EventHolder.OnAudioPause += PauseAudio;

        }

        private void OnDisable()
        {
			EventHolder.OnAudioStop -= StopAudio;
            EventHolder.OnAudioPause -= PauseAudio;
        }

        private void Update()
        {
            if (isPlaying && source.isPlaying)
            {
				float len = source.time / source.clip.length;
				soundBar.SetBar(len);
				if (slider) slider.value = source.time;
            }
        }

        public void SetClip(string filename)
        {

			clipName = filename;
			if (!isInitialized) Init();
			
			SaveLoadManager.GetAudioClip(this, filename, OnAudioClipLoaded);
		}

		void OnAudioClipLoaded(AudioClip clip)
        {
			//Debug.LogWarning(clipName);
			source.clip = clip;

			if (clip == null)
			{
				gameObject.SetActive(false);
				return;
			}
			if (slider)
			{
				slider.minValue = 0f;
				slider.maxValue = clip.length;
			}
			//SaveLoadManager.SaveAudio(clipName, clip);
		}

		void OnSliderValueChanged()
		{
			if (!slider) return;
			CancelInvoke();
			source.time = slider.value > source.clip.length ? source.clip.length : slider.value;
			float len = slider.value / source.clip.length;
			soundBar.SetBar(len);
            if (isPlaying)
            {
				float remainingTime = source.clip.length - source.time;
				Invoke(nameof(EventOnEnd), remainingTime);
			}
		}

		void PlayAudio()
		{
			StopOtherSources();
			CancelInvoke();
			btnPlay.gameObject.SetActive(false);
			btnPause.gameObject.SetActive(true);
			source.Play();
			isPlaying = true;
			float remainingTime = source.clip.length - source.time;
			Invoke(nameof(EventOnEnd), remainingTime);
		}

		void EventOnEnd()
		{
			StopAudio();
		}

		public void StopAudio()
        {
			CancelInvoke();
			isPlaying = false;
			source.Stop();
			source.time = 0f;
			btnPlay.gameObject.SetActive(true);
			btnPause.gameObject.SetActive(false);
			soundBar.SetBar(0f);
			//if (Application.isEditor) Debug.LogWarningFormat("audio {0} finished!", gameObject.name);
		}

		void PauseAudio()
        {
			CancelInvoke();
			btnPlay.gameObject.SetActive(true);
			btnPause.gameObject.SetActive(false);
			source.Pause();
			isPlaying = false;
        }

		void StopOtherSources()
        {
			GetOtherPlayers();
			allPlayers.ForEach(b => b.StopAudio());
        }

		void GetOtherPlayers()
        {
			allPlayers.Clear();
			allPlayers = FindObjectsOfType<AudioPlayerBase>().ToList();
			allPlayers.Remove(this);
		}
	}

}
