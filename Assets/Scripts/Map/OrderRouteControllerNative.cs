//Diadrasis �2023
using UnityEngine;

namespace Diadrasis.Rethymno
{

    public class OrderRouteControllerNative : Singleton<OrderRouteControllerNative>
    {
        protected OrderRouteControllerNative() { }

        public EnumsHolder.AppState appState;
        public EnumsHolder.TourMode tourMode;
        [Space]
        public EnumsHolder.InfoPanelState infoPanelState = EnumsHolder.InfoPanelState.Closed;

        [Space]
        public MarkerEngineManager markerEngine;

        [SerializeField]
        private int POI_ORDER_ID = 0;
        [SerializeField]
        private int POI_ORDER_MAX;
        [SerializeField]
        private bool IsPoiOrderedTour, IsOrderRouteInfoClosed;

        public bool IsOrderRouteActive() { return IsPoiOrderedTour; }
        //[SerializeField]
        private OnlineMapsMarker currNativePoiInOrderView;
        public OnlineMapsMarker MarkerInOrderNow() { return currNativePoiInOrderView; }

        [Space]
        public Texture2D iconInactive;
        public Texture2D poiIcon, poiIconActive, poiIconVisited;

        [Space]
        public Texture2D[] iconOrders;

        void Awake()
        {
            EventHolder.OnTourChanged += OnTourChanged;
            //EventHolder.OnGpsLocationChanged += OnGpsLocationChanged;
            EventHolder.OnStateChanged += OnStateChanged;

            EventHolder.OnInfoState += OnInfoActive;

            if (markerEngine == null) markerEngine = FindObjectOfType<MarkerEngineManager>();
        }

        public void InitializeRoute(bool isOrdered)
        {
            if (!markerEngine) return;
            IsPoiOrderedTour = isOrdered;
            if (!IsPoiOrderedTour) return;

            SetMaxOrder();
            ShowNextPoiInOrder();
        }

        /// <summary>
        /// avoid clicking other poi
        /// allow only the one in current order
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool IsInfoOpenedForCurrentPoiInView(OnlineMapsMarkerBase m)
        {
            //user is just clicked on poi
            //Debug.LogWarning("POI CLICKED");

            if (IsPoiOrderedTour)
            {
                int orderid = markerEngine.nativeMarkerEngine.GetPoiOrderID(m);

                //check allow click if not ordered poi
                if (m != currNativePoiInOrderView)
                {
                    //allow only next one
                    if (orderid == NextOrderID())
                    {
                        //and only if scale is 1
                        //which means only after current order poi
                        //has been clicked at least once
                        if (m.scale == markerEngine.PoiSettings().GetNativeMarkerScale())
                            return true;
                    }

                    return false;
                }
                else//if clicked poi is current ordered poi
                {
                    //if its not the last one in order
                    if (orderid != POI_ORDER_MAX)
                    {
                        //scale up next ordered poi
                        //to indicate that it can be clicked
                        OnlineMapsMarkerBase mNext = GetNativePoiWithOrderID(NextOrderID());
                        if (mNext != null)
                            mNext.scale = markerEngine.PoiSettings().GetNativeMarkerScale();

                        //ShowNextPoiInOrder();
                    }
                }
            }

            //if it is not an ordered route then allow click to continue execution
            return true;
        }

        void OnInfoActive(EnumsHolder.InfoPanelState val)
        {
            bool isActuallyClosing = infoPanelState == EnumsHolder.InfoPanelState.HalfOpen && val == EnumsHolder.InfoPanelState.Closed;

            infoPanelState = val;

            if (isActuallyClosing)
            {
                //info is about to close
                //do something
                if (!IsOrderRouteInfoClosed)
                {
                    IsOrderRouteInfoClosed = true;
                    return;
                }

                //check ordered pois
                if (IsPoiOrderedTour)
                {
                    bool isPoiMode = appState == EnumsHolder.AppState.PoiSelected;
                    if (!isPoiMode) return;
                    ShowNextPoiInOrder();
                }
            }
        }

        void OnStateChanged(EnumsHolder.AppState state)
        {
            appState = state;

            bool isPoiMode = appState == EnumsHolder.AppState.PoisView || appState == EnumsHolder.AppState.PoiSelected;

            if (!isPoiMode)
            {
                ResetTour();
            }
        }

        void OnTourChanged(EnumsHolder.TourMode mode)
        {
            tourMode = mode;
        }

        void SetMaxOrder()
        {
            POI_ORDER_MAX = 0;

            string id = markerEngine.nativeMarkerEngine.GetDataID(markerEngine.nativeMarkerEngine.poiNativeMarkers[0]);
            PoiEntity poiEntity = DataManager.Instance.GetPoiEntity(id);
            if (poiEntity != null)
            {
                poiIcon = poiEntity.poiIcon;
                poiIconActive = poiEntity.poiIconActive;
                poiIconVisited = poiEntity.poiIconVisited;
            }

            foreach (OnlineMapsMarker m in markerEngine.nativeMarkerEngine.poiNativeMarkers)
            {
                int d = markerEngine.nativeMarkerEngine.GetPoiOrderID(m);

                int mIndx = d - 1;
                m.texture = iconOrders[mIndx];
                m.Init();

                if (d > POI_ORDER_MAX) POI_ORDER_MAX = d;
            }

            OnlineMaps.instance.Redraw();
        }

        public void ResetTour()
        {
            IsPoiOrderedTour = false;
            POI_ORDER_ID = 0;
            IsOrderRouteInfoClosed = false;
        }

        void RestartOrderTour()
        {
            SetMaxOrder();
            ShowNextPoiInOrder();
        }

        private void ShowNextPoiInOrder()
        {
            if (POI_ORDER_ID <= 0)
            {
                POI_ORDER_ID = 1;
            }
            else
            {
                POI_ORDER_ID = NextOrderID();

                if (POI_ORDER_ID > POI_ORDER_MAX)
                {
                    POI_ORDER_ID = 0;
                    RestartOrderTour();
                }
            }

            OnlineMapsMarker mdraft = GetNativePoiWithOrderID(POI_ORDER_ID);

            if (mdraft == null)
            {
                Debug.Log("Ordered POI is NULL");
                return;
            }

            currNativePoiInOrderView = mdraft;

            foreach (OnlineMapsMarker m in markerEngine.nativeMarkerEngine.poiNativeMarkers)
            {
                int orderid = markerEngine.nativeMarkerEngine.GetPoiOrderID(m);

                if (orderid == POI_ORDER_ID)
                {
                    m.scale = markerEngine.PoiSettings().GetNativeMarkerScale();// 1.2f;
                    m.texture = poiIcon;
                    m.Init();
                }
                else if (orderid < POI_ORDER_ID)
                {
                    m.scale = markerEngine.PoiSettings().GetNativeMarkerScale();// * 0.5f;
                    m.texture = poiIconVisited;
                    m.Init();
                }
                else
                {
                    m.scale = markerEngine.PoiSettings().GetNativeMarkerScale() * 0.5f;
                }
            }

            OnlineMaps.instance.Redraw();
        }

        private OnlineMapsMarker GetNativePoiWithOrderID(int id)
        {
            foreach (OnlineMapsMarker m in markerEngine.nativeMarkerEngine.poiNativeMarkers)
            {
                int orderid = markerEngine.nativeMarkerEngine.GetPoiOrderID(m);
                if (orderid == id) return m;
            }

            return null;
        }

        private int NextOrderID()
        {
            int orderid = markerEngine.nativeMarkerEngine.GetPoiOrderID(currNativePoiInOrderView) + 1;
            return orderid;
        }



    }

}
