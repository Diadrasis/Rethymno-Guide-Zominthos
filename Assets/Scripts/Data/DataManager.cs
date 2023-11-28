//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class DataManager : Singleton<DataManager>
    {
        protected DataManager() { }


        private string langNow = "gr";
        public string LangNow() { return langNow; }
        public void SetLangNow() { Instance.langNow = SelectedLanguage == EnumsHolder.Language.GREEK ? "gr" : "en"; }

        public EnumsHolder.Platform platform = EnumsHolder.Platform.PC;
        [Space]
        public EnumsHolder.AppState appState = EnumsHolder.AppState.None;
        [Space]
        public EnumsHolder.Language SelectedLanguage = EnumsHolder.Language.GREEK;
        [Space]
        public JsonDataDatabase jsonDatabase;
        public JsonDataDatabase Database {  get { return Instance.jsonDatabase; } }
        [Space]
        public JsonTermsDatabase termsDatabase;
        [Space]
        public JsonTermsDatabase messagesDatabase;

        [Space]
        public bool UseServer;

        [Space]
        public bool SkipRoutesPanelIfOneAreaOneRoute = true;

        [Space]
        [Header("SELECT AREAS FOR THIS VERSION")]
        public int[] areasToShowIds;
        public bool useSelectedAreas;

        [Space]
        public bool skipPeriodInfo = true;

        public bool IsMobile() 
        {
#if UNITY_EDITOR 
            return platform == EnumsHolder.Platform.MOBILE;
#elif UNITY_STANDALONE
            return false;
#elif UNITY_ANDROID || UNITY_IOS
            return true;
#else
            return false;
#endif
        }

        //#if UNITY_EDITOR
        //        private void Update()
        //        {
        //            if (Input.GetKeyDown(KeyCode.Space)) ChangeLanguage();
        //        }
        //#endif

        [ContextMenu("Change Language")]
        public void ChangeLanguage()
        {
            switch (SelectedLanguage)
            {
                case EnumsHolder.Language.GREEK:
                    Instance.langNow = "en";
                    SelectedLanguage = EnumsHolder.Language.ENGLISH;
                    break;
                case EnumsHolder.Language.ENGLISH:
                    Instance.langNow = "gr";
                    SelectedLanguage = EnumsHolder.Language.FRENCH;
                    break;
                //case EnumsHolder.Language.FRENCH:
                //    Instance.langNow = "de";
                //    SelectedLanguage = EnumsHolder.Language.GERMAN;
                //    break;
                //case EnumsHolder.Language.GERMAN:
                //    Instance.langNow = "ru";
                //    SelectedLanguage = EnumsHolder.Language.RUSSIAN;
                //    break;
                //case EnumsHolder.Language.RUSSIAN:
                //    Instance.langNow = "gr";
                //    SelectedLanguage = EnumsHolder.Language.GREEK;
                //    break;
                default:
                    Instance.langNow = "gr";
                    SelectedLanguage = EnumsHolder.Language.GREEK;
                    break;
            }

            PlayerPrefs.SetString(nameof(langNow), Instance.langNow);

            EventHolder.OnLanguageChanged?.Invoke();
        }

        public void SetManualLanguage(EnumsHolder.Language lang)
        {

            switch (lang)
            {
                case EnumsHolder.Language.GREEK:
                    Instance.langNow = "gr";
                    SelectedLanguage = EnumsHolder.Language.GREEK;
                    break;
                case EnumsHolder.Language.ENGLISH:
                    Instance.langNow = "en";
                    SelectedLanguage = EnumsHolder.Language.ENGLISH;
                    break;
                case EnumsHolder.Language.FRENCH:
                    Instance.langNow = "fr";
                    SelectedLanguage = EnumsHolder.Language.FRENCH;
                    break;
                case EnumsHolder.Language.GERMAN:
                    Instance.langNow = "de";
                    SelectedLanguage = EnumsHolder.Language.GERMAN;
                    break;
                case EnumsHolder.Language.RUSSIAN:
                    Instance.langNow = "ru";
                    SelectedLanguage = EnumsHolder.Language.RUSSIAN;
                    break;
                default:
                    Instance.langNow = "gr";
                    SelectedLanguage = EnumsHolder.Language.GREEK;
                    break;
            }

            PlayerPrefs.SetString(nameof(langNow), Instance.langNow);

            EventHolder.OnLanguageChanged?.Invoke();
        }

        public bool HasSavedLanguage() { return PlayerPrefs.HasKey(nameof(langNow)); }

        private void Awake()
        {
            if(!Application.isEditor)
            {
                platform = Application.isMobilePlatform ? EnumsHolder.Platform.MOBILE : EnumsHolder.Platform.PC;
            }
            if (HasSavedLanguage())
            {
                //load language
                Instance.langNow = PlayerPrefs.GetString(nameof(langNow));

                SelectedLanguage = Instance.langNow switch
                {
                    "gr" => EnumsHolder.Language.GREEK,
                    "en" => EnumsHolder.Language.ENGLISH,
                    "fr" => EnumsHolder.Language.FRENCH,
                    "de" => EnumsHolder.Language.GERMAN,
                    "ru" => EnumsHolder.Language.RUSSIAN,
                    _ => EnumsHolder.Language.GREEK,
                };
            }
            else 
            {
                //set from inspector
                Instance.langNow = SelectedLanguage == EnumsHolder.Language.GREEK ? "gr" : "en";
            }
            EventHolder.OnStateChanged += OnStateChanged;

            //if (Application.isEditor)
            //Debug.Log("HasDeviceSavedJsons = " + SaveLoadManager.HasDeviceSavedJsons());

            //if (termsDatabase) termsDatabase.ReadJsonFilesOverridde();

        }
        void OnStateChanged(EnumsHolder.AppState state) { appState = state; }

        private void Start()
		{
            if (termsDatabase) termsDatabase.ReadJsonFilesOverridde();

            SaveLoadManager.CheckFolderExistsAndCreate();
            if(UseServer) ServerManager.Instance.Init();

            //read messages db again
            messagesDatabase.ReadJsonFilesOverridde();
        }

        public List<AreaEntity> AreaEntities() { return Database.areaEntities; }

        public RouteEntity FirstRoute() { return Database.GetFirstRoute(); }
        public PeriodEntity GetLastPeriod()
        {
            RouteEntity route = FirstRoute();
            int x = route.periods.Count - 1;
            return route.periods[x];    
        }

        public List<PeriodEntity> GetPreviousPeriods()
        {
            RouteEntity route = FirstRoute();
            int x = route.periods.Count - 1;
            List<PeriodEntity> periods = new();
            periods.AddRange(route.periods);//avoid parallel connection
            periods.Remove(GetLastPeriod());
            return periods;
        }

        public bool ServerFilesExistsInDisk()
        {

            List<string> imageFilesFromJson = Database.GetTotalImageFilesFromJsonsInDisk(out List<string> audios, out List<string> videos);
            int jsonDiskImages = imageFilesFromJson.Count;
            int imagesInDisk = SaveLoadManager.GetDiskImagesLength(out List<string> imageFilesInDisk);

            List<string> missingImages = new List<string>();

            //for each image in json
            foreach (string imageFromJson in imageFilesFromJson)
            {
                //check if image file is in disk 
                if (!imageFilesInDisk.Contains(imageFromJson.WithNoExtension()))
                {
                    //if not a big image
                    if (SaveLoadManager.IsImageInResources(imageFromJson))
                    {
                        //add in missing file
                        missingImages.Add(imageFromJson);
                    }
                }
            }

#if UNITY_EDITOR

            Debug.Log("missingFiles are " + missingImages.Count);
            Debug.Log("filesFromJson are " + imageFilesFromJson.Count);
#endif

            bool imagesOK = missingImages.Count < imagesInDisk / 3f;// == 0 && imagesInDisk > 0;
#if UNITY_EDITOR
            Debug.LogFormat(imagesOK + " json images = {0} and disk images = {1}", jsonDiskImages, imagesInDisk);
#endif
            if (!imagesOK) return false;

            int jsonDiskAudios = audios.Count;// Database.GetTotalAudioFilesFromJsonsInDisk();
            int audiosInDisk = SaveLoadManager.GetDiskAudiosLength();
            float diaf = jsonDiskAudios - audiosInDisk;
            bool audiosOK = diaf == 0 ? true : diaf < jsonDiskAudios/3f;
#if UNITY_EDITOR
            Debug.LogFormat(audiosOK + " json audios = {0} and disk audios = {1}", jsonDiskAudios, audiosInDisk);
#endif
            if (!audiosOK) return false;

            int jsonDiskVideos = videos.Count;// Database.GetTotalVideoFilesFromJsonsInDisk();
            int videosInDisk = SaveLoadManager.GetDiskVideosLength();
            diaf = jsonDiskVideos - videosInDisk;
            bool videosOK = diaf == 0 ? true : diaf < jsonDiskVideos / 3f;

#if UNITY_EDITOR
            Debug.LogFormat(videosOK + " json videos = {0} and disk videos = {1}", jsonDiskVideos, videosInDisk);
#endif

            //return true;
            return videosOK;
        }

        public void DeletedOldData()
        {

        }

        /// <summary>
        /// returns true if jsons exists in disk and have all data
        /// </summary>
        /// <returns></returns>
        public bool UpdateDatabaseFromDisk()
        {
            //if (Instance.jsonDatabase.GetTotalImageFilesInDiskFromDiskJsons() != SaveLoadManager.GetDiskImagesLength())
            //    return false;
            return Database.UpdateDatabaseFromDisk();
        }

        public void ReadDefaultJsonFiles()
        {
            Database.ReadDefaultJsonFilesOverridde();
        }

        public bool HasAnyData()
        {
            return Database != null && Database.HasAnyData();
        }

        public void PrepareDataToRead()
        {
            if (Instance.ServerFilesExistsInDisk())
            {
                if (!Instance.UpdateDatabaseFromDisk())
                {
#if UNITY_EDITOR
                    Debug.Log("<color=red>[ERROR]</color> Missing files found = loading from resources");
#endif
                    Instance.ReadDefaultJsonFiles();
                }
            }
            else
            {
#if UNITY_EDITOR
                    Debug.Log("<color=red>[ERROR]</color> Missing files found = loading from resources");
#endif
                Instance.ReadDefaultJsonFiles();
            }
        }

        public bool AreaHasOneRoute(AreaEntity area)
        {
            if (!SkipRoutesPanelIfOneAreaOneRoute) return false;
            return Database.areaEntities.Count == 1 && area.routes.Count == 1;
        }

        public List<string> GetImagesUniqueFilenames() 
        {
            List<string> images = Database.GetImagesUniqueFilenames();
            List<string> not_existing_images = new List<string>();
            string clearName = string.Empty;
            foreach (var img in images)
            {
                clearName = img.Trim();
                if (!SaveLoadManager.IsFileImageExist(clearName, false))//save image even if is in resources for later check
                {
                    if (!clearName.IsNull())
                        not_existing_images.Add(clearName);
                }
            }
            
            return not_existing_images;
        }
        public List<string> GetAudiosUniqueFilenames()
        {
            List<string> audios = Database.GetAudiosUniqueFilenames();
            List<string> not_existing_audios = new List<string>();
            string clearName = string.Empty;
            foreach (var aud in audios)
            {
                clearName = aud.Trim();
                if (!SaveLoadManager.IsFileAudioExist(clearName))
                    if (!clearName.IsNull())
                        not_existing_audios.Add(clearName);
            }
            return not_existing_audios;
        }


    public List<string> GetVideosUniqueFilenames() { return Database.GetVideosUniqueFilenames(); }

        public List<string> GetPoisFirstImagesUniqueFilenames() { return Database.GetPoisFirstImagesUniqueFilenames(); }

        public List<string> GetBeaconsUUIDs() { return Database.GetBeaconsUUIDs(); }

        /// <summary>
        /// returns the text for current language
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="localizedTexts"></param>
        /// <returns></returns>
        public string GetTraslatedText(cLocalizedText[] localizedTexts)
        {
            string val = string.Empty;
            if (localizedTexts == null) return val;
            foreach (cLocalizedText _text in localizedTexts)
            {
                if (_text.key == Instance.LangNow()) val = _text.text;
            }
            return val;
        }


        public string GetTermText(string _key)
        {
            cLocalizedText[] texts = termsDatabase.GetText(_key);
            return texts == null ? "" : Instance.GetTraslatedText(texts);
        }

        public string GetMessageText(string _key)
        {
            cLocalizedText[] texts = messagesDatabase.GetText(_key);
            return texts == null ? string.Empty : Instance.GetTraslatedText(texts);
        }

        public AreaEntity GetAreaEntity(string area_id) { return Database.GetAreaEntity(area_id); }
        public AreaEntity GetAreaEntityFromPeriod(PeriodEntity period)
        {
           return Database.GetAreaFromPeriod(period);
        }
        public PoiEntity GetPoiEntity(string poi_id) { return Database.GetPoiEntity(poi_id); }
        public PoiEntity GetBeaconPoiEntity(string beacon_id) { return Database.GetBeaconPoiEntity(beacon_id); }
        public PeriodEntity GetPeriodEntity(int periodID) 
        {
            List<PeriodEntity> periods = Database.GetFirstRoute().periods;
            return periods.Find(b=>b.period.period_id == periodID); 
        }

        #region EDITOR ONLY
#if UNITY_EDITOR
        [ContextMenu("OPEN DATA FOLDER")]
        void OpenDataFolder()
        {
            SaveLoadManager.OpenDataFolder();
        }

        [ContextMenu("DELETE ALL DATA")]
        void DeleteData()
        {
            PlayerPrefs.DeleteAll();
            SaveLoadManager.DeleteAllDataFolders();
        }
#endif

        #endregion

    }

}
