//Diadrasis ©2023
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static OnlineMapsAMapSearchResult;

namespace Diadrasis.Rethymno
{

    public class ServerManager : Singleton<ServerManager>
    {
        protected ServerManager() { }

        public ServerSettings settings;

        [Space]
        [ReadOnly]
        public bool isInternetOn;
        [ReadOnly]
        public NetworkReachability networkReachability = NetworkReachability.NotReachable;

        [ReadOnly]
        [SerializeField]
        private bool userAcceptCarrierDataNetwork;
        public bool UserAcceptCarrierDataNetwork { get { return userAcceptCarrierDataNetwork; } set { userAcceptCarrierDataNetwork = value; } }
        public bool IsWiFi()
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork
                   || (userAcceptCarrierDataNetwork && Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork);
        }


        [Header("[---UI Elements---]")]
        public GameObject CanvasUpdate;//, panelFirstTime;//, panelSecondTime;
        public UnityEngine.UI.Slider slider;
        public TMPro.TextMeshProUGUI txt, txtProgress;


        private readonly string lastUpdateSucceedKey = "lastUpdateSucceeded";
        private bool isCommunicatingWithServer;

        [ReadOnly]
        public int MaxTextureSize;


        List<string> missingImages = new List<string>();
        private float tempSizeForMissingImages;
        public string ImagesFileSize { get { return tempSizeForMissingImages.ToString()+" Mb"; }  }

        private void Awake()
        {
            tempSizeForMissingImages = 1;

            userAcceptCarrierDataNetwork = SaveLoadManager.IsUserAcceptedCarrierDataNetwork();

            EventHolder.OnFilesDownloadComplete += OnFilesDownloadComplete;
            EventHolder.OnJsonsDownloadComplete += OnJsonsDownloadComplete;
            EventHolder.OnDownloadJsonFailed += DownloadJsonFailed;
            EventHolder.OnUpdateFinished += OnUpdateComplete;
            EventHolder.OnUpdateFailed += OnUpdateFailed;

#if !UNITY_EDITOR
            settings.EditorDebug = false;
#endif
        }

        private void Start()
        {
             MaxTextureSize = DeviceChecker.Instance.MaxTextureSize;// SystemInfo.maxTextureSize;
             MaxTextureSize = MaxTextureSize < 4096 ? MaxTextureSize : 4096;//we are on mobile where max size is 2048

            //MaxTextureSize = 2048;


            //check for new update
            if (SaveLoadManager.IsApplicationFirstTimeUpdated())
            {
                Invoke(nameof(DelayInitialize), 1.5f);
            }
        }

        public void Init()
        {
            StartCoroutine(CheckInternetConnection());
        }

        #region CheckInternetConnection

        //executed every >> settings.internetCheckTimeInterval
        private IEnumerator CheckInternetConnection()
        {
            OnlineMaps.instance.CheckServerConnection(OnCheckConnectionComplete);
            yield return new WaitForSeconds(3f);
            while (true)
            {
                OnlineMaps.instance.CheckServerConnection(OnCheckConnectionComplete);
                yield return new WaitForSeconds(settings.internetCheckTimeInterval);
            }
        }

        private void OnCheckConnectionComplete(bool status)
        {
            isInternetOn = status;

            networkReachability = Application.internetReachability;
        }

        #endregion

        #region Check for new updates

        void DelayInitialize()
        {
            StartCoroutine(ReadServerJsons());
        }

