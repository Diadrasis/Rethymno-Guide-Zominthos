//Diadrasis ©2023
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static OnlineMapsAMapSearchResult;

namespace Diadrasis.Rethymno
{
    [CreateAssetMenu(fileName = "Data_Database", menuName = "Data/New Data Database")]
    [Serializable]
    public class JsonDataDatabase : ScriptableObject
    {

        public TextAsset jsonAreas, jsonImages, jsonPeriods, jsonPois, jsonRouteTypes, jsonRoutes, jsonVideos, jsonBeacons;


        [HideInInspector]
        [SerializeField]
        private List<cArea> cAreas = new List<cArea>();
        [HideInInspector]
        [SerializeField]
        private List<cRoute> cRoutes = new List<cRoute>();
        [HideInInspector]
        [SerializeField]
        private List<cRouteType> cRouteTypes = new List<cRouteType>();
        [HideInInspector]
        [SerializeField]
        private List<cPeriod> cPeriods = new List<cPeriod>();
        [HideInInspector]
        [SerializeField]
        private List<cPoi> cPois = new List<cPoi>();
        [HideInInspector]
        [SerializeField]
        private List<cImage> cImages = new List<cImage>();
        [HideInInspector]
        [SerializeField]
        private List<cIcon> cIcons = new List<cIcon>();
        [HideInInspector]
        [SerializeField]
        private List<cVideo> cVideos = new List<cVideo>();
        //[HideInInspector]
        [SerializeField]
        private List<cBeacon> cBeacons = new List<cBeacon>();

        [HideInInspector]
        [SerializeField]
        private List<string> imagesFilenames = new List<string>();
        [HideInInspector]
        [SerializeField]
        private List<string> audiosFilenames = new List<string>();
        [HideInInspector]
        [SerializeField]
        private List<string> videosFilenames = new List<string>();
        [HideInInspector]
        [SerializeField]
        private List<string> beaconsUUIDs = new List<string>();
        public List<string> GetBeaconsUUIDs() { return beaconsUUIDs; }

        public PoiEntity GetBeaconPoiEntity(string beaconUUID)
        {
            //if (cBeacons.Count <= 0) return null;
            //cBeacon beacon = cBeacons.Find(b => b.beacon_uuid.Replace("-","") == beaconUUID || b.beacon_uuid == beaconUUID);
            //if (beacon == null) return null;

            if (beaconsUUIDs.Count <= 0 || beaconUUID.IsNull()) return null;
            string id = beaconUUID.Substring(beaconUUID.Length - 4);
            PoiEntity poiEntity = poiEntities.Find(b => b.ID.ToString() == id);
            return poiEntity;
        }

        public List<cArea> GetAreas() { return cAreas; }

        public List<string> GetImagesUniqueFilenames() { return imagesFilenames; }
        public List<string> GetAudiosUniqueFilenames() { return audiosFilenames; }
        public List<string> GetVideosUniqueFilenames() { return videosFilenames; }

        public List<cPeriod> GetPeriods() { return cPeriods; }
        public List<cRouteType> GetRouteTypes() { return cRouteTypes; }

        [Space]
        public int totalAreas;
        public int totalRoutes, totalRouteTypes, totalPeriods, totalPois, totalImages, totalIcons, totalVideos;
       
        [Header("Unique filenames")]
        public int totalImageFiles;
        public int totalVideoFiles;
        public int totalAudioFiles;

        [Space]
        public int totalPoiEntities;

        [Space]
        [HideInInspector]
        public List<AreaEntity> areaEntities = new List<AreaEntity>();

        [HideInInspector]
        [SerializeField]
        private List<RouteEntity> routeEntities = new List<RouteEntity>();

        [HideInInspector]
        [SerializeField]
        private List<PoiEntity> poiEntities = new List<PoiEntity>();

        public AreaEntity GetAreaEntity(string area_id) { return areaEntities.Find(b => b.ID.ToString() == area_id); }
        public PoiEntity GetPoiEntity(string poi_id) { return poiEntities.Find(b => b.ID.ToString() == poi_id); }

        public AreaEntity GetAreaFromPeriod(PeriodEntity period)
        {
            RouteEntity routeEntity = routeEntities.Find(b=>b.periods.Contains(period));
            if (routeEntity == null) return null;
            return areaEntities.Find(b=>b.ID == routeEntity.route.area_id);
        }
                     
