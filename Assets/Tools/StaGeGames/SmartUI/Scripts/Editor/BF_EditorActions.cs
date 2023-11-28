//StaGe Games ©2021
using StaGeGames.BestFit.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StaGeGames.BestFit.EditorSpace
{
    public class BF_EditorActions : MonoBehaviour
	{
        [MenuItem("GameObject/SmartUI/Canvas/Create Smart Portrait", false, 0)]
        [MenuItem("StaGe Games/UI/Canvas/Create Smart Portrait")]
        public static void CreateSmartCanvasPortrait()
        {
            List<Canvas> canvases = FindObjectsOfType<Canvas>().ToList();

            int sortOrder = 0;

            if (canvases.Count > 0)
            {
                canvases.Sort((p1, p2) => p1.sortingOrder.CompareTo(p2.sortingOrder));

                List<int> ods = new List<int>();
                canvases.ForEach((b)=>ods.Add(b.sortingOrder));
                sortOrder = Mathf.Max(ods.ToArray());
                sortOrder++;
            }

            GameObject gb = new GameObject("[Smart] Canvas Portrait");
            Canvas canvas = gb.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            CanvasScaler canvasScaler = gb.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
            canvasScaler.matchWidthOrHeight = 0.5f;
            GraphicRaycaster graphicRaycaster = gb.AddComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            CreateSmartLogo(canvas);

            AddSmartResizeManager();

            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (!eventSystem)
            {
                GameObject ev = new GameObject("[EventSystem]");
                ev.AddComponent<EventSystem>();
            }

            Selection.activeGameObject = gb;
            SceneView.FrameLastActiveSceneView();
        }

        [MenuItem("GameObject/SmartUI/Canvas/Create Smart Landscape", false, 0)]
        [MenuItem("StaGe Games/UI/Canvas/Create Smart Landscape")]
        public static void CreateSmartCanvasLandscape()
        {
            List<Canvas> canvases = FindObjectsOfType<Canvas>().ToList();

            int sortOrder = 0;

            if (canvases.Count > 0)
            {
                canvases.Sort((p1, p2) => p1.sortingOrder.CompareTo(p2.sortingOrder));

                List<int> ods = new List<int>();
                canvases.ForEach((b) => ods.Add(b.sortingOrder));
                sortOrder = Mathf.Max(ods.ToArray());
                sortOrder++;
            }

            GameObject gb = new GameObject("[Smart] Canvas Landscape");
            Canvas canvas = gb.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            CanvasScaler canvasScaler = gb.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            GraphicRaycaster graphicRaycaster = gb.AddComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            CreateSmartLogo(canvas);

            AddSmartResizeManager();

            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (!eventSystem)
            {
                GameObject ev = new GameObject("[EventSystem]");
                ev.AddComponent<EventSystem>();
                ev.AddComponent<StandaloneInputModule>();
            }

            Selection.activeGameObject = gb;
            SceneView.FrameLastActiveSceneView();
        }

        //// Adding a new context menu item
        //[MenuItem("GameObject/Create Empty Parent", true)]
        //static bool ValidateLogSelectedTransformName()
        //{
        //    // disable menu item if no transform is selected.
        //    return Selection.activeTransform != null;
        //}

        

        // Put menu item at top near other "Create" options
        [MenuItem("GameObject/SmartUI/Create Smart Image", false, 0)] //10
        private static void CreateSmartImage(MenuCommand menuCommand)
        {
            //GameObject selected = menuCommand.context as GameObject;
            GameObject selected = Selection.activeObject as GameObject;

            if(selected is null)
            {
                Debug.LogWarning("Select gameObject first!");
                return;
            }

            // Create a empty game object with same name
            GameObject item = new GameObject("Smart_Image");

            // adjust hierarchy accordingly
            GameObjectUtility.SetParentAndAlign(item, selected.transform.gameObject);
            item.transform.localScale = Vector3.one;
            item.AddComponent<Image>();
            BestFitter smartImage = item.AddComponent<BestFitter>();
            smartImage.pivotMode = EnumUtils.PivotPoint.Center;
            smartImage.resizeMode = EnumUtils.ResizeMode.Static;
            smartImage.staticWidth = 100f;
            smartImage.staticHeight = 100f;
            smartImage.Init();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(item, "Created " + item.name);
            Selection.activeObject = item;
            Debug.Log("Created a Smart Image in " + selected.name + ".");
        }

        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        [MenuItem("GameObject/SmartUI/Create Smart Image", true)]
        static bool ValidateCreateSmartImage()
        {
            // Return false if no transform is selected.
            return Selection.activeTransform != null && Selection.activeTransform.root.GetComponent<Canvas>();
        }

        public static void CreateSmartLogo(Canvas canv = null)
        {
            if (!canv) canv = FindObjectOfType<Canvas>();
            if (!canv) return;
            GameObject gb = canv.gameObject;

            GameObject smartLogo = new GameObject("Smart Logo");
            smartLogo.transform.SetParent(gb.transform, true);
            smartLogo.transform.localScale = Vector3.one;
            Image img = smartLogo.AddComponent<Image>();
            img.color = BF_EditorUtils.HexColor("#318396", Color.black);
            BestFitter smartResize = smartLogo.AddComponent<BestFitter>();
            smartResize.pivotMode = EnumUtils.PivotPoint.TopRight;
            smartResize.resizeMode = EnumUtils.ResizeMode.Static;
            smartResize.staticWidth = 200f;
            smartResize.staticHeight = 100f;
            smartResize.Init();

            Text txt = new GameObject("Text").AddComponent<Text>();
            txt.transform.SetParent(smartLogo.transform);
            txt.rectTransform.anchorMin = Vector2.zero;
            txt.rectTransform.anchorMax = Vector2.one;
            txt.rectTransform.pivot = Vector2.one/2f;
            txt.rectTransform.offsetMin = Vector2.zero;
            txt.rectTransform.offsetMax = Vector2.zero;
            txt.color = Color.white;
            txt.fontStyle = FontStyle.BoldAndItalic;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.resizeTextMaxSize = 40;
            txt.resizeTextForBestFit = true;
            txt.text = "Smart UI";

            smartLogo.AddComponent<EditorActiveOnly>();
        }

        [MenuItem("StaGe Games/UI/Canvas/Create Smart Info Panel")]
        public static void CreateSmartInfoPanel()
        {
            CreateSmartInfoPanel(null);
        }
        public static void CreateSmartInfoPanel(Canvas canv = null)
        {
            if (!canv) canv = FindObjectOfType<Canvas>();
            if (!canv) return;
            GameObject gb = canv.gameObject;

            GameObject infoPanel = new GameObject("Info Panel");
            infoPanel.transform.SetParent(gb.transform);
            infoPanel.transform.localScale = Vector3.one;
            Image img = infoPanel.AddComponent<Image>();
            img.color = BF_EditorUtils.HexColor("#ABABAB", Color.black);
            BestResize smartResize = infoPanel.AddComponent<BestResize>();
            smartResize.pivotMode = EnumUtils.PivotPoint.Center;
            smartResize.resizeMode = EnumUtils.ResizeMode.AdaptParentSize;
            smartResize.fParentSizePercentageScale = 90f;
            smartResize.Init();

            Text txt = new GameObject("Text").AddComponent<Text>();
            txt.transform.SetParent(infoPanel.transform);
            txt.rectTransform.anchorMin = Vector2.zero;
            txt.rectTransform.anchorMax = Vector2.one;
            txt.rectTransform.pivot = Vector2.one / 2f;
            txt.rectTransform.offsetMin = Vector2.zero;
            txt.rectTransform.offsetMax = Vector2.zero;
            txt.color = Color.white;
            txt.fontStyle = FontStyle.BoldAndItalic;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.resizeTextMaxSize = 60;
            txt.resizeTextForBestFit = true;
            txt.text = "Info Panel";

            txt.gameObject.AddComponent<EditorActiveOnly>();
        }


        [MenuItem("StaGe Games/UI/Navigation/None")]
        public static void SetNavigationUI()
        {
            Selectable[] buttons = FindObjectsOfType<Selectable>();
            
            foreach (Selectable b in buttons)
            {
                Navigation nav = b.navigation;
                nav.mode = Navigation.Mode.None;
                b.navigation = nav;
                //Debug.Log(b.name);
            }

            Debug.LogWarning("Action completed for "+ buttons.Length + " ui elements");
        }

        [MenuItem("StaGe Games/UI/Navigation/Auto")]
        public static void SetNavigationUIauto()
        {
            Selectable[] buttons = FindObjectsOfType<Selectable>();

            foreach (Selectable b in buttons)
            {
                Navigation nav = b.navigation;
                nav.mode = Navigation.Mode.Automatic;
                b.navigation = nav;
                //Debug.Log(b.name);
            }

            Debug.LogWarningFormat("Action completed for {0} ui elements", buttons.Length);
        }

        [MenuItem("StaGe Games/UI/Apply Smart Resize to All Canvases")]
        public static void ApplySmartResizeAllCanvases()
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canv in canvases)
            {
                BestFit.BestResize[] smarts = canv.GetComponentsInChildren<BestFit.BestResize>(true);
                foreach (BestFit.BestResize sm in smarts) sm.Init();
                Debug.LogWarningFormat("Action completed for {0} ui smart elements", smarts.Length);
            }
        }

        public static void ApplySmartResizeAllCanvases(bool allowInactive)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canv in canvases)
            {
                BestFit.BestResize[] smarts = canv.GetComponentsInChildren<BestFit.BestResize>(allowInactive);
                foreach (BestFit.BestResize sm in smarts) sm.Init();
                //Debug.LogWarningFormat("Action completed for {0} ui smart elements", smarts.Length);
            }
        }

        public static void ApplySmartResizeCanvas(Canvas canv, bool allowInactive)
        {
            if (!canv) return;
            BestFit.BestResize[] smarts = canv.GetComponentsInChildren<BestFit.BestResize>(allowInactive);
            foreach (BestFit.BestResize sm in smarts) sm.Init();
            //Debug.LogWarningFormat("Action completed for {0} ui smart elements", smarts.Length);
        }

        public static void ApplyParentSmartResize(RectTransform target, bool allowInactive)
        {
            if (!target) return;
            BestFit.BestResize[] smarts = target.GetComponentsInChildren<BestFit.BestResize>(allowInactive);
            foreach (BestFit.BestResize sm in smarts) sm.Init();
            //Debug.LogWarningFormat("Action completed for {0} ui smart elements", smarts.Length);
        }

        public static void ApplyChildSmartResize(RectTransform target, bool allowInactive)
        {
            if (!target) return;
            BestFit.BestResize[] smarts = target.GetComponentsInChildren<BestFit.BestResize>(allowInactive);
            foreach (BestFit.BestResize sm in smarts) if (sm != target) sm.Init();
            //Debug.LogWarningFormat("Action completed for {0} ui smart elements", smarts.Length);
        }

        public static void ApplySmartResizeTo(GameObject[] objs)
        {
            foreach(GameObject gb in objs)
            {
                if (!gb) continue;
                BestResize sm = gb.GetComponent<BestResize>();
                if (sm) sm.Init();
            }
           // Debug.LogWarningFormat("Action completed for {0} ui smart elements", objs.Length);
        }

        public static bool GetChildSmartResizes(RectTransform target, bool allowInactive)
        {
            if (!target) return false;
            BestFit.BestResize[] smarts = target.GetComponentsInChildren<BestFit.BestResize>(allowInactive);
            return smarts.Length > 1;
        }

        [MenuItem("StaGe Games/UI/Apply Smart Resize to active only")]
        public static void ApplySmartResize()
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();

            foreach (Canvas canv in canvases)
            {

                BestFit.BestResize[] smarts = canv.GetComponentsInChildren<BestFit.BestResize>(true);

                foreach (BestFit.BestResize b in smarts)
                {
                    b.Init();
                }

                //Debug.LogWarningFormat("Action completed for {0} ui smart elements", smarts.Length);
            }
        }

        [MenuItem("StaGe Games/UI/Scene/Add Smart Resize Manager")]
        public static void AddSmartResizeManager()
        {
            SmartManager smartManager = FindObjectOfType<SmartManager>();

            if (smartManager != null)
            {
                Selection.activeGameObject = smartManager.gameObject;
                SceneView.FrameLastActiveSceneView();
                return;
            }

            GameObject sm = new GameObject("[SmartManager]");
            smartManager = sm.AddComponent<SmartManager>();
            smartManager.Init();
            Selection.activeGameObject = smartManager.gameObject;
            SceneView.FrameLastActiveSceneView();
        }

        [MenuItem("StaGe Games/UI/Scene/Add Screen Orientation Checker")]
        public static void AddScreenOrientationChecker()
        {
            SmartScreenOrientationChecker screenChecker = FindObjectOfType<SmartScreenOrientationChecker>();

            if (screenChecker != null)
            {
                Selection.activeGameObject = screenChecker.gameObject;
                SceneView.FrameLastActiveSceneView();
                return;
            }
            
            GameObject sm = new GameObject("[SmartScreen]");
            screenChecker = sm.AddComponent<SmartScreenOrientationChecker>();
            //screenChecker.Init();
            Selection.activeGameObject = screenChecker.gameObject;
            SceneView.FrameLastActiveSceneView();
        }

        [MenuItem("GameObject/UI/Smart UI/Add Smart Resize Manager", false, 0)]
        public static void CreateSmartResizeManager()
        {
            SmartManager smartManager = FindObjectOfType<SmartManager>();

            if (smartManager != null)
            {
                Selection.activeGameObject = smartManager.gameObject;
                SceneView.FrameLastActiveSceneView();
                return;
            }

            GameObject sm = new GameObject("[SmartManager]");
            smartManager = sm.AddComponent<SmartManager>();
            smartManager.Init();
            Selection.activeGameObject = smartManager.gameObject;
            SceneView.FrameLastActiveSceneView();
        }

        #region TODO
        /*
        public static void AddEditorOnlyClass(GameObject gb)
        {
            string copyPath = "Assets/" + "EditorActiveOnly" + ".cs";
            Debug.Log("Creating Classfile: " + copyPath);
            if (File.Exists(copyPath) == false)
            { // do not overwrite
                using (StreamWriter outfile =
                    new StreamWriter(copyPath))
                {
                    outfile.WriteLine("using UnityEngine;");
                    outfile.WriteLine("using System.Collections;");
                    outfile.WriteLine("namespace StaGeGames.EditorTools{");
                    outfile.WriteLine("public class " + "EditorActiveOnly" + " : MonoBehaviour {");
                    outfile.WriteLine(" ");
                    outfile.WriteLine(" ");
                    outfile.WriteLine(" // Use this for initialization");
                    outfile.WriteLine(" void Awake () {");
                    outfile.WriteLine(" gameObject.SetActive(false);");
                    outfile.WriteLine(" }");
                    outfile.WriteLine(" ");
                    outfile.WriteLine(" ");
                    //outfile.WriteLine(" // Update is called once per frame");
                    //outfile.WriteLine(" void Update () {");
                    //outfile.WriteLine(" ");
                    //outfile.WriteLine(" }");
                    outfile.WriteLine("}");
                    outfile.WriteLine("}");
                }//File written
            }
            AssetDatabase.Refresh();
            gb.AddComponent(Type.GetType("EditorActiveOnly"));
        }
        */
        #endregion

    }
}
