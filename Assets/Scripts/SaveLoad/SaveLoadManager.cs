//Diadrasis ©2023
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace Diadrasis.Rethymno 
{

	public class SaveLoadManager : MonoBehaviour
	{
        static Texture2D textureToReleaseMemory;

        #region Server

        private static readonly string firstUpdateSucceedKey = "first_time_update";
        private static readonly string acceptCarrierNetwork = "userAcceptCarrierDataNetwork";
        public static bool IsApplicationFirstTimeUpdated() { return PlayerPrefs.HasKey(firstUpdateSucceedKey) && PlayerPrefs.GetInt(firstUpdateSucceedKey) == 1; }
        public static void SaveFirstTimeUpdated() { PlayerPrefs.SetInt(firstUpdateSucceedKey, 1); PlayerPrefs.Save(); }

        public static void SaveAcceptCarrierNetwork() { PlayerPrefs.SetInt(acceptCarrierNetwork, 1); PlayerPrefs.Save(); }
        public static bool IsUserAcceptedCarrierDataNetwork() { return PlayerPrefs.HasKey(firstUpdateSucceedKey) && PlayerPrefs.GetInt(acceptCarrierNetwork) == 1; }

        /// <summary>
        /// these are the big size textures which are not downloaded 
        /// due to max texture size that this device supports
        /// </summary>
        public static List<string> BigImagesNames
        {
            get { return PlayerPrefsX.GetStringArray("bigImagesNames").ToList(); }
        }

        public static void BigImageAddToList(string imageName)
        {
#if UNITY_EDITOR
            Debug.Log("<color=green>BigImageAddToList >> " + imageName + "</color>");
#endif
            List<string> list = PlayerPrefsX.GetStringArray("bigImagesNames").ToList();
            if (!list.Contains(imageName))
            {
                list.Add(imageName);
                PlayerPrefsX.SetStringArray("bigImagesNames", list.ToArray());
                PlayerPrefs.Save();
            }
        }

        public static void BigImageRemoveFromList(string imageName)
        {
            List<string> list = PlayerPrefsX.GetStringArray("bigImagesNames").ToList();
            if (list.Contains(imageName))
            {
                list.Remove(imageName);
                PlayerPrefsX.SetStringArray("bigImagesNames", list.ToArray());
                PlayerPrefs.Save();
            }
        }

        #endregion


        #region POIS

        public static bool IsPoiVisitedOffSite(string id)
        {
			return PlayerPrefs.GetString(id + "_offsite") == "visited";
        }

		public static void SavePoiAsVisitedOffSite(string id)
        {
            if (!PlayerPrefs.HasKey(id +"_offsite"))
                PlayerPrefs.SetString(id +"_offsite", "visited");

            List<string> allSavePois = PlayerPrefsX.GetStringArray("visited_pois").ToList();
            if (!allSavePois.Contains(id + "_offsite"))
            {
                allSavePois.Add(id + "_offsite");
                PlayerPrefsX.SetStringArray("visited_pois", allSavePois.ToArray());
            }

            PlayerPrefs.Save();
        }

        public static bool IsPoiVisitedOnSite(string id)
        {
            return PlayerPrefs.GetString(id + "_onsite") == "visited_onsite";
        }

        public static void SavePoiAsVisitedOnSite(string id)
        {
            if (!PlayerPrefs.HasKey(id + "_onsite"))
                PlayerPrefs.SetString(id + "_onsite", "visited_onsite");

            List<string> allSavePois = PlayerPrefsX.GetStringArray("visited_pois").ToList();
            if (!allSavePois.Contains(id + "_onsite"))
            {
                allSavePois.Add(id + "_onsite");
                PlayerPrefsX.SetStringArray("visited_pois", allSavePois.ToArray());
            }

            PlayerPrefs.Save();
        }

        public static void DeleteVisitedPois()
        {
            //if (Application.isEditor) Debug.Log("DeleteVisitedPois");

            List<string> allSavePois = PlayerPrefsX.GetStringArray("visited_pois").ToList();
            foreach (string s in allSavePois)
                PlayerPrefs.DeleteKey(s);
            
            PlayerPrefs.DeleteKey("visited_pois");
            PlayerPrefs.Save();
        }

        #endregion

        #region files

        #region folder creation

        public static void CheckFolderExistsAndCreate()
        {
            CreateProjectFolders();
        }

        public static void CreateProjectFolders()
        {
            string appDataPath = GetAppDataFolderPath();

            //Debug.LogWarning(GetPath_ForAudios());

            if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);
            if (!Directory.Exists(GetPath_ForAudios())) Directory.CreateDirectory(GetPath_ForAudios());
            if (!Directory.Exists(GetPath_ForImages())) Directory.CreateDirectory(GetPath_ForImages());
            if (!Directory.Exists(GetPath_ForJsons())) Directory.CreateDirectory(GetPath_ForJsons());
        }

        #endregion


        #region SAVE Json Data

        public static bool HasDeviceSavedJsons()
        {
            foreach (string s in GlobalUtils.jsonExportFiles)
            {
                string path = Path.Combine(GetPath_ForJsons(), s.WithNoExtension());
                if (!FileExistsInDisk(path))
                    return false;
            }
            return true;
        }

        public static bool FileExistsInDisk(string fileUrl)
        {
            return File.Exists(fileUrl);
        }

        public static bool JsonExistsInDisk(string filename)
        {
            return File.Exists(Path.Combine(GetPath_ForJsons(), filename));
        }

        public static bool DirectoryExistsInDisk(string fileUrl)
        {
            return Directory.Exists(Path.GetDirectoryName(fileUrl));
        }

        public static bool IsFileExist(string filename, string path, Ext fileExtension)
        {
            return File.Exists(Path.Combine(path, filename + MyExtension(fileExtension)));
        }

        public static bool IsDiskImageExist(string filename)
        {
            if (filename.IsNull()) return false;
            return File.Exists(Path.Combine(GetPath_ForImages(), filename));
        }

        public static bool IsFileImageExist(string filename, bool checkResources = true)
        {
            if (filename.IsNull()) return false;
            if (checkResources) return IsImageInResources(filename) || File.Exists(Path.Combine(GetPath_ForImages(), filename.WithNoExtension()));
            return File.Exists(Path.Combine(GetPath_ForImages(), filename.WithNoExtension()));
        }

        public static bool IsFileAudioExist(string filename)
        {
            if (filename.IsNull()) return false;
            return File.Exists(Path.Combine(GetPath_ForAudios(), filename));
        }

        public static bool IsFileVideoExist(string filename)
        {
            if (filename.IsNull()) return false;
            return File.Exists(Path.Combine(GetPath_ForVideos(), filename));
        }

        public static void SaveData(string dataToSave, string dataFileName, Ext fileExtension)
        {
            string tempPath = GetPath_ForJsons();

            tempPath = Path.Combine(tempPath, dataFileName.WithNoExtension());// + MyExtension(fileExtension));

            //if(Application.isEditor)
                //Debug.Log(tempPath);

            byte[] myBytes = Encoding.UTF8.GetBytes(dataToSave);

            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }
            //else
            //{
            //    //avoid violation
            //    //delete first?
            //    return;
            //}

            try
            {
                File.WriteAllBytes(tempPath, myBytes);
                if (Application.isEditor)
                {
                    Debug.Log("Saved " + dataFileName + " to: " + tempPath.Replace("/", "\\"));
                }

            }
            catch (Exception e)
            {
                if (Application.isEditor)
                {
                    Debug.LogWarning("Failed To save " + dataFileName + " to: " + tempPath.Replace("/", "\\"));
                    Debug.LogWarning("Error: " + e.Message);
                }

            }

        }

        public static void SaveStringAsJsonFile(string data, string fileName, bool should_override)
        {
            if (!should_override)
            {
                if (IsFileExist(fileName.WithNoExtension(), GetPath_ForJsons(), Ext.NULL))
                {
                    if (Application.isEditor)
                        Debug.Log(fileName.WithNoExtension() + " EXISTs..saving stopped!");
                    return;
                }
            }

            //save as file locally
            SaveData(data, fileName.WithNoExtension(), Ext.NULL);

            //AddFileNameToSavedList(fileName);
        }

        public static void AddFileNameToSavedList(string fileName)
        {
            List<string> savedFiles = GetSavedFiles();
            if (!savedFiles.Contains(fileName))
            {
                savedFiles.Add(fileName);
                PlayerPrefsX.SetStringArray("savedFiles", savedFiles.ToArray());
            }
        }

        public static List<string> GetSavedFiles()
        {
            return PlayerPrefsX.GetStringArray("savedFiles").ToList();
        }

        #endregion

        #region Save Audio file

        public static void SaveAudio(string filename, AudioClip clip, bool trim = false)
        {
            if (File.Exists(Path.Combine(GetPath_ForAudios(false), filename)))// + ".wav")))
            {
                Debug.LogWarning("AUDIO EXISTS");
                return;
            }
            SavWav.SaveToPath(GetPath_ForAudios(false), filename, clip, trim);
        }

        #endregion

        #region Save Image file

        public static void SaveFirstImagesEditor(byte[] myBytes, string dataFileName)
        {
            string tempPath = GetAppDataFolderPath();
            tempPath = Path.Combine(tempPath, "FirstImages");
            tempPath = Path.Combine(tempPath, dataFileName);
            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }
            try
            {
                File.WriteAllBytes(tempPath, myBytes);
            }
            catch (Exception e)
            {
                if (Application.isEditor)
                {
                    Debug.LogWarning("Failed To save " + dataFileName + " to: " + tempPath.Replace("/", "\\"));
                    Debug.LogWarning("Error: " + e.Message);
                }
            }
        }
        public static void SaveIconsEditor(byte[] myBytes, string dataFileName)
        {
            string tempPath = GetAppDataFolderPath();
            tempPath = Path.Combine(tempPath, "All_Icons");
            tempPath = Path.Combine(tempPath, dataFileName);
            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }
            try
            {
                File.WriteAllBytes(tempPath, myBytes);
            }
            catch (Exception e)
            {
                if (Application.isEditor)
                {
                    Debug.LogWarning("Failed To save " + dataFileName + " to: " + tempPath.Replace("/", "\\"));
                    Debug.LogWarning("Error: " + e.Message);
                }
            }
        }
        public static void SaveImageData(byte[] myBytes, string dataFileName, out bool isDone)
        {

            string tempPath = GetPath_ForImages();

            tempPath = Path.Combine(tempPath, dataFileName);

            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }
            //else
            //{
            //    //avoid violation
            //    //delete first?
            //    return;
            //}

            try
            {
                //File.WriteAllBytes(tempPath, myBytes);

                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                using (var writer = new BinaryWriter(fileStream))
                {
                    writer.Write(myBytes, 0, myBytes.Length);
                }


            }
            catch (Exception e)
            {
                if (Application.isEditor)
                {
                    Debug.LogWarning("Failed To save " + dataFileName + " to: " + tempPath.Replace("/", "\\"));
                    Debug.LogWarning("Error: " + e.Message);
                }
            }

            isDone = true;

        }

        #endregion

        #region Save Video file

        public static void SaveVideoData(byte[] myBytes, string dataFileName, out bool isDone, Action<string> call)
        {
            isDone = false;

            string tempPath = GetPath_ForVideos();

            tempPath = Path.Combine(tempPath, dataFileName);// Path.GetFileNameWithoutExtension(dataFileName));

            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }
            //else
            //{
            //    //avoid violation
            //    //delete first?
            //    return;
            //}

            try
            {
                File.WriteAllBytes(tempPath, myBytes);
                // if (Application.isEditor)
                Debug.Log("Saved " + dataFileName + " to: " + tempPath.Replace("/", "\\"));

                isDone = true;

            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To save " + dataFileName + " to: " + tempPath.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
                call?.Invoke("Failed To save " + dataFileName + "\nError: " + e.Message);
            }

           

        }

        #endregion

        #region LOAD FILE

        public static List<string> GetFilesFileNames()
        {
            DirectoryInfo info = new DirectoryInfo(GetPath_ForJsons());

            FileInfo[] fileInfo = info.GetFiles().OrderByDescending(p => p.LastWriteTimeUtc).ToArray();

            List<string> allNames = new List<string>();

            foreach (FileInfo file in fileInfo)
            {
                if (file.Extension == ".json")
                {
                    string s = file.Name;//.FullName;
                    allNames.Add(s);
                }
            }

            return allNames;
        }

        public static List<string> GetAudioFileNamesOrderTime()
        {
            DirectoryInfo info = new DirectoryInfo(GetPath_ForAudios());

            FileInfo[] fileInfo = info.GetFiles().OrderByDescending(p => p.LastWriteTimeUtc).ToArray();

            List<string> allNames = new List<string>();

            foreach (FileInfo file in fileInfo)
            {
                if (file.Extension == ".wav")
                {
                    string s = file.Name;//.FullName;
                    allNames.Add(s);
                }
            }

            return allNames;
        }

        public static List<string> GetAudioFileNamesOrderNames()
        {
            DirectoryInfo info = new DirectoryInfo(GetPath_ForAudios());

            FileInfo[] fileInfo = info.GetFiles().OrderByDescending(p => p.Name).ToArray();//.LastWriteTimeUtc).ToArray();

            List<string> allNames = new List<string>();

            foreach (FileInfo file in fileInfo)
            {
                if (file.Extension == ".wav")
                {
                    string s = file.Name;//.FullName;
                    allNames.Add(s);
                }
            }

            return allNames;
        }

        public static bool IsImageInResources(string filename)
        {
            string imgResourcesPath = GetPath_ForImages(true) + filename.WithNoExtension();
            return Resources.Load(imgResourcesPath) != null;
        }

        public static int GetResourcesImagesLength() { return Resources.LoadAll<Texture2D>(GetPath_ForImages(true)).Length; }

        /// <summary>
        /// returns all images files length saved in disk
        /// </summary>
        /// <returns></returns>
        public static int GetDiskImagesLength(out List<string> filenames)
        {
            filenames = new List<string>();
            if (!Directory.Exists(GetPath_ForImages())) return 0;
            DirectoryInfo info = new DirectoryInfo(GetPath_ForImages());
            foreach(FileInfo fileInfo in info.GetFiles())
            {
                filenames.Add(fileInfo.Name);
            }
            return info.GetFiles().ToArray().Length;
        }

        public static List<string> GetDiskImageFilenames()
        {
            List<string> files = new List<string>();
            if (!Directory.Exists(GetPath_ForImages())) return files;
            DirectoryInfo info = new DirectoryInfo(GetPath_ForImages());
            foreach(FileInfo fileInfo in info.GetFiles().ToArray())
            {
                if(!files.Contains(fileInfo.Name)) files.Add(fileInfo.Name);
            }
            return files;
        }

        /// <summary>
        /// returns all audio files length saved in disk
        /// </summary>
        /// <returns></returns>
        public static int GetDiskAudiosLength()
        {
            if (!Directory.Exists(GetPath_ForAudios())) return 0;
            DirectoryInfo info = new DirectoryInfo(GetPath_ForAudios());
            return info.GetFiles().ToArray().Length;
        }

        public static List<string> GetDiskAudioFilenames()
        {
            List<string> files = new List<string>();
            if (!Directory.Exists(GetPath_ForAudios())) return files;
            DirectoryInfo info = new DirectoryInfo(GetPath_ForAudios());
            foreach (FileInfo fileInfo in info.GetFiles().ToArray())
            {
                if (!files.Contains(fileInfo.Name)) files.Add(fileInfo.Name);
            }
            return files;
        }

        /// <summary>
        /// returns all video files length saved in disk
        /// </summary>
        /// <returns></returns>
        public static int GetDiskVideosLength()
        {
            if (!Directory.Exists(GetPath_ForVideos())) return 0;
            DirectoryInfo info = new DirectoryInfo(GetPath_ForVideos());
            return info.GetFiles().ToArray().Length;
        }

        public static List<string> GetDiskVideoFilenames()
        {
            List<string> files = new List<string>();
            if (!Directory.Exists(GetPath_ForVideos())) return files;
            DirectoryInfo info = new DirectoryInfo(GetPath_ForVideos());
            foreach (FileInfo fileInfo in info.GetFiles().ToArray())
            {
                if (!files.Contains(fileInfo.Name)) files.Add(fileInfo.Name);
            }
            return files;
        }

        public static List<string> GetImagesFileNamesOrdedByName()
        {
            DirectoryInfo info = new DirectoryInfo(GetPath_ForImages());

            FileInfo[] fileInfo = info.GetFiles().OrderByDescending(p => p.Name).ToArray();//.LastWriteTimeUtc).ToArray();

            List<string> allNames = new List<string>();

            foreach (FileInfo file in fileInfo)
            {
               // if (file.Extension == ".wav")
               // {
                    //string s = file.Name;//.FullName;
                    allNames.Add(file.Name);
               // }
            }

            return allNames;
        }

        public static bool LoadJsonFileAsStringFromDisk(string dataFileName, out string resultText)
        {
            string tempPath = GetPath_ForJsons();
            tempPath = Path.Combine(tempPath, dataFileName.WithNoExtension());

            //Exit if Directory or File does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                if (Application.isEditor)
                    Debug.Log(dataFileName + " Directory does not exist at " + tempPath);

                resultText = string.Empty;
                return false;
            }

            if (!File.Exists(tempPath))
            {
                if (Application.isEditor)
                    Debug.Log(dataFileName + " does not exist in " + tempPath);

                resultText = string.Empty;
                return false;
            }

            //Load saved Json
            try
            {
                byte[] newBytes = File.ReadAllBytes(tempPath);

                //if (Application.isEditor) Debug.Log("Loaded " + dataFileName + " from: " + tempPath.Replace("/", "\\"));

                resultText = Encoding.UTF8.GetString(newBytes);

                if (resultText.ToUpper().Contains("DOCTYPE"))//<!DOCTYPE html>
                {
                    resultText = string.Empty;
                    return false;
                }

                return true;

            }
            catch (Exception e)
            {
                if (Application.isEditor)
                {
                    Debug.LogWarning("Failed To Load " + dataFileName + " from: " + tempPath.Replace("/", "\\"));
                    Debug.LogWarning("Error: " + e.Message);
                }
                resultText = string.Empty;
                return false;
            }

        }

        ///Load file from application persistentDataPath
        public static T LoadData<T>(string dataFileName, Ext fileExtension)
        {
            string tempPath = GetPath_ForJsons(); //Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + MyExtension(fileExtension));

            //Exit if Directory or File does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                if (Application.isEditor)
                {
                    Debug.Log(dataFileName + " Directory does not exist at " + tempPath);
                }
                return default(T);
            }

            if (!File.Exists(tempPath))
            {
                if (Application.isEditor)
                {
                    Debug.Log(dataFileName + " does not exist in " + tempPath);
                }
                return default(T);
            }

            //Load saved Json
            byte[] newBytes = null;
            try
            {
                newBytes = File.ReadAllBytes(tempPath);
                if (Application.isEditor)
                {
                    Debug.Log("Loaded " + dataFileName + " from: " + tempPath.Replace("/", "\\"));
                }
            }
            catch (Exception e)
            {
                if (Application.isEditor)
                {
                    Debug.LogWarning("Failed To Load " + dataFileName + " from: " + tempPath.Replace("/", "\\"));
                    Debug.LogWarning("Error: " + e.Message);
                }
            }

            //if (fileExtension == Ext.TXT || fileExtension == Ext.XML) {
            //Convert to json string
            string newData = Encoding.UTF8.GetString(newBytes);
            //}

            //Convert to Object
            //object resultValue =  JsonUtility.FromJson<T>(newData);
            return (T)Convert.ChangeType(newData, typeof(T));
        }

        #region AUDIO

        public static void SaveAudioFile(string filename, byte[] data, out bool isDone)
        {
            SavWav.SaveAudioFromByteArray(filename, data, false, out isDone);
        }

        public static void GetAudioClip(MonoBehaviour instance, string filename, Action<AudioClip> callback)
        {
            instance.StartCoroutine(GetAudioClip(filename, callback));
        }

        public static AudioType GetAudioTypeFromFilename(string _filename)
        {
            if (_filename.EndsWith("mp3"))
            {
                return AudioType.MPEG;
            }
            else if (_filename.EndsWith("ogg"))
            {
                return AudioType.OGGVORBIS;
            }
            if (_filename.EndsWith("wav"))
            {
                return AudioType.WAV;
            }
            //else if (_filename.EndsWith("mpeg"))
            //{
            //    return AudioType.MPEG;
            //}
            else
            {
                return AudioType.UNKNOWN;
            }
        }

        public static IEnumerator GetAudioClip(string filename, Action<AudioClip> callback)
        {
           // if (Application.isEditor) Debug.Log(filename);

            string clipPath = GetPath_ForAudios(true) + filename.WithNoExtension();
            //if(Application.isEditor) Debug.Log(clipPath);

            AudioClip clip = Resources.Load<AudioClip>(clipPath);
            if (clip)
            {
                callback(clip);
                //if (Application.isEditor) Debug.Log("Audio loaded from Resources");
                yield break;
            }

            //look in disk
            clipPath = GetPath_ForAudios() + filename.WithNoExtension();

            //if (Application.isEditor) Debug.Log(clipPath);

            if (!FileExistsInDisk(clipPath))
            {
                callback(null);
                //if (Application.isEditor) Debug.Log("Audio file is missing from disk!");
                yield break;
            }

            Uri uri = new Uri(clipPath, UriKind.Absolute);//get absolute path for mobile >> file:///
            //if (Application.isEditor) Debug.Log(uri);

            AudioType audioType = GetAudioTypeFromFilename(filename);
            if (audioType == AudioType.UNKNOWN)
            {
                callback(null);
                yield break;
            }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            using (var uwr = UnityWebRequestMultimedia.GetAudioClip(uri,  GetAudioTypeFromFilename(filename)))
#else
            using (var uwr = UnityWebRequestMultimedia.GetAudioClip(clipPath, GetAudioTypeFromFilename(filename)))
            #endif
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    if (Application.isEditor) Debug.Log("Audio clip ERROR." + uwr.error);
                    callback(null);
                    yield break;
                }

                DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;

                if (dlHandler.isDone)
                {
                    callback(DownloadHandlerAudioClip.GetContent(uwr));
                    //if (Application.isEditor) Debug.Log("Audio loaded from disk");
                }
                else
                {
                    callback(null);
                   // if (Application.isEditor) Debug.Log("The download process is not completely finished.");
                }
            }
        }

        private static float[] ConvertByteToFloat(byte[] array)
        {
            float[] floatArr = new float[array.Length / 4];
            for (int i = 0; i < floatArr.Length; i++)
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(array, i * 4, 4);
                }
                floatArr[i] = BitConverter.ToSingle(array, i * 4) / 0x80000000;
            }
            return floatArr;
        }

        public static async Task<AudioClip> LoadAudioClip(string filename)
        {
            return await LoadClip(filename);
        }

        static async Task<AudioClip> LoadClip(string path)
        {
            AudioClip clip = null;
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                uwr.SendWebRequest();

                // wrap tasks in try/catch, otherwise it'll fail silently
                try
                {
                    while(!uwr.isDone) await Task.Delay(5);

                    if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.Log($"{uwr.error}");
                    }
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
                catch (Exception err)
                {
                    Debug.Log($"{err.Message}, {err.StackTrace}");
                }
            }

            return clip;
        }

