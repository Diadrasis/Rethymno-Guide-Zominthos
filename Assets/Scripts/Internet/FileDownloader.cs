//Diadrasis ©2023 - Stathis Georgiou
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Threading;
using System.IO.Compression;
using static Diadrasis.Rethymno.EnumsHolder;
using System.Linq;

namespace Diadrasis.Rethymno 
{

	public class FileDownloader : Singleton<FileDownloader>
	{
        private List<string> filesUrls = new List<string>();

        public ServerSettings settings;
        public TMPro.TextMeshProUGUI progressText;
        public Slider slider;

        UnityWebRequest uwr;

        [ContextMenu("Async Media")]
        public void DownloadMediaAsync()
        {
            filesUrls.Clear();
            filesUrls.AddRange(DataManager.Instance.GetImagesUniqueFilenames());
            filesUrls.AddRange(DataManager.Instance.GetAudiosUniqueFilenames());
            filesUrls.AddRange(DataManager.Instance.GetVideosUniqueFilenames());

            progressText.text = "0 %";
            if (slider) slider.minValue = 0;
            if (slider) slider.maxValue = filesUrls.Count;
            if (slider) slider.value = 0;

            StartMultimediaDownload();
        }
        [ContextMenu("Async Jsons")]
        public void DownloadJsonsAsync()
        {
            filesUrls.Clear();
            filesUrls.AddRange(GlobalUtils.jsonExportFiles.ToList());

            progressText.text = "0 %";
            if (slider) slider.minValue = 0;
            if (slider) slider.maxValue = filesUrls.Count;
            if (slider) slider.value = 0;

            StartJsonsDownload();
        }

        public void DownloadJsonFiles()
        {
            filesUrls.Clear();
            filesUrls.AddRange(GlobalUtils.jsonExportFiles.ToList());

            progressText.text = "0 %";
            if (slider) slider.minValue = 0;
            if (slider) slider.maxValue = filesUrls.Count;
            if (slider) slider.value = 0;

            StartCoroutine(DownloadFiles(false));
        }

        public void DownloadMultimediaFiles()
        {
            filesUrls.Clear();

            if (Application.isEditor) { Debug.LogFormat("DownloadMultimediaFiles are {0}", DataManager.Instance.GetImagesUniqueFilenames().Count);}

            //if (DataManager.Instance.useSelectedAreas)
            //{
            //    filesUrls.AddRange(DataManager.Instance.Get());
            //}
            //else
            //{
                filesUrls.AddRange(DataManager.Instance.GetImagesUniqueFilenames());
           // }
            filesUrls.AddRange(DataManager.Instance.GetAudiosUniqueFilenames());
            filesUrls.AddRange(DataManager.Instance.GetVideosUniqueFilenames());

            progressText.text = "0 %";
            if (slider) slider.minValue = 0;
            if (slider) slider.maxValue = filesUrls.Count;
            if (slider) slider.value = 0;

            StartCoroutine(DownloadFiles(true));
        }

        [ContextMenu("TEST DOWNLOAD")]
        void TestDownload()
        {
            filesUrls.Clear();
            filesUrls.Add("ulf_43_EFA RETHIMNOU -2023-478.JPG");
            progressText.text = "0 %";
            if (slider) slider.minValue = 0;
            if (slider) slider.maxValue = filesUrls.Count;
            if (slider) slider.value = 0;

            StartCoroutine(DownloadFiles(true));
        }

        #region download as files

