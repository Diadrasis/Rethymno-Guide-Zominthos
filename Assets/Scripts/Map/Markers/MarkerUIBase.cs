//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{

	public abstract class MarkerUIBase : MonoBehaviour
	{

		public Button btn;
		public RawImage img;
		public TMPro.TextMeshProUGUI txt, txtOrder;

        public void SetOrderText(int val) 
        {
            if (txtOrder)
            {
                txtOrder.text = val.ToString();
                txtOrder.gameObject.SetActive(true);
            }
        }

        [HideInInspector]
		public AreaEntity areaEntity;
		//[HideInInspector]
		public PoiEntity poiEntity;

		public string iconName;

		//indicates if marker can be visible
		//online maps declare visibility if marker is on screen
		//but we want to not be visible if zoom level is < 12
		private bool zoomHide;

        public void ManualHide(bool val) { zoomHide = !val; }

        public bool IsArea, IsGuiMarker, isAutoZoom;

		public bool ShouldZoomVisible() { return zoomHide; }

        EventTrigger trigger;

        /// <summary>
        /// longitude = x
        /// latitude = y
        /// </summary>
        public Vector2 pos;

		public virtual void Init() 
		{

            CreateTrigger();


            if (isAutoZoom)
            {
                OnlineMaps.instance.OnChangeZoom += OnChangeZoom;
                CheckScale(OnlineMaps.instance.floatZoom);
            }
            else { zoomHide = true; }


		}

		/// <summary>
		/// This method is called when the zoom changes
		/// </summary>
		private void OnChangeZoom()
		{
			//Debug.Log(OnlineMaps.instance.zoom);
			CheckScale(OnlineMaps.instance.floatZoom);
		}

		void CheckScale(float val)
        {
			zoomHide = true;
			float scal = 1f;
			if(val >= 18) { scal = 1f; }
			else if (val < 18 && val >= 16.8) { scal = 0.9f; }
			else if (val < 16.8 && val >= 16) { scal = 0.8f; }
			else if(val < 16 && val > 14) { scal = 0.5f; }
			else if(val <=14 && val > 13) { scal = 0.25f; }
			else if (val <= 13 && val >= 12) { scal = 0.15f; }
			else { /* hide marker*/ gameObject.SetActive(false); zoomHide = false; }

			transform.localScale = Vector3.one * scal;
		}

        public void SetScale(Vector3 scal) { transform.localScale = scal; }
        public Vector3 GetScale() { return transform.localScale; }

		public void SetPosition(Vector2 p) { pos = p; }

		public void SetText(string val) { if(txt) txt.text = val; }


		public void SetTexture(string _texName)
        {
            if (img) img.texture = SaveLoadManager.LoadTexture(_texName, GlobalUtils.iconMarkerEmpty);// IsArea ? GlobalUtils.iconMarkerButtonEmpty : GlobalUtils.iconMarkerEmpty);
		}

        public void SetTexture(Texture2D tex)
        {
            if (img) img.texture = tex;
        }

        public void SetIcon(Texture2D icon)
        {
			//Debug.Log("<color=yellow>Set Icon</color>");
			if (img) img.texture = icon;
		}

        void CreateTrigger()
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }

        UnityAction<BaseEventData> callBack;

        public void RemoveAllListeners()
        {
            triggerEntry.callback.RemoveAllListeners();
            triggerEntry2.callback.RemoveAllListeners();
        }

        EventTrigger.Entry triggerEntry, triggerEntry2;

        public void AddListener(UnityAction<BaseEventData> call)
        {
            //btn.onClick.AddListener(call);

            callBack = call;

            triggerEntry = new EventTrigger.Entry();
            triggerEntry.eventID = EventTriggerType.PointerDown;
            triggerEntry.callback.AddListener(OnClickDown);
            trigger.triggers.Add(triggerEntry);

            triggerEntry2 = new EventTrigger.Entry();
            triggerEntry2.eventID = EventTriggerType.PointerUp;
            triggerEntry2.callback.AddListener(OnClickUp);
            trigger.triggers.Add(triggerEntry2);
        }

        float clickHoldTime = 0f;
        void OnClickDown(BaseEventData eventData)
        {
            clickHoldTime = Time.realtimeSinceStartup;

        }
        void OnClickUp(BaseEventData eventData)
        {
            float releaseTime = Time.realtimeSinceStartup - clickHoldTime;

            //Debug.Log(releaseTime);

            if (releaseTime < 0.25f)
            {
                callBack(eventData);
            }
        }

        private void OnDestroy()
        {
			if(OnlineMaps.instance) OnlineMaps.instance.OnChangeZoom -= OnChangeZoom;
			if(btn) btn.onClick.RemoveAllListeners();
        }
    }

}