#endregion

#region VIDEO


        public static void GetVideoClip(MonoBehaviour instance, string filename, Action<VideoClip, string> callback)
        {
            instance.StartCoroutine(GetVideoClip(filename, callback));
        }

        public static IEnumerator GetVideoClip(string filename, Action<VideoClip, string> callback)
        {
            string clipPath = GetPath_ForVideos(true) + filename;// Path.GetFileNameWithoutExtension(filename);
            //if (Application.isEditor) Debug.Log(clipPath);

            VideoClip clip = Resources.Load<VideoClip>(clipPath);
            if (clip)
            {
                callback(clip, string.Empty);
                //if (Application.isEditor) Debug.Log("Video loaded from Resources");
                yield break;
            }

            //look in disk
            clipPath = GetPath_ForVideos() + filename; //+ Path.GetFileNameWithoutExtension(filename);
           

            if (!FileExistsInDisk(clipPath)) 
            {
                callback(null, string.Empty);
                //if (Application.isEditor) Debug.Log("Video file is missing from disk!");
                yield break;
            }

            Uri uri = new Uri(clipPath, UriKind.Absolute);//get absolute path for mobile >> file:///
            //if (Application.isEditor) Debug.Log(uri);

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            callback(null, uri.ToString());
#else
            callback(null, clipPath);
#endif
            //if (Application.isEditor) Debug.Log("Video url loaded from Disk");

        }

        #endregion

        #region IMAGE


        public static Texture2D LoadTexture(string filename, string defaultTexture)
        {
            if(filename.IsNull()) return Resources.Load<Texture2D>(GetPath_ForImages(true) + defaultTexture);
            //check resources first
            string imgResourcesPath = GetPath_ForImages(true) + filename.WithNoExtension();

            //if (Application.isEditor) Debug.Log(imgResourcesPath);

            textureToReleaseMemory = Resources.Load<Texture2D>(imgResourcesPath);
            if (textureToReleaseMemory != null)
            {
                 //if (Application.isEditor) Debug.Log("Loaded " + filename + " from: Resources");
                return textureToReleaseMemory;
            }

            //look in disk
            string imgDiskPath = GetPath_ForImages() + filename.WithNoExtension();// Path.GetFileNameWithoutExtension(filename);
            if (!FileExistsInDisk(imgDiskPath))
            {
                if (Application.isEditor) Debug.LogWarning("Failed To Load " + filename + " from: " + imgDiskPath.Replace("/", "\\"));
                if (Application.isEditor) Debug.Log("LOADING DEFAULT >> " + defaultTexture);
                return Resources.Load<Texture2D>(GetPath_ForImages(true) + defaultTexture);
            }

            try
            {
                if(textureToReleaseMemory == null) { textureToReleaseMemory = new Texture2D(2, 2); /*Debug.Log("NEW TEXTURE!!!!");*/ }
                //byte[] a = Decompress(File.ReadAllBytes(imgDiskPath));
                textureToReleaseMemory.LoadImage(File.ReadAllBytes(imgDiskPath)); //(a); //auto-resize the texture dimensions.
                //if (Application.isEditor) Debug.Log("Disk Loaded " + filename + " from: " + imgDiskPath.Replace("/", "\\"));

            }
            catch (Exception e)
            {
                if (Application.isEditor)
                {
                    Debug.LogWarning("Failed To Load " + filename + " from: " + imgDiskPath.Replace("/", "\\"));
                    Debug.LogWarning("Error: " + e.Message);
                }
                textureToReleaseMemory = Resources.Load<Texture2D>(GetPath_ForImages(true) + defaultTexture);
            }

            return textureToReleaseMemory;
        }


        #endregion

        #region JSON

        public static string LoadDataJsonAsString(string dataFileName, bool hasExtensionInName)
        {
            string tempPath = GetPath_ForJsons(); //Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + (hasExtensionInName ? "" : MyExtension(Ext.JSON)));

            //Exit if Directory or File does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                if (Application.isEditor) Debug.Log(dataFileName + " Directory does not exist at " + tempPath);
                return null;
            }

            if (!File.Exists(tempPath))
            {
                if (Application.isEditor) Debug.Log(dataFileName + " does not exist in " + tempPath);
                return null;
            }

            //Load saved Json
            byte[] newBytes = null;
            try
            {
                newBytes = File.ReadAllBytes(tempPath);
                if (Application.isEditor)
                    Debug.Log("Loaded " + dataFileName + " from: " + tempPath.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                if (Application.isEditor)
                {
                    Debug.LogWarning("Failed To Load " + dataFileName + " from: " + tempPath.Replace("/", "\\"));
                    Debug.LogWarning("Error: " + e.Message);
                }
            }

            //Convert to json string
            string newData = Encoding.UTF8.GetString(newBytes);

            //Convert to Object
            //object resultValue =  JsonUtility.FromJson<T>(newData);
            return newData;
        }