        IEnumerator DownloadFiles(bool isMedia)
        {
            float t = Time.realtimeSinceStartup;

            string serverfolderurl = settings.ServerRootFolder();

            for (int i = 0; i < filesUrls.Count; i++)
            {
                if (filesUrls[i].StartsWith("ulf_43_EFA")) Debug.LogWarning("################## FOUND ###################");

                string url = filesUrls[i];//.Trim();

                //Debug.LogWarning(url);

                string filePath = string.Empty;
                FileType type = GetFileType(url);

                switch (type)
                {
                    case FileType.IMAGE:
                        filePath = SaveLoadManager.GetPath_ForImages();
                        break;
                    case FileType.AUDIO:
                        filePath = SaveLoadManager.GetPath_ForAudios();
                        break;
                    case FileType.VIDEO:
                        filePath = SaveLoadManager.GetPath_ForVideos();
                        break;
                    case FileType.JSON:
                        filePath = SaveLoadManager.GetPath_ForJsons();
                        break;
                    default:
                    case FileType.UNKNOWN:
                        //abort
                        continue;
                }

                filePath = Path.Combine(filePath, url.WithNoExtension());// url);

                //if (Application.isEditor) Debug.LogWarning(filePath);

                if (!SaveLoadManager.FileExistsInDisk(filePath))
                {

                    //var uwr = new UnityWebRequest(new Uri(serverfolderurl + url), UnityWebRequest.kHttpVerbGET);
                    using (uwr = UnityWebRequest.Get(new Uri(serverfolderurl + url)))
                    {
                        var dh = new DownloadHandlerFile(filePath);
                        dh.removeFileOnAbort = true;
                        uwr.downloadHandler = dh;


                        yield return uwr.SendWebRequest();
                        if (uwr.result != UnityWebRequest.Result.Success)
                        {
#if UNITY_EDITOR
                            Debug.LogWarning("[" + serverfolderurl + url + ">> ]" + uwr.error);
#endif
                            if (!isMedia)
                            {
                                EventHolder.OnDownloadJsonFailed?.Invoke(url);
                            }
                        }
//#if UNITY_EDITOR
//                        else { Debug.Log("File successfully downloaded and saved to " + filePath); }
//#endif

                        uwr.Dispose();
                    }

                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }

                if (slider && progressText)
                {
                    slider.value += 1f;
                    float p = 100f * slider.value / filesUrls.Count;
                    progressText.text = p.ToString("F0") + " %";
                }
            }

            yield return new WaitForEndOfFrame();

            if (isMedia)
            {
                EventHolder.OnFilesDownloadComplete?.Invoke();
            }
            else
            {
                EventHolder.OnJsonsDownloadComplete?.Invoke();
            }

            float td = Time.realtimeSinceStartup - t;

            if (settings.EditorDebug)
                Debug.LogWarningFormat("Done DownloadFiles in {0} seconds", td);

            yield break;
        }

        #endregion

        #region ASYNC

        private async void StartJsonsDownload()
        {
            await DownloadFilesAsync();
            EventHolder.OnJsonsDownloadComplete?.Invoke();
        }

        private async void StartMultimediaDownload()
        {
            await DownloadFilesAsync();
            EventHolder.OnFilesDownloadComplete?.Invoke();
        }

        private async Task DownloadFilesAsync()
        {
            string serverfolderurl = settings.ServerRootFolder();

            for (int i = 0; i < filesUrls.Count; i++)
            {
                string url = filesUrls[i];
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(new Uri(serverfolderurl + url));
                var downloadOperation = www.SendWebRequest();

                while (!downloadOperation.isDone)
                {
                    await Task.Delay(10);
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
#if UNITY_EDITOR
                    Debug.Log("ERROR " + filesUrls[i]);
                    Debug.LogWarning("ERROR_1 " + www.error);
                    Debug.LogWarning("ERROR_2 " + www.downloadHandler.error);
#endif
                }
                else
                {
                    await SaveImageAsync(www.downloadHandler.data, url);
                }

                //currentIndex = i + 1;
                // progressText.text = $"Downloading {currentIndex} of {imageUrls.Count} images...";

                if (slider && progressText)
                {
                    slider.value += 1f;
                    float p = 100f * slider.value / filesUrls.Count;
                    progressText.text = p.ToString("F0") + " %";
                }
            }
        }

        private async Task SaveImageAsync(byte[] bytes, string imageName)
        {
            bytes = SaveLoadManager.Compress(bytes);
            string filePath = SaveLoadManager.GetPath_ForImages();
            filePath = Path.Combine(filePath, imageName);

            using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }


        #endregion


        private FileType GetFileType(string _filename)
        {
            _filename = _filename.ToLower();

            if (_filename.EndsWith(new string[] { "mp3", "ogg", "wav", "aiff" }))
            {
                return FileType.AUDIO;
            }
            else if (_filename.EndsWith(new string[] { "png", "jpeg", "jpg", "bmp", "tif", "tga", "psd" }))
            {
                return FileType.IMAGE;
            }
            else if (_filename.EndsWith(new string[] { "3gp", "mp4", "m4v", "mov", "mpg", "mpeg", "webm" }))
            {
                return FileType.VIDEO;
            }
            else if (_filename.EndsWith("json"))
            {
                return FileType.JSON;
            }
            else 
            {
                return FileType.UNKNOWN;
            }
        }

        void OnApplicationQuit()
        {
#if UNITY_EDITOR
            var constructor = SynchronizationContext.Current.GetType().GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
            var newContext = constructor.Invoke(new object[] { Thread.CurrentThread.ManagedThreadId });
            SynchronizationContext.SetSynchronizationContext(newContext as SynchronizationContext);
#endif
        }

    }

}
