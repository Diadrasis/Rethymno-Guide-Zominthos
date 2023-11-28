//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class MarkerEngineNative : MarkerEngineBase
    {

        public List<OnlineMapsMarker> poiNativeMarkers = new List<OnlineMapsMarker>();
        public List<OnlineMapsMarker> areaNativeMarkers = new List<OnlineMapsMarker>();
        public List<OnlineMapsMarkerBase> nativePoiMarkersOnsiteVisited = new List<OnlineMapsMarkerBase>();

        public void ShowAreaMarkers(bool val) { areaNativeMarkers.ForEach(b => b.enabled = val); }

        private OnlineMapsMarker _markerOnSiteNear;

        public bool IsInUse;

        private void Awake()
        {
            //native
            poiNativeMarkers.Clear();
            areaNativeMarkers.Clear();

            EventHolder.OnBeaconTriggered += OnBeaconPoiClick;

        }

        public void OnLocationChanged(Vector2 pos)
        {

            double distance = Mathf.Infinity;
            float allowedDistance = OnSiteManager.Instance.CurrentTriggerDistanceForPois;

            //check poi on view if is far to close info panel
            if (_markerOnSiteNear != null)
            {
                if (engineManager.InfoIsNotClosed())
                {
                    float _dist = OnlineMapsUtils.DistanceBetweenPoints(pos, _markerOnSiteNear.position).magnitude * 1000f;
                    if (_dist > allowedDistance)
                    {
                        _markerOnSiteNear = null;
                        //close panel
                        EventHolder.OnInfoHide?.Invoke();

                        SetPoiIconsToDefault();
                    }

                    return;
                }
            }

            OnlineMapsMarker _marker = null;

            foreach (OnlineMapsMarker marker in poiNativeMarkers)
            {
                //if it is the last triggered marker then skip
                if (marker == _markerOnSiteNear) continue;

                //should we gps trigger visited marker?
                if (!engineManager.PoiSettings().allowVisitedMarkerToBeSelectedAgain)
                {
                    if (nativePoiMarkersOnsiteVisited.Contains(marker)) continue;
                }

                // Calculate the distance in km
                float dist = OnlineMapsUtils.DistanceBetweenPoints(pos, marker.position).magnitude * 1000f;
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
                OnlineMapsMarker m = _marker as OnlineMapsMarker;
                string id = GetDataID(_marker);
                PoiEntity poiEntity = DataManager.Instance.GetPoiEntity(id);
                Debug.Log(DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_title));
            }

#endif

            if (_marker == null) return;

            //allow only marker in current order to be activated
            if (OrderRouteControllerNative.Instance.IsOrderRouteActive())
            {
                if (_marker != OrderRouteControllerNative.Instance.MarkerInOrderNow())
                    return;
            }


            //if in range
            if (distance < allowedDistance)
            {
                if (!engineManager.PoiSettings().allowPoiSelectionIfInfoPanelIsOpen)
                {
                    //if full panel enabled do not select the new poi
                    if (engineManager.InfoIsNotClosed()) return;
                }

                //assign the new poi
                _markerOnSiteNear = _marker;

                //if info panel is fully opened 
                //close info first and then yield wait a little 
                //to trigger info panel so that can be calculate title dimensions in time
                if (engineManager.infoPanelState == EnumsHolder.InfoPanelState.FullOpen)
                {
                    EventHolder.OnInfoHide?.Invoke();
                    Invoke(nameof(PoiNativeClickOnSite), 0.4f);
                }
                else
                {
                    PoiNativeClickOnSite();
                }

            }
            else//check if info is open and close it
            {
                //if route or period panel selection is closed
                //and the pois are visible on map
                if (engineManager.infoPanelState == EnumsHolder.InfoPanelState.Closed) return;

                if (_markerOnSiteNear != null)
                {
                    _markerOnSiteNear = null;
                    //close panel
                    EventHolder.OnInfoHide?.Invoke();

                    SetPoiIconsToDefault();
                }
            }
        }

        #region AREAS

        public void DestroyAreaNativeMarkers()
        {
            OnlineMapsMarkerManager.instance.items.RemoveAll(b => b.tags.Contains("area"));
            areaNativeMarkers.Clear();
            OnlineMaps.instance.Redraw();
        }

        public void CreateNativeAreaMarker(AreaEntity areaEntity)
        {
            OnlineMapsMarker dynamicMarker =
                OnlineMapsMarkerManager.instance.Create(areaEntity.area.geoPosition, SaveLoadManager.LoadTexture(areaEntity.area.area_icon, GlobalUtils.iconMarkerEmpty));

            // Create new XML and store it in customData.
            OnlineMapsXML xmlData = new OnlineMapsXML("MarkerData");
            xmlData.Create(nameof(cArea.area_id), areaEntity.ID);
            dynamicMarker["data"] = xmlData;

            dynamicMarker.OnClick += OnNativeMarkerAreaClick;
            dynamicMarker.scale = mapController.settings.areaMarker.nativeMarkerScale; //1.3f;
            dynamicMarker.tags.Add("area");

            if (Application.isEditor)
                Debug.LogWarningFormat("Area Marker {0} created", areaEntity.ID);

            areaNativeMarkers.Add(dynamicMarker);
        }

        private void OnNativeMarkerAreaClick(OnlineMapsMarkerBase _marker)
        {
            OnlineMapsMarker m = _marker as OnlineMapsMarker;
            string id = GetDataAreaID(_marker);
            AreaEntity areaEntity = DataManager.Instance.GetAreaEntity(id);
            if (areaEntity == null) return;
            EventHolder.OnNativePoiAreaClick?.Invoke(areaEntity);
        }

        #endregion

        #region POIS

       
        public void CreatePoisNativeMarkers(List<PoiEntity> pois, Vector2 areaCenter)
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
            bool IsPoiOrderedTour = pois.FindAll(b => b.OrderID() > 0).Count > 1;
            OrderRouteControllerNative.Instance.ResetTour();

            if (Application.isEditor)
                Debug.LogFormat("---[POIS {0} ORDER]---", IsPoiOrderedTour ? "HAVE" : "DON'T HAVE");

            if (engineManager.PoiSettings().createPoisInRow)
            {
                StartCoroutine(CreateNativePoisInRow(pois, IsPoiOrderedTour));
            }
            else
            {
                //create pois instantly
                foreach (PoiEntity data in pois)
                    CreateNativePoiMarker(data);

                ApplySortOrderForPoiNativeMarkers();

                OrderRouteControllerNative.Instance.InitializeRoute(IsPoiOrderedTour);

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

#if UNITY_EDITOR
            if (mapController.settings.EditorDebug)
            {
                Invoke(nameof(GetNativePoisMinimumDistance), 1f);
            }
#endif
        }

        void ApplySortOrderForPoiNativeMarkers()
        {
            //change the sort order of the markers
            if (engineManager.PoiSettings().changeSortOrderOfPOIMarkers)
            {
                // Sets a new comparer.
                OnlineMapsMarkerFlatDrawer drawer = (OnlineMapsTileSetControl.instance.markerDrawer as OnlineMapsMarkerFlatDrawer);
                if (drawer != null) drawer.markerComparer = new MarkerComparer();
            }
        }

        IEnumerator CreateNativePoisInRow(List<PoiEntity> pois, bool haveOrder)
        {
            // bool applyFilter = refCenter != Vector2.one && mapController.settings.dontCreateMarkerIfFarFromArea;
            foreach (PoiEntity data in pois)
            {
                CreateNativePoiMarker(data);
                yield return new WaitForEndOfFrame();
            }
            ApplySortOrderForPoiNativeMarkers();

            OrderRouteControllerNative.Instance.InitializeRoute(haveOrder);

            OnlineMaps.instance.Redraw();
            yield break;
        }

        void CreateNativePoiMarker(PoiEntity poiEntity)
        {
            // Debug.Log("CreateNativePoiMarker " + DataManager.Instance.GetText(poiEntity.poi.poi_title));

            OnlineMapsMarker dynamicMarker =
                OnlineMapsMarkerManager.instance.Create(poiEntity.poi.geoPosition, mapController.GetPoiIcon(poiEntity));

            // Create new XML and store it in customData.
            OnlineMapsXML xmlData = new OnlineMapsXML("MarkerData");
            xmlData.Create(nameof(cPoi.poi_id), poiEntity.ID);
            xmlData.Create(nameof(cPoi.poi_order), poiEntity.OrderID());
            dynamicMarker["data"] = xmlData;

            //should be enabled only at off-site mode
            //if is on-site mode allow click if this marker is active (near user)
            if (engineManager.tourMode == EnumsHolder.TourMode.OffSite || engineManager.PoiSettings().allowPoiClick)
                dynamicMarker.OnClick += OnNativeMarkerPoiClick;

            dynamicMarker.scale = engineManager.PoiSettings().GetNativeMarkerScale();// 1.3f;
            dynamicMarker.tags.Add("poi");
            poiNativeMarkers.Add(dynamicMarker);
        }

        public void CreateNativeMarker(Vector2 pos, Texture2D tex, out OnlineMapsMarker marker)
        {
            marker = OnlineMapsMarkerManager.instance.Create(pos, tex);
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

            _markerOnSiteNear = poiNativeMarkers.Find(b => GetDataID(b) == poiEntity.ID.ToString());

            if (_markerOnSiteNear == null) return;

            //if info panel is fully opened 
            //close info first and then yield wait a little 
            //to trigger info panel so that can be calculate title dimensions in time
            if (engineManager.infoPanelState == EnumsHolder.InfoPanelState.FullOpen)
            {
                EventHolder.OnInfoHide?.Invoke();
                Invoke(nameof(PoiNativeClickOnSite), 0.4f);
            }
            else
            {
                PoiNativeClickOnSite();
            }
        }
        private void PoiNativeClickOnSite()
        {
            //check on-site mode 
            if (engineManager.tourMode != EnumsHolder.TourMode.OnSite) return;

            //store as viewed?
            if (!nativePoiMarkersOnsiteVisited.Contains(_markerOnSiteNear) || engineManager.PoiSettings().allowTriggeredPoiToBeTriggeredAgain)
            {
                if (SystemInfo.supportsVibration)
                    Handheld.Vibrate();

                if (!nativePoiMarkersOnsiteVisited.Contains(_markerOnSiteNear))
                    nativePoiMarkersOnsiteVisited.Add(_markerOnSiteNear);
                //should be clickable by user after?
                //allow click
                if (!engineManager.PoiSettings().allowPoiClick)
                {
                    if (engineManager.PoiSettings().allowVisitedMarkerToBeSelectedAgain)
                    {
                        _markerOnSiteNear.OnClick += OnNativeMarkerPoiClick;
                    }
                    else
                    {
                        _markerOnSiteNear.OnClick -= OnNativeMarkerPoiClick;
                    }
                }
                else
                {
                    if (!engineManager.PoiSettings().allowVisitedMarkerToBeSelectedAgain)
                    {
                        _markerOnSiteNear.OnClick -= OnNativeMarkerPoiClick;
                    }
                }

                //change texture??
                SetPoiIconsToDefault();
                OnlineMapsMarker m = _markerOnSiteNear as OnlineMapsMarker;
                string id = GetDataID(_markerOnSiteNear);
                PoiEntity poiEntity = DataManager.Instance.GetPoiEntity(id);
                if (poiEntity != null)
                    m.texture = poiEntity.poiIconActive;

                //auto click ?
                if (engineManager.PoiSettings().enablePoiInfoIfUserIsNear) OnNativeMarkerPoiClick(_markerOnSiteNear);
            }
            else
            {
                //is already found
                return;
            }

        }

        #endregion


        private void OnNativeMarkerPoiClick(OnlineMapsMarkerBase _marker)
        {
            OnlineMapsMarker m = _marker as OnlineMapsMarker;

            if (!OrderRouteControllerNative.Instance.IsInfoOpenedForCurrentPoiInView(m))
            {
                return;
            }

            SetPoiIconsToDefault();

            string id = GetDataID(_marker);
            PoiEntity poiEntity = DataManager.Instance.GetPoiEntity(id);
            if (poiEntity == null) return;

            m.texture = poiEntity.poiIconActive;
            m.Init();

            EventHolder.OnNativePoiMarkerClick?.Invoke(poiEntity);

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


        public void GetNativePoisMinimumDistance()
        {
            float minDistance = 1000f;
            string poiA = "", poiB = "";
            string a = "", b = "";
            for (int i = 0; i < poiNativeMarkers.Count; i++)
            {
                foreach (OnlineMapsMarker m in poiNativeMarkers)
                {
                    if (GetDataID(m) == GetDataID(poiNativeMarkers[i]))
                        continue;

                    // Calculate the distance in km
                    float distance = OnlineMapsUtils.DistanceBetweenPoints(poiNativeMarkers[i].position, m.position).magnitude;
                    
                    if (distance < minDistance)
                    {
                        string id = GetDataID(poiNativeMarkers[i]);// xml.Get(nameof(cPoi.poi_id));// "poi_id");
                        PoiEntity poiEntity = DataManager.Instance.GetPoiEntity(id);
                        if (poiEntity != null) a = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_title);

                        id = GetDataID(m);// xml.Get(nameof(cPoi.poi_id));// "poi_id");
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
                    //}
                }
            }
#if UNITY_EDITOR
            if (mapController.settings.EditorDebug)
            {
                Debug.LogWarning("<color=orange>[" + poiA + "] - [" + poiB + "] = " + minDistance * 1000f + " meters</color>");
                Debug.LogWarning("<color=orange>Nearest Pois Distance is " + minDistance * 1000f + " meters</color>");
            }
#endif
        }

        public void DestroyPoiMarkers()
        {
            OnlineMapsMarkerManager.instance.items.RemoveAll(b => b.tags.Contains("poi"));
            poiNativeMarkers.Clear();
            OnlineMaps.instance.Redraw();
        }

        public void SetPoiIconsToDefault()
        {
            if (OrderRouteControllerNative.Instance.IsOrderRouteActive()) return;

            foreach (OnlineMapsMarker m in poiNativeMarkers)
            {
                string id = GetDataID(m);
                bool isVisited = engineManager.tourMode == EnumsHolder.TourMode.OffSite ? SaveLoadManager.IsPoiVisitedOffSite(id) : SaveLoadManager.IsPoiVisitedOnSite(id);

                PoiEntity poiEntity = DataManager.Instance.GetPoiEntity(id);
                if (poiEntity != null)
                {
                    if (engineManager.tourMode == EnumsHolder.TourMode.OnSite)
                    {
                        if (!nativePoiMarkersOnsiteVisited.Contains(m))
                        {
                            m.texture = poiEntity.poiIcon;
                            continue;
                        }
                    }

                    m.texture = isVisited ? poiEntity.poiIconVisited : poiEntity.poiIcon;
                    m.Init();
                }
            }
        }


        public int GetPoiOrderID(OnlineMapsMarkerBase _marker)
        {
            // Try get XML from customData.
            OnlineMapsXML xml = _marker["data"] as OnlineMapsXML;

            if (xml == null)
            {
                Debug.Log("The marker does not contain XML.");
                return 0;
            }

            return int.Parse(xml.Get(nameof(cPoi.poi_order)));
        }

        public string GetDataID(OnlineMapsMarkerBase _marker)
        {
            // Try get XML from customData.
            OnlineMapsXML xml = _marker["data"] as OnlineMapsXML;

            if (xml == null)
            {
                Debug.Log("The marker does not contain XML.");
                return "";
            }

            return xml.Get(nameof(cPoi.poi_id));
        }

        private string GetDataAreaID(OnlineMapsMarkerBase _marker)
        {
            // Try get XML from customData.
            OnlineMapsXML xml = _marker["data"] as OnlineMapsXML;

            if (xml == null)
            {
                Debug.Log("The marker does not contain XML.");
                return "";
            }

            return xml.Get(nameof(cArea.area_id));
        }

    }

}

