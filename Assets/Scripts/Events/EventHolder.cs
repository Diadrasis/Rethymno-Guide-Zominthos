//Diadrasis ©2023
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Diadrasis.Rethymno 
{

	public class EventHolder : MonoBehaviour
	{
        [Serializable]
        public class Vector2D : UnityEvent<UnityEngine.Vector2> { }

        public delegate void GuiAction();
		public static GuiAction OnHomeClick, OnReturnClick, OnExitClick, OnSideMenuClick, OnMessageHide;

		public delegate void MapAction();
		public static MapAction OnLayerToggle, OnMapLayerChanged, OnTopographicsToggle;

		public delegate void GpsAction(Vector2 pos);
		public static GpsAction OnGpsLocationChanged;

        public delegate void MarkerAction(MarkerInstance marker);
		public static MarkerAction OnGuiMarkerClick, OnARInfoTriggered;

        public delegate void BeaconAction(PoiEntity poiEntity);
        public static BeaconAction OnBeaconTriggered;

        public delegate void MarkerNativeAction(PoiEntity poiEntity);
		public static MarkerNativeAction OnNativePoiMarkerClick;

		public delegate void TourModeAction(EnumsHolder.TourMode tourMode);
		public static TourModeAction OnTourChanged;

        public delegate void UserAction(bool val);
		public static UserAction OnPoisShow, OnMenuOpen, OnMessageShow, OnMRActivate, OnARActivate;

		public delegate void InfoPanelAction(EnumsHolder.InfoPanelState panelState);
		public static InfoPanelAction OnInfoState;

		public delegate void AppStateAction(EnumsHolder.AppState appState);
		public static AppStateAction OnStateChanged;

		public delegate void RouteTypeAction(EnumsHolder.RouteType type);
		public static RouteTypeAction OnRouteTypeChanged;

		public delegate void AreaAction(AreaEntity areaEntity);
		public static AreaAction OnRoutesView, OnPeriodsView, OnAreaInfoShow, OnNativePoiAreaClick;

		public delegate void RouteAction(RouteEntity routeEntity);
		public static RouteAction OnRouteSelected, OnRouteInfoShow;

		public delegate void PeriodAction(PeriodEntity periodEntity);
		public static PeriodAction OnPeriodSelected, OnPeriodInfoShow;

        public delegate void PeriodActionVisibility(PeriodEntity periodEntity, bool isVisible);
		public static PeriodActionVisibility OnPeriodToggleVisibility;

        public delegate void PeriodActionList(PeriodEntity selectedPeriod, List<PeriodEntity> previousPeriods = null);
		public static PeriodActionList OnPeriodsSelected, OnAllPeriodsSelected;

		public delegate void GenericFloatEvent(float val);
		public static GenericFloatEvent OnPoiDistanceChanged;

		public delegate void GeneralActions();
		public static GeneralActions
			//Audio
			OnAudioStop, OnAudioPause,
			//MAP
			OnAreasView, OnPoisView, OnMapSmoothMoveStart, OnMapSmoothMoveStop, OnPoiInfoClosed, OnSetPoiIconsToDefault,
			//UI
			OnInfoShowFull, OnInfoShowHalf, OnInfoHide, OnInfoRectRefresh, OnImageShowFull,
			//Update - Internet
			OnUpdateAvailable, OnUpdateStart, OnUpdateFinished, OnUpdateFailed, OnInternetLost, OnCarrierDataNetwork,
			OnFilesDownloadComplete, OnJsonsDownloadComplete,
            //Database
            OnDatabaseReaded,
			//App
			OnLanguageChanged, OnTryToQuit,
			//Bluetooth
			OnBluetoothNotEnabled, OnCameraPermissionDenied,
            //AR, MR
            OnMRDisable, OnARDisable, OnARAbort, OnARTargetFound, OnARTargetLost,
			//HELP
			OnShowHelp, OnHelpFirstTime,
            //TEXT
            OnFontSizeNormal, OnFontSizeMedium, OnFontSizeLarge, OnFontAnySizeChanged,

            OnSplashFinished;					     

		//manage on site logic and ui messages
		public delegate void ActionMessages(int areaID = 0);
		public static ActionMessages OnGpsOn, OnGpsOff, OnGpsFar, OnGpsNear, OnGpsNearArea;


		public delegate void ColorAction(int id, Color color);
		public static ColorAction OnPeriodColorChanged;

		public delegate void StringAction(string val);
		public static StringAction OnDownloadJsonFailed;

        public delegate void TextureAction(Texture val);
        public static TextureAction OnTextureFullImage;

		public delegate void AppModeAction(EnumsHolder.ApplicationMode mode);
		public static AppModeAction OnApplicationModeChanged;
    }

}
