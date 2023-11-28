//Diadrasis ©2023 - Stathis Georgiou
using Diadrasis.Rethymno;
using StaGeGames.BestFit;
using StaGeGames.BestFit.Extens;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{

	public class UIController : MonoBehaviour
	{
		[Space]
		public EnumsHolder.ApplicationMode appModeNow = EnumsHolder.ApplicationMode.GUIDE;
        [Space]
		public EnumsHolder.AppState appState = EnumsHolder.AppState.None;
		[Space]
		public EnumsHolder.RouteType routeType = EnumsHolder.RouteType.None;
		[Space]
		public EnumsHolder.InfoPanelState infoPanelState = EnumsHolder.InfoPanelState.Closed;
		[Space]
		public EnumsHolder.GeneralPanelState menuPanelState = EnumsHolder.GeneralPanelState.Close;
        [Space]
        public EnumsHolder.GeneralPanelState messagePanelState = EnumsHolder.GeneralPanelState.Close;
        [Space]
		public GUISettings settings;

		[Space]
		[Header("[---MENU TOP---]")]
		public SmartMotion topBarMenuPanelMotion;
		public Button btnClose;
		public Button btnReturn, btnHome, btnMenu, btnHelp;

		[Space]
		[Header("[---MENU BOTTOM---]")]
		/// <summary>
		/// shows instructions to user
		/// select area, route, period etc.
		/// </summary>
		public SmartMotion infoBottomPanelMotion;
		public TMPro.TextMeshProUGUI infoBottomText;//should be runtime translated (?)

		[Header("[---MAP GUI PANELS---]")]
		public RoutesPanel routesSelectPanel;
		public PeriodsPanel periodsSelectPanel;
		private GameObject areasSelectPanel, poisSelectPanel;//created at runtime from MarkerEngineManager

		[Header("[---MAP INTERACTION---]")]
		public GameObject stopMapInteraction;
		public GameObject hideMapVisibility;
		
		[Space]
		[Header("[---INFO PANEL---]")]
		public RectTransform infoContainer;
		public Image infoContainerBackground;
		public GameObject lastItemForHalfInfoBackground;
		public ScrollRect infoScroll;
		public Button btnShowInfo, btnHideInfo, btnHideInfoMapInteraction, btnCloseInfo;
		[Space]
		public SmartMotion infoPanelMotion;
		//public float infoPanelSpeed;

		[Space]
		public ImagesController imagesController;



		[Header("[---Info Half Calculation Rects---]")]
		public bool ShowBottomInfoHelps = true;
		public RectTransform rectTitle;
		public RectTransform rectShort, rectBottomInfo;
		public RectTransform rectSpaceForHeight;//draft item to leave space at the top of scroll panel from title, because title is not scroll element

		[Header("[---Info elements---]")]
		public GameObject panelImages;
		public GameObject panelFullDesc;
		public GameObject panelShortDesc;
		public TMPro.TextMeshProUGUI shortDescText;

		private OnlineMapsControlBase control;

		private AreaEntity areaSelected;
		private RouteEntity routeSelected;
		private PeriodEntity periodSelected;

		private int currentPoiId = -100;

		InfoPanelArgs infoArgs;
        SideMenuController sideMenu;
        MapController mapController;

        public InfoPanelArgs InfoSettings() { return infoArgs; }

		[Header("[---Map Overlays---]")]
		public GameObject SpinalongaMap;

		[Header("---[Differnces from think logic]---")]
		public bool appHasOverlays;
        [Space]
        public bool selectLastPeriodOnAreaClicked = true;
        void HideMapOverlays()
        {
			if (!appHasOverlays)
			{
                sideMenu.ShowToggleTopographicButton(false);
                return;
			}

			sideMenu.ShowToggleTopographicButton(true);
            if (sideMenu.IsTopographVisible())
            {
				SpinalongaMap.transform.GetChild(0).gameObject.SetActive(true);
			}
		}

        private bool IsARexists;

		void OnARTargetLost()
        {
			if (appModeNow != EnumsHolder.ApplicationMode.AR || !IsARexists) return;
			HideInfo();
        }


		#region Initialization

		private void Awake()
		{
            sideMenu = FindObjectOfType<SideMenuController>();
            mapController = FindObjectOfType<MapController>();

			EventHolder.OnARTargetLost += OnARTargetLost;

			EventHolder.OnMapSmoothMoveStop += SmoothShowInfoHalf;
			EventHolder.OnLanguageChanged += OnLanguageChange;
			EventHolder.OnUpdateFinished += HomeAction;

			EventHolder.OnNativePoiAreaClick += OnNativePoiAreaClick;
			EventHolder.OnGuiMarkerClick += OnGuiMarkerClicked;
			EventHolder.OnARInfoTriggered += OnARInfoTriggered;


			EventHolder.OnNativePoiMarkerClick += OnNativePoiMarkerClick;

            EventHolder.OnRouteSelected += OnRouteSelected;
			EventHolder.OnPeriodsSelected += OnPeriodSelected;

			EventHolder.OnPoiInfoClosed += SetPoiSelectedToNull;

			EventHolder.OnInfoRectRefresh += OnInfoRectRefresh;

			EventHolder.OnInfoHide += HideInfoInstantly;

            //info panel motion listeners
            infoPanelMotion.OnShowStart.AddListener(OnInfoShowStart);
			infoPanelMotion.OnShowComplete.AddListener(OnInfoShowComplete);
			infoPanelMotion.OnHideComplete.AddListener(OnInfoHideComplete);
			infoPanelMotion.OnShowPercentageStart.AddListener(OnInfoShowPercentageStart);
			infoPanelMotion.OnShowPercentageComplete.AddListener(OnInfoShowPercentageComplete);

			btnShowInfo.onClick.AddListener(ShowInfoFull);
			btnHideInfo.onClick.AddListener(ShowInfoHalf);
            if(btnHideInfoMapInteraction) btnHideInfoMapInteraction.onClick.AddListener(HideInfo);

            btnCloseInfo.onClick.AddListener(HideInfo);


            btnHome.onClick.AddListener(HomeAction);
			btnReturn.onClick.AddListener(ReturnAction);
			btnClose.onClick.AddListener(ExitAction);
			btnMenu.onClick.AddListener(OnSideMenuClick);
			btnHelp.onClick.AddListener(ShowHelp);

            if (ShowBottomInfoHelps)
                infoBottomPanelMotion.OnShowComplete.AddListener(CheckStateForBottomInfo);

            infoArgs = settings.infoArgs;

            EventHolder.OnMenuOpen += OnMenuOpen;
            EventHolder.OnMessageShow += OnMessageShow;

            EventHolder.OnInfoState += OnInfoActive;

            EventHolder.OnApplicationModeChanged += OnApplicationModeChanged;

            rectBottomInfo.gameObject.SetActive(ShowBottomInfoHelps);

			//manually initialize listeners in case are disabled
			List<UI_AppStateEvent> ui_AppStateEvents = new List<UI_AppStateEvent>();
			ui_AppStateEvents = FindObjectsOfType<UI_AppStateEvent>(true).ToList();
			ui_AppStateEvents.ForEach(b => b.Init());

		}

		void ShowHelp() { EventHolder.OnShowHelp?.Invoke(); }

		void OnInfoActive(EnumsHolder.InfoPanelState val)
        {
            if (val == EnumsHolder.InfoPanelState.Closed)
            {
				//panelShortDesc.SetActive(true);
            }
        }

        void OnMessageShow(bool val) { messagePanelState = val ? EnumsHolder.GeneralPanelState.Open : EnumsHolder.GeneralPanelState.Close; }

        void OnMenuOpen(bool val) { menuPanelState = val ? EnumsHolder.GeneralPanelState.Open : EnumsHolder.GeneralPanelState.Close; }

        void OnSideMenuClick()
        {
			EventHolder.OnSideMenuClick?.Invoke();
		}

		void OnLanguageChange()
        {
			SetBottomInfoText();
			HomeAction();
		}

		void OnInfoRectRefresh()
        {
			infoContainer.ForceRebuildLayout();
        }

		IEnumerator Start()
		{
			control = OnlineMapsControlBase.instance;

			// Subscribe to map events
			control.OnMapPress += OnMapPress;
			control.OnMapRelease += OnMapRelease;
			control.OnMapDrag += OnMapDrag;

            MarkerEngineManager markerEngineManager = FindObjectOfType<MarkerEngineManager>();
			areasSelectPanel = markerEngineManager.GetAreaMarkersPanel();
			poisSelectPanel = markerEngineManager.GetPOIMarkersPanel();

			yield return new WaitForSeconds(1f);

			SetAppState(EnumsHolder.AppState.AreasView);

			//infoPanelSpeed = infoPanelMotion.moveSpeedCustom;

			HideMapOverlays();

			//yield return new WaitForSeconds(1.5f);

			//if(Application.isEditor)
			//{
			//	Debug.LogWarning("Custom Show Periods");
			//}

			//SelectLastPeriodInstantly();

            yield return null;
		}

		void SelectLastPeriodInstantly()
		{
			//if(Application.isEditor) Debug.Log("SelectLastPeriodInstantly");

            areaSelected = DataManager.Instance.AreaEntities()[0];
            mapController.AreaSelected = areaSelected;

            routeSelected = DataManager.Instance.FirstRoute();
            periodSelected = DataManager.Instance.GetLastPeriod();
            EventHolder.OnAllPeriodsSelected?.Invoke(periodSelected, DataManager.Instance.GetPreviousPeriods());
        }

		

        #endregion

        #region Interactions

        public bool IsSamePoiSelected(int id)
		{
			if (currentPoiId == id) return true;
			currentPoiId = id;
			return false;
		}

		bool isMapReleased = true;
		bool isMapDragged = false;
		void OnMapPress() 
		{
			if (!isMapReleased) return;
			isMapReleased = false;

			//if(Application.isEditor) Debug.Log("OnMapPress"); 
		}
		void OnMapRelease() 
		{
			if (isMapReleased) return;
			isMapReleased = true;
			isMapDragged = false;

			//if (Application.isEditor) Debug.Log("OnMapRelease"); 
		}
		void OnMapDrag() 
		{
			if (isMapDragged) return;
			isMapDragged = true;
			if(infoArgs.hideInfoOnMapDrag) HideInfoOnMapDrag();

			//if (Application.isEditor) Debug.Log("Map Dragging");
		}

		void OnNativePoiMarkerClick(PoiEntity poiEntity)
        {
			//if (IsSamePoiSelected(poiEntity.poi.poi_id)) return;
			ResetScrollPosition();
			AllowScrolling();

			if(poiEntity == null) return;

			//StartCoroutine(DelaySetAppState(EnumsHolder.AppState.PoiSelected));
            SetAppState(EnumsHolder.AppState.PoiSelected);

			PrepareToShowInfo(true);
		}

		void OnNativePoiAreaClick(AreaEntity areaEntity)
        {
			if (areaEntity == null) return;

			ResetScrollPosition();
			AllowScrolling();
			areaSelected = areaEntity;
			routesSelectPanel.Init(areaSelected);
			SetAppState(EnumsHolder.AppState.RoutesView);
			PrepareToShowInfo(false);
		}

		void OnARInfoTriggered(MarkerInstance marker)
        {
			ResetScrollPosition();
			AllowScrolling();
			if (marker == null || marker.data == null) return;
			PrepareToShowInfo(true);
		}

		void OnGuiMarkerClicked(MarkerInstance marker)
		{
			ResetScrollPosition();
			AllowScrolling();
            if (marker == null || marker.data == null) return;
            if (marker.data.IsArea)
			{
				if (DataManager.Instance.AreaHasOneRoute(marker.data.areaEntity))
				{
					SelectLastPeriodInstantly();
					return;
				}
				areaSelected = marker.data.areaEntity;
				routesSelectPanel.Init(areaSelected);
				SetAppState(EnumsHolder.AppState.RoutesView);
				//PrepareToShowInfo(false);
			}
			else
			{
                if (marker.data.poiEntity == null) return;
                SetAppState(EnumsHolder.AppState.PoiSelected);

				PrepareToShowInfo(true);
			}
		}

		void SetRouteTypeState(cRouteType type)
        {
            switch (type.route_type_id)
            {
				case 234:
					routeType = EnumsHolder.RouteType.InfoMonuments;
					break;
				case 235:
					routeType = EnumsHolder.RouteType.FollowNarrator;
					break;
				case 236:
					routeType = EnumsHolder.RouteType.StoriesOfDecayAndConservation;
					break;
				case 237:
					routeType = EnumsHolder.RouteType.AncientGrafiti;
					break;
				case 430:
					routeType = EnumsHolder.RouteType.VisitMuseum;
					break;
				default:
					routeType = EnumsHolder.RouteType.None;
					break;
            }

			EventHolder.OnRouteTypeChanged?.Invoke(routeType);
		}

        void OnRouteSelected(RouteEntity routeEntity)
		{
			EventHolder.OnRouteInfoShow(routeEntity);
			periodsSelectPanel.Init(routeEntity);
			routeSelected = routeEntity;

			//SetRouteTypeState(routeEntity.routeType);

			if (!DataManager.Instance.skipPeriodInfo)
			{
				if (routeEntity.HasPeriods())// GlobalUtils.HasRouteTypePeriods(routeEntity.routeType))//is Monuments type?
				{
					//it is a type of Monuments >> show periods
					SetAppState(EnumsHolder.AppState.PeriodsView);
				}
			}
            #region not in use without periods
            else
            {
				//its not type of Monuments >> show pois
				SetAppState(EnumsHolder.AppState.PoisView);

				HideMapOverlays();

				//if (routeSelected.routeType.route_type_id == 430)
    //            {
    //                if (routeSelected.route.route_id == 431)
    //                {
				//		agoraMuseum.SetActive(true);
				//		museumBackground.SetActive(true);
				//		AgoraMap.transform.GetChild(0).gameObject.SetActive(false);
				//		KerameikosMap.transform.GetChild(0).gameObject.SetActive(false);
				//		LofoiMap.transform.GetChild(0).gameObject.SetActive(false);
				//		sideMenu.ShowToggleTopographicButton(false);
				//	}
				//	else if (routeSelected.route.route_id == 638)
				//	{
				//		kerameikosMuseum.SetActive(true);
				//		museumBackground.SetActive(true);
				//		KerameikosMap.transform.GetChild(0).gameObject.SetActive(false);
				//		AgoraMap.transform.GetChild(0).gameObject.SetActive(false);
				//		LofoiMap.transform.GetChild(0).gameObject.SetActive(false);
				//		sideMenu.ShowToggleTopographicButton(false);
				//	}
				//}

                if (!routeEntity.HasPoiEntities())
				{
					SmoothShowInfoHalf();
                }
			}
            #endregion
        }

        void OnPeriodSelected(PeriodEntity selectedPeriod, List<PeriodEntity> previousPeriods = null)
		{
			EventHolder.OnPeriodInfoShow?.Invoke(selectedPeriod);
			SetAppState(EnumsHolder.AppState.PoisView);
			periodSelected = selectedPeriod;
			PrepareToShowInfo(false);
		}

		//wait for poi creation first
		void DelayPoisView()
        {
			SetAppState(EnumsHolder.AppState.PoisView);
		}

        #endregion

        #region INFO PANEL ACTIONS

		

        void PrepareToShowInfo(bool isForPoi) 
		{
			//Debug.Log("<color=orange>PrepareToShowInfo</color>");

            //#if UNITY_EDITOR
            //TODO
            //add delay time
            //wait images, audios and texts to be applied first
            //then calculate size and show
            //Debug.Log("<color=orange>[PrepareToShowInfo] TODO >> ADD DELAY TIME</color> " + isForPoi);
            //#endif
            if (isForPoi)//recalculate size before hand
            {
				Invoke(nameof(PrepareShowInfoPanel), 0.25f);
			}
			EventHolder.OnAudioStop?.Invoke();
			StartCoroutine(PrepareInfoPanel(isForPoi));//fix ui bug
		}

		IEnumerator PrepareInfoPanel(bool isForPoi)
        {
			CoroutineWithData cd = new CoroutineWithData(this, RefreshInfoPanel());
			yield return cd.coroutine;
			if (isForPoi)//show immediately
			{
				ShowInfoHalf();
			}
            else//wait smooth map move
            {
				if (!infoArgs.autoShowInfoOnMapSmoothMoveStop)
					Invoke(nameof(ShowInfoHalf), 1f); //MapController settings smooth time
            }
			yield break;
        }
		
		IEnumerator RefreshInfoPanel()
        {
			//float t = 0f;
			//         while (t < 0.5f)
			//         {
			//	ResetScrollPosition();
			//	infoContainer.ForceRebuildLayout();
			//	t += Time.deltaTime;
			//}
			yield return new WaitForEndOfFrame();
            ResetScrollPosition();
            infoContainer.ForceRebuildLayout();
            yield return "success";
		}

		void PrepareShowInfoPanel()
        {
			ResetScrollPosition();
			infoContainer.ForceRebuildLayout();
			//ShowInfoHalf();
		}

		void SmoothShowInfoHalf()
        {
			if (appState == EnumsHolder.AppState.AreasView || appState == EnumsHolder.AppState.PoiSelected) return;
			PrepareToShowInfo(true);
		}

		void ShowInfoHalf()
		{
            EventHolder.OnAudioPause?.Invoke();

            //topBarMenuPanelMotion.HidePanel();
            //stopMapInteraction.SetActive(true);

            panelShortDesc.SetActive(true);


            rectTitle.ForceRebuildLayout();

			//StartCoroutine(PrepareInfoPanel());//fix ui bug
			infoContainer.ForceRebuildLayout();
			ResetScrollPosition();

			rectSpaceForHeight.sizeDelta = new Vector2(rectSpaceForHeight.GetWidth(), rectTitle.GetHeight());

			float h = rectTitle.GetHeight();// + rectShort.GetHeight();

			if (panelShortDesc.activeSelf) h += rectShort.GetHeight();
			//if poi do not add extra height for bottom info
			if (rectBottomInfo.gameObject.activeSelf)
			{
				h += appState == EnumsHolder.AppState.PoiSelected ? 0f : rectBottomInfo.GetHeight();
			}
			float totalHeight = infoPanelMotion.GetPanelHeight();
			float percentage = Mathf.Abs((h / totalHeight) * 100f);

			infoContainer.ForceRebuildLayout();
			ResetScrollPosition();

			infoPanelMotion.ShowPanelWithPercentInTime(percentage, infoArgs.timeToMoveTitleInfo);

			infoPanelState = EnumsHolder.InfoPanelState.HalfOpen;

            EventHolder.OnInfoState?.Invoke(infoPanelState);
        }

		void ShowInfoFull()
		{
			if (appState == EnumsHolder.AppState.PoiSelected) HideBottomInfo();
			//show background to hide map
			infoContainerBackground.color = new Color(1, 1, 1, 1);
			lastItemForHalfInfoBackground.SetActive(false);
			panelShortDesc.SetActive(infoArgs.showShortDescOnFullInfo);
			infoPanelMotion.ShowPanelWithTime(infoArgs.timeToMoveFullInfo);// .ShowPanel();
			infoPanelState = EnumsHolder.InfoPanelState.FullOpen;

            EventHolder.OnInfoState?.Invoke(infoPanelState);
        }

		void HideInfo()
        {
			stopMapInteraction.SetActive(false);
			infoPanelMotion.HidePanel();
            if (infoPanelState != EnumsHolder.InfoPanelState.Closed)
            {
                infoPanelState = EnumsHolder.InfoPanelState.Closed;
                EventHolder.OnInfoState?.Invoke(infoPanelState);
            }
            EventHolder.OnAudioStop?.Invoke();
			if (appState == EnumsHolder.AppState.PoisView)
			{
				EventHolder.OnSetPoiIconsToDefault?.Invoke();
			}
			else if (appState == EnumsHolder.AppState.PoiSelected)
			{
				//info panel closed with return button, or close button
				EventHolder.OnSetPoiIconsToDefault?.Invoke();
				//reset id only here, to be able to show same info again
				//info data on other classes are already filled previously
				currentPoiId = -1;
			}
        }

		void HideInfoInstantly()
		{
			stopMapInteraction.SetActive(false);
			infoPanelMotion.HidePanelInstantly();
            if (infoPanelState != EnumsHolder.InfoPanelState.Closed)
            {
                infoPanelState = EnumsHolder.InfoPanelState.Closed;
                EventHolder.OnInfoState?.Invoke(infoPanelState);
            }
            EventHolder.OnAudioStop?.Invoke();
			if (appState == EnumsHolder.AppState.PoisView)
			{
				EventHolder.OnSetPoiIconsToDefault?.Invoke();
			}

        }

		void HideInfoOnMapDrag()
        {
			if (infoPanelMotion.isHiding || !infoPanelMotion.isVisible) return;//makes sure that is called only once
			HideInfo();
			EventHolder.OnPoiInfoClosed?.Invoke();
		}

		void OnInfoShowStart()
		{
            //hide title panel - [it is opposite => hide = show because it is nested as child at the top of info panel]
            //titlePanelMotion.HidePanel();
            btnHideInfo.gameObject.SetActive(false);
            btnShowInfo.gameObject.SetActive(false);
            btnCloseInfo.gameObject.SetActive(false);
        }
		void OnInfoShowComplete()
		{
			//show button hide
			//btnHideInfo.gameObject.SetActive(true);
			if (DataManager.Instance.IsMobile())
			{
                btnHideInfo.gameObject.SetActive(true);
                BestFitter bf = btnHideInfo.GetComponent<BestFitter>();
                bf.fParentSizePercentageScale = !btnCloseInfo.gameObject.activeSelf ? 100f : 40f;
            }
			else
			{
                btnHideInfo.gameObject.SetActive(false);
                btnCloseInfo.gameObject.SetActive(false);
            }
			//hide button show
			btnShowInfo.gameObject.SetActive(false);
			//enable scroll vertical
			AllowScrolling();
			//stop map interaction
			stopMapInteraction.SetActive(true);

			EventHolder.OnInfoShowFull?.Invoke();
		}
		void OnInfoHideComplete()
		{
			//show title panel - [it is opposite => show = hide because it is nested as child at the top of info panel]
			//titlePanelMotion.ShowPanel();
			//hide both buttons show/hide
			btnShowInfo.gameObject.SetActive(false);
			btnHideInfo.gameObject.SetActive(false);
			btnCloseInfo.gameObject.SetActive(false);
			stopMapInteraction.SetActive(false);

			if (infoPanelState != EnumsHolder.InfoPanelState.Closed)
			{
				infoPanelState = EnumsHolder.InfoPanelState.Closed;
				EventHolder.OnInfoState?.Invoke(infoPanelState);
			}
        }
		void OnInfoShowPercentageStart()
		{
			//enable scroll vertical to allow panels calculations
			//infoScroll.vertical = true;
			stopMapInteraction.SetActive(false);
			panelShortDesc.SetActive(!shortDescText.text.IsNull());
		}
		void OnInfoShowPercentageComplete()
		{
			lastItemForHalfInfoBackground.SetActive(true);
			infoContainerBackground.color = new Color(1, 1, 1, 0);

			stopMapInteraction.SetActive(false);
			//show button show
			btnShowInfo.gameObject.SetActive(HasInfoActiveElements());
			//hide button hide
			btnHideInfo.gameObject.SetActive(false);
            //disable scroll vertical

            btnCloseInfo.gameObject.SetActive(true);
            BestFitter bf = btnCloseInfo.GetComponent<BestFitter>();
			if(bf != null)
                bf.fParentSizePercentageScale = !btnShowInfo.gameObject.activeSelf ? 100f : 40f;

            Invoke(nameof(StopScrolling), .25f);

			stopMapInteraction.SetActive(false);

			EventHolder.OnInfoShowHalf?.Invoke();
		}

		private bool HasInfoActiveElements()
        {
			return panelFullDesc.activeSelf || panelImages.activeSelf;
        }

		void ResetScrollPosition() { infoScroll.verticalNormalizedPosition = 1; }
		void AllowScrolling() { infoScroll.vertical = true; }
		void StopScrolling() { infoScroll.vertical = false; }

		#endregion

		IEnumerator DelaySetAppState(EnumsHolder.AppState state)
        {
			yield return new WaitForSeconds(0.75f);
			SetAppState(state);
			yield break;
		}

        void OnApplicationModeChanged(EnumsHolder.ApplicationMode mode)
        {
            appModeNow = mode;

            switch (mode)
            {
                case EnumsHolder.ApplicationMode.GUIDE:
                default:
                    btnHome.gameObject.SetActive(appState != EnumsHolder.AppState.AreasView);
					HideInfo();
					break;
                case EnumsHolder.ApplicationMode.AR:
                    btnHome.gameObject.SetActive(true);
                    break;
                
            }
           
        }

        void SetAppState(EnumsHolder.AppState state)
        {
			if (appState == state) return;
			appState = state;
			EventHolder.OnStateChanged?.Invoke(state);

			btnHome.gameObject.SetActive(appState != EnumsHolder.AppState.AreasView);
			if (selectLastPeriodOnAreaClicked)
			{
				btnReturn.gameObject.SetActive(appState == EnumsHolder.AppState.PoiSelected);
			}
			else
			{
                btnReturn.gameObject.SetActive(appState != EnumsHolder.AppState.AreasView);
            }
			btnClose.gameObject.SetActive(appState == EnumsHolder.AppState.AreasView);
			//btnLayersToggle.transform.parent.gameObject.SetActive(appState == EnumsHolder.AppState.AreasView);

			switch (appState)
            {
                case EnumsHolder.AppState.None:
                    break;
                case EnumsHolder.AppState.AreasView:
					EventHolder.OnAreasView?.Invoke();//>> map zoom on areas (MapController)
					HideInfoInstantly();
					Invoke(nameof(HideInfoInstantly), infoArgs.timeToMoveTitleInfo);
					HidePanels();
					ShowAreasPanel(true);
					ShowBottomInfoDelayed();
					HideMapOverlays();
					routeType = EnumsHolder.RouteType.None;
					EventHolder.OnRouteTypeChanged?.Invoke(routeType);
					break;
                case EnumsHolder.AppState.RoutesView:
                    //HideInfo();
                    HideInfoInstantly();
					HidePanels();
					ShowRoutesPanel(true);
					//map zoom in area
					EventHolder.OnRoutesView?.Invoke(areaSelected);
					EventHolder.OnAreaInfoShow?.Invoke(areaSelected);
					routeType = EnumsHolder.RouteType.None;
					EventHolder.OnRouteTypeChanged?.Invoke(routeType);
					PrepareToShowInfo(false);
					ShowBottomInfoDelayed();
					HideMapOverlays();
					break;
                case EnumsHolder.AppState.PeriodsView:
					//HideInfo();
					HideInfoInstantly();
					HidePanels();
					ShowPeriodsPanel(true);
					//map zoom in area
					EventHolder.OnPeriodsView?.Invoke(areaSelected);
					EventHolder.OnRouteInfoShow?.Invoke(routeSelected);
					PrepareToShowInfo(false);
					ShowBottomInfoDelayed();
					HideMapOverlays();
					break;
                case EnumsHolder.AppState.PoisView:
					//HideInfo();
					HideInfoInstantly();
					HideAllExceptPoisPanel();
					ShowPoisPanel(true);
					ShowBottomInfoDelayed();
                    if (Application.isEditor) Debug.Log("<color=yellow>SHOW PERIODS INFO</color>");
                    break;
                case EnumsHolder.AppState.PoiSelected:
					HideInfoInstantly();
					PrepareToShowInfo(true);
					HideBottomInfo();
					break;
                default:
                    break;
            }
        }

		#region BOTTOM INFO PANEL ACTIONS

		void HideBottomInfo() 
		{
            if (!ShowBottomInfoHelps) return;
            CancelInvoke(nameof(ShowBottomInfoInvoked));
			infoBottomPanelMotion.HidePanelInstantly(); 
		}
		void ShowBottomInfoDelayed() 
		{
			//if (Application.isEditor) Debug.Log("ShowBottomInfoDelayed "+appState.ToString());

			if (!ShowBottomInfoHelps) return;

			HideBottomInfo();

			switch (appState)
            {
                case EnumsHolder.AppState.AreasView:
                case EnumsHolder.AppState.RoutesView:
                case EnumsHolder.AppState.PeriodsView:
                case EnumsHolder.AppState.PoisView:
					HideInfoInstantly();
                    break;
                case EnumsHolder.AppState.None:
				case EnumsHolder.AppState.PoiSelected:
				default:
					return;
            }
			SetBottomInfoText();
			Invoke(nameof(ShowBottomInfoInvoked), settings.waitTimeToShowBottomInfo);
        }

        void SetBottomInfoText()
		{
            if (!ShowBottomInfoHelps) return;

            string _term = string.Empty;
			switch (appState)
			{
				case EnumsHolder.AppState.AreasView:
					_term = keyTerm.select_area.ToString();// GlobalUtils.termSelectArea;
					break;
				case EnumsHolder.AppState.RoutesView:
					_term = keyTerm.select_route_type.ToString();//GlobalUtils.termSelectRouteType;
					break;
				case EnumsHolder.AppState.PeriodsView:
					_term = keyTerm.select_period.ToString();//GlobalUtils.termSelectPeriod;
					break;
				case EnumsHolder.AppState.PoisView:
					_term = keyTerm.select_poi.ToString();//GlobalUtils.termSelectPoi;
					break;
			}
			infoBottomText.text = DataManager.Instance.GetTermText(_term);
		}
			
		void ShowBottomInfoInvoked()
        {
            if (!ShowBottomInfoHelps) return;

            if (appState == EnumsHolder.AppState.PoiSelected) return;

			if (appState == EnumsHolder.AppState.PoisView)
			{
                if (Application.isEditor) Debug.Log("<color=yellow>SHOW PERIODS INFO</color>");
                return;
			}
            infoBottomPanelMotion.ShowPanel();
		}

		void CheckStateForBottomInfo()
        {
            if (!ShowBottomInfoHelps) return;
            if (Application.isEditor) Debug.Log("CheckStateForBottomInfo");
			if (appState == EnumsHolder.AppState.PoiSelected || appState == EnumsHolder.AppState.PoisView)
			{
				CancelInvoke(nameof(ShowBottomInfoInvoked));
				infoBottomPanelMotion.HidePanelInstantly();
			}
		}

        #endregion

        #region MENU BUTTONS ACTIONS
        void HomeAction()
        {
			if (imagesController.IsFullImagePanelOpened())
			{
				imagesController.HideFullImagePanel();
			}

			//HideInfo();
			HideInfoInstantly();

			//clear selected data
			areaSelected = null;
			periodSelected = null;
			routeSelected = null;
			

			//return to areas view on map
			ShowAreasPanel(true);
			ShowRoutesPanel(false);
			ShowPeriodsPanel(false);
			ShowPoisPanel(false);

			//bottom info >> select area

			SetAppState(EnumsHolder.AppState.AreasView);
		}

		void ReturnAction()
		{

            if (imagesController.IsFullImagePanelOpened())
            {
				imagesController.HideFullImagePanel();
				return;
            }

			if (menuPanelState == EnumsHolder.GeneralPanelState.Open)
			{
				OnSideMenuClick();
				return;
			}

			if (messagePanelState == EnumsHolder.GeneralPanelState.Open)
			{
				EventHolder.OnMessageHide?.Invoke();
				return;
			}

			//check app state
			switch (appState)
            {
                case EnumsHolder.AppState.None:
				default:
					break;
                case EnumsHolder.AppState.AreasView:
					//bottom info >> select area
					EventHolder.OnAreasView?.Invoke();//>> map zoom on areas (MapController)
					break;
                case EnumsHolder.AppState.RoutesView:
					//bottom info >> select route type
					switch (infoPanelState)
					{
						case EnumsHolder.InfoPanelState.Closed:
						case EnumsHolder.InfoPanelState.HalfOpen:
						default:
							HomeAction();
							break;
						//case EnumsHolder.InfoPanelState.HalfOpen:
						//	HideInfo();
						//	break;
						case EnumsHolder.InfoPanelState.FullOpen:
							if(!infoArgs.autoShowInfoOnMapSmoothMoveStop) ShowInfoHalf();
							break;
					}
					break;
                case EnumsHolder.AppState.PeriodsView:
					//bottom info >> select period
					switch (infoPanelState)
					{
						case EnumsHolder.InfoPanelState.Closed:
						case EnumsHolder.InfoPanelState.HalfOpen:
						default:
							//return to routes view
							SetAppState(EnumsHolder.AppState.RoutesView);
							break;
						//case EnumsHolder.InfoPanelState.HalfOpen:
						//	HideInfo();
						//	break;
						case EnumsHolder.InfoPanelState.FullOpen:
							if (!infoArgs.autoShowInfoOnMapSmoothMoveStop) ShowInfoHalf();
							break;
					}
					break;
                case EnumsHolder.AppState.PoisView:
					//bottom info >> select poi

					switch (infoPanelState)
					{
						case EnumsHolder.InfoPanelState.Closed:
						case EnumsHolder.InfoPanelState.HalfOpen:
						default:

							if (DataManager.Instance.skipPeriodInfo)
							{
                                SetAppState(EnumsHolder.AppState.RoutesView);
                            }
							else
							{
								if (routeSelected.HasPeriods()) //GlobalUtils.HasRouteTypePeriods(routeSelected.routeType))
								{
									SetAppState(EnumsHolder.AppState.PeriodsView);
								}
								else
								{
									SetAppState(EnumsHolder.AppState.RoutesView);
								}
							}
							break;
						//case EnumsHolder.InfoPanelState.HalfOpen:
						//	HideInfo();
						//	break;
						case EnumsHolder.InfoPanelState.FullOpen:
							ShowInfoHalf();
							break;
					}
					break;
                case EnumsHolder.AppState.PoiSelected:
					switch (infoPanelState)
					{
						case EnumsHolder.InfoPanelState.Closed:
						case EnumsHolder.InfoPanelState.HalfOpen:
						default:
							SetAppState(EnumsHolder.AppState.PoisView);
							//zoom map on pois
							EventHolder.OnPoisView?.Invoke();

							//EventHolder.OnAreaInfoShow?.Invoke(areaSelected);

							
							if (routeSelected != null)
                            {
								if (DataManager.Instance.skipPeriodInfo)
								{
									EventHolder.OnRouteInfoShow?.Invoke(routeSelected);
								}
								else
								{
									if (routeSelected.HasPeriods())//GlobalUtils.HasRouteTypePeriods(routeSelected.routeType))
									{
										EventHolder.OnPeriodInfoShow?.Invoke(periodSelected);
									}
									else
									{
										EventHolder.OnRouteInfoShow?.Invoke(routeSelected);
									}
								}

                            }

                            PrepareToShowInfo(false);



							break;
						//case EnumsHolder.InfoPanelState.HalfOpen:
						//	HideInfo();
						//	break;
						case EnumsHolder.InfoPanelState.FullOpen:
							ShowInfoHalf();
							break;
					}
					break;

            }
        }

        void ExitAction()
		{
			if(menuPanelState == EnumsHolder.GeneralPanelState.Open)
			{
				OnSideMenuClick();
                return;
			}

            if (messagePanelState == EnumsHolder.GeneralPanelState.Open)
            {
                EventHolder.OnMessageHide?.Invoke();
                return;
            }

            //check app state
            switch (appState)
			{
				case EnumsHolder.AppState.None:
				case EnumsHolder.AppState.AreasView:
				default:
					EventHolder.OnTryToQuit?.Invoke();
					break;
			}
		}

#endregion

		#region Visibility

        void ShowRoutesPanel(bool val) { routesSelectPanel.Show(val); }
		void ShowPeriodsPanel(bool val) { periodsSelectPanel.Show(val); }
		void ShowAreasPanel(bool val) { areasSelectPanel.SetActive(val); }
		void ShowPoisPanel(bool val) 
		{ 
			poisSelectPanel.SetActive(val);
			EventHolder.OnPoisShow?.Invoke(val);
		}

		void HideAllExceptPoisPanel()
        {
			ShowAreasPanel(false);
			ShowRoutesPanel(false);
			ShowPeriodsPanel(false);
		}

		void HidePanels()
        {
			ShowPoisPanel(false);
			ShowAreasPanel(false);
			ShowRoutesPanel(false);
			ShowPeriodsPanel(false);
        }

#endregion

        void SetPoiSelectedToNull() { currentPoiId = -100; }

    }

}
