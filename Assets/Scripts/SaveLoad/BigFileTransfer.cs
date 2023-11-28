//Diadrasis ©2023 - Stathis Georgiou
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
    /// <summary>
    /// https://forum.unity.com/threads/async-file-write-to-disk-mobile-platforms-solved.496449/
    /// </summary>
	public class BigFileTransfer : MonoBehaviour
	{
        public static BigFileTransfer instance;
        public delegate void OnWriteComplete(bool success);
        void Awake()
        {
            instance = this;
        }

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                DoClientCode();
            }
        }

        void DoClientCode()
        {
            StartCoroutine(DownloadAndThenWrite());
        }

        IEnumerator DownloadAndThenWrite()
        {
            WWW www = new WWW("http://192.168.1.28/data.bin");
            yield return www;

            string fileSavePath = Application.persistentDataPath + "/data.bin";
            Debug.LogWarning("will save data on: " + fileSavePath);
            byte[] tmp_data = www.bytes;
            www.Dispose();

            BigFileTransfer.WriteAllBytesAsync(tmp_data, fileSavePath, success =>
            {
                if (success)
                {
                    Debug.LogWarning("file has been written successfully in: " + fileSavePath);
                }
                else
                {
                    Debug.LogWarning("could not write file in: " + fileSavePath);
                }
            });
        }

        public static void WriteAllBytesAsync(byte[] data, string savePath, OnWriteComplete callback = null)
        {
            Debug.LogWarning("save path API start: " + savePath);
            if (instance.dataPool.ContainsKey(savePath))
            {
                instance.dataPool[savePath] = data;
            }
            else
            {
                instance.dataPool.Add(savePath, data);
            }

            instance.SaveDataThreaded(savePath);

            if (instance.completedFlagPool.ContainsKey(savePath))
            {
                instance.completedFlagPool[savePath] = false;
            }
            else
            {
                instance.completedFlagPool.Add(savePath, false);
            }

            if (instance.callbackPool.ContainsKey(savePath))
            {
                instance.callbackPool[savePath] = callback;
            }
            else
            {
                instance.callbackPool.Add(savePath, callback);
            }
            instance.StartCoroutine(instance.NowCheckForResourceUnload(savePath));
        }

        IEnumerator NowCheckForResourceUnload(string savePath)
        {
            yield return null;
            while (true)
            {
                bool flag = false;
                if (completedFlagPool.ContainsKey(savePath))
                {
                    flag = completedFlagPool[savePath];
                }
                else
                {
                    throw new Exception("could not find the flag in the pool!");
                }
                if (flag)
                {
                    break;
                }
                yield return null;
            }
            //Resources.UnloadUnusedAssets();
            if (completedFlagPool.ContainsKey(savePath))
            {
                completedFlagPool.Remove(savePath);
            }

            if (completedFlagPool.ContainsKey(savePath))
            {
                completedFlagPool.Remove(savePath);
            }
        }

        private Dictionary<string, byte[]> dataPool = new Dictionary<string, byte[]>();
        private Dictionary<string, Thread> threadPool = new Dictionary<string, Thread>();
        private Dictionary<string, bool> completedFlagPool = new Dictionary<string, bool>();
        private Dictionary<string, OnWriteComplete> callbackPool = new Dictionary<string, OnWriteComplete>();


        void SaveDataThreaded(string savePath)
        {
            Thread thread = new Thread(() => SaveDataTaskThreaded(savePath));
            if (threadPool.ContainsKey(savePath))
            {
                threadPool[savePath] = thread;
            }
            else
            {
                threadPool.Add(savePath, thread);
            }
            thread.Start();
        }

        void SaveDataTaskThreaded(string savePath)
        {
            byte[] data = null;
            if (dataPool.ContainsKey(savePath))
            {
                data = dataPool[savePath];
            }
            else
            {
                CallCB(savePath, false);
                throw new Exception("could not find the data to write on dataPool!");
            }
            File.WriteAllBytes(savePath, data);
            Debug.LogWarning("data saved!- used threading for path: " + savePath);
            data = null;

            CallCB(savePath, true);

            if (dataPool.ContainsKey(savePath))
            {
                dataPool.Remove(savePath);
            }
            if (threadPool.ContainsKey(savePath))
            {
                threadPool.Remove(savePath);
            }

            if (callbackPool.ContainsKey(savePath))
            {
                callbackPool.Remove(savePath);
            }
        }
        void CallCB(string savePath, bool flag)
        {
            OnWriteComplete cb = null;
            if (callbackPool.ContainsKey(savePath))
            {
                cb = callbackPool[savePath];
            }

            if (cb != null)
            {
                cb(flag);
            }
        }
    }

}
