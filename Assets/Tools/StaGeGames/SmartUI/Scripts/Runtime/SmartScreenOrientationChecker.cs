//Stathis Georgiou ©2021
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StaGeGames.BestFit
{
	public class SmartScreenOrientationChecker : MonoBehaviour
	{
		private static SmartScreenOrientationChecker instance = null;
        public static SmartScreenOrientationChecker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SmartScreenOrientationChecker>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject(typeof(SmartScreenOrientationChecker).Name);
						instance = go.AddComponent<SmartScreenOrientationChecker>();
                    }
                    //else { Destroy(instance); }
                }
                return instance;
            }
        }
        public enum OrientMode { NULL, LANDSCAPE, PORTRAIT }
		public OrientMode orientMode = OrientMode.NULL;
		public OrientMode orientModeNow = OrientMode.NULL;
		[SerializeField]
		private Vector2 ScreenSize;

		[Header("Resize on screen orientation has changed")]
		[SerializeField]
		private bool allowSmartResizesToInit = true;

		[Header("Refresh if size of parentRect has changed. [LateUpdate]")]
		[SerializeField]
		private bool allowSmartResizesCheckInUpdate = false;

		[Space(10)]
		[SerializeField]
		List<BestResize> smartResizers = new List<BestResize>();
		
		private int width, height;

		[Space(20)]
		[SerializeField]
		private UnityEvent OnOrientationHasChanged;
		[SerializeField]
		private UnityEvent OnLandscape, OnPortrait;
		[SerializeField]
		private UnityEvent OnBeforeResize, OnResizeComplete;

		/// <summary>
		/// When a smart resize item instatiated or already exists in the scene
		/// <para>OnCreateRegister to be added in smartResizers List</para>
		/// <para>OnDestroyUnregister to be removed from smartResizers List</para>
		/// </summary>
		/// <param name="sr"></param>
		public delegate void RegisterEvent(BestResize sr);
		public static RegisterEvent OnCreateRegister, OnDisableUnregister, OnDestroyUnregister;

		private void Awake()
        {
			if (instance == null) { instance = this; }

			Init();
			
			OnCreateRegister += RegisterNewItem;
			OnDisableUnregister += UnregisterItem;
			OnDestroyUnregister += UnregisterItem;
		}

        public void Init()
        {
			smartResizers.Clear();
			Canvas[] canvases = FindObjectsOfType<Canvas>();
			//get all smart resizers in scene
			foreach (Canvas cv in canvases)
				smartResizers.AddRange(cv.transform.GetComponentsInChildren<BestResize>(false));
		}

		private void RegisterNewItem(BestResize smartResize)
        {
			if (smartResizers == null) smartResizers = new List<BestResize>();
			if (!smartResizers.Contains(smartResize)) smartResizers.Add(smartResize);
        }

		private void UnregisterItem(BestResize smartResize)
        {
            if (smartResizers == null) return;
            if (smartResizers.Contains(smartResize)) smartResizers.Remove(smartResize);
        }

		private void Start()
        {
#if UNITY_EDITOR
			GetEditorOrientation();
#else
			GetDeviceOrientation(); 
#endif
			ApplyNewOrientation();
		}

		void CheckResizing()
        {
			//Debug.Log("CheckResizing");
			foreach (BestResize sm in smartResizers)
			{
				if (sm.IsParentSizeChanged()) sm.Init();
			}
		}

        void LateUpdate()
		{
			if (allowSmartResizesCheckInUpdate) CheckResizing();

#if UNITY_EDITOR
			EditorCheck();
#else
			ReleaseChack();
#endif
		}				

		void ApplyNewOrientation()
        {
#if UNITY_EDITOR
			//Debug.LogWarning("ApplyNewOrientation");
			//Debug.LogWarning("OnBeforeResize");
#endif

			CancelInvoke();

			float totalTime = 0f;
			foreach (BestResize sm in smartResizers) {
				if (sm)
				{
					float smTime = sm.GloballyInit();
					if (totalTime < smTime) totalTime = smTime;
				}
			}
			Invoke(nameof(RaiseEventRefreshCompleted), totalTime);

#if UNITY_EDITOR
			//Debug.Log("total time = " + totalTime);
#endif
		}

		private void RaiseEventRefreshCompleted()
        {
			OnResizeComplete?.Invoke();
			CancelInvoke();

#if UNITY_EDITOR
			//Debug.LogWarning("OnResizeComplete");
#endif
		}

#if !UNITY_EDITOR
		void ReleaseChack()
        {
			if (OrientationHasChanged())
			{
				OnBeforeResize?.Invoke();

				if (GetDeviceOrientation() == 0) { OnLandscape?.Invoke(); }
				else { OnPortrait?.Invoke(); }

				if (allowSmartResizesToInit)
				{
					Invoke(nameof(ApplyNewOrientation), 0.2f);
				}
			}
		}

		bool OrientationHasChanged()
        {
			if ((width == Screen.width && height == Screen.height)) return false;
			return true;
        }

		private int GetDeviceOrientation()
		{
			width = Screen.width;
			height = Screen.height;
			ScreenSize = new Vector2(width, height);
			if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
			{
				orientMode = OrientMode.LANDSCAPE;
				orientModeNow = OrientMode.LANDSCAPE;
				return 0;
			}
			else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
			{
				orientMode = OrientMode.PORTRAIT;
				orientModeNow = OrientMode.PORTRAIT;
				return 1;
			}
			else
			{
				orientMode = OrientMode.LANDSCAPE;
				orientModeNow = OrientMode.LANDSCAPE;
				return 0;
			}
		}
#endif



		#region Editor

#if UNITY_EDITOR

		void EditorCheck()
		{
			if (EditorOrientationHasChanged())
			{
				OnBeforeResize?.Invoke();

				if (GetEditorOrientation() == 0) { OnLandscape?.Invoke(); }
				else { OnPortrait?.Invoke(); }

				if (allowSmartResizesToInit)
				{
					Invoke(nameof(ApplyNewOrientation), 0.2f);
				}

				OnOrientationHasChanged?.Invoke();
			}
		}

		bool EditorOrientationHasChanged()
		{
			Vector2 screensize = GetMainGameViewSize(); // Debug.Log(screensize);
			if ((width == (int)screensize.x && height == (int)screensize.y)) { return false; }
			return true;
		}

		private int GetEditorOrientation()
		{
			Vector2 screensize = GetMainGameViewSize();
			width = (int)screensize.x;
			height = (int)screensize.y;
			ScreenSize = new Vector2(width, height);

			if (Screen.width > Screen.height)
			{
				orientMode = OrientMode.LANDSCAPE;
				orientModeNow = OrientMode.LANDSCAPE;
				return 0;
			}
			else
			{
				orientMode = OrientMode.PORTRAIT;
				orientModeNow = OrientMode.PORTRAIT;
				return 1;
			}
		}

		Vector2 GetMainGameViewSize()
		{
			System.Type T = System.Type.GetType("UnityEditor.GameView, UnityEditor");
			System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
			return (Vector2)Res;
		}

#endif

#endregion

	}

}