        public RouteEntity GetFirstRoute() { return routeEntities[0]; }
        public void GetAllUniqueFilesFromEntities()
        {
            if(Application.isEditor) Debug.LogWarning("GetAllUniqueFilesFromEntities");


            imagesFilenames.Clear();
            audiosFilenames.Clear();
            videosFilenames.Clear();
            beaconsUUIDs.Clear();

            foreach (cImage img in cImages)
            {
                imagesFilenames.AddToList(img.image_file);
            }

            foreach (cArea area in cAreas)
            {
                imagesFilenames.AddToList(area.area_icon);
                imagesFilenames.AddToList(area.area_image);
            }

            foreach(cRoute route in cRoutes)
            {
                imagesFilenames.AddToList(route.route_icon);
            }

            foreach (cRouteType routeType in cRouteTypes)
            {
                imagesFilenames.AddToList(routeType.route_type_icon);
                imagesFilenames.AddToList(routeType.route_type_poi_icon);
                imagesFilenames.AddToList(routeType.route_type_poi_icon_active);
                imagesFilenames.AddToList(routeType.route_type_poi_icon_visited);
            }

            foreach (cPeriod per in cPeriods)
            {
                imagesFilenames.AddToList(per.period_icon);
                imagesFilenames.AddToList(per.period_poi_icon);
                imagesFilenames.AddToList(per.period_poi_icon_active);
                imagesFilenames.AddToList(per.period_poi_icon_visited);
            }

            foreach (cVideo vid in cVideos)
            {
                videosFilenames.AddToList(vid.video_file);
            }

            foreach (cPoi poi in cPois)
            {
                foreach (cLocalizedText narr in poi.poi_narration)
                    audiosFilenames.AddToList(narr.text);

                beaconsUUIDs.AddToList(poi.poi_id.ToString());
            }

            totalImageFiles = imagesFilenames.Count;
            totalAudioFiles = audiosFilenames.Count;
            totalVideoFiles = videosFilenames.Count;

#if UNITY_EDITOR
            Debug.LogFormat("Total Unique Image files are {0}", imagesFilenames.Count);
            Debug.LogFormat("Total Unique Video files are {0}", videosFilenames.Count);
            Debug.LogFormat("Total Unique Audio files are {0}", audiosFilenames.Count);
#endif
        }

        void GetUniqueFilenamesFromSelectedAreas()
        {

            if (Application.isEditor) Debug.LogWarning("GetUniqueFilenamesFromSelectedAreas");

            imagesFilenames.Clear();
            audiosFilenames.Clear();
            videosFilenames.Clear();
            beaconsUUIDs.Clear();
            List<cImage> myImages = new List<cImage>();

            foreach (AreaEntity areaEntity in areaEntities)
            {
                imagesFilenames.AddToList(areaEntity.area.area_icon);
                imagesFilenames.AddToList(areaEntity.area.area_image);
                myImages = cImages.FindAll(b => b.poi_id == areaEntity.ID);

                if (myImages.Count > 0)
                {
                    foreach (cImage image in myImages)
                    {
                        imagesFilenames.AddToList(image.image_file);
                    }
                }
            }

            List<cImage> poiImages = new List<cImage>();
            List<cVideo> poiVideos = new List<cVideo>();

            foreach (RouteEntity routeEntity in routeEntities)
            {
                imagesFilenames.AddToList(routeEntity.route.route_icon);

                myImages = cImages.FindAll(b => b.poi_id == routeEntity.route.route_id);

                if (myImages.Count > 0)
                {
                    foreach (cImage image in myImages)
                    {
                        imagesFilenames.AddToList(image.image_file);
                    }
                }

                foreach (PeriodEntity periodEntity in routeEntity.periods)
                {
                    imagesFilenames.AddToList(periodEntity.period.period_icon);
                    imagesFilenames.AddToList(periodEntity.period.period_poi_icon);
                    imagesFilenames.AddToList(periodEntity.period.period_poi_icon_active);
                    imagesFilenames.AddToList(periodEntity.period.period_poi_icon_visited);

                    foreach (PoiEntity poiEntity in periodEntity.poiEntities)
                    {
                        foreach (cLocalizedText narr in poiEntity.poi.poi_narration)
                            audiosFilenames.AddToList(narr.text);

                        beaconsUUIDs.AddToList(poiEntity.poi.poi_id.ToString());

                        poiImages.AddRange(poiEntity.images);
                        poiVideos.AddRange(poiEntity.videos);
                    }
                }
            }

            foreach (cImage img in poiImages)
            {
                imagesFilenames.AddToList(img.image_file);
            }

            foreach (cVideo vid in poiVideos)
            {
                videosFilenames.AddToList(vid.video_file);
            }

            totalImageFiles = imagesFilenames.Count;
            totalAudioFiles = audiosFilenames.Count;
            totalVideoFiles = videosFilenames.Count;

#if UNITY_EDITOR
            Debug.LogFormat("Total Unique Image files are {0}", imagesFilenames.Count);
            Debug.LogFormat("Total Unique Video files are {0}", videosFilenames.Count);
            Debug.LogFormat("Total Unique Audio files are {0}", audiosFilenames.Count);
#endif

        }



