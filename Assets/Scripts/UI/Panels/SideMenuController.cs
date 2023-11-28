//Diadrasis ©2023 - Stathis Georgiou
using Spinaloga;
using StaGeGames.BestFit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{

	public class SideMenuController : MonoBehaviour
	{
        [Space]
        public EnumsHolder.AppState appState;

        [Space]
        public EnumsHolder.GeneralPanelState menuPanelState = EnumsHolder.GeneralPanelState.Close;

        [Header("[---SIDE MENU elements---]")]
		public SmartMotion motionSideMenu;
		public Button btnHideSideMenu;

		[Header("[---App elements---]")]
		public Button btnChangeLanguage;
        public TMPro.TextMeshProUGUI textLangLabel;
        public Button btnInfo;

        [Header("[---Period elements---]")]
        public Button btnPrehistoric;
        public Button btnByzantine, btnModern;
        public Image prehistoricIcon, byzantineIcon, modernIcon;
        public ButtonToggleItem toggleItemPrehistoric, toggleItemByzantine, toggleItemModern;

        [Header("[---MAP elements---]")]
		public Button btnLayersToggle;
		private ButtonToggleItem toggleItemLayers;
        public Button btnToggleTopographics;
        public GameObject lineTopographics;
		private ButtonToggleItem toggleItemTopographics;
        public Button btnToggleTourMode;
        private ButtonToggleItem toggleItemTourMode;
        public Button btnToggleAccuracyCheck;
        private ButtonToggleItem toggleItemAccuracyCheck;

        [Space]
        public Slider sliderPoiDistance;
        public TMPro.TextMeshProUGUI txtDistPoi;

        UIController uiControl;

        [ReadOnly]
        public bool IsMR_Exists, IsAR_Exists;

        //textLangLabel
        void OnLanguageChanged()
        {
            //textLangLabel.text = DataManager.Instance.SelectedLanguage == EnumsHolder.Language.GREEK ? "EN" : "EL";

            string s = "EL";
            switch (DataManager.Instance.SelectedLanguage)
            {
                case EnumsHolder.Language.GREEK:
                    s = "EL";
                    break;
                case EnumsHolder.Language.ENGLISH:
                    s = "EN";
                    break;
                case EnumsHolder.Language.FRENCH:
                    s = "FR";
                    break;
                case EnumsHolder.Language.GERMAN:
                    s = "DE";
                    break;
                case EnumsHolder.Language.RUSSIAN:
                    s = "RU";
                    break;
                default:
                    s = "EL";
                    break;
            }
            textLangLabel.text = s;

        }

        private AreaEntity selectedArea;
        private RouteEntity selectedRoute;

        void OnGuiMarkerClicked(MarkerInstance marker)
        {
            if (marker == null || marker.data == null) return;
            if (marker.data.IsArea)
            {
                selectedArea = marker.data.areaEntity;
            }
        }

        void OnRouteSelected(RouteEntity route) 
        { 
            selectedRoute = route;

            //check if we have all periods
            if(selectedRoute.periods.Count > 0)
            {
                //193 minoan
                //197 byzantine
                //196 modern
                PeriodEntity myPeriod = selectedRoute.periods.Find(b => b.period.period_id == 193);
                bool hasPrehistoric = myPeriod != null && myPeriod.period.period_id == 193;
                if (hasPrehistoric)
                {
                    if (myPeriod.periodIconVisited != null)
                    {
                        prehistoricIcon.sprite = myPeriod.periodIconVisited.ToSprite();
                    }
                    else { prehistoricIcon.sprite = myPeriod.periodIcon.ToSprite(); }
                }

                if (hasPrehistoric) hasPrehistoric = myPeriod.poiEntities.Count > 0;

                btnPrehistoric.interactable = hasPrehistoric;
                toggleItemPrehistoric.SetPeriodEnabled(hasPrehistoric);

                myPeriod = selectedRoute.periods.Find(b => b.period.period_id == 196);
                bool hasModern= myPeriod != null && myPeriod.period.period_id == 196;
                if (hasModern)
                {
                    if (myPeriod.periodIconVisited != null)
                    {
                        modernIcon.sprite = myPeriod.periodIconVisited.ToSprite();
                    }
                    else { modernIcon.sprite = myPeriod.periodIcon.ToSprite(); }
                }
                if (hasModern) hasModern = myPeriod.poiEntities.Count > 0;

                btnModern.interactable = hasModern;
                toggleItemModern.SetPeriodEnabled(hasModern);

                myPeriod = selectedRoute.periods.Find(b => b.period.period_id == 197);
                bool hasByzantine = myPeriod != null && myPeriod.period.period_id == 197;
                if (hasByzantine)
                {
                    if (myPeriod.periodIconVisited != null)
                    {
                        byzantineIcon.sprite = myPeriod.periodIconVisited.ToSprite();
                    }
                    else { byzantineIcon.sprite = myPeriod.periodIcon.ToSprite(); }
                }
                if (hasByzantine) hasByzantine = myPeriod.poiEntities.Count > 0;

                btnByzantine.interactable = hasByzantine;
                toggleItemByzantine.SetPeriodEnabled(hasByzantine);
            }
        }


        private void Awake()
        {
            EventHolder.OnDatabaseReaded += OnDatabaseReaded;

            EventHolder.OnGuiMarkerClick += OnGuiMarkerClicked;
            EventHolder.OnRouteSelected += OnRouteSelected;
        }

        IEnumerator Start()
		{
            uiControl = FindObjectOfType<UIController>();

            EventHolder.OnLanguageChanged += OnLanguageChanged;

            EventHolder.OnStateChanged += OnAppStateChanged;

            EventHolder.OnAllPeriodsSelected += OnAllPeriodsSelected;


            //textLangLabel.text = DataManager.Instance.SelectedLanguage == EnumsHolder.Language.GREEK ? "EN" : "EL";
            OnLanguageChanged();

            //1684 ottoman
            //1685 venetian
            //1686 contemporary
            btnPrehistoric.onClick.AddListener(() => PeriodToggle(193));
            btnByzantine.onClick.AddListener(() => PeriodToggle(197));
            btnModern.onClick.AddListener(() => PeriodToggle(196));

            toggleItemLayers = btnLayersToggle.GetComponent<ButtonToggleItem>();
			toggleItemTopographics = btnToggleTopographics.GetComponent<ButtonToggleItem>();
            toggleItemTourMode = btnToggleTourMode.GetComponent<ButtonToggleItem>();
            toggleItemAccuracyCheck = btnToggleAccuracyCheck.GetComponent<ButtonToggleItem>();

            //btnMenu from UIController
            EventHolder.OnSideMenuClick += ToggleMenuPanel;
			btnHideSideMenu.onClick.AddListener(() => motionSideMenu.HidePanel());

			btnChangeLanguage.onClick.AddListener(ChangeLanguage);

            //show btnHideSideMenu on open side menu
            motionSideMenu.OnShowStart.AddListener(() => ShowHideMenuButton(true));//enables fullscreen hide button which enables the feature when the user taps anywhere on the screen to hide the side menu
			motionSideMenu.OnHideStart.AddListener(() => ShowHideMenuButton(false)); //disables btnHideSideMenu on side menu close

            //buttons listeners
            btnLayersToggle.onClick.AddListener(MapLayerToggle);//toggle map layer => custom or google.sattelite
			btnToggleTopographics.onClick.AddListener(TopographicsToggle);//show/hide topographics on map
            btnToggleTourMode.onClick.AddListener(TourModeToggle);
            btnToggleAccuracyCheck.onClick.AddListener(AccuracyToggle);//enable/disable accuracy check
            OnSiteManager.Instance.settings.useFilterOnGpsUpdateDistance = false;

            EventHolder.OnMenuOpen += OnMenuOpen;
            if (DataManager.Instance.IsMobile())
            {
                EventHolder.OnGpsFar += HideTourMode;
                EventHolder.OnGpsNear += ShowTourMode;
                EventHolder.OnTourChanged += OnTourChanged;

                sliderPoiDistance.onValueChanged.AddListener((b) => OnSliderPoiDistance(sliderPoiDistance));

                sliderPoiDistance.maxValue = OnSiteManager.Instance.settings.triggerMaxDistanceForPois;

                if (!PlayerPrefs.HasKey("MaxTriggerDistanceForPois"))
                {
                    sliderPoiDistance.maxValue = OnSiteManager.Instance.settings.triggerMaxDistanceForPois;
                    PlayerPrefs.SetFloat("MaxTriggerDistanceForPois", OnSiteManager.Instance.settings.triggerMaxDistanceForPois);
                }
                else
                {
                    sliderPoiDistance.maxValue = PlayerPrefs.GetFloat("MaxTriggerDistanceForPois");
                }

                if (PlayerPrefs.HasKey("triggerDistanceForPois"))
                {
                    sliderPoiDistance.value = PlayerPrefs.GetFloat("triggerDistanceForPois");
                }
                else
                {
                    sliderPoiDistance.value = OnSiteManager.Instance.settings.triggerStartDistanceForPois;
                }

                OnSiteManager.Instance.CurrentTriggerDistanceForPois = sliderPoiDistance.value;

                txtDistPoi.text = sliderPoiDistance.value.ToString() + " m";
            }
            else
            {
                btnToggleTourMode.gameObject.SetActive(false);
            }

            btnInfo.onClick.AddListener(ShowInfo);

            yield return new WaitForSeconds(0.5f);

            MapController mapController = FindObjectOfType<MapController>();
            if (mapController != null)
            {
                if (!mapController.settings.visual.applyVisualSettings)//not custom layer
                {
                    //set satellite as map default layer 
                    toggleItemLayers.SwapAction();
                }
            }

            //hide overlay
            if (!uiControl.settings.showOverlayAtStart)
            {
                TopographicsToggle();
            }

        }

        void ShowInfo()
        {
            InitSplash.Instance.ShowAsInfo();
            motionSideMenu.HidePanel();
        }

        void OnAllPeriodsSelected(PeriodEntity newestPeriod, List<PeriodEntity> previousPeriods = null)
        {
            //set all period buttons as selected
            //btnOttoman.image.ChangeAlpha(0f);
            //btnVenetian.image.ChangeAlpha(0f);
            //btnContemporary.image.ChangeAlpha(0f);

            toggleItemByzantine.SetPeriodEnabled(true);
            toggleItemPrehistoric.SetPeriodEnabled(true);
            toggleItemModern.SetPeriodEnabled(true);

            //create pois for all periods
            EventHolder.OnPeriodsSelected?.Invoke(newestPeriod, previousPeriods);
        }

        //193 otminoan
        //197 byzantine
        private Image GetPeriodImage(int periodID)
        {
            switch (periodID)
            {
                case 197:
                    return btnByzantine.image;
                case 196:
                    return btnModern.image;
                case 193:
                    return btnPrehistoric.image;
                default: 
                    return null;
            }
        }

        void OnDatabaseReaded()
        {
            if (Application.isEditor)
                Debug.Log("<color=yellow>OnDatabaseReaded</color>");

            //PeriodEntity period = DataManager.Instance.GetPeriodEntity(1684);
            //ottomanIcon.sprite = period.periodIcon.ToSprite();
            //period = DataManager.Instance.GetPeriodEntity(1685);
            //venetianIcon.sprite = period.periodIcon.ToSprite();
            //period = DataManager.Instance.GetPeriodEntity(1686);
            //contemporaryIcon.sprite = period.periodIcon.ToSprite();

            //switch (periodID)
            //{
            //    case 1684:
            //        return btnOttoman.image;
            //    case 1685:
            //        return btnVenetian.image;
            //    case 1686:
            //        return btnContemporary.image;
            //    default:
            //        return null;
            //}

        }

        void PeriodToggle(int periodID)
        {
            if (selectedRoute == null || selectedRoute.route.route_id <= 0) return;

            PeriodEntity periodSelected = selectedRoute.periods.Find(b => b.period.period_id == periodID);// DataManager.Instance.GetPeriodEntity(periodID);

            bool isActive = false;

            switch (periodID)
            {
                case 197:
                    toggleItemByzantine.SwapAction();
                    isActive = toggleItemByzantine.IsEnabled();
                    break;
                case 196:
                    toggleItemModern.SwapAction();
                    isActive = toggleItemModern.IsEnabled();
                    break;
                case 193:
                    toggleItemPrehistoric.SwapAction();
                    isActive = toggleItemPrehistoric.IsEnabled();
                    break;
                default:
                    break;
            }


            //transparent means period is selected
            EventHolder.OnPeriodToggleVisibility?.Invoke(periodSelected, isActive);
        }

       

        void OnSliderPoiDistance(Slider slider)
        {
            txtDistPoi.text = slider.value.ToString() + " m";
            OnSiteManager.Instance.CurrentTriggerDistanceForPois = slider.value;
            PlayerPrefs.SetFloat("triggerDistanceForPois", slider.value);
            EventHolder.OnPoiDistanceChanged?.Invoke(slider.value);
        }

        void OnTourChanged(EnumsHolder.TourMode tourMode)
        {
            switch (tourMode)
            {
                case EnumsHolder.TourMode.OffSite:
                default:
                    toggleItemTourMode.SetSprite(true);//default state
                    break;
                case EnumsHolder.TourMode.OnSite:
                    toggleItemTourMode.SetSprite(false);
                    break;

            }
        }

        void HideTourMode(int area_id)
        {
            btnToggleTourMode.gameObject.SetActive(false);
            OnTourChanged(EnumsHolder.TourMode.OffSite);
        }
        void ShowTourMode(int area_id)
        {
            btnToggleTourMode.gameObject.SetActive(true);
            OnTourChanged(EnumsHolder.TourMode.OnSite);
            EventHolder.OnTourChanged?.Invoke(EnumsHolder.TourMode.OnSite);
        }

        void OnMenuOpen(bool val) { menuPanelState = val ? EnumsHolder.GeneralPanelState.Open : EnumsHolder.GeneralPanelState.Close; }

        void OnAppStateChanged(EnumsHolder.AppState state)
		{
            appState = state;

            if(state == EnumsHolder.AppState.AreasView)
            {
                selectedArea = null;
                selectedRoute = null;
            }
        }

		void ChangeLanguage()
        {
			DataManager.Instance.ChangeLanguage();
		}

		void ToggleMenuPanel()
        {
			motionSideMenu.TogglePanelAppearance();
		}

		void MapLayerToggle()
		{
			toggleItemLayers.SwapAction();
			EventHolder.OnLayerToggle?.Invoke();
		}

        private int flagVisible = 0;
        public bool IsTopographVisible() { return flagVisible % 2 == 0; }

        public void ShowToggleTopographicButton(bool val)
        {
            btnToggleTopographics.gameObject.SetActive(val);
            lineTopographics.SetActive(val);
        }

        void TopographicsToggle() 
		{
            flagVisible++;
            toggleItemTopographics.SwapAction();
			EventHolder.OnTopographicsToggle?.Invoke();
		}

        void TourModeToggle()
        {
            //toggleItemTourMode.SwapAction();
            if (OnSiteManager.Instance.tourMode == EnumsHolder.TourMode.OffSite)
            {
                if (!OnSiteManager.Instance.IsGpsRunning() || !Input.location.isEnabledByUser)
                {

#if UNITY_ANDROID                 
                    // Make sure Android users uses fine precision location
                    if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                    {
                        var callbacks = new PermissionCallbacks();
                        callbacks.PermissionDenied += GPS_PermissionDenied;
                        callbacks.PermissionGranted += GPS_PermissionGranted;
                        callbacks.PermissionDeniedAndDontAskAgain += GPS_PermissionDeniedAndDontAskAgain;
                        Permission.RequestUserPermission(Permission.FineLocation, callbacks);
                        return;
                    }
#endif

                    MessageManager.Instance.DelayCheckGPS();
                    toggleItemTourMode.SwapAction();//.SetSprite(false);
                    EventHolder.OnTourChanged?.Invoke(EnumsHolder.TourMode.OffSite);
                    motionSideMenu.HidePanel();
                    return;
                }
                else 
                {
                    OnSiteManager.Instance.SearchNearestArea();

                    if (OnSiteManager.Instance.locationMode == EnumsHolder.LocationMode.FAR
                        || OnSiteManager.Instance.locationMode == EnumsHolder.LocationMode.NULL)
                    {
                        MessageManager.Instance.DelayCheckGPS();
                        toggleItemTourMode.SwapAction();//.SetSprite(false);
                        EventHolder.OnTourChanged?.Invoke(EnumsHolder.TourMode.OffSite);
                        motionSideMenu.HidePanel();
                        return;
                    }
                }
                EventHolder.OnTourChanged?.Invoke(EnumsHolder.TourMode.OnSite);
            }
            else
            {
                EventHolder.OnTourChanged?.Invoke(EnumsHolder.TourMode.OffSite);
            }
        }

#if UNITY_ANDROID

        internal void GPS_PermissionDeniedAndDontAskAgain(string permissionName)
        {
            //Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
        }

        internal void GPS_PermissionGranted(string permissionName)
        {
            //Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
        }

        internal void GPS_PermissionDenied(string permissionName)
        {
            //Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
        }

#endif

       

        void AccuracyToggle()
        {
            toggleItemAccuracyCheck.SwapAction();
            bool val = OnSiteManager.Instance.settings.useFilterOnGpsUpdateDistance;
            OnSiteManager.Instance.settings.useFilterOnGpsUpdateDistance = !val;
        }

        void ShowHideMenuButton(bool val) 
		{
			EventHolder.OnMenuOpen?.Invoke(val); 
			btnHideSideMenu.gameObject.SetActive(val); 
		}
    }

}
