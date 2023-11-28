//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class OrderRouteControllerGUI : Singleton<OrderRouteControllerGUI>
    {
        protected OrderRouteControllerGUI() { }

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
        private MarkerInstance currGUIPoiInOrderView;
        public MarkerInstance MarkerInOrderNow() { return currGUIPoiInOrderView; }

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

        private int NextOrderID()
        {
            return currGUIPoiInOrderView.data.poiEntity.OrderID() + 1;
        }

        /// <summary>
        /// avoid clicking other poi
        /// allow only the one in current order
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool IsInfoOpenedForCurrentPoiInView(MarkerInstance m)
        {
            //user is just clicked on poi
            //Debug.LogWarning("POI CLICKED");

            if (IsPoiOrderedTour)
            {
                //check allow click if not ordered poi
                if (m != currGUIPoiInOrderView)
                {
                    //allow only next one
                    //if (m.data.poiEntity.OrderID() == NextOrderID())
                    //{
                        //and only if scale is 1
                        //which means only after current order poi
                        //has been clicked at least once
                        if(m.data.GetScale() == Vector3.one)
                            return true;
                    //}

                    return false;
                }
                else//if clicked poi is current ordered poi
                {
                    //if its not the last one in order
                    if (m.data.poiEntity.OrderID() != POI_ORDER_MAX)
                    {
                        //scale up next ordered poi
                        //to indicate that it can be clicked
                        MarkerInstance mNext = GetGUIPoiWithOrderID(NextOrderID());
                        if (mNext != null)
                            mNext.data.SetScale(Vector3.one);

                        ShowNextPoiInOrder();
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
                //if (IsPoiOrderedTour)
                //{
                //    bool isPoiMode = appState == EnumsHolder.AppState.PoiSelected;
                //    if (!isPoiMode) return;
                //    ShowNextPoiInOrder();
                //}
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

            PoiEntity poiEntity = markerEngine.uGUIMarkerEngine.poiGUIMarkers[0].data.poiEntity;
            if (poiEntity != null)
            {
                poiIcon = poiEntity.poiIcon;
                poiIconActive = poiEntity.poiIconActive;
                poiIconVisited = poiEntity.poiIconVisited;
            }

            foreach (MarkerInstance m in markerEngine.uGUIMarkerEngine.poiGUIMarkers)
            {
                int d = m.data.poiEntity.OrderID();

                //int mIndx = d - 1;
                m.data.SetTexture(poiIcon);// (iconOrders[mIndx]);
                m.data.SetOrderText(d);

                if (d > POI_ORDER_MAX) POI_ORDER_MAX = d;
            }
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
                    return;
                }
            }

            MarkerInstance mdraft = GetGUIPoiWithOrderID(POI_ORDER_ID);

            if (mdraft == null)
            {
                Debug.Log("Ordered POI is NULL");
                return;
            }

            currGUIPoiInOrderView = mdraft;

            foreach (MarkerInstance m in markerEngine.uGUIMarkerEngine.poiGUIMarkers)
            {
                int orderid = m.data.poiEntity.OrderID();

                if (orderid == POI_ORDER_ID /*|| orderid == NextOrderID()*/)
                {
                    m.data.SetScale(Vector3.one);// markerEngine.PoiSettings().GetNativeMarkerScale();// 1.2f;
                    m.data.SetTexture(poiIcon);
                }
                else if (orderid < POI_ORDER_ID)
                {
                    m.data.SetScale(Vector3.one);// * 0.5f;
                    m.data.SetTexture(poiIconVisited);
                }
                else
                {
                    m.data.SetScale(Vector3.one * 0.5f);
                }
            }



        }

        private MarkerInstance GetGUIPoiWithOrderID(int id)
        {
            return markerEngine.uGUIMarkerEngine.poiGUIMarkers.Find(b => b.data.poiEntity.OrderID() == id);
        }


    }

}


