//Diadrasis ©2023
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno
{

    public class MarkerEngineManager : MonoBehaviour
	{
        public EnumsHolder.AppState appState;
        public EnumsHolder.TourMode tourMode;
        public EnumsHolder.TriggerMode triggerMode;
        [Space]
        public EnumsHolder.InfoPanelState infoPanelState = EnumsHolder.InfoPanelState.Closed;

        [Space]
        public MarkerEngineNative nativeMarkerEngine;
        [Space]
        public MarkerEngineGUI uGUIMarkerEngine;

        [Space]
        public bool IsRoute_or_Period_InfoOnScreen;

        public Transform canvasTransform;
		public Transform prefabMarkersContainer;
       
        private RectTransform poiMarkersContainer, areaMarkersContainer;
        public RectTransform mapUserMarkerContainer;

        public RectTransform ContainerAreaMarkers() { return areaMarkersContainer; }
        public RectTransform ContainerPoiMarkers() { return poiMarkersContainer; }

        public GameObject GetAreaMarkersPanel() { return areaMarkersContainer.gameObject; }
        public GameObject GetPOIMarkersPanel() { return poiMarkersContainer.gameObject; }

        public Transform canvasMap;

        [Space]
        public float BEST_ZOOM_POIS;
        public Vector2 BEST_CENTER_POIS;

        MapController mapController;

        MapPoiArgs settingsPOI;
        [HideInInspector]
        public AudioItemsController audioController;

        public bool NarrationIsPlaying()
        {
            if(Application.isEditor) Debug.Log("IsNarrationPlaying = " + audioController.IsNarrationPlaying());
            return PoiSettings().checkPoiInfoIfNarrationPlaying && audioController.IsNarrationPlaying();
        }

        public MapPoiArgs PoiSettings() { return settingsPOI; }

        void OnInfoActive(EnumsHolder.InfoPanelState val)
        {
            infoPanelState = val;
        }

        private void Awake()
        {
            mapController = FindObjectOfType<MapController>();
            audioController = FindObjectOfType<AudioItemsController>();

            settingsPOI = mapController.settings.poisOptions;

            //create ui container (panel) for pois
            poiMarkersContainer = Instantiate(prefabMarkersContainer, canvasMap) as RectTransform;
            poiMarkersContainer.name = "POIS";
            poiMarkersContainer.SetAsFirstSibling();
            //create ui container (panel) for areas
            areaMarkersContainer = Instantiate(prefabMarkersContainer, canvasMap) as RectTransform;
            areaMarkersContainer.name = "AREAS";
            areaMarkersContainer.SetAsFirstSibling();

            EventHolder.OnTourChanged += OnTourChanged;
            EventHolder.OnGpsLocationChanged += OnGpsLocationChanged;
            EventHolder.OnStateChanged += OnStateChanged;

            EventHolder.OnInfoState += OnInfoActive;

            EventHolder.OnRouteInfoShow += OnRouteInfoShow;
            EventHolder.OnPeriodInfoShow += OnPeriodInfoShow;
            EventHolder.OnNativePoiMarkerClick += OnNativePoiMarkerClick;
            EventHolder.OnGuiMarkerClick += OnGuiMarkerClick;
        }

        private void Start()
        {
            EventHolder.OnPoiInfoClosed += SetPoiIconsToDefault;
            EventHolder.OnSetPoiIconsToDefault += SetPoiIconsToDefault;

            GameObject _native = nativeMarkerEngine.transform.parent.gameObject;
            if(_native) _native.SetActive(settingsPOI.createNativePoiMarkers);
            nativeMarkerEngine.IsInUse = settingsPOI.createNativePoiMarkers;

            GameObject _ugui = uGUIMarkerEngine.transform.parent.gameObject;
            if(_ugui) _ugui.SetActive(!settingsPOI.createNativePoiMarkers);
            uGUIMarkerEngine.IsInUse = !settingsPOI.createNativePoiMarkers;
        }

        void OnRouteInfoShow(RouteEntity route) { IsRoute_or_Period_InfoOnScreen = true; }
        void OnPeriodInfoShow(PeriodEntity period) { IsRoute_or_Period_InfoOnScreen = true; }
        void OnNativePoiMarkerClick(PoiEntity poi) { IsRoute_or_Period_InfoOnScreen = false; }
        void OnGuiMarkerClick(MarkerInstance marker) { IsRoute_or_Period_InfoOnScreen = false; }    

        public bool InfoIsNotClosed() { return infoPanelState != EnumsHolder.InfoPanelState.Closed; }

        void OnGpsLocationChanged(Vector2 pos)
        {
            if (tourMode != EnumsHolder.TourMode.OnSite) return;
            if(triggerMode != EnumsHolder.TriggerMode.GPS) return;
            bool isPoiMode = appState == EnumsHolder.AppState.PoisView || appState == EnumsHolder.AppState.PoiSelected;
            if (!isPoiMode) return;

            if (settingsPOI.createNativePoiMarkers)
            {
                nativeMarkerEngine.OnLocationChanged(pos);
            }
            else
            {
                uGUIMarkerEngine.OnLocationChanged(pos);
            }
        }

        void OnStateChanged(EnumsHolder.AppState state)
        {
            appState = state;

            bool isPoiMode = appState == EnumsHolder.AppState.PoisView || appState == EnumsHolder.AppState.PoiSelected;
            if (settingsPOI.createNativePoiMarkers)
            {
                if (!isPoiMode) nativeMarkerEngine.nativePoiMarkersOnsiteVisited.Clear();
            }
            else
            {
                if (!isPoiMode) uGUIMarkerEngine.guiPoiMarkersOnsiteVisited.Clear();
            }

        }

        void OnTourChanged(EnumsHolder.TourMode mode)
        {
            tourMode = mode;
        }

        void OnTriggerModeChanged(EnumsHolder.TriggerMode mode)
        {
            triggerMode = mode;
        }

        public void CreateAreaMarkers(List<AreaEntity> areaEntities, bool IsNative)
        {
            if (IsNative) { nativeMarkerEngine.DestroyAreaNativeMarkers(); }
            else { uGUIMarkerEngine.DestroyAreaGuiMarkers(); }

            if (areaEntities.Count <= 0) return;

            foreach (AreaEntity area in areaEntities)
            {
                if (IsNative) { nativeMarkerEngine.CreateNativeAreaMarker(area); }
                else { uGUIMarkerEngine.CreateAreaGUIMarker(area); }
            }
            EventHolder.OnAreasView?.Invoke();

            GetAreasMinDistance(areaEntities);

            if (IsNative) OnlineMaps.instance.Redraw();
        }

        public void CreatePoiMarkers(List<PoiEntity> pois, bool IsNative, Vector2 areaCenter)
        {
            if (pois.Count <= 0)//get area selected pos, zoom
            {
                List<Vector2> poisPositions = new List<Vector2>();
                poisPositions.Add(mapController.AreaSelected.area.geoPosition);
                GetBestZoomForPositions(poisPositions, out Vector2 pos, out int zoom);
                BEST_ZOOM_POIS = zoom - 1f;
                BEST_CENTER_POIS = pos;
                
                return;
            }

            if (IsNative) { nativeMarkerEngine.CreatePoisNativeMarkers(pois, areaCenter); }
            else { uGUIMarkerEngine.CreateGuiPoiMarkers(pois, areaCenter); }

        }
      
        #region Destroy Update Actions

        public void DestroyPoiMarkers()
        {
            if (settingsPOI.createNativePoiMarkers) { nativeMarkerEngine.DestroyPoiMarkers(); OnlineMaps.instance.Redraw(); }
            else { uGUIMarkerEngine.DestroyGuiPoiMarkers(); }
        }


        #endregion

        #region Center Map

        public void ApplyMapPoisBestZoomPosition()
        {
            //CenterMapOnNativeMarkers(poiNativeMarkers);
            OnlineMaps.instance.position = BEST_CENTER_POIS;
            OnlineMaps.instance.floatZoom = BEST_ZOOM_POIS;
        }

        public void CenterMapOnGUIMarkers(List<MarkerInstance> markers, int zoomOutValue = 0)
        {
            if (markers == null || markers.Count <= 0) return;
            List<Vector2> positions = new List<Vector2>();
            foreach (MarkerInstance marker in markers) positions.Add(marker.data.pos);

            Vector2 center;
            int zoom;

            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(positions.ToArray(), out center, out zoom);

            // Change the position and zoom of the map.
            OnlineMaps.instance.position = center;
            OnlineMaps.instance.zoom = zoom + zoomOutValue;
        }

        public void GetBestPosAndZoomForGUIMarkers(List<MarkerInstance> markers, out Vector2 center, out int zoom)
        {
            if (markers == null || markers.Count <= 0)
            {
                center = OnlineMaps.instance.position;
                zoom = OnlineMaps.instance.zoom;
                return;
            }
            List<Vector2> positions = new List<Vector2>();
            foreach (MarkerInstance marker in markers) positions.Add(marker.data.pos);
            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(positions.ToArray(), out center, out zoom);
        }

        public void CenterOnMarker(MarkerInstance marker)
        {
            List<Vector2> positions = new List<Vector2>();
            positions.Add(marker.data.pos);

            Vector2 center;
            int zoom;

            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(positions.ToArray(), out center, out zoom);

            // Change the position and zoom of the map.
            OnlineMaps.instance.position = center;
            OnlineMaps.instance.zoom = zoom;
        }

        public void CenterMapOnNativeMarkers(List<OnlineMapsMarker> markers)
        {
            Vector2 center;
            int zoom;

            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(markers.ToArray(), out center, out zoom);

            // Change the position and zoom of the map.
            OnlineMaps.instance.position = center;
            OnlineMaps.instance.zoom = zoom;// + 1;
        }

        public void GetBestPosAndZoomForNativeMarkers(List<OnlineMapsMarker> markers, out Vector2 center, out int _zoom)
        {
            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(markers.ToArray(), out center, out _zoom);
        }

        public void CenterMapOnPosition(Vector2 pos)
        {
            List<Vector2> positions = new List<Vector2>();
            positions.Add(pos);

            Vector2 center;
            int zoom;

            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(positions.ToArray(), out center, out zoom);

            // Change the position and zoom of the map.
            OnlineMaps.instance.position = center;
            OnlineMaps.instance.zoom = zoom;
        }

        public void GetBestZoomAndPosition(List<Vector2> poisPositions)
        {
            GetBestZoomForPositions(poisPositions, out Vector2 pos, out int zoom);
            BEST_ZOOM_POIS = zoom - 1f;
            BEST_CENTER_POIS = pos;
        }

        public void GetBestZoomForPositions(List<Vector2> positions, out Vector2 center, out int zoom)
        {
            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(positions.ToArray(), out center, out zoom);
        }

        public float GetBestZoomForPos(Vector2 pos)
        {
            List<Vector2> positions = new List<Vector2>();
            positions.Add(pos);

            Vector2 center;
            int zoom;

            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(positions.ToArray(), out center, out zoom);

            // Change the position and zoom of the map.
            //OnlineMaps.instance.position = center;
            return zoom;
        }

        public void CenterOnMarker(Vector2 pos)
        {
            List<Vector2> positions = new List<Vector2>();
            positions.Add(pos);

            Vector2 center;
            int zoom;

            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(positions.ToArray(), out center, out zoom);

            // Change the position and zoom of the map.
            OnlineMaps.instance.position = center;
            OnlineMaps.instance.zoom = zoom;
        }

#endregion

        #region General functions for markers

        void GetAreasMinDistance(List<AreaEntity> areaEntities)
        {
            float minDistance = 100000f;
            string a = "", b = "";
            for (int i = 0; i < areaEntities.Count; i++)
            {
                foreach (AreaEntity m in areaEntities)
                {
                    if (m.ID == areaEntities[i].ID)
                        continue;
                    // Calculate the distance in km
                    float distance = OnlineMapsUtils.DistanceBetweenPoints(areaEntities[i].area.geoPosition, m.area.geoPosition).magnitude;
                    if (distance > 1 / 1000f)
                    {
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            string id = areaEntities[i].ID.ToString();
                            AreaEntity areaEntity = DataManager.Instance.GetAreaEntity(id);
                            if (areaEntity != null) a = DataManager.Instance.GetTraslatedText(areaEntity.area.area_title);

                            id = m.ID.ToString();
                            areaEntity = DataManager.Instance.GetAreaEntity(id);
                            if (areaEntity != null) b = DataManager.Instance.GetTraslatedText(areaEntity.area.area_title);
                        }
                    }
                }
            }
#if UNITY_EDITOR
            if (mapController.settings.EditorDebug)
            {
                Debug.LogWarning("<color=orange>[" + a + "] - [" + b + "] = " + minDistance * 1000f + " meters</color>");
                Debug.LogWarning("Min Areas Distance is " + minDistance * 1000f + " meters");
            }
#endif
        }

        void SetPoiIconsToDefault()
        {

            if (settingsPOI.createNativePoiMarkers)
            {
                if (OrderRouteControllerNative.Instance.IsOrderRouteActive()) return;
                nativeMarkerEngine.SetPoiIconsToDefault();
            }
            else
            {
                if (OrderRouteControllerGUI.Instance.IsOrderRouteActive()) return;
                uGUIMarkerEngine.SetPoiIconsToDefault();
            }
        }


#endregion


    }

    [Serializable]
    public class MarkerData
    {
        public MarkerUIBase markerUI;
        public double longitude;
        public double latitude;
        public Vector2 pos;
    }

    [Serializable]
    public class MarkerInstance
    {
        public MarkerUIBase data;
        public GameObject gameObject;
        public RectTransform transform;
        
    }

}