#endregion

#endregion

        #region DELETE FILE

        public static void DeleteAllDataFolders()
        {
            if (Directory.Exists(GetAppDataFolderPath())) { Directory.Delete(GetAppDataFolderPath(), true); }
            Directory.CreateDirectory(GetAppDataFolderPath());
        }

        public static void DeleteJsonsFolder()
        {
            if (Directory.Exists(GetPath_ForJsons())) { Directory.Delete(GetPath_ForJsons(), true); }
            Directory.CreateDirectory(GetPath_ForJsons());
        }

        public static void DeleteAudioFolder()
        {
            if (Directory.Exists(GetPath_ForAudios())) { Directory.Delete(GetPath_ForAudios(), true); }
            Directory.CreateDirectory(GetPath_ForAudios());
        }

        public static void DeleteImagesFolder()
        {
            if (Directory.Exists(GetPath_ForImages())) { Directory.Delete(GetPath_ForImages(), true); }
            Directory.CreateDirectory(GetPath_ForImages());
        }

        public static bool DeleteData(string dataFileName, Ext fileExtension = Ext.NULL)
        {
            bool success = false;

            //Load Data
            string tempPath = GetPath_ForJsons();
            tempPath = Path.Combine(tempPath, dataFileName + MyExtension(fileExtension));

            //Exit if Directory or File does not exist
            if (!File.Exists(Path.GetDirectoryName(tempPath)))
            {
                if (Application.isEditor)
                {
                    Debug.Log(dataFileName + " Directory does not exist to " + tempPath);
                }
                return false;
            }

            if (!File.Exists(tempPath))
            {
                if (Application.isEditor)
                {
                    Debug.Log(dataFileName + " does not exist > " + tempPath);
                }
                return false;
            }

            try
            {
                File.Delete(tempPath);

                if (Application.isEditor)
                {
                    Debug.Log(dataFileName + " deleted from: " + tempPath.Replace("/", "\\"));
                }

                success = true;

            }
            catch (Exception e)
            {
                if (Application.isEditor)
                {
                    Debug.LogWarning("Failed To Delete " + dataFileName + ": " + e.Message);
                }
            }

            return success;
        }

        #endregion

        #region GET PATH

