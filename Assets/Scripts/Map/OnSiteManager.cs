//Diadrasis ©2023
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Diadrasis.Rethymno
{

    public class OnSiteManager : Singleton<OnSiteManager>
    {
        protected OnSiteManager() { }

        public EnumsHolder.AppState appState = EnumsHolder.AppState.None;
        [Space]
        public GpsSettings settings;
        [Space]
        public EnumsHolder.GpsStatus gpsStatus = EnumsHolder.GpsStatus.OFF;
        public EnumsHolder.LocationMode locationMode = EnumsHolder.LocationMode.NULL;
        public EnumsHolder.TourMode tourMode = EnumsHolder.TourMode.OffSite;
        public EnumsHolder.TriggerMode triggerMode = EnumsHolder.TriggerMode.GPS;
        [Space]
        public bool createUserMarkerNative;
        public Vector2 userPosition;
        public OnlineMapsMarker userMarker;
        public MarkerInstance userMarkerGui;
        public GameObject _markerObject;

        private MapController mapController;
        private OnlineMapsLocationService gps;
        private AreaEntity areaNow;
        public AreaEntity areaNear;

        [Space]
        [SerializeField]
        private List<AreaEntity> areaEntities = new List<AreaEntity>();
       
        [Space]
        public int circlePoints = 32;
        public Color circleborderColor, circleborderColorSatellite;
        private bool drawCircleAroundUser;
        private OnlineMapsDrawingElement circle;

        [Space]
        [SerializeField]
        private int faultFlags = 0;

        [Space]
        public UnityEvent EventPoiOnSite;
        public UnityEvent EventPoiOffSite;

        private float _CurrentTriggerDistanceForPois;
        public float CurrentTriggerDistanceForPois 
        { 
            get { return _CurrentTriggerDistanceForPois == 0 ? settings.triggerStartDistanceForPois : _CurrentTriggerDistanceForPois; }
            set { _CurrentTriggerDistanceForPois = value; }
        }

        public bool IsBeaconActive
        {
            get { return tourMode == EnumsHolder.TourMode.OnSite && triggerMode == EnumsHolder.TriggerMode.BEACON; }
        }

        public void ToggleUserCircle()
        {            
            drawCircleAroundUser = !drawCircleAroundUser;

            if (!drawCircleAroundUser)
            {
                if (circle != null)
                    OnlineMapsDrawingElementManager.RemoveItem(circle);
            }
            else
            {
                DrawCirclePoiDist();
            }
        }

        void OnPoiDistanceChanged(float distance)
        {
            DrawCirclePoiDist();
        }

        [Space]
        public TMPro.TextMeshProUGUI txt;

        private void Awake()
        {
            EventHolder.OnPoiDistanceChanged += OnPoiDistanceChanged;
            EventHolder.OnStateChanged += OnStateChanged;
            EventHolder.OnAreasView += OnAreasView;
            EventHolder.OnAreaInfoShow += GetSelectedArea;
            EventHolder.OnTourChanged += OnTourChanged;
            EventHolder.OnMapLayerChanged += OnMapLayerChanged;

#if !UNITY_EDITOR
            settings.EditorDebug = false;
            txt.gameObject.SetActive(false);
#endif
        }

        void Start()
        {
            areaEntities = DataManager.Instance.AreaEntities();

            mapController = FindObjectOfType<MapController>();
            gps = OnlineMapsLocationService.instance;

            if (DataManager.Instance.IsMobile())
            {

                // Subscribe to compass event
                gps.OnCompassChanged += OnCompassChanged;

                gps.updateDistance = settings.gpsUpdateDistance;
                gps.TryStartLocationService();

                gps.OnLocationInited += OnLocationInited;
                gps.OnLocationChanged += OnGpsLocationChanged;
                //gps.OnFindLocationByIPComplete += OnFindLocationByIPComplete;

                gps.disableEmulatorInPublish = true;

                if (createUserMarkerNative)
                {
                    userMarker = mapController.CreateUserMarker();

                    userMarker.align = OnlineMapsAlign.Center;
                    userMarker.scale = 1f;
                }
                else
                {
                    userMarkerGui = mapController.CreateGuiUserMarker();
                    _markerObject = userMarkerGui.gameObject;

                }

                //if(IsGpsON()) { SearchNearestArea(); }

                StartCoroutine(CheckGpsStatusLoop());

            }
            else
            {
                gps.enabled = false;
            }
        }

        void OnTourChanged(EnumsHolder.TourMode mode)
        {
            tourMode = mode;

            DrawCirclePoiDist();

            if (createUserMarkerNative)
            {
                userMarker.enabled = tourMode == EnumsHolder.TourMode.OnSite && !mapController.RouteIsMuseum();
                if(userMarker.enabled == false) DeleteCircle();
                OnlineMaps.instance.Redraw();
            }
            else
            {
                userMarkerGui.data.ManualHide(tourMode != EnumsHolder.TourMode.OnSite || mapController.RouteIsMuseum());
                _markerObject.SetActive(tourMode == EnumsHolder.TourMode.OnSite && !mapController.RouteIsMuseum());
                if (_markerObject.activeSelf == false) DeleteCircle();
            }

            if (appState == EnumsHolder.AppState.AreasView)
            {
                switch (tourMode)
                {
                    case EnumsHolder.TourMode.OffSite:
                    default:
                        EventHolder.OnAreasView?.Invoke();
                        break;
                    case EnumsHolder.TourMode.OnSite:
                        if (areaNear != null)
                            EventHolder.OnRoutesView?.Invoke(areaNear);
                        break;

                }
            }
        }

        void OnTriggerModeChanged(EnumsHolder.TriggerMode mode)
        {
            triggerMode = mode;
        }

        public void CheckUserMarkerVisibility()
        {
            if (createUserMarkerNative)
            {
                userMarker.enabled = tourMode == EnumsHolder.TourMode.OnSite && !mapController.RouteIsMuseum();
                DrawCirclePoiDist(); 
                OnlineMaps.instance.Redraw();
            }
            else
            {
                userMarkerGui.data.ManualHide(tourMode != EnumsHolder.TourMode.OnSite || mapController.RouteIsMuseum());
                _markerObject.SetActive(tourMode == EnumsHolder.TourMode.OnSite && !mapController.RouteIsMuseum());
                DrawCirclePoiDist(); 
            }
        }

        public bool IsGpsDisabled()
        {
            if (Application.isEditor)
            {
                return !gps.useGPSEmulator;
            }
            else
            {
                return Input.location.isEnabledByUser == false;
            }
        }

        public bool IsGpsEnabled()
        {
            if (Application.isEditor)
            {
                return gps.useGPSEmulator;
            }
            else
            {
                return /*Input.location.isEnabledByUser && */Input.location.status == LocationServiceStatus.Running;
            }
        }

        public bool IsGpsInitializing()
        {
            if (Application.isEditor)
            {
                return !gps.useGPSEmulator;
            }
            else
            {
                return Input.location.status == LocationServiceStatus.Initializing;
            }
        }

        public bool IsGpsRunning()
        {
            if (Application.isEditor)
            {
                return gps.useGPSEmulator;
            }
            else
            {
                return Input.location.status == LocationServiceStatus.Running;
            }
        }
        private IEnumerator IsCompassAvailable()
        {
            bool compass = false;

            for (int i = 0; i < 10; i++)
            {
                if (Input.compass.trueHeading != 0)
                {
                    compass = true;
                }

                yield return new WaitForSeconds(0.05f);
            }

            yield return compass;
        }

        public void MoveMapToUser()
        {
           // OnlineMaps.instance.position = createUserMarkerNative ? userMarker.position : userMarkerGui.data.pos;

            //GET BEST POS AND ZOOM ################################################
            Vector2 pos = Vector2.zero;
            if (createUserMarkerNative) { pos = userMarker.position; }
            else {  pos = userMarkerGui.data.pos; }

            //#######################################################################

            mapController.SmoothMoveMap(pos, 19.7f, false);
        }

        /// <summary>
        /// This method is called when the compass value is changed.
        /// </summary>
        /// <param name="f">New compass value (0-1)</param>
        private void OnCompassChanged(float f)
        {

            if (createUserMarkerNative)
            {

            }
            else
            {
                Vector3 rot = userMarkerGui.transform.localEulerAngles;
                rot.z = -1f * f * 360;
                userMarkerGui.transform.localEulerAngles = rot;
            }
        }

        

        private IEnumerator CheckGpsStatusLoop()
        {
            while (true)
            {
                //txt.text = string.Empty;

                //yield return new WaitForSeconds(0.3f);

                //txt.text = Input.location.status.ToString();
                //txt.text += " " + Input.location.isEnabledByUser;

                if (Application.isEditor)
                {

                    if (gps.useGPSEmulator)
                    {
                        // DebugMsg("GPS ON");
                        EventHolder.OnGpsOn?.Invoke();
                        gpsStatus = EnumsHolder.GpsStatus.ON;
                    }
                    else
                    {
                        // DebugMsg("GPS OFF");
                        EventHolder.OnGpsOff?.Invoke();
                        gpsStatus = EnumsHolder.GpsStatus.OFF;
                    }
                }
                else
                {
                    if (IsGpsEnabled())//allowed
                    {
                        if (Input.location.status != LocationServiceStatus.Initializing)//is initialized
                        {
                            if (Input.location.status == LocationServiceStatus.Running)//working
                            {
                                // DebugMsg("GPS ON");
                                EventHolder.OnGpsOn?.Invoke();
                                gpsStatus = EnumsHolder.GpsStatus.ON;
                            }
                            else //failed
                            {
                                // DebugMsg("GPS OFF");
                                EventHolder.OnGpsOff?.Invoke();
                                gpsStatus = EnumsHolder.GpsStatus.OFF;
                            }
                        }
                    }
                    else
                    {
                        // DebugMsg("GPS OFF");
                        EventHolder.OnGpsOff?.Invoke();
                        gpsStatus = EnumsHolder.GpsStatus.OFF;
                    }
                }

                //if (userMarker != null) userMarker.enabled = IsGpsON() && tourMode == EnumsHolder.TourMode.OnSite;
                //if (userMarkerGui != null) userMarkerGui.gameObject.SetActive(IsGpsON() && tourMode == EnumsHolder.TourMode.OnSite);

                yield return new WaitForSeconds(settings.gpsCheckTimeInterval);
            }
        }

        

        void OnStateChanged(EnumsHolder.AppState state) 
        {
            appState = state;

            //hide for other app states
            EventPoiOffSite?.Invoke();

            DrawCirclePoiDist();

            switch (appState)
            {
                default:
                    if (tourMode == EnumsHolder.TourMode.OnSite)
                    {
                        Invoke(nameof(CheckUserMarkerVisibility), 0.5f);
                    }
                    break;
                case EnumsHolder.AppState.PoisView:
                case EnumsHolder.AppState.PoiSelected:
                    if (tourMode == EnumsHolder.TourMode.OnSite)
                    {
                        EventPoiOnSite?.Invoke();
                        Invoke(nameof(CheckUserMarkerVisibility), 0.5f);
                    }
                    else
                    {
                        EventPoiOffSite?.Invoke();
                    }
                    break;
            }
        }

        void GetSelectedArea(AreaEntity area) { areaNow = area; }

        void OnAreasView()
        {
            areaNow = null;
            //if (IsGpsON()) { }
        }

        void OnLocationInited()
        {
            //userPosition = gps.position;
            OnGpsLocationChanged(gps.position);
        }

        //void OnFindLocationByIPComplete()
        //{
        //	userPosition = gps.position;
        //}

        /// <summary>
        /// gps.distance meters
        /// gps.speed km/h
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="speed"></param>
        /// <returns>True if we have extreme movement</returns>
        private bool IsNewGpsPositionFault(double dist, float speed)
        {
            //preferred walking speed 
            //1.42 m/s or 5.1 km/h
            float currSpeed = speed / 1000f;

            DebugMsg("speed = " + currSpeed + "m/s");

            float gpsUpdateDistance = gps.updateDistance;

            float currDist = (float)dist * 1000f;

            DebugMsg("dist = " + currDist + " meters");

            if (currSpeed > 2f || currDist > gpsUpdateDistance)
            {
                faultFlags++;
                if (faultFlags <= 3)
                {
                    return true;
                }
                else
                {
                    faultFlags = 0;
                }
            }
            if (faultFlags > 0)
            {
                faultFlags--;
                return false;
            }

            return false;
        }

        bool androidCheckLocationPermission;

        Vector2 draftPos;

        void OnGpsLocationChanged(Vector2 pos)
        {
            DebugMsg("Location Changed: " + pos);

            //DeleteCircle();

            //if (tourMode == EnumsHolder.TourMode.OffSite) return;

            draftPos = pos;

            //if (appState == EnumsHolder.AppState.PoisView || appState == EnumsHolder.AppState.PoiSelected)
            //{
            //    //DrawCirclePoiDist();
            //    if (settings.useFilterOnGpsUpdateDistance)
            //    {
            //        if (IsNewGpsPositionFault(gps.distance, gps.speed))
            //        {
            //            DebugMsg("FAULT GPS value");
            //            return;
            //        }
            //    }
            //}

            if (triggerMode == EnumsHolder.TriggerMode.GPS)
            {
                EventHolder.OnGpsLocationChanged?.Invoke(pos);
            }

            userPosition = pos;

            if (createUserMarkerNative)
            {
                userMarker.position = pos;
            }
            else
            {
                if (userMarkerGui != null) userMarkerGui.data.SetPosition(userPosition);
            }

            switch (appState)
            {
                case EnumsHolder.AppState.AreasView:
                case EnumsHolder.AppState.None:
                    SearchNearestArea();
                    break;
                case EnumsHolder.AppState.PoisView:
                    SearchNearestPOI();
                    DrawCirclePoiDist();
                    SearchNearestArea();
                    break;
                case EnumsHolder.AppState.PoiSelected:
                    DrawCirclePoiDist();
                    SearchNearestArea();
                    break;
                default:
                    break;
            }

        }

        public void SearchNearestArea()
        {
            DebugMsg("<color=orange>Search Nearest AREA</color>"); 

            if (IsGpsEnabled() == false) return;

            float dist = Mathf.Infinity;
            float areaDist = Mathf.Infinity;
            int areaID = 0;
            string areaName = "";

            foreach (AreaEntity _area in areaEntities)
            {
                areaDist = GetDistanceBetweenPoints(userPosition, _area.area.geoPosition);
                if (areaDist < dist)
                {
                    dist = areaDist;
                    areaID = _area.ID;
                    areaName = DataManager.Instance.GetTraslatedText(_area.area.area_title);
                    areaNear = _area;
                }
            }

           // Debug.Log(dist);

            //if far
            if (dist > settings.minDistanceForOnSiteMode)
            {
                //alert far from areas
                EventHolder.OnGpsFar?.Invoke();

                locationMode = EnumsHolder.LocationMode.FAR;

                EventHolder.OnTourChanged?.Invoke(EnumsHolder.TourMode.OffSite);

                areaNear = null;

                DebugMsg("GPS FAR");
            }
            else
            {
                EventHolder.OnGpsNear?.Invoke(areaID);
                locationMode = EnumsHolder.LocationMode.NEAR_AREA;
                DebugMsg("GPS NEAR to " + areaName);
            }
        }

        bool isPaused = false;
        bool waitForGpsToBeEnabled;
        private void OnApplicationFocus(bool hasFocus)
        {
            isPaused = !hasFocus;

            if (isPaused)
            {
                //wait gps to be enabled?
                if (!IsGpsEnabled()) waitForGpsToBeEnabled = true;
            }
            else
            {
                if (waitForGpsToBeEnabled)
                {
                    if (!IsGpsEnabled()) StartGPS();
                    waitForGpsToBeEnabled = false;
                }
            }
        }

        public void StopGPS()
        {
            //stop high power location attribution
            gps.StopLocationService();
        }

        public void StartGPS()
        {
            if (Input.location.status == LocationServiceStatus.Running) return;
            StopGPS();
            //stop high power location attribution
            gps.TryStartLocationService();
        }

        private void SearchNearestPOI()
        {
            if (Application.isEditor) { Debug.Log("<color=orange>Search Nearest POI</color>"); }
        }

        float GetDistanceBetweenPoints(Vector2 pA, Vector2 pB)
        {
            // Calculate the distance in meters between locations.
            return OnlineMapsUtils.DistanceBetweenPoints(pA, pB).magnitude * 1000f;
        }

        void DrawCirclePoiDist()
        {
            DeleteCircle();

            if (appState != EnumsHolder.AppState.PoisView && appState != EnumsHolder.AppState.PoiSelected) return;
            if (tourMode == EnumsHolder.TourMode.OffSite || mapController.RouteIsMuseum()) return;
            //if (triggerMode == EnumsHolder.TriggerMode.BEACON) return;

            if (userMarkerGui == null) return;

            float radiusKM = CurrentTriggerDistanceForPois / 1000f;

            // Get the coordinates of user
            double lng, lat;
            lng = createUserMarkerNative ? userMarker.position.x : userMarkerGui.data.pos.x;
            lat = createUserMarkerNative ? userMarker.position.y : userMarkerGui.data.pos.y;

            OnlineMaps map = OnlineMaps.instance;

            // Get the coordinate at the desired distance
            double nlng, nlat;
            OnlineMapsUtils.GetCoordinateInDistance(lng, lat, radiusKM, 90, out nlng, out nlat);

            double tx1, ty1, tx2, ty2;

            // Convert the coordinate under cursor to tile position
            map.projection.CoordinatesToTile(lng, lat, 20, out tx1, out ty1);

            // Convert remote coordinate to tile position
            map.projection.CoordinatesToTile(nlng, nlat, 20, out tx2, out ty2);

            // Calculate radius in tiles
            double r = tx2 - tx1;

            int segments = circlePoints;

            // Create a new array for points
            OnlineMapsVector2d[] points = new OnlineMapsVector2d[segments];

            // Calculate a step
            double step = 360d / segments;

            // Calculate each point of circle
            for (int i = 0; i < segments; i++)
            {
                double px = tx1 + Math.Cos(step * i * OnlineMapsUtils.Deg2Rad) * r;
                double py = ty1 + Math.Sin(step * i * OnlineMapsUtils.Deg2Rad) * r;
                map.projection.TileToCoordinates(px, py, 20, out lng, out lat);
                points[i] = new OnlineMapsVector2d(lng, lat);
            }

            Color col = mapController.IsMapTypeCustom() ? circleborderColor : circleborderColorSatellite;

            // Create a new polygon to draw a circle
            circle = OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingPoly(points, col, 3));
            // Add an element to the map.
            OnlineMapsDrawingElementManager.AddItem(circle);
            map.Redraw();

        }

        void DeleteCircle()
        {
            if (circle != null)
                OnlineMapsDrawingElementManager.RemoveItem(circle);
        }

        void OnMapLayerChanged()
        {
            DrawCirclePoiDist();
        }

        void DebugMsg(string msg)
        {
#if UNITY_EDITOR
            if (settings.EditorDebug)
            {
                Debug.Log("<color=orange>[GPS] " + msg + "</color>");
            }
#endif
        }
    }

}