        /// <summary>
        /// reads bytes on the fly
        /// </summary>
        /// <returns>if any update is available</returns>
        private IEnumerator ReadServerJsons()
        {

            List<cArea> cAreas = new List<cArea>();
            List<cRoute> cRoutes = new List<cRoute>();
            List<cRouteType> cRouteTypes = new List<cRouteType>();
            List<cPeriod> cPeriods = new List<cPeriod>();
            List<cPoi> cPois = new List<cPoi>();
            List<cImage> cImages = new List<cImage>();
            List<cIcon> cIcons = new List<cIcon>();
            List<cVideo> cVideos = new List<cVideo>();
            List<cBeacon> cBeacons = new List<cBeacon>();

            float t = Time.realtimeSinceStartup;

            string serverfolderurl = settings.ServerRootFolder();
            List<string> JsonFiles = GlobalUtils.jsonExportFiles.ToList();

            string txt = string.Empty;
            string _result = string.Empty;

            foreach (string jsonName in JsonFiles)
            {
                using (var uwr = UnityWebRequest.Get(new Uri(serverfolderurl + jsonName)))
                {
                    uwr.disposeDownloadHandlerOnDispose = true;

                    yield return uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                    {
                        if (settings.EditorDebug) Debug.Log(jsonName + " ERROR." + uwr.error);
                        uwr.Dispose();
                        continue;
                    }
                    else if (uwr.result == UnityWebRequest.Result.Success)
                    {
                        txt = uwr.downloadHandler.text.Trim();

                        if (txt.IsNull()) continue;

                        _result = JsonHelper.FixJson(txt);

                        if (jsonName == GlobalUtils.jsonAreasFilename)
                        {
                            cAreas = JsonHelper.FromJson<cArea>(_result).ToList();
                        }
                        else if (jsonName == GlobalUtils.jsonRoutesFilename)
                        {
                            cRoutes = JsonHelper.FromJson<cRoute>(_result).ToList();
                        }
                        else if (jsonName == GlobalUtils.jsonRouteTypesFilename)
                        {
                            cRouteTypes = JsonHelper.FromJson<cRouteType>(_result).ToList();
                        }
                        else if (jsonName == GlobalUtils.jsonPeriodsFilename)
                        {
                            cPeriods = JsonHelper.FromJson<cPeriod>(_result).ToList();
                        }
                        else if (jsonName == GlobalUtils.jsonPoisFilename)
                        {
                            cPois = JsonHelper.FromJson<cPoi>(_result).ToList();
                        }
                        else if (jsonName == GlobalUtils.jsonImagesFilename)
                        {
                            cImages = JsonHelper.FromJson<cImage>(_result).ToList();
                        }
                        else if (jsonName == GlobalUtils.jsonVideosFilename)
                        {
                            cVideos = JsonHelper.FromJson<cVideo>(_result).ToList();
                        }
                    }

                    uwr.Dispose();
                }

            }

            if (settings.EditorDebug)
            {
                Debug.LogWarning("<color=yellow>DONE Downloading and Reading JSONS >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> </color>");
                Debug.Log("<color=yellow>(!) Check if Disk Files = Jsons Files</color>");
            }

            List<string> imageFilesFromJson = new List<string>();
            List<string> audioFilesFromJson = new List<string>();
            List<string> videoFilesFromJson = new List<string>();


            #region new logic

            List<cArea> myAreas = new List<cArea>();
            if (DataManager.Instance.useSelectedAreas)
            {
                int[] ids = DataManager.Instance.areasToShowIds;

                if (ids.Length > 0)
                {
                    foreach (int id in ids)
                    {
                        myAreas.Add(cAreas.Find(b => b.area_id == id));
                    }
                }
            }

            if (myAreas.Count <= 0) { myAreas.Clear(); myAreas = cAreas; }
            else
            {
                cAreas = myAreas;
            }

            foreach (cArea area in cAreas)
            {
                //ADD AREA ICON & IMAGE
                imageFilesFromJson.AddToList(area.area_icon);
                imageFilesFromJson.AddToList(area.area_image);

                List<cImage> myImages = cImages.FindAll(b => b.poi_id == area.area_id);

                //ADD AREA IMAGES
                if (myImages.Count > 0)
                {
                    foreach (cImage image in myImages)
                    {
                        imageFilesFromJson.AddToList(image.image_file);
                    }
                }

                //get routes for this area
                List<cRoute> areaRoutes = cRoutes.FindAll(b => b.area_id == area.area_id);

                foreach (cRoute route in areaRoutes)
                {

                    myImages = cImages.FindAll(b => b.poi_id == route.route_id);

                    //ADD ROUTE IMAGES
                    if (myImages.Count > 0)
                    {
                        foreach (cImage image in myImages)
                        {
                            imageFilesFromJson.AddToList(image.image_file);
                        }
                    }

                    //group pois to periods
                    foreach (cPeriod _period in cPeriods)
                    {
                        //find pois for this period and create poi entities
                        List<cPoi> poisOfPeriod = cPois.FindAll(b => b.period_id == _period.period_id && b.route_id == route.route_id);

                        foreach (cPoi p in poisOfPeriod)
                        {
                            myImages = cImages.FindAll(b => b.poi_id == p.poi_id);

                            cPeriod myPeriod = cPeriods.Find(b => b.period_id == p.period_id);

                            if (myPeriod.period_id > 0)
                            {
                                imageFilesFromJson.AddToList(_period.period_icon);
                                imageFilesFromJson.AddToList(_period.period_poi_icon);
                                imageFilesFromJson.AddToList(_period.period_poi_icon_active);
                                imageFilesFromJson.AddToList(_period.period_poi_icon_visited);
                            }

                            //ADD POI IMAGES
                            if (myImages.Count > 0)
                            {
                                foreach (cImage image in myImages)
                                {
                                    imageFilesFromJson.AddToList(image.image_file);
                                }
                            }

                            foreach (cLocalizedText narr in p.poi_narration)
                                audioFilesFromJson.AddToList(narr.text);

                            if (cVideos.Count > 0)
                            {
                                cVideo video = cVideos.Find(b => b.poi_id == p.poi_id);
                                videoFilesFromJson.AddToList(video.video_file);
                            }

                        }
                    }
                }

            }



            #endregion

            #region deprecated
            /*

            foreach (cImage img in cImages)
            {
                imageFilesFromJson.AddToList(img.image_file);
            }

            foreach (cArea area in cAreas)
            {
                imageFilesFromJson.AddToList(area.area_icon);
                imageFilesFromJson.AddToList(area.area_image);
            }

            foreach (cRouteType rout in cRouteTypes)
            {
                imageFilesFromJson.AddToList(rout.route_type_icon);
                imageFilesFromJson.AddToList(rout.route_type_poi_icon);
                imageFilesFromJson.AddToList(rout.route_type_poi_icon_active);
                imageFilesFromJson.AddToList(rout.route_type_poi_icon_visited);
            }

            foreach (cPeriod per in cPeriods)
            {
                imageFilesFromJson.AddToList(per.period_icon);
                imageFilesFromJson.AddToList(per.period_poi_icon);
                imageFilesFromJson.AddToList(per.period_poi_icon_active);
                imageFilesFromJson.AddToList(per.period_poi_icon_visited);
            }

            foreach (cVideo vid in cVideos)
            {
                videoFilesFromJson.AddToList(vid.video_file);
            }

            foreach (cPoi poi in cPois)
            {
                foreach (cLocalizedText narr in poi.poi_narration)
                    audioFilesFromJson.AddToList(narr.text);
            }

            */
            #endregion

            int totalImageFiles = imageFilesFromJson.Count;
            int totalAudioFiles = audioFilesFromJson.Count;
            int totalVideoFiles = videoFilesFromJson.Count;

            int audiosInDisk = SaveLoadManager.GetDiskAudiosLength();
            int imagesInDisk = SaveLoadManager.GetDiskImagesLength(out List<string> imageFilesInDisk);
            int videosInDisk = SaveLoadManager.GetDiskVideosLength();

            missingImages.Clear();

            //for each image in json
            foreach (string imageFromJson in imageFilesFromJson)
            {
                //check if image file is in disk 
                if (!imageFilesInDisk.Contains(imageFromJson.WithNoExtension()))
                {
                    //add in missing file
                    missingImages.Add(imageFromJson);
                }
            }

            foreach (string img in missingImages) Debug.LogWarning(img);

            int missingAudios = totalAudioFiles - audiosInDisk;
            int missingVideos = totalVideoFiles - videosInDisk;

            if (settings.EditorDebug) Debug.LogWarningFormat("Missing Images = {0}", missingImages.Count);
            if (settings.EditorDebug) Debug.LogWarningFormat("Missing Audios = {0}", missingAudios);
            if (settings.EditorDebug) Debug.LogWarningFormat("Missing Videos = {0}", missingVideos);

            if (missingImages.Count > 5)//should we use a percent (eg. 80%) for missing files instead of none?
            {
                tempSizeForMissingImages = missingImages.Count / 2f;//if images are 3 then total size will be 1.5Mb
                ShowMessageUpdate();
            }
            else
            if (missingVideos > 3)//(videosInDisk != totalVideoFiles)
            {
                if (settings.EditorDebug) Debug.LogWarning("[Videos] NEW UPDATE AVAILABLE");
                ShowMessageUpdate();
            }
            else
            if (missingAudios > 3)//(audiosInDisk != totalAudioFiles)
            {
                if (settings.EditorDebug) Debug.LogWarning("[Audios] NEW UPDATE AVAILABLE");
                ShowMessageUpdate();
            }
            else
            {
                if (settings.EditorDebug)
                {
                    Debug.Log("<color=yellow>(!)All DONE No update needed >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>></color>");
                }
            }
        }

