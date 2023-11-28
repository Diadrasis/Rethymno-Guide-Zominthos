//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
#if UNITY_EDITOR
	[CustomEditor(typeof(JsonDataDatabase))]
	public class CreateJsonDatabaseEditor : Editor
	{
		private JsonDataDatabase script;
		Color colDefault;
		private GUIStyle style;

		private void OnEnable()
		{
			script = (JsonDataDatabase)target;

			style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleLeft;
			style.fontSize = 11;
		}


		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			base.OnInspectorGUI();

			colDefault = GUI.color;
			EditorGUI.BeginChangeCheck();

            if (script.IsJsonMissing())
            {
				GUILayout.Space(20);
				style.fontSize = 19;
                style.normal.textColor = Color.cyan;
                GUILayout.Label("Please first assign all json files!", style);
                GUILayout.Space(30f);
                style.fontSize = 11;
                style.normal.textColor = Color.white;
				return;
			}

			EditorGUILayout.Space(10f);
			GUILayout.Label("[Use bellow button to create the database]", style);

			if (GUILayout.Button("Read Json files - Override"))
			{
                if (EditorUtility.DisplayDialog("Retrieve data from json files",
                "All items and changes will be overridden!", "Read", "Abort"))
                {
                    script.ReadDefaultJsonFilesOverridde();
                    SaveMyChanges();
                }
            }

			EditorGUILayout.Space(10f);
			if (GUILayout.Button("Count Total Unique Images"))
			{
				//if (EditorUtility.DisplayDialog("Retrieve data from json file",
				//"All items and changes will be overridden!", "Read", "Abort"))
				//{
				//	SaveChanges();
				//}

				script.GetAllUniqueFilesFromEntities();
			}

			//EditorGUILayout.Space(50f);
			//if (GUILayout.Button("Get total elements info"))
			//{
			//             script.GetTotals();
			//             SaveChanges();
			//         }

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		public void SaveMyChanges()
		{
			Debug.Log(script.name + " Saved!");
			EditorUtility.SetDirty(script);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}



	}
#endif

}
