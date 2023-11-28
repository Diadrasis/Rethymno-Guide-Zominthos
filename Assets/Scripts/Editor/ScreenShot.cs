using UnityEngine;
using UnityEditor;
using System;
using System.IO;

/// <summary>
/// 02/07/2018 StaGe
/// </summary>
namespace StaGeGames
{

    public class ScreenShot
    {
        static string saveFolder = Application.persistentDataPath + "/screenshots/";
        static string todayFolder;

        //[MenuItem("Window/StaGe Games/Screenshot/Description/Usefull for publishing to app store.")]
        //[MenuItem("Window/StaGe Games/Screenshot/Description/")]
        [MenuItem("StaGe Games/Screenshot/Description/Take screenshots from game view window.")]

        [MenuItem("StaGe Games/Screenshot/Zoom/x1 %#z")]
        public static void ScreenShotA() { TakeScreenShot(1); }


        [MenuItem("StaGe Games/Screenshot/Zoom/x2")]
        public static void ScreenShotB() { TakeScreenShot(2); }

        // [MenuItem("Screenshot/Zoom/x3")]
        public static void ScreenShotC() { TakeScreenShot(3); }

        [MenuItem("StaGe Games/Screenshot/Zoom/x4")]
        public static void ScreenShotD() { TakeScreenShot(4); }

        //[MenuItem("Screenshot/Zoom/x5")]
        public static void ScreenShotE() { TakeScreenShot(5); }

        //[MenuItem("Screenshot/Zoom/x6")]
        public static void ScreenShotF() { TakeScreenShot(6); }

        static void TakeScreenShot(int val)
        {
            Vector2 size = GetMainGameViewSize();
            size *= val;
            if (!Directory.Exists(saveFolder)) { Directory.CreateDirectory(saveFolder); }
            todayFolder = DateTime.Today.Year.ToString() + "_" + DateTime.Today.DayOfYear.ToString() + "/";
            if (!Directory.Exists(saveFolder + todayFolder)) { Directory.CreateDirectory(saveFolder + todayFolder); }
            string folderSize = saveFolder + todayFolder + "size x" + val.ToString() + "/";
            if (!Directory.Exists(folderSize)) { Directory.CreateDirectory(folderSize); }
            string folderDimensions = folderSize + size.x.ToString("F0") + "x" + size.y.ToString("F0") + "/";
            if (!Directory.Exists(folderDimensions)) { Directory.CreateDirectory(folderDimensions); }
            string url = folderDimensions + TimeUtilities.GetUTCUnixTimestamp().ToString() + ".png";
            ScreenCapture.CaptureScreenshot(url, val);
            Debug.LogWarning("Screenshot with size x" + val);
            Debug.LogWarning("File folder: " + url);
        }

        static Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }


        [MenuItem("StaGe Games/Screenshot/Open Folder %#o")]
        public static void OpenSaveFolder()
        {
            bool openInsidesOfFolder = false;
            string folderPath = saveFolder.Replace(@"/", @"\");
            if (Directory.Exists(folderPath)) { openInsidesOfFolder = true; }
            try { System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + folderPath); }
            catch (System.ComponentModel.Win32Exception e) { e.HelpLink = ""; }
        }

        [MenuItem("StaGe Games/ScriptTemplates/Open Folder")]
        public static void OpenUnityInstallFolder()
        {
            bool openInsidesOfFolder = false;
            string p = EditorApplication.applicationContentsPath+ "/Resources/ScriptTemplates";

            string folderPath = p.Replace(@"/", @"\");
            if (Directory.Exists(folderPath)) { openInsidesOfFolder = true; }
            try { System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + folderPath); }
            catch (System.ComponentModel.Win32Exception e) { e.HelpLink = ""; }
        }


        [MenuItem("StaGe Games/Open Data Folder")]
        public static void OpenUnityDataFolder()
        {
            bool openInsidesOfFolder = false;
            string p = Application.persistentDataPath;

            string folderPath = p.Replace(@"/", @"\");
            if (Directory.Exists(folderPath)) { openInsidesOfFolder = true; }
            try { System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + folderPath); }
            catch (System.ComponentModel.Win32Exception e) { e.HelpLink = ""; }
        }

    }


}