//StaGe Games ©2022
using System.IO;
using UnityEditor;
using UnityEngine;

namespace StaGeGames.BestFit.EditorSpace
{

    public static class BF_EditorUtils
    {
        public static GUILayoutOption[] GLO_Numb(float lblwidth) { return new GUILayoutOption[] { GUILayout.MaxWidth(lblwidth + 50f) }; }
        public static GUILayoutOption[] GLO_Button = new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(30) };
        public static bool IsButtonReset() { return GUILayout.Button("Reset", GUILayout.Width(50f)); }
        public static GUIStyle StyleBox() { return new GUIStyle("HelpBox");}
        public static GUIStyle StyleSelection() { return new GUIStyle("selectionRect"); }
        public static GUIStyle StyleToolbarButton() { return new GUIStyle("toolbarbutton"); }
        public static GUIStyle StyleProgressBarBack() { return new GUIStyle("ProgressBarBack"); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>1=StyleSelection, 2=StyleProgressBarBack, 3=StyleBox</returns>
        public static GUIStyle StyleOptions(int val)
        {
            switch (val)
            {
                case 1:
                    return StyleSelection();
                case 2:
                    return StyleProgressBarBack();
                case 3:
                    return StyleBox();
                default:
                    return StyleBox();
            }
        }

        public static GUIStyle StyleWarningButton(float fixedWidth = 0f)
        {
            string normalColor = EditorGUIUtility.isProSkin ? "#00FFFF" : "#000000";
            string hoverColor = EditorGUIUtility.isProSkin ? "#FFFFFF" : "#000000";
            FontStyle fontStyle = EditorGUIUtility.isProSkin ? FontStyle.Normal : FontStyle.Bold;

            return GUIStyleTextCustom(12, fontStyle, normalColor, hoverColor, TextAnchor.MiddleCenter, fixedWidth);
        }

        public static GUIStyle StyleErrorButton(float fixedWidth = 0f)
        {
            string normalColor = EditorGUIUtility.isProSkin ? "#FFFFFF" : "#FFFFFF";
            string hoverColor = EditorGUIUtility.isProSkin ? "#FFFFFF" : "#FFFFFF";
            FontStyle fontStyle = EditorGUIUtility.isProSkin ? FontStyle.Normal : FontStyle.Bold;

            return GUIStyleTextCustom(12, fontStyle, normalColor, hoverColor, TextAnchor.MiddleCenter, fixedWidth);
        }

        public static GUIStyle GUIStyleRefresh(float fixedWidth = 0f)
        {
            string normalColor = EditorGUIUtility.isProSkin ? "" : "#575757";
            string hoverColor = EditorGUIUtility.isProSkin ? "" : "#575757";
            return GUIStyleTextCustom(12, FontStyle.Normal, normalColor, hoverColor, TextAnchor.MiddleCenter, fixedWidth);
        }

        public static GUIStyle StyleRecordButton(bool isRecOn)
        {
            GUIStyle style = new GUIStyle(EditorStyles.textField);
            style.richText = true;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 12;
            style.fontStyle = FontStyle.Normal;
            //the color if the button is pressed
            style.normal.textColor = isRecOn ? Color.gray : Color.white;
            style.hover.textColor = isRecOn ? Color.gray : Color.white;
            style.fixedWidth = 150;
            return style;
        }

        public static GUIStyle GUIStyleLogo()
        {
            GUIStyle style = new GUIStyle(EditorStyles.textField);
            style.richText = true;
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            return style;
        }

        public static GUIStyle GUIStyleTextCustom(int fontSize, FontStyle fontStyle, string normalTextColor, string hoverTextColor, TextAnchor textAnchor, float fixedWidth = 0f)
        {
            GUIStyle style = new GUIStyle(EditorStyles.textField);
            style.fontSize = 12;
            style.fontStyle = FontStyle.Normal;
            style.normal.textColor = HexColor(normalTextColor, Color.black);
            style.hover.textColor = HexColor(hoverTextColor, Color.black);
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = fixedWidth;
            return style;
        }

        public static Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }

        public static Color ColorButtonsImportant(Color defColor)
        {
            string hexColor = EditorGUIUtility.isProSkin ? BF_EditorParams.colButtonImportantPro : BF_EditorParams.colButtonImportant;
            return HexColor(hexColor, defColor);
        }
        public static Color ColorButtonsError(Color defColor)
        {
            string hexColor = EditorGUIUtility.isProSkin ? BF_EditorParams.colButtonImportantPro : "#8C000D";
            return HexColor("#8C000D", defColor);
        }

        public static void LockScript(bool val)
        {
            ActiveEditorTracker.sharedTracker.isLocked = val;
        }

        public static bool IsScriptLocked() { return ActiveEditorTracker.sharedTracker.isLocked; }

        public static bool CheckType(System.Type t)
        {
            return t.GetType().Name == "BestResize";
        }

        /// <summary>
        /// Returns the relative path of the package.
        /// </summary>
        public static string packageRelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PackagePath))
                    m_PackagePath = GetPackageRelativePath();

                return m_PackagePath;
            }
        }
        [SerializeField]
        private static string m_PackagePath;

        public static string GetPackageRelativePath()
        {
            string packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                // Search for default location of normal Smart UI AssetStore package
                if (Directory.Exists(packagePath + "/Assets/SmartUI/Editor Resources"))
                {
                    return "Assets/SmartUI";
                }

                // Search for potential alternative locations in the user project
                string[] matchingPaths = Directory.GetDirectories(packagePath, "SmartUI", SearchOption.AllDirectories);
                packagePath = ValidateLocation(matchingPaths, packagePath);
                if (packagePath != null) return packagePath;
            }

            return null;
        }

        private static string folderPath = "Not Found";

        /// <summary>
        /// Method to validate the location of the asset folder by making sure the GUISkins folder exists.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private static string ValidateLocation(string[] paths, string projectPath)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                // Check if any of the matching directories contain a GUISkins directory.
                if (Directory.Exists(paths[i] + "/Editor Resources"))
                {
                    folderPath = paths[i].Replace(projectPath, "");
                    folderPath = folderPath.TrimStart('\\', '/');
                    return folderPath;
                }
            }

            return null;
        }

        #region COLOR 

        /// <summary>
        /// return color from hex string or html
        /// eg. hexValue = #696969
        /// eg. hexValue = yellow
        /// if error then keeps the default color hexValue = def
        /// </summary>
        /// <param name="hexValue"></param>
        /// <param name="def"></param>
        /// red, cyan, blue, darkblue, lightblue, purple, yellow, 
        /// lime, fuchsia, white, silver, grey, black, orange, brown, 
        /// maroon, green, olive, navy, teal, aqua, magenta
        /// <returns></returns>
        public static Color HexColor(string hexValue, Color def)
        {
            if (ColorUtility.TryParseHtmlString(hexValue, out Color newCol)) return newCol;
            return def;
        }

        #endregion

    }

}
