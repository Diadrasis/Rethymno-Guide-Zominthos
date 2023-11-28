//StaGe Games ©2021
using UnityEngine;
using UnityEditor;

namespace StaGeGames.BestFit.EditorSpace
{

    [CustomEditor(typeof(SmartMotion))]
    [CanEditMultipleObjects]
    [ExecuteInEditMode]
    public class SmartMotionEditor : Editor
    {
        private static bool OptionalButtons, isPanelHidden = true, showEvents = false, editorEventMode;
        private static UnityEngine.Events.UnityEventCallState unityEventCallState = UnityEngine.Events.UnityEventCallState.EditorAndRuntime;
        private static SerializedProperty OnShowStart, OnShowComplete, OnHideStart, OnHideComplete, OnStartPercentage, OnEndPercentage;
        private static SerializedProperty OnShowStartCalls, OnShowCompleteCalls, OnHideStartCalls, OnHideCompleteCalls, OnStartPercentageCalls, OnEndPercentageCalls;
        private SerializedProperty boolisAutoSpeed, floatAutoSpeedMultiplier;
        private static SmartMotion myTarget;

        private void OnEnable()
        {
            myTarget = (SmartMotion)target;
            if (myTarget == null) return;
            //myTarget.hideFlags = HideFlags.HideInInspector;

            //Undo.undoRedoPerformed += myTarget.Init;

            boolisAutoSpeed = serializedObject.FindProperty(nameof(myTarget.isAutoSpeed));
            floatAutoSpeedMultiplier = serializedObject.FindProperty(nameof(myTarget.autoSpeedMultiplier));

            GetEvents(serializedObject);

        }

        public override void OnInspectorGUI()
        {
            if (myTarget == null) return;

            serializedObject.Update();

            Undo.RecordObject(myTarget, "SmartMotion");

            var oldColor = GUI.backgroundColor;
           // GUI.backgroundColor = BF_EditorUtils.HexColor("#C2C2C2", Color.gray);

            GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
            TextFieldStyles.richText = true;

            BF_EditorParams.StaGeLabel();

            TextFieldStyles.fontSize = 14;
            TextFieldStyles.fontStyle = FontStyle.Bold;
            TextFieldStyles.normal.textColor = Color.yellow;
            TextFieldStyles.alignment = TextAnchor.MiddleCenter;

            //GUI.backgroundColor = Color.black;

            if (!myTarget.isInEditorAlive)
            {
                GUI.color = Color.yellow;
                GUILayout.Label("Not editable because SmartResize do not need to move");
                return;
            }

            EditorGUILayout.Space();

            TextFieldStyles.fontSize = 18;

            if (!myTarget.gameObject.activeInHierarchy)
            {
                //GUI.color = Color.red;
                TextFieldStyles.fontStyle = FontStyle.Bold;
                TextFieldStyles.normal.textColor = Color.red;
                TextFieldStyles.alignment = TextAnchor.MiddleCenter;
                if (GUILayout.Button("Panel is inactive! Gameobject is disabled", TextFieldStyles)) { Debug.LogWarning("Panel is inactive!"); }
            }
            else
            {
                TextFieldStyles.normal.textColor = Color.white;
                TextFieldStyles.hover.textColor = Color.green;
                GUI.color = Color.cyan;
                isPanelHidden = !myTarget.isVisible;
                if (!isPanelHidden) { if (GUILayout.Button("Hide Target", TextFieldStyles)) { myTarget.HidePanel(); isPanelHidden = true; } }
                else { if (GUILayout.Button("Show Target", TextFieldStyles)) { myTarget.ShowPanel(); isPanelHidden = false; } }
            }

            GUI.color = oldColor;

            TextFieldStyles.fontSize = 18;
            TextFieldStyles.fontStyle = FontStyle.Bold;
            TextFieldStyles.normal.textColor = Color.white;
            TextFieldStyles.alignment = TextAnchor.MiddleCenter;


            EditorGUILayout.Space();

            //if (GUILayout.Button(btnLabel, TextFieldStyles)) {
            //    showVariables = !showVariables;
            //    btnLabel = showVariables ? "Hide Script Variables" : "Show Script Variables";
            //}
            GUI.backgroundColor = oldColor;
            GUI.color = Color.white;

            //if (showVariables) DrawDefaultInspector();

            OptionalButtons = EditorGUILayout.Foldout(OptionalButtons, "Optional Buttons", true);
            if (OptionalButtons)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("showButtons"), new GUIContent("Show Buttons","[Optional] Show Panel buttons"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hideButtons"), new GUIContent("Hide Buttons", "[Optional] Hide Panel buttons"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("toggleVisibilityButtons"), new GUIContent("Toggle Buttons", "[Optional] Toggle Visibility buttons"));
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(boolisAutoSpeed, new GUIContent("Is Auto Speed"));

            if (!myTarget.isAutoSpeed)
            {
                myTarget.moveSpeedCustom = EditorGUILayout.FloatField("Custom Move Speed", myTarget.moveSpeedCustom);
            }
            else
            {
                myTarget.autoSpeedMultiplier = EditorGUILayout.FloatField("Speed Multiplier", myTarget.autoSpeedMultiplier);
            }

           
            EditorGUILayout.Space();

            #region Events

            unityEventCallState = (UnityEngine.Events.UnityEventCallState)EditorGUILayout.EnumPopup(new GUIContent("Events Call State", "Replace all to Editor and/or Runtime\nSelect Off for custom usage"), unityEventCallState);
            if(unityEventCallState == UnityEngine.Events.UnityEventCallState.Off)
            {
                EditorGUILayout.HelpBox("With call state to Off you can set custom call for each event", MessageType.Info, false);
            }
            showEvents = EditorGUILayout.Foldout(showEvents, "Events", true);


            if (showEvents)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(OnShowStart);
                EditorGUILayout.PropertyField(OnShowComplete);
                EditorGUILayout.PropertyField(OnStartPercentage);
                EditorGUILayout.PropertyField(OnEndPercentage);
                EditorGUILayout.PropertyField(OnHideStart);
                EditorGUILayout.PropertyField(OnHideComplete);
                
            }
            EditorGUILayout.Space();
            CheckEventCalls();
            #endregion


            if (Application.isEditor)
            {
                // Ensure continuous Update calls.
                if (!EditorApplication.isPlaying)
                {
                    EditorApplication.QueuePlayerLoopUpdate();
                    SceneView.RepaintAll();
                }
            }

            //DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();

        }

