//StaGe Games ©2022
using StaGeGames.BestFit.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace StaGeGames.BestFit.Extens
{

    public static class BestExtensions
	{
        #region RectTransform

#if UNITY_EDITOR
        [MenuItem("CONTEXT/RectTransform/Best Fit [Keep Aspect Ratio]")]
        static void RectTransformSmartFitAspectRatio(MenuCommand command)
        {
            RectTransform rt = (RectTransform)command.context;
            FitUtils.BestFitKeepAspectRatio(rt);
        }

        [MenuItem("CONTEXT/RectTransform/Best Fit [Texture]")]
        static void RectTransformSmartFitTexure(MenuCommand command)
        {
            RectTransform rt = (RectTransform)command.context;
            FitUtils.BestFitTexture(rt, new BestFitTextureOptions { pivotPoint = EnumUtils.PivotPoint.Center, fitInParent = true }, true);
        }
        [MenuItem("CONTEXT/RectTransform/Best Fit [Texture]", true)]
        static bool ValidateRectTransformSmartFitTexure()
        {
            bool valid = Selection.activeTransform != null && Selection.activeTransform.root.GetComponent<Canvas>();
            if (!valid) return false;
            Image img = Selection.activeTransform.GetComponent<Image>();
            if (img)
            {
                if (img.sprite != null) { return img.sprite.texture != null; }
                if (img.material != null) { return img.material.mainTexture != null; }
                return false;
            }
            RawImage rimg = Selection.activeTransform.GetComponent<RawImage>();
            if (rimg)
            {
                if (rimg.texture != null) return true;
                if (rimg.material != null) { return rimg.material.mainTexture != null; }
            }
            return false;
        }

        [MenuItem("CONTEXT/RectTransform/Best Fit Children [Keep Aspect Ratio]")]
        static void RectTransformSmartFitChildrenAspectRatio(MenuCommand command)
        {
            RectTransform rt = (RectTransform)command.context;
            FitUtils.BestFitChildren(rt);
        }
        [MenuItem("CONTEXT/RectTransform/Best Fit Children [Keep Aspect Ratio]", true)]
        static bool ValidateRectTransformSmartFitChildrenAspectRatio()
        {
            // Return false if no transform is selected.
            bool valid = Selection.activeTransform != null && Selection.activeTransform.root.GetComponent<Canvas>();
            if (!valid) return false;
            RectTransform rt = Selection.activeTransform.GetComponent<RectTransform>();
            if (!rt) return false;
            return rt.GetTopChildren().Length > 0 && IsNotAnyComplexComponet(rt);
        }

        static bool IsNotAnyComplexComponet(RectTransform rt)
        {
            return rt.GetComponent<Slider>() == null &&
                rt.GetComponent<Toggle>() == null &&
                rt.GetComponent<Dropdown>() == null &&
                rt.GetComponent<Scrollbar>() == null &&
                rt.GetComponent<TMPro.TMP_Dropdown>() == null &&
                rt.GetComponent<ScrollRect>() == null;
        }
#endif

        public static void BestFitInitDelayed(this RectTransform rect)
        {
            BestResize smartFitter = rect.GetComponent<BestResize>();
            if (smartFitter) smartFitter.InitDelayed();
        }
        public static Vector2 RealSize(this RectTransform _rect)
        {
            if (_rect == null) { /*Debug.LogWarningFormat("[Error] RealSize for Null RectTransform. Returning Vector2.one to avoid DivideByZeroException");*/ return Vector2.one; }
            return _rect.rect.size;
        }
        public static float GetWidth(this RectTransform _rect)
        {
            if (_rect == null) { Debug.LogWarningFormat("[Error] GetWidth for Null RectTransform. Returning [1] to avoid DivideByZeroException"); return 1f; }
            return _rect.RealSize().x;
        }//rect.width; 

        public static float GetHeight(this RectTransform _rect)
        {
            if (_rect == null) { Debug.LogWarningFormat("[Error] GetHeight for Null RectTransform. Returning [1] to avoid DivideByZeroException"); return 1f; }
            return _rect.RealSize().y;
        }//.rect.height; 

        public static void ForceRebuildLayout(this RectTransform rectTransform)
        {
            if (!rectTransform) return;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            //LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        private static List<RectTransform> _rectsDraft = new List<RectTransform>();
        public static RectTransform[] GetTopChildren(this RectTransform _rect)
        {
            _rectsDraft.Clear();
            _rectsDraft = _rect.GetComponentsInChildren<RectTransform>(false).ToList();
            _rectsDraft.Remove(_rect);
            _rectsDraft = _rectsDraft.FindAll(b => b.parent == _rect);
            return _rectsDraft.ToArray();
        }

        public static bool IsCanvasChild(this RectTransform _target)
        {
            Canvas[] c = _target.GetComponentsInParent<Canvas>();
            if (c == null || c.Length <= 0) return false;
            Canvas topMostCanvas = c.Length > 1 ? c[c.Length - 1] : c[0];
            return topMostCanvas != null;
        }

        public static RectTransform GetRootCanvasRectTransform(this RectTransform _target)
        {
            Canvas[] c = _target.GetComponentsInParent<Canvas>();
            if (c.Length <= 0) return null;
            Canvas topMostCanvas = c[c.Length - 1];
            if (topMostCanvas)
            {
                //Debug.Log(topMostCanvas.name);
                return topMostCanvas.GetComponent<RectTransform>();
            }
            return null;
        }

        public static void SetBestPivot(this RectTransform rt, EnumUtils.PivotPoint mode)
        {
            float x = 0.5f, y = 0.5f;

            switch (mode)
            {
                case EnumUtils.PivotPoint.None:
                    return;
                case EnumUtils.PivotPoint.Center:
                    x = y = 0.5f;
                    break;
                case EnumUtils.PivotPoint.BottomCenter:
                    x = 0.5f;
                    y = 0f;
                    break;
                case EnumUtils.PivotPoint.BottomLeft:
                    x = 0.0f;
                    y = 0f;
                    break;
                case EnumUtils.PivotPoint.BottomRight:
                    x = 1.0f;
                    y = 0.0f;
                    break;
                case EnumUtils.PivotPoint.TopCenter:
                    x = 0.5f;
                    y = 1f;
                    break;
                case EnumUtils.PivotPoint.TopLeft:
                    x = 0.0f;
                    y = 1f;
                    break;
                case EnumUtils.PivotPoint.TopRight:
                    x = 1f;
                    y = 1f;
                    break;
                case EnumUtils.PivotPoint.LeftCenter:
                    x = 0f;
                    y = 0.5f;
                    break;
                case EnumUtils.PivotPoint.RightCenter:
                    x = 1f;
                    y = 0.5f;
                    break;
                default:
                    break;
            }

            rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(x, y);
        }

        public static void AnchorsToCorners(this RectTransform rt)
        {
            if (rt == null) return;
            RectTransform pt = rt.parent as RectTransform;

            if (pt == null) return;

            Vector2 newAnchorsMin = new Vector2(rt.anchorMin.x + rt.offsetMin.x / pt.rect.width,
                                                rt.anchorMin.y + rt.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(rt.anchorMax.x + rt.offsetMax.x / pt.rect.width,
                                                rt.anchorMax.y + rt.offsetMax.y / pt.rect.height);

            rt.anchorMin = newAnchorsMin;
            rt.anchorMax = newAnchorsMax;
            rt.offsetMin = rt.offsetMax = new Vector2(0, 0);
        }

        public static void AnchorsToCenter(this RectTransform rt)
        {
            if (rt == null) return;
            Vector2 size = rt.rect.size;
            Vector2 p = rt.position;
            if (float.IsNaN(p.x)) p.x = 0f;
            if (float.IsNaN(p.y)) p.y = 0f;

            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.position = p;
        }

        public static void SetMyPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

#if UNITY_EDITOR

        [MenuItem("CONTEXT/RectTransform/Pivot To Center")]
        public static void PivotToCenterRectTransform(MenuCommand command)
        {
            RectTransform rt = (RectTransform)command.context;

            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Vector2 size = rt.rect.size;
            Vector2 deltaPivot = rt.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rt.pivot = pivot;
            rt.localPosition -= deltaPosition;
        }


        [MenuItem("CONTEXT/RectTransform/Anchors To Corners")]
        static void AnchorsToCornerRectTransform(MenuCommand command)
        {
            RectTransform t = (RectTransform)command.context;
            RectTransform pt = t.parent as RectTransform;

            if (pt == null) return;

            Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                                t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }

        [MenuItem("CONTEXT/RectTransform/Anchors To Center")]
        static void AnchorsToCenterRectTransform(MenuCommand command)
        {
            RectTransform rt = (RectTransform)command.context;

            Vector2 size = rt.rect.size;
            Vector2 p = rt.position;

            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.position = p;

            Debug.LogWarning("AnchorsToCenterRectTransform");
        }

        

        [MenuItem("CONTEXT/RectTransform/Debug Positions")]
        public static void DebugPositionsRectTransform(MenuCommand command)
        {
            RectTransform rt = (RectTransform)command.context;
            Debug.LogWarning("anchoredPosition = " + rt.anchoredPosition);
            Debug.LogWarning("localPosition = " + rt.localPosition);
            Debug.LogWarning("position = " + rt.position);
        }


        static float Round(float value) { return Mathf.Floor(0.5f + value); }


#endif

        #endregion

        #region Vector

        /// <summary>
        /// Compare 2 Vectors. Check if this rect > other rect
        /// </summary>
        /// <returns>true if vectorSource is bigger than vectorOther else false</returns>
        public static bool IsBiggerThan(this Vector2 vectorSource, Vector2 vectorOther)
        {
            return vectorSource.x * vectorSource.y > vectorOther.x * vectorOther.y;
        }

        /// <summary>
        /// Compare 2 Vectors. Check if this rect > other rect
        /// </summary>
        /// <returns>true if vectorSource is bigger than vectorOther else false</returns>
        public static bool IsBiggerThan(this Vector3 vectorSource, Vector3 vectorOther)
        {
            return vectorSource.x * vectorSource.y * vectorSource.z > vectorOther.x * vectorOther.y * vectorOther.z;
        }

        public static bool HasZeroAxis(this Vector2 v) { return v.x == 0 || v.y == 0; }       

        public static bool EqualsTo(this Vector2 vec, Vector2 target) { return vec.ToInt() == target.ToInt(); }
        public static bool EqualsTo(this Vector3 vec, Vector3 target) { return vec.ToInt() == target.ToInt(); }

        public static bool IsZero(this Vector2 vec) { return vec == Vector2.zero; }
        public static bool IsZero(this Vector3 vec) { return vec == Vector3.zero; }

        public static Vector2Int ToInt(this Vector2 vec) { return Vector2Int.FloorToInt(vec); }
        public static Vector3Int ToInt(this Vector3 vec) { return Vector3Int.FloorToInt(vec); }

        #endregion

        #region numbers

        /// <summary>
        /// Mathf.Clamp(val, 0f, 10000f);
        /// </summary>
        /// <param name="val"></param>
        public static float ClampMe(this float val)
        {
            return Mathf.Clamp(val, 0f, 10000f);
        }

        public static float ClampMeCustom(this float val, float _min, float _max)
        {
            return  Mathf.Clamp(val, _min, _max);
        }

        public static int ClampMeCustom(this int val, int _min, int _max)
        {
            return Mathf.Clamp(val, _min, _max);
        }

        public static bool isZero(this float val) { return val == 0f; }

        #endregion

        #region GameObject

        public static T GetOrAddComponent<T>(this GameObject tr) where T : Component
        {
            T component = tr.GetComponent<T>();
            if (component == null) component = tr.AddComponent<T>() as T;
            return component;
        }

        #endregion

        #region Texture

        public static Vector2 Size(this Texture tex)
        {
            if(tex == null)
            {
                Debug.LogWarning("Null texture");
                return Vector2.one;//avoid DivideByZeroException
            }
            Vector2 _size= new Vector2(tex.width, tex.height);
            if (_size.HasZeroAxis())
            {
                Debug.LogWarning("wrong texture dimensions");
                return Vector2.one;//avoid DivideByZeroException
            }
            return _size;
        }

        public static Vector2 TexureSize(this Sprite spr)
        {
            if (spr == null)
            {
                Debug.LogWarning("Null Sprite");
                return Vector2.one;//avoid DivideByZeroException
            }
            if(spr.texture == null)
            {
                Debug.LogWarning("Null Sprite Texture");
                return Vector2.one;//avoid DivideByZeroException
            }
            Vector2 _size = new Vector2(spr.texture.width, spr.texture.height);
            if (_size.HasZeroAxis())
            {
                Debug.LogWarning("wrong Sprite texture dimensions");
                return Vector2.one;//avoid DivideByZeroException
            }
            return _size;
        }

        public static Vector2 TexureSize(this Material mat)
        {
            if (mat == null)
            {
                Debug.LogWarning("Null Material");
                return Vector2.one;//avoid DivideByZeroException
            }
            if (mat.mainTexture == null)
            {
                Debug.LogWarning("Null Material Texture");
                return Vector2.one;//avoid DivideByZeroException
            }
            Vector2 _size = new Vector2(mat.mainTexture.width, mat.mainTexture.height);
            if (_size.HasZeroAxis())
            {
                Debug.LogWarning("wrong Material texture dimensions");
                return Vector2.one;//avoid DivideByZeroException
            }
            return _size;
        }

        #endregion

    }

}