        public bool HasAnyData()
        {
            return totalAreas > 0 && totalPois > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>local json from local database</returns>
        public string GetJson(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            //if(!GlobalUtils.jsonExportFiles.Contains(name)) return false;

            if (jsonAreas != null && name.ContainsWord("area"))
            {
                return jsonAreas.text;
            }
            else if (jsonRoutes != null && name.ContainsWord("route") && !name.ContainsWord("type"))
            {
                return jsonRoutes.text;
            }
            else if (jsonRouteTypes != null && name.ContainsWord("route") && name.ContainsWord("type"))
            {
                return jsonRouteTypes.text;
            }
            else if (jsonPeriods != null && name.ContainsWord("period"))
            {
                return jsonPeriods.text;
            }
            else if (jsonPois != null && name.ContainsWord("poi"))
            {
                return jsonPois.text;
            }
            else if (jsonImages != null && name.ContainsWord("image"))
            {
                return jsonImages.text;
            }
            else if (jsonVideos != null && name.ContainsWord("video"))
            {
                return jsonVideos.text;
            }
            //else if (jsonBeacons != null && name.ContainsWord("beacon"))
            //{
            //    return jsonBeacons.text;
            //}
            return string.Empty;
        }

        public void ReadDefaultJsonFilesOverridde()
        {
            //if(jsonAreas == null)
            //{
            //    UpdateDatabaseFromDisk();
            //    return;
            //}

            ClearAll();

            string _result = string.Empty;

            //AREAS
            if (jsonAreas)
            {
                _result = JsonHelper.FixJson(jsonAreas.text);
                cAreas = JsonHelper.FromJson<cArea>(_result).ToList();
                if (cAreas.Count > 0) cAreas.ForEach(b => b.ReadGeoPosition());
            }

            //ROUTES
            if (jsonRoutes)
            {
                _result = JsonHelper.FixJson(jsonRoutes.text);
                cRoutes = JsonHelper.FromJson<cRoute>(_result).ToList();
            }

            //ROUTE TYPES
            if (jsonRouteTypes)
            {
                _result = JsonHelper.FixJson(jsonRouteTypes.text);
                cRouteTypes = JsonHelper.FromJson<cRouteType>(_result).ToList();
            }

            //PERIODS
            if (jsonPeriods)
            {
                _result = JsonHelper.FixJson(jsonPeriods.text);
                cPeriods = JsonHelper.FromJson<cPeriod>(_result).ToList();
            }

            //POIS
            if (jsonPois)
            {
                _result = JsonHelper.FixJson(jsonPois.text);
                cPois = JsonHelper.FromJson<cPoi>(_result).ToList();
                if (cPois.Count > 0) cPois.ForEach(b => b.ReadGeoPosition());
            }

            //IMAGES
            if (jsonImages)
            {
                _result = JsonHelper.FixJson(jsonImages.text);
                cImages = JsonHelper.FromJson<cImage>(_result).ToList();
            }

            //VIDEOS
            if (jsonVideos)
            {
                _result = JsonHelper.FixJson(jsonVideos.text);
                cVideos = JsonHelper.FromJson<cVideo>(_result).ToList();
            }

            //Beacons
            //if (jsonBeacons)
            //{
            //    _result = JsonHelper.FixJson(jsonBeacons.text);
            //    cBeacons = JsonHelper.FromJson<cBeacon>(_result).ToList();

            //    foreach(cBeacon beacon in cBeacons) 
            //    {
            //        beaconsUUIDs.Add(beacon.beacon_uuid); 
            //    }
            //}

            GetTotals();

            CreateAreaEntities();

            //GetPoisFirstImage();

            if (DataManager.Instance.useSelectedAreas)
            {
                GetUniqueFilenamesFromSelectedAreas();
            }
            else
            {
                GetAllUniqueFilesFromEntities();
            }

            if (Application.isPlaying)
            {
                if (Application.isEditor)
                    Debug.LogWarning("<color=green>DATABASE LOADED from RESOURCES</color>");

                EventHolder.OnDatabaseReaded?.Invoke();
            }
        }


        private List<string> allFirstImages = new List<string>();
        public List<string> GetPoisFirstImagesUniqueFilenames() { return allFirstImages; }

#if UNITY_EDITOR

        [ContextMenu("DebugPoisFirstImage")]
        void GetPoisFirstImage()
        {
            allFirstImages.Clear();

            List<cImage> myImages = new List<cImage>();

            foreach (AreaEntity areaEntity in areaEntities)
            {
                allFirstImages.AddToList(areaEntity.area.area_image);
                myImages = cImages.FindAll(b => b.poi_id == areaEntity.ID);

                if (myImages.Count > 0)
                {
                    allFirstImages.AddToList(myImages[0].image_file);
                }
            }

            foreach (RouteEntity routeEntity in routeEntities)
            {
                myImages = cImages.FindAll(b => b.poi_id == routeEntity.route.route_id);

                if (myImages.Count > 0)
                {
                    allFirstImages.AddToList(myImages[0].image_file);
                }

                foreach (PeriodEntity periodEntity in routeEntity.periods)
                {
                    foreach (PoiEntity poiEntity in periodEntity.poiEntities)
                    {
                        if (poiEntity.images.Count > 0)
                            allFirstImages.AddToList(poiEntity.images[0].image_file);
                    }
                }
            }

            Debug.LogWarning(allFirstImages.Count);

            foreach (string img in allFirstImages)
            {
                if (img.IsNull()) continue;
                Texture2D tex = SaveLoadManager.LoadTexture(img, null);
                if (tex)
                {
                    byte[] byt = img.EndsWith("png") ? tex.EncodeToPNG() : tex.EncodeToJPG();
                    SaveLoadManager.SaveFirstImagesEditor(byt, img);
                }
            }

            allFirstImages.Sort();

            string s = string.Empty;

            for (int i = 0; i < allFirstImages.Count; i++)
                s += "\n" + allFirstImages[i];

            Debug.Log(s);
        }

        [ContextMenu("Debug_ALL_ICONS")]
        void GetAllIcons()
        {
            allFirstImages.Clear();
            foreach (cArea p in cAreas)
            {
                allFirstImages.AddToList(p.area_icon);
            }

            foreach (cPeriod p in cPeriods)
            {
                allFirstImages.AddToList(p.period_icon);
                allFirstImages.AddToList(p.period_poi_icon);
                allFirstImages.AddToList(p.period_poi_icon_active);
                allFirstImages.AddToList(p.period_poi_icon_visited);
            }

            foreach (cRouteType p in cRouteTypes)
            {
                allFirstImages.AddToList(p.route_type_icon);
                allFirstImages.AddToList(p.route_type_poi_icon);
                allFirstImages.AddToList(p.route_type_poi_icon_active);
                allFirstImages.AddToList(p.route_type_poi_icon_visited);
            }

            Debug.LogWarning("ICONS are = " + allFirstImages.Count);

            foreach (string img in allFirstImages)
            {
                if (img.IsNull()) continue;
                Texture2D tex = SaveLoadManager.LoadTexture(img, null);
                if (tex)
                {
                    byte[] byt = tex.EncodeToPNG();
                    SaveLoadManager.SaveIconsEditor(byt, img);
                }
            }

            allFirstImages.Sort();

            string s = string.Empty;

            for (int i = 0; i < allFirstImages.Count; i++)
                s += "\n" + allFirstImages[i];

            Debug.Log(s);
        }

#endif

        /// <summary>
        /// returns true if jsons exists in disk and have all data
        /// </summary>
        /// <returns></returns>
        public bool UpdateDatabaseFromDisk()
        {
            ClearAll();

            string txt = string.Empty;
            string _result = string.Empty;

            //AREAS
            if (jsonAreas != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonAreasFilename, out txt))
                {
                    if (txt.IsNull()) return false;
                    _result = JsonHelper.FixJson(txt);
                    cAreas = JsonHelper.FromJson<cArea>(_result).ToList();
                    if (cAreas.Count > 0) cAreas.ForEach(b => b.ReadGeoPosition());
                }
                else
                    return false;
            }

