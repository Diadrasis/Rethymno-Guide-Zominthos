//StaGe Games ©2022
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StaGeGames.BestFit.EditorSpace
{
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class SmartManager : MonoBehaviour
    {
        [HideInInspector]
        public List<BestResize> smartResizesInScene = new List<BestResize>();
        [HideInInspector]
        public bool checkScreenChanged = true;
        public bool isActive = true;

        public void Init()
        {
            //Debug.Log("[SmartManager] Init");
        }
        
        public void ApplyResizeToActive()
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();

            foreach (Canvas canv in canvases)
            {
                BestFit.BestResize[] smartResizers = canv.GetComponentsInChildren<BestFit.BestResize>(false);
                foreach (BestFit.BestResize b in smartResizers) {  b.Init(); }
                DebugFormat("Action completed for {0} ui smart elements", smartResizers.Length);
            }
        }

        public void ApplyResizeToAll()
        {
            //Debug.Log("[SmartManager] ApplyResizeToAll");
            Canvas[] canvases = FindObjectsOfType<Canvas>();

            foreach (Canvas canv in canvases)
            {
                BestFit.BestResize[] smartResizers = canv.GetComponentsInChildren<BestFit.BestResize>(true);
                foreach (BestFit.BestResize b in smartResizers) { b.Init(); }
                DebugFormat("Action completed for {0} ui smart elements", smartResizers.Length);
            }
        }

        public void InvokeDelayApply()
        {
            Invoke(nameof(ApplyResizeToAll), 0.15f);
        }

        private void DebugFormat(string val, int count)
        {
            //if (Application.isEditor) Debug.LogWarningFormat(val, count);
        }

#if UNITY_EDITOR

        //protected void OnValidate()
        //{
        //    //Debug.Log("OnValidate " + transform.name);
        //    Init();
        //    ApplyResizeToActive();
        //}

        protected void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += _OnValidate;
        }

        protected void _OnValidate()
        {
            if (this == null) return;
            Init();
            ApplyResizeToActive();
        }

        private void Reset()
        {
            Init();
            ApplyResizeToActive();
        }

        [MenuItem("StaGe Games/Test/AddSize")]
        public static void AddTestSize()
        {
            //EditorUserBuildSettings.activeBuildTarget
            Debug.Log(EditorUserBuildSettings.activeBuildTarget);
            //GameViewUtils.AddCustomSize(GameViewUtils.GameViewSizeType.AspectRatio, GameViewSizeGroupType.Android, 666, 666, "TEST_666");
        }


#endif

    }
}
