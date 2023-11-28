//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class MapController : MonoBehaviour
	{
        public MapSettings settings;
        [Space]
        public EnumsHolder.MapLayerType mapLayerType = EnumsHolder.MapLayerType.Custom;
        public bool IsMapTypeCustom() { return mapLayerType == EnumsHolder.MapLayerType.Custom; }

        [Space]
        public GameObject stopMapInteractionPanel;

        private MarkerEngineManager markerEngineManager;

        private AreaEntity areaSelected;
        private RouteEntity routeSelected;
        public AreaEntity AreaSelected { get { return areaSelected; } set{ areaSelected = value; } }
            
        public Texture2D GetPoiIcon(PoiEntity _poi)
        {
            if (SaveLoadManager.IsPoiVisitedOffSite(_poi.ID.ToString()))
            {
                return _poi.poiIconVisited;
            }
            return _poi.poiIcon;
        }

        private void Awake()
        {
            markerEngineManager = FindObjectOfType<MarkerEngineManager>(true);
#if !UNITY_EDITOR
            settings.EditorDebug = false;
#endif

            //For iOS builds, enable the "Exit on Suspend" property in Player Settings to make the application quit and not suspend,
            if (settings.poisOptions.deleteSavedVisitedPoisOnStart)
                SaveLoadManager.DeleteVisitedPois();

        }

        void Start()
        {
            if (settings.visual.applyVisualSettings)
            {
                OnlineMaps.instance.mapType = settings.visual.mapsProvider.ToString();// "custom";
                OnlineMaps.instance.customProviderURL = settings.visual.customProviderURL;//.snazzyMapsProvider;
            }

            CreateFromDatabase();
            //ZoomOnAreas();

            EventHolder.OnGuiMarkerClick += OnMarkerClicked;
            EventHolder.OnNativePoiMarkerClick += OnNativePoiMarkerClick;

            EventHolder.OnNativePoiAreaClick += OnNativePoiAreaClick;

            EventHolder.OnAreasView += ZoomOnAreas;
            EventHolder.OnRoutesView += ZoomOnArea;
            EventHolder.OnPeriodsView += ZoomOnArea;
            EventHolder.OnPoisView += ZoomOnPois;
            EventHolder.OnRouteSelected += OnRouteSelected;
            EventHolder.OnPeriodsSelected += OnPeriodSelected;
            EventHolder.OnPeriodToggleVisibility += OnPeriodToggleVisibility;

            EventHolder.OnPoisShow += OnPoiShowAction;

            EventHolder.OnLanguageChanged += OnLanguageChanged;
            EventHolder.OnUpdateFinished += OnUpdateCompleted;

            EventHolder.OnStateChanged += OnStateChanged;

            EventHolder.OnLayerToggle += MapLayerToggle;

        }

        public OnlineMapsMarker CreateUserMarker()
        {
            markerEngineManager.nativeMarkerEngine.CreateNativeMarker(Vector2.one, settings.userMarkerIcon, out OnlineMapsMarker markerUser);
            if (markerUser == null) return null;
            markerUser.tags.Add("user");
            markerUser.enabled = false;
            markerUser.align = OnlineMapsAlign.Center;
            markerUser.scale = 1f;
            return markerUser;
        }

        public MarkerInstance CreateGuiUserMarker()
        {
            markerEngineManager.uGUIMarkerEngine.CreateUserGUIMarker(Vector2.one, settings.userMarkerIcon, out MarkerInstance markerUser);
            if (markerUser == null) return null;
            markerUser.gameObject.SetActive(false);
            return markerUser;
        }

        void MapLayerToggle()
        {
           // Debug.Log(OnlineMaps.instance.mapType.ToString());
            if (OnlineMaps.instance.mapType.ToString().ToLower().StartsWith("custom"))
            {
                OnlineMaps.instance.mapType = "google.satellite";
                mapLayerType = EnumsHolder.MapLayerType.Satellite;
            }
            else
            {
                OnlineMaps.instance.mapType = settings.visual.mapsProvider.ToString();// "custom";
                OnlineMaps.instance.customProviderURL = settings.visual.customProviderURL;//.snazzyMapsProvider;
                //OnlineMaps.instance.mapType = settings.visual.mapsProvider.ToString();
                mapLayerType = EnumsHolder.MapLayerType.Custom;
            }

            EventHolder.OnMapLayerChanged?.Invoke();
        }

        void OnStateChanged(EnumsHolder.AppState appState)
        {
            if (appState == EnumsHolder.AppState.AreasView)
            {
                areaSelected = null;
                routeSelected = null;
            }
        }

        void OnUpdateCompleted()
        {
            if (settings.EditorDebug)
                Debug.LogWarning("RE-CREATE MAP FROM SERVER UPDATES");
            // AppManager.Instance.jsonDatabase.ReadJsonFilesOverridde();
            OnlineMaps.instance.floatZoom = settings.initialArgs.GetAreasPrefferedZoom();// 17;
            CreateFromDatabase();
            //ZoomOnAreas();
        }

        void OnLanguageChanged()
        {
            //OnlineMaps.instance.language = DataManager.Instance.LangNow();
           // OnlineMaps.instance.zoom = 17;
            CreateFromDatabase();
        }

        void CreateFromDatabase()
        {
            if (DataManager.Instance.HasAnyData())
            {
                DataManager.Instance.PrepareDataToRead();
            }
            else
            {
                Debug.LogWarning("<color=pink>[ERROR] Database is EMPTY</color>");
                return;
            }           

            markerEngineManager.CreateAreaMarkers(DataManager.Instance.AreaEntities(), settings.areaMarker.createNativeMarkers);

        }

        void OnPoiShowAction(bool val)
        {
            //destroy pois on exit pois view
            if (!val) markerEngineManager.DestroyPoiMarkers();
        }

        /// <summary>
        /// hardcoded from route_types json file
        /// </summary>
        /// <returns>if type id is 430</returns>
        public bool RouteIsMuseum() { return routeSelected == null ? false : routeSelected.route.route_type_id == 430; }

        void OnRouteSelected(RouteEntity routeEntity)
        {
            //markerEngineManager.DestroyPoiMarkers();

            if (settings.EditorDebug)
            {
                Debug.LogWarning("route pois = " + routeEntity.poiEntities.Count);
                Debug.LogWarning("This route has periods = " + routeEntity.HasPeriods());
            }

            routeSelected = routeEntity;

            if (settings.EditorDebug)
                Debug.Log("Select " + DataManager.Instance.GetTraslatedText(routeEntity.route.route_title));

            if (!routeEntity.HasPeriods())//!GlobalUtils.HasRouteTypePeriods(routeEntity.routeType))
            {
                markerEngineManager.DestroyPoiMarkers();
                //create pois and assign icon 
                Vector2 areaCenter = areaSelected != null ? areaSelected.area.geoPosition : Vector2.one;
                markerEngineManager.CreatePoiMarkers(routeEntity.poiEntities, settings.poisOptions.createNativePoiMarkers, areaCenter);
            }
            else
            {
                if (DataManager.Instance.skipPeriodInfo)
                {
                    markerEngineManager.DestroyPoiMarkers();
                    //create pois and assign icon 
                    Vector2 areaCenter = areaSelected != null ? areaSelected.area.geoPosition : Vector2.one;

                    List<PoiEntity> pois = new List<PoiEntity>();
                    foreach(PeriodEntity p in routeEntity.periods)
                    {
                        pois.AddRange(p.poiEntities);
                    }

                    markerEngineManager.CreatePoiMarkers(pois, settings.poisOptions.createNativePoiMarkers, areaCenter);
                }
            }
        }

        void OnPeriodToggleVisibility(PeriodEntity period, bool isVisible)
        {
            if (!isVisible)
            {
                List<MarkerInstance> poiGUIMarkers = markerEngineManager.uGUIMarkerEngine.poiGUIMarkers;
                List<MarkerInstance> deletedMarkers = new List<MarkerInstance>();
                foreach(MarkerInstance m in poiGUIMarkers)
                {
                    PoiEntity p = period.poiEntities.Find(b => b.ID == m.data.poiEntity.poi.poi_id);
                    if (p != null) deletedMarkers.Add(m);
                }

                markerEngineManager.uGUIMarkerEngine.DestroyRangeOfMarkers(deletedMarkers);
            }
            else
            {
                Vector2 areaCenter = areaSelected != null ? areaSelected.area.geoPosition : Vector2.one;
                markerEngineManager.uGUIMarkerEngine.CreatePeriodGuiPoiMarkers(period.poiEntities, areaCenter);
            }

        }

        void OnPeriodSelected(PeriodEntity selectedPeriod, List<PeriodEntity> previousPeriods = null)
        {
#if UNITY_EDITOR
            if (settings.EditorDebug)
                Debug.Log("Select " + DataManager.Instance.GetTraslatedText(selectedPeriod.period.period_title));
#endif

            markerEngineManager.DestroyPoiMarkers();

            List<PoiEntity> _poiEntities = new List<PoiEntity>();
            _poiEntities.AddRange(selectedPeriod.poiEntities);

            //create pois and assign icon 
            Vector2 areaCenter = areaSelected != null ? areaSelected.area.geoPosition : Vector2.one;
            //markerEngineManager.CreatePoiMarkers(selectedPeriod.poiEntities, settings.poisOptions.createNativePoiMarkers, areaCenter);

            if(previousPeriods != null)
            {
                if(previousPeriods.Count > 0)
                {
#if UNITY_EDITOR
                    Debug.Log("previousPeriods = " + previousPeriods.Count);
#endif
                    foreach (PeriodEntity p in previousPeriods)
                    {
                       // markerEngineManager.CreatePoiMarkers(p.poiEntities, settings.poisOptions.createNativePoiMarkers, areaCenter);

                        _poiEntities.AddRange(p.poiEntities);
                    }
                }
            }

            markerEngineManager.CreatePoiMarkers(_poiEntities, settings.poisOptions.createNativePoiMarkers, areaCenter);
        }

        void ZoomOnPois()
        {
#if UNITY_EDITOR
            Debug.Log("ZoomOnPois");
#endif
            if (settings.poisOptions.createNativePoiMarkers)
            {
                if (markerEngineManager.nativeMarkerEngine.poiNativeMarkers.Count <= 0)
                {
                    ZoomOnArea(areaSelected);
                    return;
                }
                if (settings.useSmoothnessForAllPoisView)
                {
                    SmoothMoveMap(markerEngineManager.BEST_CENTER_POIS, markerEngineManager.BEST_ZOOM_POIS);
                }
                else
                {
                    markerEngineManager.CenterMapOnNativeMarkers(markerEngineManager.nativeMarkerEngine.poiNativeMarkers);
                }

            }
            else
            {
                if (markerEngineManager.uGUIMarkerEngine.poiGUIMarkers.Count <= 0)
                {
                    ZoomOnArea(areaSelected);
                    return;
                }

                if (settings.useSmoothnessForAllPoisView)
                {
                    SmoothMoveMap(markerEngineManager.BEST_CENTER_POIS, markerEngineManager.BEST_ZOOM_POIS);
                }
                else
                {
                    markerEngineManager.CenterMapOnGUIMarkers(markerEngineManager.uGUIMarkerEngine.poiGUIMarkers, -1);
                }

            }
        }

        void OnNativePoiAreaClick(AreaEntity areaEntity)
        {
            if (areaEntity == null) return;
            areaSelected = areaEntity;
            markerEngineManager.CenterMapOnPosition(areaEntity.area.geoPosition);
        }

        void OnNativePoiMarkerClick(PoiEntity poiEntity)
        {
            if(poiEntity == null) return;
            if (settings.poisOptions.zoomOnPoiClick && poiEntity != null)
                markerEngineManager.CenterMapOnPosition(poiEntity.poi.geoPosition);
        }

        void OnMarkerClicked(MarkerInstance marker)
        {
            if (marker == null || marker.data == null) return;
            if (marker.data.IsArea)
            {
                areaSelected = marker.data.areaEntity;
               // if (marker.data.areaEntity != null)  markerEngineManager.CenterOnMarker(marker);
            }
            else
            {
                if (marker.data.poiEntity == null) return;
                if (settings.poisOptions.zoomOnPoiClick) markerEngineManager.CenterOnMarker(marker);
            }
        }

        public void ZoomOnArea(AreaEntity areaEntity)
        {
            if (settings.areaMarker.createNativeMarkers) markerEngineManager.nativeMarkerEngine.ShowAreaMarkers(false);
            //markerEngineManager.DestroyPoiMarkers();

            if (settings.useSmoothnessForArea)
            {
                Vector2 pos = areaEntity.area.geoPosition;

                if (settings.useCustomAreaZoom)
                {
                    SmoothMoveMap(pos, settings.zoomAreaManual);
                }
                else
                {
                    float zoom = markerEngineManager.GetBestZoomForPos(pos);
                    SmoothMoveMap(pos, zoom);
                }
            }
            else
            {
                if (settings.useCustomAreaZoom)
                {
                    float zoom = settings.zoomAreaManual;
                    OnlineMaps.instance.floatZoom = zoom;
                    OnlineMaps.instance.position = areaEntity.area.geoPosition;
                    return;
                }

                markerEngineManager.CenterOnMarker(areaEntity.area.geoPosition);
            }
        }

#region SMOOTH MOVE MAP

        /// <summary>
        /// Relative position (0-1) between from and to
        /// </summary>
        private float angle;
        /// Movement trigger
        private bool isMovement;

        private Vector2 fromPosition;
        private double fromTileX, fromTileY, toTileX, toTileY;
        private float moveZoom;

#region Zoom Variables

        float timeElapsed;
        float startZoomValue;
        float endZoomValue;
        /// Zoom trigger
        private bool isZooming;

#endregion

        public void SmoothMoveMap(Vector2 toPosition, float zoomValue, bool checkDist = true)
        {
            if (stopMapInteractionPanel)
                stopMapInteractionPanel.SetActive(true);

            EventHolder.OnMapSmoothMoveStart?.Invoke();

            //Invoke(nameof(HideStopMapInteractionPanel), settings.smoothTime);

            // from current map position
            fromPosition = OnlineMaps.instance.position;

            // calculates tile positions
            moveZoom = OnlineMaps.instance.floatZoom;

            if (MathUtils.SimilarFloats(moveZoom, zoomValue) && MathUtils.V2Equal(fromPosition, toPosition))
            {
                if (settings.EditorDebug) Debug.Log("No need to move zoom");

                isZooming = false;
                isMovement = false;
                HideStopMapInteractionPanel();
                return;
            }

            OnlineMaps.instance.projection.CoordinatesToTile(fromPosition.x, fromPosition.y, Mathf.RoundToInt(moveZoom), out fromTileX, out fromTileY);
            OnlineMaps.instance.projection.CoordinatesToTile(toPosition.x, toPosition.y, Mathf.RoundToInt(moveZoom), out toTileX, out toTileY);

            double dist = OnlineMapsUtils.Magnitude(fromTileX, fromTileY, toTileX, toTileY);

           // Debug.LogWarning(dist);

            //Prepare smooth zoom
            isZooming = settings.smoothType == EnumsHolder.SmoothType.MoveFirst ? false : true;
            timeElapsed = 0f;
            startZoomValue = OnlineMaps.instance.floatZoom;
            endZoomValue = zoomValue; /*markerEngineManager.GetBestZoomForPos(toPosition);*/

            if (checkDist)
            {
                // if tile offset < 4, then start smooth movement
                if (dist < 10)
                {
                    // set relative position 0
                    angle = 0;

                    // start movement
                    isMovement = settings.smoothType == EnumsHolder.SmoothType.ZoomFirst ? false : true;
                }
                else // too far
                {
                    OnlineMaps.instance.position = toPosition;
                }
            }
            else
            {
                // set relative position 0
                angle = 0;

                // start movement
                isMovement = settings.smoothType == EnumsHolder.SmoothType.ZoomFirst ? false : true;
            }
        }

        void HideStopMapInteractionPanel() 
        {
            if (stopMapInteractionPanel)
            {
                if (!stopMapInteractionPanel.activeSelf) return;
                stopMapInteractionPanel.SetActive(false);
            }

            EventHolder.OnMapSmoothMoveStop?.Invoke();
        }

        private void Update()
        {
            if (isZooming)
            {
                if (timeElapsed < settings.smoothTime)
                {
                    OnlineMaps.instance.floatZoom = Mathf.Lerp(startZoomValue, endZoomValue, timeElapsed / settings.smoothTime);
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    //Debug.LogWarning("ZOOM STOPPED");
                    isZooming = false;
                    if (settings.smoothType == EnumsHolder.SmoothType.ZoomFirst)
                    {
                        isMovement = true;
                    }
                    else
                    {
                        //if (settings.smoothType == EnumsHolder.SmoothType.MoveAndZoom)
                        //{
                        HideStopMapInteractionPanel();
                        //}
                    }
                }
            }
            // if not movement then return
            if (!isMovement) return;

            // update relative position
            angle += Time.deltaTime / settings.smoothTime;

            if (angle > 1)
            {
                //Debug.LogWarning("MOVE STOPPED");
                // stop movement
                isMovement = false;
                if (settings.smoothType == EnumsHolder.SmoothType.MoveFirst)
                {
                    isZooming = true;
                }
                else
                {
                    HideStopMapInteractionPanel();
                }
                angle = 1;
            }

            // Set new position
            double px = (toTileX - fromTileX) * angle + fromTileX;
            double py = (toTileY - fromTileY) * angle + fromTileY;
            OnlineMaps.instance.projection.TileToCoordinates(px, py, Mathf.RoundToInt(moveZoom), out px, out py);
            OnlineMaps.instance.SetPosition(px, py);
        }

#endregion

        //we use this because creating areas has a delay and zoom fails at start
        //bool isFirstTimePassed;

        [ContextMenu("Zoom On Areas")]
        public void ZoomOnAreas()
        {
            //Debug.Log("ZoomOnAreas " + isFirstTimePassed);
            if (!settings.areaMarker.createNativeMarkers)
            {
                if (settings.useSmoothnessForAllAreasView)
                {
                    markerEngineManager.GetBestPosAndZoomForGUIMarkers(markerEngineManager.uGUIMarkerEngine.areaGUIMarkers, out Vector2 center, out int zoom);

                    if (settings.initialArgs.useCustomAreasCenter)
                        center = settings.initialArgs.customAreasCenter;

#if UNITY_EDITOR
                    if (settings.drawCircleAtCenterOfAreasZoom)
                    {
                        //create test circle at center position
                        DrawCircleAtPos(center);
                    }
#endif
                    //zoom--;
                    SmoothMoveMap(center, settings.zoomAreaManual);
                    //SmoothMoveMap(center, settings.initialArgs.useCustomAreasCenter ? settings.initialArgs.GetAreasPrefferedZoom() : zoom);
                }
                else
                {
                    markerEngineManager.CenterMapOnGUIMarkers(markerEngineManager.uGUIMarkerEngine.areaGUIMarkers);
                    //isFirstTimePassed = true;
                }
            }
            else
            {
                if (settings.areaMarker.createNativeMarkers) markerEngineManager.nativeMarkerEngine.ShowAreaMarkers(true);
                if (settings.useSmoothnessForAllAreasView /*&& isFirstTimePassed*/)
                {
                    markerEngineManager.GetBestPosAndZoomForNativeMarkers(markerEngineManager.nativeMarkerEngine.areaNativeMarkers, out Vector2 center, out int zoom);

                    if (settings.initialArgs.useCustomAreasCenter)
                        center = settings.initialArgs.customAreasCenter;

                    zoom += 1;
                    //SmoothMoveMap(center, settings.initialArgs.useCustomAreasCenter ? settings.initialArgs.GetAreasPrefferedZoom() : zoom);

                    SmoothMoveMap(center, settings.zoomAreaManual);
                }
                else
                {
                    markerEngineManager.CenterMapOnNativeMarkers(markerEngineManager.nativeMarkerEngine.areaNativeMarkers);
                    //isFirstTimePassed = true;
                }
            }

            //native markers
            //HideAllMarkers();
            //markerEngineManager.areaNativeMarkers.ForEach(b => b.enabled = true);
        }

        [ContextMenu("Zoom On Greece")]
        public void ZoomOnGreece()
        {
            // Initializes the position and zoom
            OnlineMaps.instance.floatZoom = settings.initialArgs.GetAreasPrefferedZoom();
            OnlineMaps.instance.position = settings.initialArgs.useCustomAreasCenter ? settings.initialArgs.customAreasCenter : settings.initialArgs.greeceMapPos;
        }

        [ContextMenu("INIT MAP")]
        public void InitMap()
        {
            // Lock map zoom range
            OnlineMaps.instance.zoomRange = new OnlineMapsRange(6, settings.initialArgs.zoomGreece);

            // Lock map coordinates range
            OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(35, 20, 41, 28);

            //ResetMap();

            //OnlineMapsControlBase.instance.OnMapDrag += DragMapStarted;

            // TryLoadMarkers();

        }


#region EDITOR ONLY

        void DrawCircleAtPos(Vector2 pos)
        {

#if UNITY_EDITOR

            /// <summary>
            /// Radius of the circle
            /// </summary>
            float radiusKM = 0.1f;

            /// <summary>
            /// Number of segments
            /// </summary>
            int segments = 32;

            OnlineMapsDrawingElementManager.RemoveAllItems();


            // Create a new marker under cursor
            OnlineMapsMarkerManager.CreateItem(pos.x, pos.y, "Marker " + OnlineMapsMarkerManager.CountItems);

            OnlineMaps map = OnlineMaps.instance;

            // Get the coordinate at the desired distance
            double nlng, nlat;
            OnlineMapsUtils.GetCoordinateInDistance(pos.x, pos.y, radiusKM, 90, out nlng, out nlat);

            double tx1, ty1, tx2, ty2;

            // Convert the coordinate under cursor to tile position
            map.projection.CoordinatesToTile(pos.x, pos.y, 20, out tx1, out ty1);

            // Convert remote coordinate to tile position
            map.projection.CoordinatesToTile(nlng, nlat, 20, out tx2, out ty2);

            // Calculate radius in tiles
            double r = tx2 - tx1;

            // Create a new array for points
            OnlineMapsVector2d[] points = new OnlineMapsVector2d[segments];

            // Calculate a step
            double step = 360d / segments;

            // Calculate each point of circle
            for (int i = 0; i < segments; i++)
            {
                double px = tx1 + System.Math.Cos(step * i * OnlineMapsUtils.Deg2Rad) * r;
                double py = ty1 + System.Math.Sin(step * i * OnlineMapsUtils.Deg2Rad) * r;
                map.projection.TileToCoordinates(px, py, 20, out double lng, out double lat);
                points[i] = new OnlineMapsVector2d(lng, lat);
            }

            // Create a new polygon to draw a circle
            OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingPoly(points, Color.red, 3));

#endif

        }


#endregion


    }

}