            //ROUTES
            if (jsonRoutes != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonRoutesFilename, out txt))
                {
                    if (txt.IsNull()) return false;
                    _result = JsonHelper.FixJson(txt);
                    cRoutes = JsonHelper.FromJson<cRoute>(_result).ToList();
                }
                else
                    return false;
            }
            //ROUTE TYPES
            if (jsonRouteTypes != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonRouteTypesFilename, out txt))
                {
                    if (txt.IsNull()) return false;
                    _result = JsonHelper.FixJson(txt);
                    cRouteTypes = JsonHelper.FromJson<cRouteType>(_result).ToList();
                }
                else
                    return false;
            }

            //PERIODS
            if (jsonPeriods != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonPeriodsFilename, out txt))
                {
                    if (txt.IsNull()) return false;
                    _result = JsonHelper.FixJson(txt);
                    cPeriods = JsonHelper.FromJson<cPeriod>(_result).ToList();

                   // if (Application.isEditor) Debug.LogWarning("cPeriods.Count = " + cPeriods.Count);
                }
                else
                    return false;
            }

            //POIS
            if (jsonPois != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonPoisFilename, out txt))
                {
                    if (txt.IsNull()) return false;
                    _result = JsonHelper.FixJson(txt);
                    cPois = JsonHelper.FromJson<cPoi>(_result).ToList();
                    if (cPois.Count > 0) cPois.ForEach(b => b.ReadGeoPosition());
                }
                else
                    return false;
            }

            //IMAGES
            if (jsonImages != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonImagesFilename, out txt))
                {
                    if (txt.IsNull()) return false;
                    _result = JsonHelper.FixJson(txt);
                    cImages = JsonHelper.FromJson<cImage>(_result).ToList();
                }
                else
                    return false;
            }

            //Videos
            if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonVideosFilename, out txt))
            {
                if (!string.IsNullOrEmpty(txt))
                {
                    _result = JsonHelper.FixJson(txt);
                    cVideos = JsonHelper.FromJson<cVideo>(_result).ToList();
                }
            }

            //Beacons
            if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonBeaconsFilename, out txt))
            {
                if (!string.IsNullOrEmpty(txt))
                {
                    _result = JsonHelper.FixJson(txt);
                    cBeacons = JsonHelper.FromJson<cBeacon>(_result).ToList();
                }
            }

            GetTotals();

            CreateAreaEntities();

            //GetPoisFirstImage();

            if (DataManager.Instance.useSelectedAreas)
            {
                GetUniqueFilenamesFromSelectedAreas();
            }
            else
            {
                GetAllUniqueFilesFromEntities();
            }

            if (Application.isEditor)
                Debug.LogWarning("<color=green>DATABASE LOADED from SERVER UPDATES</color>");

            EventHolder.OnDatabaseReaded?.Invoke();

            return totalImageFiles > 0;
        }

        public List<string> GetUniqueImageFilenames()
        {
            return imagesFilenames;
        }
        public List<string> GetUniqueAudioFilenames()
        {
            return audiosFilenames;
        }
        public List<string> GetUniqueVideoFilenames()
        {
            return videosFilenames;
        }


        /// <summary>
        /// returns all images length from saved jsons in disk
        /// </summary>
        /// <returns></returns>
        public List<string> GetTotalImageFilesFromJsonsInDisk(out List<string> audioNamesFromJson, out List<string> videoNamesFromDisk)
        {
            audioNamesFromJson = new List<string>();
            videoNamesFromDisk = new List<string>();
            if (!SaveLoadManager.HasDeviceSavedJsons()) return new List<string>();// -1;

            List<string> filenames = new List<string>();

            List<cArea> _areas = new List<cArea>();
            List<cRouteType> _routeTypes = new List<cRouteType>();
            List<cRoute> _routes = new List<cRoute>();
            List<cPeriod> _periods = new List<cPeriod>();
            List<cPoi> _pois = new List<cPoi>();
            List<cImage> _images = new List<cImage>();
            List<cVideo> _videos = new List<cVideo>();

           string txt = string.Empty;
            string _result = string.Empty;

            //AREAS
            if (jsonAreas != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonAreasFilename, out txt))
                {
                    if (!txt.IsNull())
                    {
                        _result = JsonHelper.FixJson(txt);
                        _areas = JsonHelper.FromJson<cArea>(_result).ToList();
                    }
                }
            }

            //ROUTE TYPES
            if (jsonRouteTypes != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonRouteTypesFilename, out txt))
                {
                    if (!txt.IsNull())
                    {
                        _result = JsonHelper.FixJson(txt);
                        _routeTypes = JsonHelper.FromJson<cRouteType>(_result).ToList();
                    }
                }
            }

            //ROUTES
            if (jsonRoutes != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonRoutesFilename, out txt))
                {
                    if (!txt.IsNull())
                    {
                        _result = JsonHelper.FixJson(txt);
                        _routes = JsonHelper.FromJson<cRoute>(_result).ToList();
                    }
                }
            }

            //PERIODS
            if (jsonPeriods != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonPeriodsFilename, out txt))
                {
                    if (!txt.IsNull())
                    {
                        _result = JsonHelper.FixJson(txt);
                        _periods = JsonHelper.FromJson<cPeriod>(_result).ToList();
                    }
                }
            }

            //IMAGES
            if (jsonPois != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonPoisFilename, out txt))
                {
                    if (!txt.IsNull())
                    {
                        _result = JsonHelper.FixJson(txt);
                        _pois = JsonHelper.FromJson<cPoi>(_result).ToList();
                    }
                }
            }

            //IMAGES
            if (jsonImages != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonImagesFilename, out txt))
                {
                    if (!txt.IsNull())
                    {
                        _result = JsonHelper.FixJson(txt);
                        _images = JsonHelper.FromJson<cImage>(_result).ToList();
                    }
                }
            }

            if (jsonVideos != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonVideosFilename, out txt))
                {
                    if (!txt.IsNull())
                    {
                        _result = JsonHelper.FixJson(txt);
                        _videos = JsonHelper.FromJson<cVideo>(_result).ToList();
                    }
                }
            }

            List<cArea> myAreas = new List<cArea>();
            if (DataManager.Instance.useSelectedAreas)
            {
                int[] ids = DataManager.Instance.areasToShowIds;

                if (ids.Length > 0)
                {
                    foreach (int id in ids)
                    {
                        myAreas.Add(_areas.Find(b => b.area_id == id));
                    }
                }
            }

            if (myAreas.Count <= 0) { myAreas.Clear(); myAreas = _areas; }
            else
            {
                _areas = myAreas;
            }

            foreach (cArea area in _areas)
            {
                //ADD AREA ICON & IMAGE
                filenames.AddToList(area.area_icon);
                filenames.AddToList(area.area_image);

                List<cImage> myImages = _images.FindAll(b => b.poi_id == area.area_id);
               
                //ADD AREA IMAGES
                if (myImages.Count > 0)
                {
                    foreach (cImage image in myImages)
                    {
                        filenames.AddToList(image.image_file);
                    }
                }

                //get routes for this area
                List<cRoute> areaRoutes = _routes.FindAll(b => b.area_id == area.area_id);

                foreach (cRoute route in areaRoutes)
                {

                    myImages = _images.FindAll(b => b.poi_id == route.route_id);

                    //ADD ROUTE IMAGES
                    if (myImages.Count > 0)
                    {
                        foreach (cImage image in myImages)
                        {
                            filenames.AddToList(image.image_file);
                        }
                    }

                    //group pois to periods
                    foreach (cPeriod _period in _periods)
                    {
                        //find pois for this period and create poi entities
                        List<cPoi> poisOfPeriod = _pois.FindAll(b => b.period_id == _period.period_id && b.route_id == route.route_id);

                        foreach (cPoi p in poisOfPeriod)
                        {
                            myImages = _images.FindAll(b => b.poi_id == p.poi_id);

                            cPeriod myPeriod = _periods.Find(b => b.period_id == p.period_id);

                            if (myPeriod.period_id > 0)
                            {
                                filenames.AddToList(_period.period_icon);
                                filenames.AddToList(_period.period_poi_icon);
                                filenames.AddToList(_period.period_poi_icon_active);
                                filenames.AddToList(_period.period_poi_icon_visited);
                            }

                            //ADD POI IMAGES
                            if (myImages.Count > 0)
                            {
                                foreach (cImage image in myImages)
                                {
                                    filenames.AddToList(image.image_file);
                                }
                            }

                            foreach (cLocalizedText narr in p.poi_narration)
                                audioNamesFromJson.AddToList(narr.text);

                            if(_videos.Count > 0)
                            {
                                cVideo video = _videos.Find(b => b.poi_id == p.poi_id);
                                videoNamesFromDisk.AddToList(video.video_file);
                            }

                        }
                    }
                }

            }

            return filenames;
        }

        /// <summary>
        /// returns all audio length from saved jsons in disk
        /// </summary>
        /// <returns></returns>
        public int GetTotalAudioFilesFromJsonsInDisk()
        {

            if (!SaveLoadManager.HasDeviceSavedJsons()) return -1;

            List<string> filenames = new List<string>();

            string txt = string.Empty;
            string _result = string.Empty;

            //Audios
            if (jsonPois != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonPoisFilename, out txt))
                {
                    if (txt.IsNull()) return 0;
                    _result = JsonHelper.FixJson(txt);
                    List<cPoi> pois = JsonHelper.FromJson<cPoi>(_result).ToList();
                    if (pois.Count > 0)
                    {
                        foreach (cPoi poi in cPois)
                        {
                            foreach (cLocalizedText narr in poi.poi_narration)
                                filenames.AddToList(narr.text);
                        }
                    }
                }
                else
                    return 0;
            }

          

            //if is zero then return -1
            //because appmanager compares with files length
            //and we want to avoid to be true if no files found
            int _res = /*filenames.Count == 0 ? -1 :*/ filenames.Count;

            return _res;
        }

        /// <summary>
        /// returns all video files length from saved jsons in disk
        /// </summary>
        /// <returns></returns>
        public int GetTotalVideoFilesFromJsonsInDisk()
        {

            if (!SaveLoadManager.HasDeviceSavedJsons()) return -1;

            List<string> filenames = new List<string>();

            string txt = string.Empty;
            string _result = string.Empty;

            //Videos
            if (jsonVideos != null)
            {
                if (SaveLoadManager.LoadJsonFileAsStringFromDisk(GlobalUtils.jsonVideosFilename, out txt))
                {
                    if (txt.IsNull()) return 0;
                    _result = JsonHelper.FixJson(txt);
                    List<cVideo> videos = JsonHelper.FromJson<cVideo>(_result).ToList();
                    if (videos.Count > 0)
                    {
                        foreach (cVideo vid in cVideos)
                        {
                            filenames.AddToList(vid.video_file);
                        }
                    }
                }
                else
                    return 0;
            }



            //if is zero then return -1
            //because appmanager compares with files length
            //and we want to avoid to be true if no files found
            int _res = /*filenames.Count == 0 ? -1 :*/ filenames.Count;

            return _res;
        }


        public void GetTotals()
        {
            totalAreas = cAreas.Count;
            totalRoutes = cRoutes.Count;
            totalRouteTypes = cRouteTypes.Count;
            totalPeriods = cPeriods.Count;
            totalPois = cPois.Count;
            totalImages = cImages.Count;
            totalVideos = cVideos.Count;
        }

        private void CreateAreaEntities()
        {

            routeEntities.Clear();

            List<cArea> myAreas = new List<cArea>();

            if (DataManager.Instance.useSelectedAreas)
            {
                int[] ids = DataManager.Instance.areasToShowIds;

                if(ids.Length > 0)
                {
                    foreach(int id in ids)
                    {
                        myAreas.Add(cAreas.Find(b=>b.area_id == id));
                    }
                }
            }

            if(myAreas.Count <= 0) { myAreas.Clear(); myAreas = cAreas; }
            else
            {
                cAreas = myAreas;
            }

            foreach (cArea area in cAreas)
            {
                AreaEntity areaEntity = new AreaEntity();
                areaEntity.area = area;
                areaEntity.CreateImages();

                areaEntity.areaImages = cImages.FindAll(b => b.poi_id == area.area_id);

                //get routes for this area
                List<cRoute> _routes = cRoutes.FindAll(b => b.area_id == area.area_id);

#if UNITY_EDITOR
                if (_routes.Count <= 0) { Debug.LogWarning("ΒΑΛΕ ΤΟ AREA ID στο json Routes!!!!"); }
#endif

                foreach (cRoute route in _routes)
                {
                    RouteEntity routeEntity = new RouteEntity();
                    routeEntity.route = route;
                    routeEntity.routeType = cRouteTypes.Find(b => b.route_type_id == route.route_type_id);

                    routeEntity.routeImages = cImages.FindAll(b => b.poi_id == route.route_id);

                    //group pois to periods
                    foreach (cPeriod _period in cPeriods)
                    {
                        Texture2D _poiIcon = SaveLoadManager.LoadTexture(_period.period_poi_icon, GlobalUtils.iconMarkerEmpty);
                        Texture2D _poiIconActive = SaveLoadManager.LoadTexture(_period.period_poi_icon_active, GlobalUtils.iconMarkerEmpty);
                        Texture2D _poiIconVisited = SaveLoadManager.LoadTexture(_period.period_poi_icon_visited, GlobalUtils.iconMarkerEmpty);

                        PeriodEntity periodPoisEntity = new PeriodEntity();
                        periodPoisEntity.period = _period;
                        periodPoisEntity.periodIcon = SaveLoadManager.LoadTexture(_period.period_icon, GlobalUtils.GetDefaultPeriodIcon(_period.period_id));
                        periodPoisEntity.periodIconVisited = _poiIconVisited;

                        //Debug.LogWarning(_period.period_icon);

                        //foreach (cPoi poi in cPois) 
                        //{
                        //    Debug.LogWarning("----------------------------------");
                        //    Debug.LogWarning("Pois id = " + poi.poi_id);
                        //    Debug.LogWarning("Period id = " + poi.period_id);
                        //    Debug.LogWarning("Route id = " + poi.route_id);
                        //    Debug.LogWarning("----------------------------------");
                        //}

                        //find pois for this period and create poi entities
                        List<cPoi> _pois = cPois.FindAll(b => b.period_id == _period.period_id && b.route_id == route.route_id);

                        foreach (cPoi p in _pois)
                        {
                            PoiEntity poiEntity = new PoiEntity();
                            poiEntity.poi = p;
                            poiEntity.images = cImages.FindAll(b => b.poi_id == p.poi_id);
                            poiEntity.videos = cVideos.FindAll(b => b.poi_id == p.poi_id);
                            poiEntity.SetPoiIcons(_poiIcon, _poiIconActive, _poiIconVisited);
                            periodPoisEntity.poiEntities.Add(poiEntity);

                            if (!poiEntities.Contains(poiEntity)) poiEntities.Add(poiEntity);
                        }

                        routeEntity.periods.Add(periodPoisEntity);
                    }


                    areaEntity.routes.Add(routeEntity);
                    routeEntities.Add(routeEntity);
                }

                areaEntities.Add(areaEntity);

            }

            //get route icons
            foreach (cRouteType routeType in cRouteTypes)
            {
                if (CreateRouteIcons(routeType, out List<cIcon> icons))
                {
                    //cIcons.AddRange(icons);
                    foreach (cIcon icon in icons)
                    {
                        if (!cIcons.Contains(icon)) cIcons.Add(icon);
                    }
                }
            }

            //get period icons
            foreach (cPeriod _period in cPeriods)
            {
                if (CreatePeriodIcons(_period, out List<cIcon> icons))
                {
                    //cIcons.AddRange(icons);
                    foreach (cIcon icon in icons)
                    {
                        if (!cIcons.Contains(icon)) cIcons.Add(icon);
                    }
                }
            }

            totalPoiEntities = poiEntities.Count;
            totalIcons = cIcons.Count;

        }

        private bool CreateRouteIcons(cRouteType routeType, out List<cIcon> icons)
        {
            icons = new List<cIcon>();
            bool hasIcons = !routeType.route_type_icon.IsNull() &&
                            !routeType.route_type_poi_icon.IsNull() &&
                            !routeType.route_type_poi_icon_active.IsNull() &&
                            !routeType.route_type_poi_icon_visited.IsNull();

            if (!routeType.route_type_icon.IsNull() && !hasIcons)
            {
                cIcon _icon = new cIcon
                {
                    isPeriod = false,
                    route_type_id = routeType.route_type_id,
                    filename = routeType.route_type_icon,
                    iconType = EnumsHolder.IconType.ROUTE
                };
                icons.Add(_icon);
                return true;
            }

            if (!hasIcons) return false;

            cIcon icon = new cIcon
            {
                isPeriod = false,
                route_type_id = routeType.route_type_id,
                filename = routeType.route_type_icon,
                iconType = EnumsHolder.IconType.ROUTE
            };
            icons.Add(icon);
            icon = new cIcon
            {
                isPeriod = false,
                route_type_id = routeType.route_type_id,
                filename = routeType.route_type_poi_icon,
                iconType = EnumsHolder.IconType.POI
            };
            icons.Add(icon);
            icon = new cIcon
            {
                isPeriod = false,
                route_type_id = routeType.route_type_id,
                filename = routeType.route_type_poi_icon_active,
                iconType = EnumsHolder.IconType.POI_ACTIVE
            };
            icons.Add(icon);
            icon = new cIcon
            {
                isPeriod = false,
                route_type_id = routeType.route_type_id,
                filename = routeType.route_type_poi_icon_visited,
                iconType = EnumsHolder.IconType.POI_VISITED
            };
            icons.Add(icon);

            return true;
        }

        private bool CreatePeriodIcons(cPeriod period, out List<cIcon> icons)
        {
            icons = new List<cIcon>();
            bool hasIcons = !period.period_icon.IsNull() &&
                            !period.period_poi_icon.IsNull() &&
                            !period.period_poi_icon_active.IsNull() &&
                            !period.period_poi_icon_visited.IsNull();

            if (!hasIcons) return false;

            cIcon icon = new cIcon
            {
                isPeriod = true,
                period_id = period.period_id,
                filename = period.period_icon,
                iconType = EnumsHolder.IconType.PERIOD
            };
            icons.Add(icon);
            icon = new cIcon
            {
                isPeriod = true,
                period_id = period.period_id,
                filename = period.period_poi_icon,
                iconType = EnumsHolder.IconType.POI
            };
            icons.Add(icon);
            icon = new cIcon
            {
                isPeriod = true,
                period_id = period.period_id,
                filename = period.period_poi_icon_active,
                iconType = EnumsHolder.IconType.POI_ACTIVE
            };
            icons.Add(icon);
            icon = new cIcon
            {
                isPeriod = true,
                period_id = period.period_id,
                filename = period.period_poi_icon_visited,
                iconType = EnumsHolder.IconType.POI_VISITED
            };
            icons.Add(icon);

            return true;
        }

        private void ClearAll()
        {
            audiosFilenames.Clear();
            videosFilenames.Clear();
            imagesFilenames.Clear();
            cAreas.Clear(); cRoutes.Clear(); cRouteTypes.Clear();
            cPeriods.Clear(); cPois.Clear(); cImages.Clear(); 
            cIcons.Clear(); cVideos.Clear();
            areaEntities.Clear();
            poiEntities.Clear();

            cBeacons.Clear();
            beaconsUUIDs.Clear();
        }

        public bool IsJsonMissing()
        {
            return jsonImages == null || jsonAreas == null || jsonPois == null || jsonRoutes == null;
        }

    }

}