        private static void GetEvents(SerializedObject target)
        {
            OnShowStart = target.FindProperty("OnShowStart"); 
            OnShowComplete = target.FindProperty("OnShowComplete");
            OnHideStart = target.FindProperty("OnHideStart");
            OnHideComplete = target.FindProperty("OnHideComplete");
            OnStartPercentage = target.FindProperty("OnShowPercentageStart");
            OnEndPercentage = target.FindProperty("OnShowPercentageComplete");
        }

        private static void CheckEventCalls()
        {
            CheckEvent(OnShowStart, OnShowStartCalls);
            CheckEvent(OnShowComplete, OnShowCompleteCalls);
            CheckEvent(OnHideStart, OnHideStartCalls);
            CheckEvent(OnHideComplete, OnHideCompleteCalls);
            CheckEvent(OnStartPercentage, OnStartPercentageCalls);
            CheckEvent(OnHideComplete, OnEndPercentageCalls);
        }

        private static void CheckEvent(SerializedProperty main, SerializedProperty calls)
        {
            calls = main.FindPropertyRelative("m_PersistentCalls.m_Calls");
            if (calls.arraySize > 0)
            {
                EditorGUILayout.PropertyField(main);

                if (unityEventCallState != UnityEngine.Events.UnityEventCallState.Off)
                {
                    for (int i = 0; i < calls.arraySize; ++i)
                    {
                        if (calls.GetArrayElementAtIndex(i).FindPropertyRelative("m_CallState") != null)
                            calls.GetArrayElementAtIndex(i).FindPropertyRelative("m_CallState").intValue = (int)unityEventCallState;
                    }
                }
            }
        }


    }

}
