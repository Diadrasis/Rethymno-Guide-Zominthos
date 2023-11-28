//Stathis Georgiou ©2021
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace StaGeGames.BestFit.EditorSpace
{
    [InitializeOnLoad]
    [CustomEditor(typeof(SmartManager))]
    //[ExecuteAlways]
    public class SmartManagerEditor : Editor
    {
        static SmartManagerEditor()
        {
            EditorSceneManager.sceneOpened += SceneOpenedCallback;
            //EditorApplication.hierarchyChanged += InitializeThis;
        }

       // [InitializeOnLoadMethod]
       // static void OnProjectLoadedInEditor()
       // {
            //Debug.Log("Project loaded in Unity Editor");
            //Selection.activeObject = FindObjectOfType<SmartManager>().gameObject;
        //}

        static SmartManager myTarget;
        static Vector2 screenSize;

        private void OnEnable()
        {
            //Debug.Log("On Enabled");
            InitializeThis();
            CheckSize();
            myTarget.ApplyResizeToAll();
        }

        private void InitializeThis()
        {
            myTarget = (SmartManager)target;
            if (myTarget == null) return;
            screenSize = BF_EditorUtils.GetMainGameViewSize();
            EditorApplication.update += CheckSize;
        }

        static void SceneOpenedCallback(Scene scene, UnityEditor.SceneManagement.OpenSceneMode _mode)
        {
            Debug.LogFormat("SCENE {0} LOADED", scene.name);
            CheckSize();
            SmartManager smartManager = FindObjectOfType<SmartManager>();
            if (smartManager) Selection.activeObject = FindObjectOfType<SmartManager>().gameObject;
        }

        private static void CheckSize()
        {
            if (myTarget == null) return;
            if (!myTarget.gameObject.activeSelf || !myTarget.gameObject.activeInHierarchy) return;
            if (!myTarget.isActive) return;
            if (myTarget.checkScreenChanged)
            {
                if (Application.isEditor)
                {
                    // Ensure continuous Update calls.
                    if (!Application.isPlaying)
                    {
                        EditorApplication.QueuePlayerLoopUpdate();
                        SceneView.RepaintAll();
                        if (BF_EditorUtils.GetMainGameViewSize() != screenSize)
                        {
                            screenSize = BF_EditorUtils.GetMainGameViewSize();
                            myTarget.InvokeDelayApply();
                            //Debug.Log("[SmartManager] Game View changed to " + screenSize.x + " X " + screenSize.y);
                        }
                    }
                }
            }
        }

        // Custom GUILayout progress bar.
        void ProgressBar(float value, string label)
        {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }
        Bounds bounds;
        public override void OnInspectorGUI()
        {
            if (myTarget == null) return;
            if (!myTarget.gameObject.activeSelf || !myTarget.gameObject.activeInHierarchy) return;

            ProgressBar(100f / 100.0f, "Smart Manager");

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = BF_EditorUtils.HexColor("#C2C2C2", Color.gray);

            GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
            TextFieldStyles.richText = true;

            BF_EditorParams.StaGeLabel();

            TextFieldStyles.fontSize = 14;
            TextFieldStyles.fontStyle = FontStyle.Bold;
            TextFieldStyles.normal.textColor = Color.yellow;
            TextFieldStyles.alignment = TextAnchor.MiddleCenter;

            GUI.backgroundColor = oldColor;
            EditorGUILayout.Space(20);
            myTarget.isActive = EditorGUILayout.Toggle("Is Active ?", myTarget.isActive);
            if (!myTarget.isActive) return;
            EditorGUILayout.Space();

            EditorGUILayout.Space(30);
            myTarget.checkScreenChanged = EditorGUILayout.Toggle("Auto Apply On Screen Size Changed?", myTarget.checkScreenChanged);
            EditorGUILayout.Space();

            GUI.backgroundColor = Color.gray;

            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            if (GUILayout.Button("Apply Resize [ALL]")) { myTarget.ApplyResizeToAll(); }
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            if (GUILayout.Button("Apply Resize [Active Only]")) { myTarget.ApplyResizeToActive(); }
            EditorGUILayout.Space(30);
        }

    }

}