#region File Paths

        public static readonly string dataFolderName = "data";
        /// <summary>
        /// multimedia/images/
        /// </summary>
        public static readonly string imagesFolderPath = "multimedia/images/";
        /// <summary>
        /// multimedia/audios/
        /// </summary>
        public static readonly string audiosFolderPath = "multimedia/audios/";
        public static readonly string videosFolderPath = "multimedia/videos/";
        public static readonly string jsonsFolderPath = "jsons/";

       

#endregion

        public static void OpenDataFolder()
        {
#if UNITY_EDITOR
            //Debug.LogWarning(GetAppDataFolderPath());
            //Application.OpenURL(Application.persistentDataPath);// (GetAppDataFolderPath());

            bool openInsidesOfFolder = false;
            string folderPath = GetAppDataFolderPath().Replace(@"/", @"\");
            Debug.LogWarning(folderPath);
            if (Directory.Exists(folderPath)) { openInsidesOfFolder = true; }
            try { System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + folderPath); }
            catch (System.ComponentModel.Win32Exception e) { e.HelpLink = ""; }
#endif
        }

        /// <summary>
        /// Gets the path of data folder - multi platform.
        /// </summary>
        public static string GetPath_ForJsons(bool fromResources = false)
        {
            if (fromResources) return Path.Combine(dataFolderName, jsonsFolderPath);
            return Path.Combine(GetAppDataFolderPath(), jsonsFolderPath);
        }

        public static string GetPath_ForAudios(bool fromResources = false)
        {
            if (fromResources) return Path.Combine(dataFolderName, audiosFolderPath);
            return Path.Combine(GetAppDataFolderPath(), audiosFolderPath);
        }

        public static string GetPath_ForImages(bool fromResources = false)
        {
            if (fromResources) return Path.Combine(dataFolderName, imagesFolderPath);
            return Path.Combine(GetAppDataFolderPath(), imagesFolderPath);
        }

        public static string GetStreamingPath_ForImages()
        {
            return Path.Combine(Application.streamingAssetsPath, imagesFolderPath);
        }

        public static string GetPath_ForVideos(bool fromResources = false)
        {
            if (fromResources) return Path.Combine(dataFolderName, videosFolderPath);
            return Path.Combine(GetAppDataFolderPath(), videosFolderPath);
        }

        public static string GetAppDataFolderPath()
        {
            return Path.Combine(Application.persistentDataPath, dataFolderName);
        }

