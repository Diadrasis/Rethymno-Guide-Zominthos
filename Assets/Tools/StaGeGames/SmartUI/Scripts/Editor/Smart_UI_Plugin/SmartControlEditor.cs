//StaGe Games ©2021
using UnityEngine;
using UnityEditor;
using StaGeGames.BestFit.Utils;

namespace StaGeGames.BestFit.EditorSpace
{
	[CustomEditor(typeof(SmartControl))]
	[ExecuteAlways]
	public class SmartControlEditor : Editor
	{
		SmartControl targetControl;
		GUIStyle currentStyle = null;

        private void OnEnable()
        {
			targetControl = target as SmartControl;
			if (targetControl == null) { return; }
		}

		public override void OnInspectorGUI()
        {
			if (targetControl == null) { return; }
			GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
			TextFieldStyles.richText = true;
			BF_EditorParams.StaGeLabel();
			if (Application.isEditor)
			{
				if (!targetControl.smartMotion.isInEditorAlive)
				{
					GUI.color = Color.yellow;
					GUILayout.Label("Inactive because SmartResize do not need to move");
					return;
                }
				GUI.color = Color.cyan;
				GUILayout.Label("Controls motion in Scene view from Smart Control Panel");
			}
		}


		void OnSceneGUI()
        {
			if (EditorApplication.isPlaying) return;

			InitStyles();

			if (targetControl == null) { return; }

			if (Application.isEditor)
			{
				if (!targetControl.smartMotion.isInEditorAlive)
				{
					GUI.color = Color.yellow;
					GUILayout.Label("Not editable because SmartResize do not need to move");
					return;
				}
			}

			if (targetControl.smartMotion == null && targetControl.smartResize == null)
			{
				if(targetControl.smartResize == null) Debug.LogWarning("[Smart Control] Applies only on elements with SmartUI");
				DestroyImmediate(targetControl);
				return;
			}

            if (targetControl.smartMotion && !targetControl.smartMotion.isInEditorAlive)
            {
                //GUI.color = Color.yellow;
                //GUILayout.Label("Not active because SmartResize do not need to move");
                return;
            }

            Handles.color = Color.blue;

			Handles.BeginGUI();

			GUILayout.BeginArea(new Rect(Screen.width/2f, 10f, Screen.width - Screen.width/2f, 80));
			GUILayout.Label("  Selected: "+targetControl.gameObject.name+"  ", currentStyle);
			GUILayout.EndArea();

			//GUILayout.BeginArea(new Rect(Screen.width - 110, Screen.height - 150, 100, 150));

			//GUILayout.Box(MakeTex(2, 2, Color.black));

			//GUILayout.Label("[Smart Motion]", currentStyle);
			//if (GUILayout.Button("Focus"))
			//{
			//	Selection.activeGameObject = t.gameObject;
			//	SceneView.FrameLastActiveSceneView();
			//}
			//else if (GUILayout.Button("Show"))
			//{
			//	t.smartMotion.ShowPanel();
			//}
			//else if (GUILayout.Button("Hide"))
			//{
			//	t.smartMotion.HidePanel();
			//}

			//GUILayout.EndArea();
			Handles.EndGUI();

			Handles.BeginGUI();

			GUILayout.Window(2, new Rect(Screen.width - 125, Screen.height - 125, 120, 70), (id) => {

				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("FT", "Focus Target")))
				{
					Selection.activeGameObject = targetControl.gameObject;
					SceneView.FrameLastActiveSceneView();
				}
				
				if (targetControl.smartResize != null)
                {
					if (GUILayout.Button(new GUIContent("FP", "Focus Parent")))
					{
						Selection.activeGameObject = targetControl.smartResize.rectParent.gameObject;
						SceneView.FrameLastActiveSceneView();
						Selection.activeGameObject = targetControl.gameObject;
					}
					//if (targetControl.smartResize.rectParent != targetControl.smartResize.rectRealParent)
					//{
					//	if (GUILayout.Button(new GUIContent("FRP", "Focus Real Parent")))
					//	{
					//		Selection.activeGameObject = targetControl.smartResize.rectRealParent.gameObject;
					//		SceneView.FrameLastActiveSceneView();
					//		Selection.activeGameObject = targetControl.gameObject;
					//	}
					//}
				}
				GUILayout.EndHorizontal();

				//if(t.smartMotion && t.smartResize)
    //            {
				//	if (!t.smartResize.pivotMode.ToString().ToLower().Contains("center")) t.smartResize.motionMode = (GuiUtilities.MotionMode)EditorGUILayout.EnumPopup("", t.smartResize.motionMode);
				//}

				if (targetControl.smartMotion != null)
				{
					if (!targetControl.smartMotion.isVisible)
					{
						if (GUILayout.Button(new GUIContent("Show", "Show Panel")))
						{
							targetControl.smartMotion.ShowPanel();
						}
					}
					else if (targetControl.smartMotion.isVisible)
					{
						if (GUILayout.Button(new GUIContent("Hide", "Hide Panel")))
						{
							targetControl.smartMotion.HidePanel();
						}
					}
				}
				
			}, "Smart Control");

			Handles.EndGUI();

			if (Application.isEditor)
			{
				// Ensure continuous Update calls.
				if (!Application.isPlaying)
				{
					EditorApplication.QueuePlayerLoopUpdate();
					SceneView.RepaintAll();
				}
			}

			//Handles.DrawWireArc(t.transform.position, t.transform.up, -t.transform.right,
			//				360, t.shieldArea);
		}


		void InitStyles()
		{
			if (currentStyle == null)
			{
				currentStyle = new GUIStyle(GUI.skin.box);
				currentStyle.alignment = TextAnchor.MiddleCenter;
				currentStyle.normal.background = MakeTex(2, 2, BF_EditorUtils.HexColor("#318396", Color.grey));
			}
		}

		Texture2D MakeTex(int width, int height, Color col)
		{
			Color[] pix = new Color[width * height];
			for (int i = 0; i < pix.Length; ++i)
			{
				pix[i] = col;
			}
			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}

	}

}