        /// <summary>
        /// show message: there is a new update
        /// </summary>
        void ShowMessageUpdate()
        {
            if (settings.EditorDebug) Debug.Log("<color=yellow>(!)Show Message Update</color>");
            EventHolder.OnUpdateAvailable?.Invoke();
        }


        void OnUpdateFailed()
        {
            PlayerPrefs.SetInt(lastUpdateSucceedKey, 0);
        }

        void OnUpdateComplete()
        {
            PlayerPrefs.SetInt(lastUpdateSucceedKey, 1);
        }

        #endregion

        #region Download Files
        public void StartUpdating()
        {
            if (!isInternetOn)
            {
                EventHolder.OnInternetLost?.Invoke();
                return;
            }
            if (!IsWiFi())
            {
                //open messages panel
                //inform user that is not connected to wifi
                //and if accepts to download updates
                //set userAcceptCarrierDataNetwork to true
                //and then invoke EventHolder.OnUpdateStart
                EventHolder.OnCarrierDataNetwork?.Invoke();
                return;
            }

            if (!isCommunicatingWithServer)
            {
                isCommunicatingWithServer = true;
                CanvasUpdate.SetActive(true);

                GetDownloadJsonFiles();
            }
        }

        void GetDownloadJsonFiles()
        {
            txt.text = GetTermText(keyMessage.downloading_data);
            FileDownloader.Instance.DownloadJsonFiles();
        }