#endregion

        #region GET FILE EXTENSION

        public enum Ext { TXT, WAV, XML, JPG, PNG, JPEG, MP4, JSON,  NULL }

        public static string MyExtension(Ext xt)
        {
            return xt switch
            {
                Ext.TXT => ".txt",
                Ext.WAV => ".wav",
                Ext.XML => ".xml",
                Ext.JPG => ".jpg",
                Ext.PNG => ".png",
                Ext.JPEG => ".jpeg",
                Ext.MP4 => ".mp4",
                Ext.JSON => ".json",
                _ => string.Empty,
            };
        }

        #endregion


        #region " BYTE[] COMPRESSION / DECOMPRESSION "

        /// <summary>
        /// Compress the loaded bytes.
        /// </summary>
        public static void CompressLoadedBytes(byte[] bytes)
        {
            bytes = Compress(bytes);
        }

        /// <summary>
        /// Decompress the loaded bytes.
        /// </summary>
        public static void DecompressLoadedBytes(byte[] bytes)
        {
            bytes = Decompress(bytes);
        }

        public static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }
        private static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        /// <summary>
        /// Execute a comparison between the loaded data and the compressed data, 
        /// so as to estimate the rate compression. 
        /// </summary>
        /// <returns></returns>
        public double GetRateWithCompression(byte[] bytes)
        {
            var compressed = Compress(bytes);
            double percent = (compressed.Length / (double)bytes.Length) * 100;
            percent = System.Math.Round(100 - percent, 2);
            return percent;
        }

        #endregion


        #region " BASE64 STRING CONVERSION "

        public static byte[] FromBase64String(string data)
        {
            return System.Convert.FromBase64String(data);
        }

        public static string ToBase64String(byte[] data)
        {
            return System.Convert.ToBase64String(data);
        }

        #endregion


        #endregion

    }

}
