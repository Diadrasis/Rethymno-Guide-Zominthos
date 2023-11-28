//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
#if UNITY_EDITOR
	[CustomEditor(typeof(LocaleItem))]
	public class LocaleItemEditor : Editor
	{
		private LocaleItem script;
		Color colDefault;
		private GUIStyle style;
		EnumsHolder.Language lang;

		private void OnEnable()
		{
			script = (LocaleItem)target;

			style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 14;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			base.OnInspectorGUI();

			EditorGUILayout.BeginHorizontal();

			EditorGUI.BeginChangeCheck();

			lang = (EnumsHolder.Language)EditorGUILayout.EnumPopup("Select Language:", lang);

			if (GUILayout.Button("Apply Text"))
			{
				ApplyText();
			}

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}

			EditorGUILayout.EndHorizontal();
		}

		void ApplyText()
        {
			DataManager dataManager = FindObjectOfType<DataManager>(true);
			if (!dataManager)
			{
				if (EditorUtility.DisplayDialog("DataManager is missing",
					"DtataManager instance is not in the scene", "OK"))
				{
					return;
				}
			}
			script.ApplyCurrentLanguage(lang);
			EditorUtility.SetDirty(script);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

    }
#endif
}