        void OnJsonsDownloadComplete()
        {
            if (DataManager.Instance.UpdateDatabaseFromDisk())
            {
                GetDownloadMultimediaFiles();
            }
            else
            {
                DownloadFailed();
            }
        }

        void GetDownloadMultimediaFiles()
        {
            txt.text = GetTermText(keyMessage.downloading_files);
            FileDownloader.Instance.DownloadMultimediaFiles();//.DownloadAsync();
        }

        void OnFilesDownloadComplete()
        {
            txt.text = string.Empty;

            if (settings.EditorDebug)
                Debug.LogWarning("<color=green>Downloading files COMPLETED</color>");

            SaveLoadManager.SaveFirstTimeUpdated();

            EventHolder.OnUpdateFinished?.Invoke();

            CanvasUpdate.SetActive(false);
            isCommunicatingWithServer = false;

           // StartCoroutine(ContinueDownload());
        }

        void DownloadJsonFailed(string json)
        {
            //get locally assign json text
            string v = DataManager.Instance.Database.GetJson(json).Trim();
            //try to save as json file if not exists
            SaveLoadManager.SaveStringAsJsonFile(v, json, false);
            if (settings.EditorDebug)
                Debug.LogWarning("[ERROR] Failed to retrieve jsons, save local json");
        }

        void DownloadFailed()
        {
            if (settings.EditorDebug)
                Debug.LogWarning("[ERROR] Database failed to update");

            CanvasUpdate.SetActive(false);
            isCommunicatingWithServer = false;
            EventHolder.OnUpdateFailed?.Invoke();
            //should we reload default database?
        }

        #endregion

        [ContextMenu("GetServerImagesSize")]
        public void GetServerFilesSize(Action<string> callback)
        {
            StartCoroutine(GetServerRootDataSize(callback));// (GetServerDataFilesSize));
        }
        IEnumerator GetServerRootDataSize(Action<string> callback)
        {
            UnityWebRequest www = UnityWebRequest.Get(settings.PHP_GetSizeURL());
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                www.Dispose();
                yield return new WaitForEndOfFrame();
                //try again
                www = UnityWebRequest.Get(settings.PHP_GetSizeURLfallback());
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                    callback(string.Empty);
                    yield break;
                }
            }

            if (int.TryParse(www.downloadHandler.text, out int siz))
            {
                float p = 1f * siz / 2f;
                callback(p.ToString("F2"));
            }
            else
            {
                callback(www.downloadHandler.text); //(totalServerDataFileSize.ToSize(MathExtensions.SizeUnits.MB) + " Mb");
            }
        }

        private string GetTermText(keyMessage _key)
        {
            return DataManager.Instance.GetMessageText(_key.ToString());
        }




    }

}
