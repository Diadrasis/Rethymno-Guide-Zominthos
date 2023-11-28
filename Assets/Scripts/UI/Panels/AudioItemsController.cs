//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class AudioItemsController : MonoBehaviour
	{

		public AudioPlayerBase narrationClass, testimonyClass;

        public GameObject SpaceNarration;

        public bool IsNarrationPlaying() { return narrationClass != null ? narrationClass.IsPlaying : false; }

        [Space]
        public bool useTestimonyAsAudioToo;

        private int currentPoiId = -100;
        public bool IsSamePoiSelected(int id)
        {
            if (currentPoiId == id) return true;
            currentPoiId = id;
            return false;
        }

        private void Awake()
		{
            EventHolder.OnNativePoiAreaClick += OnNativePoiAreaClick;
            EventHolder.OnGuiMarkerClick += OnMarkerClicked;

            EventHolder.OnARInfoTriggered += OnARInfoTriggered;

            EventHolder.OnNativePoiMarkerClick += OnNativePoiMarkerClick;

            EventHolder.OnRouteInfoShow += SetFromRoute;
			EventHolder.OnPeriodInfoShow += SetFromPeriod;
			EventHolder.OnAreaInfoShow += SetFromArea;

            EventHolder.OnPoiInfoClosed += SetPoiSelectedToNull;
        }

        void OnNativePoiMarkerClick(PoiEntity poiEntity)
        {
            if (poiEntity == null) return;
            //if (IsSamePoiSelected(poiEntity.poi.poi_id)) return;
            SetFromPoi(poiEntity);
        }

        void OnNativePoiAreaClick(AreaEntity areaEntity)
        {
            if (areaEntity == null) return;
            SetFromArea(areaEntity);
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
                //if (IsSamePoiSelected(marker.data.poiEntity.poi.poi_id)) return;
                SetFromPoi(marker.data.poiEntity);
            }
        }

        void SetFromArea(AreaEntity areaEntity)
        {
            SpaceNarration.SetActive(false);
            narrationClass.gameObject.SetActive(false);
            testimonyClass.gameObject.SetActive(false);
        }

        void SetFromRoute(RouteEntity routeEntity)
        {
            SpaceNarration.SetActive(false);
            narrationClass.gameObject.SetActive(false);
            testimonyClass.gameObject.SetActive(false);
        }

        void SetFromPeriod(PeriodEntity periodEntity)
        {
            SpaceNarration.SetActive(false);
            narrationClass.gameObject.SetActive(false);
            testimonyClass.gameObject.SetActive(false);
        }

        void SetFromPoi(PoiEntity poiEntity)
        {
            if(poiEntity == null) return;
            string _narrationFile = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_narration);

            if (_narrationFile.IsNull())
            {
                SpaceNarration.SetActive(false);
                narrationClass.gameObject.SetActive(false);
            }
            else
            {
                //SpaceNarration.SetActive(true);
                narrationClass.gameObject.SetActive(true);
                narrationClass.SetClip(_narrationFile);

                //SpaceNarration.SetActive(narrationClass.HasAudioClip());
                Invoke(nameof(CheckSpace), 0.7f);
            }

            string _testimonyFile = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_testimony);

            if (_testimonyFile.IsNull())
            {
                testimonyClass.gameObject.SetActive(false);
            }
            else
            {
                if (useTestimonyAsAudioToo)
                {
                    testimonyClass.gameObject.SetActive(true);
                    testimonyClass.SetClip(_testimonyFile);
                }
            }

        }

        void CheckSpace() 
        {
            CancelInvoke();
            SpaceNarration.SetActive(narrationClass.HasAudioClip()); 
        }

        void SetPoiSelectedToNull() { currentPoiId = -100; }

    }

}
