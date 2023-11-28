//Diadrasis ©2023 - Stathis Georgiou
using UnityEngine;

namespace Diadrasis.Rethymno
{
    public class AppStateEnabler : MonoBehaviour
	{
        [System.Flags]
        public enum AppModeSelection { GUIDE = 0, AR = 1 }
        [Header("Select which mode allow object to be active")]
        public AppModeSelection appModeSelection;
        public EnumsHolder.ApplicationMode appModeNow;

        [System.Flags]
        public enum AppStateSelection { None = 0, AreasView = 1, RoutesView = 1 << 1, PeriodsView = 1 << 2, PoisView = 1 << 3, PoiSelected = 1 << 4 }
        [Header("Select which states allow object to be active")]
        public AppStateSelection appStateSelection;
        public EnumsHolder.AppState appStateNow;

        [System.Flags]
        public enum TourModeSelection { None = 0, OffSite = 1, OnSite = 1 << 1 }
        [Header("Select which modes allow object to be active")]
        public TourModeSelection tourModeSelection;//OffSite, OnSite
        public EnumsHolder.TourMode tourModeNow = EnumsHolder.TourMode.OffSite;

        [System.Flags]
        public enum RouteTypeSelection { None = 0, InfoMonuments = 1, VisitMuseum = 1 << 1, FollowNarrator = 1 << 2, StoriesOfDecayAndConservation = 1 << 3, AncientGrafiti = 1 << 4 }
        [Header("Select which modes allow object to be active")]
        public RouteTypeSelection routeTypeSelection;
        public EnumsHolder.RouteType routeTypeNow = EnumsHolder.RouteType.None;

        void Start()
		{
            AddListeners();
        }

        void AddListeners()
        {
            EventHolder.OnStateChanged += OnStateChanged;
            EventHolder.OnTourChanged += OnTourChanged;
            EventHolder.OnRouteTypeChanged += OnRouteTypeChanged;
            EventHolder.OnApplicationModeChanged += OnApplicationModeChanged;
        }

        void OnTourChanged(EnumsHolder.TourMode mode)
        {
            tourModeNow = mode;
            CheckStates();
        }

        void OnStateChanged(EnumsHolder.AppState state)
        {
            appStateNow = state;
            CheckStates();
        }

        void OnApplicationModeChanged(EnumsHolder.ApplicationMode mode)
        {
            appModeNow = mode;
            CheckStates();
        }

        void OnRouteTypeChanged(EnumsHolder.RouteType type)
        {
            routeTypeNow = type;
            CheckStates();
        }

        void CheckStates()
        {
            bool isOnAppMode = appModeSelection.ToString().Contains(appModeNow.ToString()) || ((int)appModeSelection) < 0;//all

            bool IsOnAppState = appStateSelection.ToString().Contains(appStateNow.ToString()) || ((int)appStateSelection) < 0;//all

            bool IsOnTourMode = tourModeSelection.ToString().Contains(tourModeNow.ToString()) || ((int)tourModeSelection) < 0;//all

            bool IsOnRouteType = routeTypeSelection.ToString().Contains(routeTypeNow.ToString()) || ((int)routeTypeSelection) < 0;//all

            gameObject.SetActive(isOnAppMode && IsOnAppState && IsOnTourMode && IsOnRouteType);
        }
    }
}
