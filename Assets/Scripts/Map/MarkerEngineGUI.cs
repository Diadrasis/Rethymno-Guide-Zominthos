//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class MarkerEngineGUI : MarkerEngineBase
    {
        [Space]
		public List<MarkerInstance> guiPoiMarkersOnsiteVisited = new List<MarkerInstance>();

		[Space]
		public List<MarkerInstance> poiGUIMarkers = new List<MarkerInstance>();
		public List<MarkerInstance> areaGUIMarkers = new List<MarkerInstance>();

		[Space]
		public GameObject prefabMarkerPoiGui;
		public GameObject prefabMarkerAreaGui, prefabUserMarkerGui;

		private MarkerInstance _markerGuiOnSiteNear;
        private MarkerInstance markerUser, _markerMessage;

        [Space]
        public bool UseOrderForPois;
        [Space]
        public bool IsInUse;

        private void Awake()
        {
			poiGUIMarkers.Clear();
			areaGUIMarkers.Clear();
            EventHolder.OnBeaconTriggered += OnBeaconPoiClick;
        }

        private new void Start()
        {
            base.Start();
            map.OnMapUpdated += UpdateMarkers;
            OnlineMapsCameraOrbit.instance.OnCameraControl += UpdateMarkers;
        }

        public void OnLocationChanged(Vector2 pos)
        {
            if (engineManager.NarrationIsPlaying()) return;
            if (mapController.RouteIsMuseum()) return;

            double distance = Mathf.Infinity;
            float allowedDistance = OnSiteManager.Instance.CurrentTriggerDistanceForPois;

            //check poi on view if is far to close info panel
            if (_markerGuiOnSiteNear != null)
            {
                if (engineManager.InfoIsNotClosed())
                {
                    // if (!engineManager.PoiSettings().allowPoiSelectionIfInfoPanelIsOpen) { return; }
                    if (engineManager.PoiSettings().HidePoiInfoIfUserIsFar)
                    {
                        float _dist = OnlineMapsUtils.DistanceBetweenPoints(pos, _markerGuiOnSiteNear.data.pos).magnitude * 1000f;
                        if (_dist > allowedDistance)
                        {
                            _markerGuiOnSiteNear = null;
                            //close panel
                            EventHolder.OnInfoHide?.Invoke();

                            SetPoiIconsToDefault();
                        }
                        return;
                    }
                }
            }

            MarkerInstance _marker = null;

            foreach (MarkerInstance marker in poiGUIMarkers)
            {
                //if it is the last triggered marker then skip
                if (marker == _markerGuiOnSiteNear) continue;

                //should we gps trigger visited marker?
                if (!engineManager.PoiSettings().allowVisitedMarkerToBeSelectedAgain)
                {
                    if (guiPoiMarkersOnsiteVisited.Contains(marker)) continue;
                }

                // Calculate the distance in km
                float dist = OnlineMapsUtils.DistanceBetweenPoints(pos, marker.data.pos).magnitude * 1000f;
                if (dist < distance)
                {
                    _marker = marker;
                    distance = dist;
                }
            }

#if UNITY_EDITOR

            Debug.Log("Meters: " + distance.ToString("F2"));
            if (_marker != null)
            {
                Debug.Log(DataManager.Instance.GetTraslatedText(_marker.data.poiEntity.poi.poi_title));
            }

#endif

            if (_marker == null) return;

            //allow only marker in current order to be activated
            //if (OrderRouteControllerGUI.Instance.IsOrderRouteActive())
            //{
            //    if (_marker != OrderRouteControllerGUI.Instance.MarkerInOrderNow())
            //        return;
            //}

            //in range
            if (distance < allowedDistance)
            {
                if (!engineManager.PoiSettings().allowPoiSelectionIfInfoPanelIsOpen)
                {
                    //if full panel enabled do not select the new poi
                    // if (engineManager.InfoIsNotClosed()) return;

                    //if info panel is open
                    if (engineManager.infoPanelState != EnumsHolder.InfoPanelState.Closed)
                    {
                        bool isNewpoiTrigger = true;
                        if (_markerMessage != null && _markerMessage.data != null)
                        {
                            PoiEntity p = _markerMessage.data.poiEntity;
                            isNewpoiTrigger = p != null && p.ID != _marker.data.poiEntity.ID;
                        }

                        if (isNewpoiTrigger)
                        {
                            //show message to user
                            EventHolder.OnPoiTriggeredWhileOnInfoView?.Invoke();
                        }

                        _markerMessage = _marker;
                        return;
                    }
                }

                _markerGuiOnSiteNear = _marker;

                //if info panel is fully opened 
                //close info first and then yield wait a little 
                //to trigger info panel so that can be calculate title dimensions in time
                if (engineManager.infoPanelState == EnumsHolder.InfoPanelState.FullOpen)
                {
                    EventHolder.OnInfoHide?.Invoke();
                    Invoke(nameof(PoiGUIClickOnSite), 0.4f);
                }
                else
                {
                    PoiGUIClickOnSite();
                }
            }
            else//check if info is open and close it
            {
                if (!engineManager.PoiSettings().HidePoiInfoIfUserIsFar) return;
                //if route or period panel selection is closed
                //and the pois are visible on map
                if (engineManager.infoPanelState == EnumsHolder.InfoPanelState.Closed) return;

                if (_markerGuiOnSiteNear != null)
                {
                    _markerGuiOnSiteNear = null;
                    //close panel
                    EventHolder.OnInfoHide?.Invoke();

                    SetPoiIconsToDefault();
                }
            }
        }

        #region AREAS

        public void DestroyAreaGuiMarkers()
        {
            areaGUIMarkers.ForEach(b => Destroy(b.gameObject));
            areaGUIMarkers.Clear();
        }

        public void CreateAreaGUIMarker(AreaEntity areaEntity)
        {
            GameObject markerGameObject = Instantiate(prefabMarkerAreaGui) as GameObject;
            markerGameObject.name = GlobalUtils.GetText(GlobalUtils.langEN, areaEntity.area.area_title);

            RectTransform rectTransform = markerGameObject.transform as RectTransform;
            rectTransform.SetParent(engineManager.ContainerAreaMarkers());
            markerGameObject.transform.localScale = Vector3.one * mapController.settings.areaMarker.guiMarkerScale;

            MarkerInstance marker = new MarkerInstance();
            marker.data = markerGameObject.GetComponent<MarkerUIBase>();
            marker.data.areaEntity = areaEntity;
            marker.data.IsGuiMarker = true;
            marker.data.Init();
            marker.gameObject = markerGameObject;
            marker.transform = rectTransform;

            //marker.data.btn.onClick.AddListener(() => EventHolder.OnGuiMarkerClick?.Invoke(marker));
            marker.data.AddListener((b) => EventHolder.OnGuiMarkerClick?.Invoke(marker));

            areaGUIMarkers.Add(marker);

            UpdateMarkers();
        }

        #endregion

        #region POIS


        public void DestroyGuiPoiMarkers()
        {
            poiGUIMarkers.ForEach(b => Destroy(b.gameObject));
            poiGUIMarkers.Clear();
        }

        public void DestroyRangeOfMarkers(List<MarkerInstance> markers)
        {
            List<MarkerInstance> markersToDelete = new List<MarkerInstance>();
            foreach (MarkerInstance marker in poiGUIMarkers)
            {
                //MarkerInstance m = markers.Find(b => b.data.poiEntity.poi.poi_id == marker.data.poiEntity.poi.poi_id);
                foreach(MarkerInstance markerToRemove in markers)
                {
                    if(markerToRemove == marker)
                    {
                        markersToDelete.Add(marker);
                        break;
                    }
                }
            }

            foreach (MarkerInstance markerToRemove in markersToDelete)
            {
                poiGUIMarkers.Remove(markerToRemove);
            }

            markersToDelete.ForEach(b => Destroy(b.gameObject));
        }

        public void CreateGuiPoiMarkers(List<PoiEntity> pois, Vector2 areaCenter, string _iconPoiFilename = "")
        {

            if (pois == null || pois.Count <= 0)
            {
                EventHolder.OnAreasView?.Invoke();
                return;
            }

            pois = GetFixedPois(pois, areaCenter);

            //GET BEST POS AND ZOOM ################################################
            List<Vector2> poisPositions = new List<Vector2>();
            foreach (PoiEntity p in pois)
            {
                if (!poisPositions.Contains(p.poi.geoPosition)) poisPositions.Add(p.poi.geoPosition);
            }
            engineManager.GetBestZoomAndPosition(poisPositions);
            //#######################################################################

            //do pois have order?
            bool IsPoiOrderedTour = UseOrderForPois && pois.FindAll(b => b.OrderID() > 0).Count > 1;
            OrderRouteControllerGUI.Instance.ResetTour();

            //if (Application.isEditor)
            //    Debug.LogFormat("---[POIS {0} ORDER]---", IsPoiOrderedTour ? "HAVE" : "DON'T HAVE");

            if (engineManager.PoiSettings().createPoisInRow)
            {
                StartCoroutine(CreateGUIPoisInRow(pois, IsPoiOrderedTour));
            }
            else
            {
                //create pois instantly
                foreach (PoiEntity data in pois)
                    CreateGUIPoiMarker(data);

                //ApplySortOrderForPoiGUIMarkers();

                OrderRouteControllerGUI.Instance.InitializeRoute(IsPoiOrderedTour);

                OnlineMaps.instance.Redraw();
            }

            //MAP SMOOTH ACTIONS
            if (mapController.settings.useSmoothnessForAllPoisView)
            {
                mapController.SmoothMoveMap(engineManager.BEST_CENTER_POIS, engineManager.BEST_ZOOM_POIS);
            }
            else
            {
                engineManager.ApplyMapPoisBestZoomPosition();
            }

            UpdateMarkers();

#if UNITY_EDITOR
            if (mapController.settings.EditorDebug)
            {
                Invoke(nameof(GetGuiPoisMinimumDistance), 1f);
            }
#endif

        }

        //void ApplySortOrderForPoiGUIMarkers()
        //{

        //}

        public void CreatePeriodGuiPoiMarkers(List<PoiEntity> pois, Vector2 areaCenter, string _iconPoiFilename = "")
        {

            if (pois == null || pois.Count <= 0)
            {
                EventHolder.OnAreasView?.Invoke();
                return;
            }

            pois = GetFixedPois(pois, areaCenter);

            //GET BEST POS AND ZOOM ################################################
            List<Vector2> poisPositions = new List<Vector2>();
            foreach (PoiEntity p in pois)
            {
                if (!poisPositions.Contains(p.poi.geoPosition)) poisPositions.Add(p.poi.geoPosition);
            }
            engineManager.GetBestZoomAndPosition(poisPositions);
            //#######################################################################

            //do pois have order?
            bool IsPoiOrderedTour = UseOrderForPois && pois.FindAll(b => b.OrderID() > 0).Count > 1;
            OrderRouteControllerGUI.Instance.ResetTour();

#if UNITY_EDITOR
            if (mapController.settings.EditorDebug)
            {
                Debug.LogFormat("---[POIS {0} ORDER]---", IsPoiOrderedTour ? "HAVE" : "DON'T HAVE");
            }
#endif

            if (engineManager.PoiSettings().createPoisInRow)
            {
                StartCoroutine(CreateGUIPoisInRow(pois, IsPoiOrderedTour));
            }
            else
            {
                //create pois instantly
                foreach (PoiEntity data in pois)
                    CreateGUIPoiMarker(data);

                OrderRouteControllerGUI.Instance.InitializeRoute(IsPoiOrderedTour);

                OnlineMaps.instance.Redraw();
            }

            //MAP SMOOTH ACTIONS
            //if (mapController.settings.useSmoothnessForAllPoisView)
            //{
            //    mapController.SmoothMoveMap(engineManager.BEST_CENTER_POIS, engineManager.BEST_ZOOM_POIS);
            //}
            //else
            //{
            //    engineManager.ApplyMapPoisBestZoomPosition();
            //}

            UpdateMarkers();

#if UNITY_EDITOR
            if (mapController.settings.EditorDebug)
            {
                Invoke(nameof(GetGuiPoisMinimumDistance), 1f);
            }
#endif

        }

        void CreateGUIPoiMarker(PoiEntity poiEntity)
        {
            GameObject markerGameObject = Instantiate(prefabMarkerPoiGui) as GameObject;
            markerGameObject.name = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_title);
            RectTransform rectTransform = markerGameObject.transform as RectTransform;
            rectTransform.SetParent(engineManager.ContainerPoiMarkers());
            markerGameObject.transform.localScale = Vector3.one;

            MarkerInstance marker = new MarkerInstance();
            marker.data = markerGameObject.GetComponent<MarkerUIBase>();
            marker.data.poiEntity = poiEntity;
            marker.data.IsGuiMarker = true;

            //scale = engineManager.PoiSettings().GetNativeMarkerScale();// 1.3f;

            string id = poiEntity.ID.ToString();
            bool isVisited = SaveLoadManager.IsPoiVisitedOffSite(id);
            marker.data.SetIcon(isVisited ? poiEntity.poiIconVisited : poiEntity.poiIcon);

            marker.data.iconName = isVisited ? poiEntity.poiIconVisited.name : poiEntity.poiIcon.name;
            marker.data.Init();
            marker.gameObject = markerGameObject;
            marker.transform = rectTransform;

            //should be enabled only at off-site mode
            //if is on-site mode allow click if this marker is active (near user)
            if (engineManager.tourMode == EnumsHolder.TourMode.OffSite || engineManager.PoiSettings().allowPoiClick)
                marker.data.AddListener((b) => OnGuiMarkerPoiClick(marker));

            UpdateMarker(marker);

            poiGUIMarkers.Add(marker);
        }

        IEnumerator CreateGUIPoisInRow(List<PoiEntity> pois, bool haveOrder)
        {
            // bool applyFilter = refCenter != Vector2.one && mapController.settings.dontCreateMarkerIfFarFromArea;
            foreach (PoiEntity data in pois)
            {
                CreateGUIPoiMarker(data);
                yield return new WaitForEndOfFrame();
            }
            //ApplySortOrderForPoiGUIMarkers();

            OrderRouteControllerGUI.Instance.InitializeRoute(haveOrder);

            OnlineMaps.instance.Redraw();
            yield break;
        }              
               

        public void CreateUserGUIMarker(Vector2 pos, Texture2D tex, out MarkerInstance marker)
        {
            GameObject markerGameObject = Instantiate(prefabUserMarkerGui) as GameObject;
            markerGameObject.name = "USER";

            PoiMarkerUI poiMarkerUI = markerGameObject.GetComponent<PoiMarkerUI>();
            poiMarkerUI.SetTexture(tex);

            RectTransform rectTransform = markerGameObject.transform as RectTransform;
            rectTransform.SetParent(engineManager.mapUserMarkerContainer);
            markerGameObject.transform.localScale = Vector3.one * mapController.settings.userMarkerScale;

            markerUser = new MarkerInstance();
            markerUser.data = markerGameObject.GetComponent<MarkerUIBase>();// as AreaMarkerUI;
            markerUser.data.IsGuiMarker = true;
            markerUser.data.Init();
            markerUser.gameObject = markerGameObject;
            markerUser.transform = rectTransform;

            //marker.data.AddListener((b) => EventHolder.OnGuiMarkerClick?.Invoke(marker));

            marker = markerUser;

            UpdateMarkers();
        }



        #region ON-SITE

        void OnBeaconPoiClick(PoiEntity poiEntity)
        {
            if (!IsInUse)
            {
                EventHolder.OnBeaconTriggered -= OnBeaconPoiClick;
                return;
            }

            if (!engineManager.PoiSettings().allowPoiSelectionIfInfoPanelIsOpen)
            {
                //if full panel enabled do not select the new poi
                if (engineManager.InfoIsNotClosed()) return;
            }

            if(Application.isEditor) Debug.LogWarning("@@@ OnBeaconPoiClick");

            _markerGuiOnSiteNear = poiGUIMarkers.Find(b => b.data.poiEntity.ID == poiEntity.ID);

            if (_markerGuiOnSiteNear == null)
            {
                Debug.Log("BEACON MARKER NOT FOUND");
                return;
            }

            //if info panel is fully opened 
            //close info first and then yield wait a little 
            //to trigger info panel so that can be calculate title dimensions in time
            if (engineManager.infoPanelState == EnumsHolder.InfoPanelState.FullOpen)
            {
                EventHolder.OnInfoHide?.Invoke();
                Invoke(nameof(PoiGUIClickOnSite), 0.4f);
            }
            else
            {
                PoiGUIClickOnSite();
            }
        }

        public void PoiGUIClickOnSite()
        {
            //check on-site mode 
            if (engineManager.tourMode != EnumsHolder.TourMode.OnSite) return;

            if (engineManager.NarrationIsPlaying()) return;

            //store as viewed?
            if (!guiPoiMarkersOnsiteVisited.Contains(_markerGuiOnSiteNear) || engineManager.PoiSettings().allowTriggeredPoiToBeTriggeredAgain)
            {
                if (SystemInfo.supportsVibration)
                    Handheld.Vibrate();

                guiPoiMarkersOnsiteVisited.Add(_markerGuiOnSiteNear);

                //should be clickable by user?
                //allow click
                if (!engineManager.PoiSettings().allowPoiClick)
                {
                    //allow click = add listeners
                    if (engineManager.PoiSettings().allowVisitedMarkerToBeSelectedAgain)
                    {
                        _markerGuiOnSiteNear.data.AddListener((b) => OnGuiMarkerPoiClick(_markerGuiOnSiteNear));
                    }
                    else
                    {
                        _markerGuiOnSiteNear.data.RemoveAllListeners();
                    }
                }
                else
                {
                    if (!engineManager.PoiSettings().allowVisitedMarkerToBeSelectedAgain)
                    {
                        _markerGuiOnSiteNear.data.RemoveAllListeners();
                    }
                }

                //change texture??
                SetPoiIconsToDefault();
                _markerGuiOnSiteNear.data.SetIcon(_markerGuiOnSiteNear.data.poiEntity.poiIconActive);

                //auto click ?
                if (engineManager.PoiSettings().enablePoiInfoIfUserIsNear) OnGuiMarkerPoiClick(_markerGuiOnSiteNear);
            }
            else
            {
                //is already found
                return;
            }

        }

        #endregion

        private void OnGuiMarkerPoiClick(MarkerInstance _marker)
        {
            if (!OrderRouteControllerGUI.Instance.IsInfoOpenedForCurrentPoiInView(_marker))
            {
                Debug.Log("IsInfoOpenedForCurrentPoiInView");
                return;
            }

            SetPoiIconsToDefault();

            string id = _marker.data.poiEntity.ID.ToString();
            _marker.data.SetIcon(_marker.data.poiEntity.poiIconActive);

            PoiEntity poiEntity = DataManager.Instance.GetPoiEntity(id);
            if (poiEntity == null) return;
            EventHolder.OnGuiMarkerClick?.Invoke(_marker);

            if (engineManager.tourMode == EnumsHolder.TourMode.OffSite)
            {
                SaveLoadManager.SavePoiAsVisitedOffSite(id);
            }
            else
            {
                SaveLoadManager.SavePoiAsVisitedOnSite(id);
            }
        }


        #endregion

        public void UpdateMarkers()
        {
            if (markerUser != null) UpdateMarker(markerUser);
            foreach (MarkerInstance marker in poiGUIMarkers) UpdateMarker(marker);
            foreach (MarkerInstance marker in areaGUIMarkers) UpdateMarker(marker);
        }

        private void UpdateMarker(MarkerInstance marker)
        {
            if (!control) control = OnlineMapsTileSetControl.instance;

            double px = marker.data.pos.x;
            double py = marker.data.pos.y;

            Vector2 screenPosition = control.GetScreenPosition(px, py);
            if (forwarder != null)
            {
                if (!map.InMapView(px, py))
                {
                    marker.gameObject.SetActive(false);
                    return;
                }

                screenPosition = forwarder.MapToForwarderSpace(screenPosition);
            }

            if (screenPosition.x < 0 || screenPosition.x > Screen.width ||
                screenPosition.y < 0 || screenPosition.y > Screen.height)
            {
                marker.gameObject.SetActive(false);
                return;
            }

            RectTransform markerRectTransform = marker.transform;

            if (!marker.gameObject.activeSelf && marker.data.ShouldZoomVisible()) marker.gameObject.SetActive(true);

            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(markerRectTransform.parent as RectTransform, screenPosition, null, out point);
            markerRectTransform.localPosition = point;
        }


        public void SetPoiIconsToDefault()
        {

            if (OrderRouteControllerGUI.Instance.IsOrderRouteActive()) return;

            foreach (MarkerInstance m in poiGUIMarkers)
            {
                string id = m.data.poiEntity.ID.ToString();
                bool isVisited = engineManager.tourMode == EnumsHolder.TourMode.OffSite ? SaveLoadManager.IsPoiVisitedOffSite(id) : SaveLoadManager.IsPoiVisitedOnSite(id);

                if (engineManager.tourMode == EnumsHolder.TourMode.OnSite)
                {
                    if (!guiPoiMarkersOnsiteVisited.Contains(m))
                    {
                        m.data.SetIcon(m.data.poiEntity.poiIcon);
                        continue;
                    }
                }

                m.data.SetIcon(isVisited ? m.data.poiEntity.poiIconVisited : m.data.poiEntity.poiIcon);
            }
        }

        public void GetGuiPoisMinimumDistance()
        {
            float minDistance = 1000f;
            string poiA = "", poiB = "";
            string a = "", b = "";

            for (int i = 0; i < poiGUIMarkers.Count; i++)
            {
                foreach (MarkerInstance m in poiGUIMarkers)
                {
                    if (m.data.poiEntity.poi.poi_id == poiGUIMarkers[i].data.poiEntity.poi.poi_id)
                        continue;

                    // Calculate the distance in km
                    float distance = OnlineMapsUtils.DistanceBetweenPoints(poiGUIMarkers[i].data.pos, m.data.pos).magnitude;

                    if (distance < minDistance)
                    {
                        string id = poiGUIMarkers[i].data.poiEntity.ID.ToString();
                        PoiEntity poiEntity = DataManager.Instance.GetPoiEntity(id);
                        if (poiEntity != null) a = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_title);

                        id = m.data.poiEntity.ID.ToString();
                        poiEntity = DataManager.Instance.GetPoiEntity(id);
                        if (poiEntity != null) b = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_title);

                        if (!a.IsNull() && !b.IsNull())
                        {
                            if (a != b)
                            {
                                poiA = a;
                                poiB = b;
                                minDistance = distance;
                            }
                        }
                    }

                }
            }
#if UNITY_EDITOR
            if (mapController.settings.EditorDebug)
            {
                Debug.LogWarning("[" + a + "] - [" + b + "] = " + minDistance * 1000f + " meters");
                Debug.LogWarning("Min Distance is " + minDistance * 1000f + " meters");
            }
#endif
        }

    }

}

