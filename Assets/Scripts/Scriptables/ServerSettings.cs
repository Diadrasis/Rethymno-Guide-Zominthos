//Diadrasis ©2023
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[CreateAssetMenu(fileName = "Server_Settings", menuName = "Settings/New Server Settings")]
	[Serializable]
	public class ServerSettings : ScriptableObject
	{
		[Header("[EDITOR ONLY]--------------------")]
		public bool EditorDebug = true;
		[Header("-----------------------------------")]

		[SerializeField]
		private string serverFolder;
        [SerializeField]
        private string serverFolderFallback;
        public string ServerRootFolder() { return serverFolder; }
        public string ServerRootFolderFallback() { return serverFolderFallback; }

        [SerializeField]
		private string phpGetSizeUrl;
        [SerializeField]
        private string phpGetSizeUrlFallback;
        public string PHP_GetSizeURL() { return phpGetSizeUrl; }
        public string PHP_GetSizeURLfallback() { return phpGetSizeUrlFallback; }

        [Header("Time Interval to check internet connection and updates")]
		public float internetCheckTimeInterval = 10f;
		[Space]
		public float requestFileAbortTime = 3f;
		[Space]
		public bool avoidDownloadBigImages = true;
		[Space]
		public bool WaitEachFileSaveComplete = true;

    }

}
