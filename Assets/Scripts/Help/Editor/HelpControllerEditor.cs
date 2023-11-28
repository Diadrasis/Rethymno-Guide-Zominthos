//Diadrasis �2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Diadrasis.Rethymno
{
    [CustomEditor(typeof(HelpController))]
    public class HelpControllerEditor : Editor
    {
        private HelpController script;

        protected virtual void OnEnable()
        {
            script = (HelpController)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10f);

            GUI.color = Color.cyan;

            if (GUILayout.Button("Create Pages", GUILayout.Width(150f)))
            {
                if (EditorUtility.DisplayDialog("Retrieve data from database",
                "Current pages will be deleted!", "Create", "Abort"))
                {
                    script.DeletePages();
                    script.SetupHelp();
                    SaveChanges();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

        }

        public void SaveChanges()
        {
            Debug.Log(script.name + " Saved!");
            EditorUtility.SetDirty(script);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

}
