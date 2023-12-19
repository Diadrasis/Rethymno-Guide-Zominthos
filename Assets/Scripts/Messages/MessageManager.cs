//Diadrasis ©2023 - Stathis Georgiou
using StaGeGames.BestFit;
using StaGeGames.BestFit.Extens;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno
{

    public class MessageManager : Singleton<MessageManager>
    {
        protected MessageManager() { }

        [Space]
        public EnumsHolder.ApplicationMode appModeNow = EnumsHolder.ApplicationMode.GUIDE;
        [Space]
        public EnumsHolder.AppState appState = EnumsHolder.AppState.None;
        [Space]
        public EnumsHolder.GeneralPanelState panelState = EnumsHolder.GeneralPanelState.Close;

        private JsonTermsDatabase messagesDb;

        [Header("[---UI Elements---]")]
        public GameObject canvasRoot;
        public RectTransform rectRoot;
        public SmartMotion motionPanel;
        public BestFitter fitter;
        public TMPro.TextMeshProUGUI txtTitle, txtDesc, txtActionBtn, txtCancelBtn;

        public Button btnAction, btnCancel;

        private string _title, _desc, _action;

        [ReadOnly]
        public keyMessage keyMessageTitle, keyMessageDesc, keyMessageAction, keyMessageCancel;


        void OnLanguageChanged()
        {
            txtActionBtn.text = GetTermText(keyMessageAction.ToString());
            txtCancelBtn.text = GetTermText(keyMessageCancel.ToString());

            _title = GetTermText(keyMessageTitle.ToString());

            _action = GetTermText(keyMessageAction.ToString());

            if (keyMessageDesc == keyMessage.total_filesize || keyMessageDesc == keyMessage.calculating_size)
            {
                string val = GetTermText(keyMessage.total_filesize.ToString()) + currentFileSize;
                _desc = val;
            }
            else
            {
                _desc = GetTermText(keyMessageDesc.ToString());
            }

            if (_title.IsNull() && _desc.IsNull()) return;

            ShowMessage(_title, _desc, _action);
        }

        public void SetDesc(string val)
        {
            txtDesc.text = val;
            txtDesc.gameObject.SetActive(!val.IsNull());
        }

        private int stateAreaViews;

        private void Awake()
        {
            motionPanel.OnShowStart.AddListener(() => EventHolder.OnMessageShow?.Invoke(true));
            motionPanel.OnHideStart.AddListener(() => EventHolder.OnMessageShow?.Invoke(false));
            motionPanel.OnHideComplete.AddListener(() => motionPanel.Init());

            fitter.IgnoreFitHeight = false;

            EventHolder.OnUpdateCompletedRequestUserToApplyChanges += OnUpdateCompletedRequestUserToApplyChanges;
            EventHolder.OnUpdateFinished += OnFirstUpdateFinished;
            EventHolder.OnUpdateFailed += ShowMessageUpdateFailed;
            EventHolder.OnUpdateAvailable += OnUpdateAvailable;

            EventHolder.OnBluetoothNotEnabled += ShowBluetoothNotEnabledMessage;
            EventHolder.OnCameraPermissionDenied += ShowCameraPermissionDeniedMessage;

            EventHolder.OnApplicationModeChanged += OnApplicationModeChanged;

            EventHolder.OnLanguageChanged += OnLanguageChanged;

            //check gps
            //EventHolder.OnSplashFinished += CheckGpsStatus;

            // if(!SaveLoadManager.IsApplicationFirstTimeUpdated())
            // EventHolder.OnUpdateFinished += DelayCheckGPS;
        }

        bool flagForUpdate;
        void OnUpdateAvailable()
        {
            flagForUpdate = true;
        }

        void Start()
        {
            canvasRoot.SetActive(true);
            fitter.IgnoreFitHeight = true;
            fitter.Init();

            messagesDb = DataManager.Instance.messagesDatabase;

            EventHolder.OnStateChanged += OnAppStateChanged;
            EventHolder.OnTryToQuit += TryToQuit;

            // motionPanel.OnHideComplete.AddListener(HideRootCanvas);
            btnCancel.onClick.AddListener(HideMessage);
            btnAction.onClick.RemoveAllListeners();

            EventHolder.OnMessageShow += OnMessageShow;
            EventHolder.OnMessageHide += HideMessage;
            EventHolder.OnGpsNear += OnGpsNear;
            EventHolder.OnGpsOn += OnGpsNear;
            EventHolder.OnTourChanged += OnTourChanged;

            EventHolder.OnPoiTriggeredWhileOnInfoView += OnPoiTriggeredWhileOnInfoView;
        }

        void OnPoiTriggeredWhileOnInfoView()
        {
            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(HideMessage);

            txtCancelBtn.text = GetTermText(keyMessage.button_ok.ToString());

            _title = GetTermText(keyMessage.near_new_poi.ToString());
            _desc = string.Empty;
            _action = string.Empty;

            keyMessageAction = keyMessage.NULL;
            keyMessageCancel = keyMessage.button_ok;
            keyMessageTitle = keyMessage.near_new_poi;
            keyMessageDesc = keyMessage.NULL;

            ShowMessage(_title, _desc, _action);
        }

        void OnUpdateCompletedRequestUserToApplyChanges()
        {
            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(HideMessage);
            btnCancel.onClick.AddListener(() => EventHolder.OnUpdateFinished?.Invoke());
            btnAction.onClick.RemoveAllListeners();

            txtCancelBtn.text = GetTermText(keyMessage.button_ok.ToString());

            _title = GetTermText(keyMessage.update_completed_request.ToString());
            _desc = string.Empty;
            _action = string.Empty;

            keyMessageAction = keyMessage.NULL;
            keyMessageCancel = keyMessage.button_ok;
            keyMessageTitle = keyMessage.update_completed_request;
            keyMessageDesc = keyMessage.NULL;

            ShowMessage(_title, _desc, _action);
        }

        void OnMessageShow(bool val)
        {
            panelState = val ? EnumsHolder.GeneralPanelState.Open : EnumsHolder.GeneralPanelState.Close;
        }

        int firstTimeGpsMessage;
        void OnGpsNear(int draft)
        {
            if (stateAreaViews <= 1) return;

            if (firstTimeGpsMessage == 0)
            {
                firstTimeGpsMessage++;
                DelayCheckGPS();
            }
        }

        void OnApplicationModeChanged(EnumsHolder.ApplicationMode mode)
        {
            appModeNow = mode;
        }

        void OnAppStateChanged(EnumsHolder.AppState state)
        {
            appState = state;
            CheckAppState();
        }

        void CheckAppState()
        {
            if (DataManager.Instance.IsMobile())
            {
                if (appState == EnumsHolder.AppState.AreasView)
                {
                    stateAreaViews++;
                    //if (stateAreaViews <= 1)//check updates only at start
                    //{
                    //    Invoke(nameof(DelayInitialize), 1.5f);
                    //}
                    if (flagForUpdate)
                    {
                        CheckStartUpdates(false);
                    }
                    else if (flagGpsStatus)
                    {
                        flagGpsStatus = false;
                        CheckGpsStatus();
                    }
                }
            }
        }

        public void InitializeAtStart()
        {
            if (stateAreaViews <= 1)//check updates only at start
            {
                Invoke(nameof(DelayInitialize), 1.5f);
            }
        }

        void DelayInitialize()
        {
            //check updates
            if (!CheckStartUpdates())//if internet is available update
            {
                //else if internet not available check gps status
                Invoke(nameof(CheckGpsStatus), 5f);
            }
        }

        private bool CheckStartUpdates(bool IsFirstTimeCheck = true)
        {
            if (IsFirstTimeCheck)
            {
                if (SaveLoadManager.IsApplicationFirstTimeUpdated()) return false;
            }

            if (!ServerManager.Instance.isInternetOn) return false;

            flagForUpdate = false;

            if (!ServerManager.Instance.IsWiFi())// Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                //ask user for CarrierDataNetwork permission

                btnCancel.onClick.RemoveAllListeners();
                btnCancel.onClick.AddListener(CancelCarrierDataNetwork);
                btnCancel.onClick.AddListener(ShowHelpFirstTime);

                btnAction.onClick.RemoveAllListeners();
                btnAction.onClick.AddListener(AcceptCarrierDataNetwork);
                btnAction.onClick.AddListener(HideMessage);
                btnAction.onClick.AddListener(ShowHelpFirstTime);

                txtActionBtn.text = GetTermText(keyMessage.button_update.ToString());
                txtCancelBtn.text = GetTermText(keyMessage.button_cancel.ToString());

                _title = GetTermText(keyMessage.carrier_network.ToString());
                _desc = GetTermText(keyMessage.calculating_size.ToString()); // string.Empty;
                _action = GetTermText(keyMessage.button_update.ToString());

                keyMessageAction = keyMessage.button_update;
                keyMessageCancel = keyMessage.button_cancel;
                keyMessageTitle = keyMessage.carrier_network;
                keyMessageDesc = keyMessage.total_filesize;


                ShowMessage(_title, _desc, _action);

                if (IsFirstTimeCheck)
                {
                    ServerManager.Instance.GetServerFilesSize(ShowFileSize);
                }
                else
                {
                    ShowFileSize(ServerManager.Instance.ImagesFileSize);
                }

            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                // Debug.Log("CheckStartUpdates 3");

                btnCancel.onClick.RemoveAllListeners();
                btnCancel.onClick.AddListener(HideMessage);
                btnCancel.onClick.AddListener(DelayCheckGPS);
                btnCancel.onClick.AddListener(ShowHelpFirstTime);

                btnAction.onClick.RemoveAllListeners();
                btnAction.onClick.AddListener(AcceptFirstUpdate);
                btnAction.onClick.AddListener(HideMessage);
                btnAction.onClick.AddListener(ShowHelpFirstTime);

                txtActionBtn.text = GetTermText(keyMessage.button_update.ToString());
                txtCancelBtn.text = GetTermText(keyMessage.button_cancel.ToString());

                _title = GetTermText(keyMessage.new_update.ToString());
                _desc = GetTermText(keyMessage.calculating_size.ToString()); //string.Empty;
                _action = GetTermText(keyMessage.button_update.ToString());

                keyMessageAction = keyMessage.button_update;
                keyMessageCancel = keyMessage.button_cancel;
                keyMessageTitle = keyMessage.new_update;
                keyMessageDesc = keyMessage.total_filesize;

                ShowMessage(_title, _desc, _action);

                if (IsFirstTimeCheck)
                {
                    ServerManager.Instance.GetServerFilesSize(ShowFileSize);
                }
                else
                {
                    ShowFileSize(ServerManager.Instance.ImagesFileSize);
                }

            }


            return true;
        }

        void ShowHelpFirstTime()
        {
            EventHolder.OnHelpFirstTime?.Invoke();
        }

        [ReadOnly]
        public string currentFileSize;
        void ShowFileSize(string s)
        {
            currentFileSize = s;
            keyMessageDesc = keyMessage.total_filesize;
            SetDesc(GetTermText(keyMessage.total_filesize.ToString()) + s);
        }

        private bool hasAcceptedFirstUpdate;

        private void AcceptFirstUpdate()
        {
            hasAcceptedFirstUpdate = true;

            ServerManager.Instance.StartUpdating();

        }

        void OnFirstUpdateFinished()
        {
            //if (!hasAcceptedFirstUpdate) return;
            //SaveLoadManager.SaveFirstTimeUpdated();
            DelayCheckGPS();
        }

        void ShowMessageUpdateFailed()
        {
            Invoke(nameof(OnUpdateFailed), 1.5f);
        }

        void OnUpdateFailed()
        {
            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(HideMessage);
            btnCancel.onClick.AddListener(DelayCheckGPS);
            btnAction.onClick.RemoveAllListeners();

            txtCancelBtn.text = GetTermText(keyMessage.button_ok.ToString());

            _title = GetTermText(keyMessage.update_failed.ToString());
            _desc = string.Empty;
            _action = string.Empty;

            keyMessageAction = keyMessage.NULL;
            keyMessageCancel = keyMessage.button_ok;
            keyMessageTitle = keyMessage.update_failed;
            keyMessageDesc = keyMessage.NULL;

            ShowMessage(_title, _desc, _action);
        }

        private void CancelCarrierDataNetwork()
        {
            HideMessage();
            DelayCheckGPS();
            ServerManager.Instance.UserAcceptCarrierDataNetwork = false;
        }

        private void AcceptCarrierDataNetwork()
        {
            HideMessage();
            ServerManager.Instance.UserAcceptCarrierDataNetwork = true;
            SaveLoadManager.SaveAcceptCarrierNetwork();
            AcceptFirstUpdate();
        }

        public void DelayCheckGPS()
        {
            if (Application.isEditor) Debug.Log("DelayCheckGPS");
            Invoke(nameof(CheckGpsStatus), 2.5f);
        }

        private bool IsNotInMenu() { return appState != EnumsHolder.AppState.AreasView; }

        void m_CheckGpsStatus(int x)
        {
            if (Application.isEditor) Debug.Log("m_CheckGpsStatus");
            DelayCheckGPS();
        }

        bool flagGpsStatus, userChooseTourMode;

        void UserHasChoosedTourMode() { userChooseTourMode = true; }

        void OnTourChanged(EnumsHolder.TourMode mode)
        {
            userChooseTourMode = false;
        }

        //first time entrance
        void CheckGpsStatus()
        {

            if (appState != EnumsHolder.AppState.AreasView)
            {
                flagGpsStatus = true;
                return;
            }

            //if (Application.isEditor) Debug.Log("CheckGpsStatus");

            if (OnSiteManager.Instance.IsGpsInitializing()) return;

            if (OnSiteManager.Instance.IsGpsDisabled())
            {
                //show gps is disabled message
                btnCancel.onClick.RemoveAllListeners();
                btnCancel.onClick.AddListener(HideMessage);
                btnAction.onClick.RemoveAllListeners();

                txtCancelBtn.text = GetTermText(keyMessage.button_ok.ToString());

                _title = GetTermText(keyMessage.gps_disabled.ToString());
                _desc = string.Empty;
                _action = string.Empty;

                keyMessageAction = keyMessage.NULL;
                keyMessageCancel = keyMessage.button_ok;
                keyMessageTitle = keyMessage.gps_disabled;
                keyMessageDesc = keyMessage.NULL;

                ShowMessage(_title, _desc, _action);

                if (DataManager.Instance.IsMobile())
                {
                    EventHolder.OnGpsNear += m_CheckGpsStatus;
                }
            }
            //check user gps
            else if (OnSiteManager.Instance.IsGpsRunning() == false)
            {
                //show gps is disabled message
                btnCancel.onClick.RemoveAllListeners();
                btnCancel.onClick.AddListener(HideMessage);
                btnAction.onClick.RemoveAllListeners();
                btnAction.onClick.AddListener(HideMessage);
                btnAction.onClick.AddListener(OnSiteManager.Instance.StartGPS);

                txtCancelBtn.text = GetTermText(keyMessage.button_cancel.ToString());

                _title = GetTermText(keyMessage.gps_initialize_failed.ToString());
                _desc = string.Empty;
                _action = GetTermText(keyMessage.button_ok.ToString());

                keyMessageAction = keyMessage.NULL;
                keyMessageCancel = keyMessage.button_cancel;
                keyMessageTitle = keyMessage.gps_initialize_failed;
                keyMessageDesc = keyMessage.NULL;

                ShowMessage(_title, _desc, _action);

                if (DataManager.Instance.IsMobile())
                {
                    EventHolder.OnGpsNear += m_CheckGpsStatus;
                }
            }
            else
            {
                if (DataManager.Instance.IsMobile())
                {
                    EventHolder.OnGpsNear -= m_CheckGpsStatus;
                }

                switch (OnSiteManager.Instance.locationMode)
                {
                    case EnumsHolder.LocationMode.NULL:
                    case EnumsHolder.LocationMode.FAR:
                    default:
                        //show location far message
                        btnCancel.onClick.RemoveAllListeners();
                        btnCancel.onClick.AddListener(HideMessage);
                        btnCancel.onClick.AddListener(OnSiteManager.Instance.StopGPS);
                        //btnCancel.onClick.AddListener(UserHasChoosedTourMode);
                        btnAction.onClick.RemoveAllListeners();

                        txtCancelBtn.text = GetTermText(keyMessage.button_ok.ToString());

                        _title = GetTermText(keyMessage.gps_far_areas.ToString());
                        _desc = string.Empty;
                        _action = string.Empty;

                        keyMessageAction = keyMessage.NULL;
                        keyMessageCancel = keyMessage.button_ok;
                        keyMessageTitle = keyMessage.gps_far_areas;
                        keyMessageDesc = keyMessage.NULL;

                        ShowMessage(_title, _desc, _action);

                        break;
                    case EnumsHolder.LocationMode.NEAR_AREA:

                        if (userChooseTourMode) break;
                        //show location near message
                        btnCancel.onClick.RemoveAllListeners();
                        btnCancel.onClick.AddListener(HideMessage);
                        btnCancel.onClick.AddListener(() => EventHolder.OnTourChanged?.Invoke(EnumsHolder.TourMode.OffSite));
                        btnCancel.onClick.AddListener(UserHasChoosedTourMode);

                        btnAction.onClick.RemoveAllListeners();
                        btnAction.onClick.AddListener(() => EventHolder.OnTourChanged?.Invoke(EnumsHolder.TourMode.OnSite));
                        btnAction.onClick.AddListener(HideMessage);
                        btnAction.onClick.AddListener(UserHasChoosedTourMode);

                        txtActionBtn.text = GetTermText(keyMessage.button_yes.ToString());
                        txtCancelBtn.text = GetTermText(keyMessage.button_no.ToString());

                        _title = GetTermText(keyMessage.gps_inside_area.ToString());
                        _desc = string.Empty;
                        _action = GetTermText(keyMessage.button_yes.ToString());

                        keyMessageAction = keyMessage.button_yes;
                        keyMessageCancel = keyMessage.button_no;
                        keyMessageTitle = keyMessage.gps_inside_area;
                        keyMessageDesc = keyMessage.NULL;

                        ShowMessage(_title, _desc, _action);

                        break;
                    case EnumsHolder.LocationMode.NEAR_POI:

                        //????

                        break;


                }

            }
        }

        bool isTryingToQuit;
        void TryToQuit()
        {
            if (IsNotInMenu()) return;

            //if (canvasRoot.activeSelf)
            //{
            //    HideMessage();
            //    return;
            //}

            //show quit options message
            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(HideMessage);
            btnAction.onClick.RemoveAllListeners();
            btnAction.onClick.AddListener(QuitApp);
            btnAction.onClick.AddListener(HideMessage);

            txtCancelBtn.text = GetTermText(keyMessage.button_cancel.ToString());

            _title = GetTermText(keyMessage.quitting_title.ToString());
            _desc = GetTermText(keyMessage.quitting_desc.ToString());
            _action = GetTermText(keyMessage.button_quit.ToString());

            isTryingToQuit = true;

            keyMessageAction = keyMessage.button_quit;
            keyMessageCancel = keyMessage.button_cancel;
            keyMessageTitle = keyMessage.quitting_title;
            keyMessageDesc = keyMessage.quitting_desc;

            ShowMessage(_title, _desc, _action);
        }

        public void ShowCameraPermissionDeniedMessage()
        {
            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(HideMessage);
            btnAction.onClick.RemoveAllListeners();

            txtCancelBtn.text = GetTermText(keyMessage.button_ok.ToString());

            _title = GetTermText(keyMessage.camera_denied.ToString());
            _desc = string.Empty;
            _action = string.Empty;

            keyMessageAction = keyMessage.NULL;
            keyMessageCancel = keyMessage.button_ok;
            keyMessageTitle = keyMessage.camera_denied;
            keyMessageDesc = keyMessage.NULL;

            ShowMessage(_title, _desc, _action);
        }

        public void ShowBluetoothNotEnabledMessage()
        {
            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(HideMessage);
            btnAction.onClick.RemoveAllListeners();

            txtCancelBtn.text = GetTermText(keyMessage.button_ok.ToString());

            _title = GetTermText(keyMessage.ble_disabled.ToString());
            _desc = string.Empty;
            _action = string.Empty;

            keyMessageAction = keyMessage.NULL;
            keyMessageCancel = keyMessage.button_ok;
            keyMessageTitle = keyMessage.ble_disabled;
            keyMessageDesc = keyMessage.NULL;

            ShowMessage(_title, _desc, _action);
        }


        void ShowMessage(string title, string desc, string action)
        {
            SetDesc(string.Empty);

            if (!isTryingToQuit)
                if (appModeNow == EnumsHolder.ApplicationMode.AR) return;

            canvasRoot.SetActive(true);
            txtTitle.text = title;
            txtDesc.text = desc;
            txtActionBtn.text = action;

            txtDesc.gameObject.SetActive(!desc.IsNull());
            btnAction.gameObject.SetActive(!action.IsNull());
            RefreshPanel();

            Invoke(nameof(RefreshPanel), 0.25f);
            motionPanel.ShowPanelWithTime(0.25f);
        }

        void HideMessage()
        {
            SetDesc(string.Empty);

            isTryingToQuit = false;
            btnAction.onClick.RemoveAllListeners();
            motionPanel.HidePanel();
        }

        void HideRootCanvas()
        {
            canvasRoot.SetActive(false);
        }

        void RefreshPanel()
        {
            rectRoot.ForceRebuildLayout();
            fitter.Init();
        }

        void QuitApp()
        {
            isTryingToQuit = true;
            if (!InitSplash.Instance.TryInit(true))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        private string GetTermText(string _key)
        {
            return DataManager.Instance.GetMessageText(_key);
        }

    }

}
