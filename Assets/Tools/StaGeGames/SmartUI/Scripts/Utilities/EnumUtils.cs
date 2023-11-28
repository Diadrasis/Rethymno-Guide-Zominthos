//StaGe Games ©2022
using StaGeGames.BestFit.Extens;
using UnityEngine;

namespace StaGeGames.BestFit.Utils
{
    public class EnumUtils
    {
        #region Enums

        public enum MotionMode { Horizontally, Vertically, Diagonally, Free }
        public enum StickMode { Internal, External, Free }
        public enum PivotPoint
        {
            None,
            Center,
            BottomCenter,
            BottomLeft,
            BottomRight,
            TopCenter,
            TopLeft,
            TopRight,
            LeftCenter,
            RightCenter
        };

        /// <summary>
        /// ResizeMode
        /// <para>[None]: (x,y) free size, user custom size</para>
        /// <para>[Parent]: (x,y) preserve size of parent with percentage scale</para>
        /// <para>[Child]: (x,y) preserve size child(s) with percentage scale</para>
        /// <para>[Texture]: (x,y) preserve size of texture with percentage scale</para>
        /// <para>[Fake Ratio]: (x,y) preserve custom aspect ratio with percentage scale</para>
        /// <para>[Static]: set static size delta</para>
        /// </summary>
        public enum ResizeMode
        {
            /// <summary>
            /// free size, user can set custom size (at runtime too)
            /// </summary>
            None,
            /// <summary>
            /// (x,y) preserve size of parent with percentage scale
            /// </summary>
            AdaptParentSize,
            /// <summary>
            /// (x,y) preserve size of texture with percentage scale
            /// </summary>
            AdaptTextureSize,
            /// <summary>
            /// (x,y) preserve custom aspect ratio with percentage scale
            /// </summary>
            AdaptMockRatio,
            /// <summary>
            /// set static size delta
            /// </summary>
            Static,
            /// <summary>
            /// (x,y) preserve size of child(s) with percentage scale
            /// </summary>
            AdaptChildSize
        }

        public enum MockRatioMode
        {
            SelectPreset,
            CustomRatio,
            PrefferedDimensions
        }

        public enum InitialPositionMode
        {
            Static,
            SelfSizePercentage,
            ParentSizePercentage
        }


        ///works also as an extension
        ///add >> using StaGeGames.SmartUI.Extens
        ///example >> rt.SetPivot(pivot);
        public static void SetPivot(RectTransform rt, PivotPoint mode)
        {
            float x = 0.5f, y = 0.5f;

            switch (mode)
            {
                case PivotPoint.None:
                    return;
                case PivotPoint.Center:
                    x = y = 0.5f;
                    break;
                case PivotPoint.BottomCenter:
                    x = 0.5f;
                    y = 0f;
                    break;
                case PivotPoint.BottomLeft:
                    x = 0.0f;
                    y = 0f;
                    break;
                case PivotPoint.BottomRight:
                    x = 1.0f;
                    y = 0.0f;
                    break;
                case PivotPoint.TopCenter:
                    x = 0.5f;
                    y = 1f;
                    break;
                case PivotPoint.TopLeft:
                    x = 0.0f;
                    y = 1f;
                    break;
                case PivotPoint.TopRight:
                    x = 1f;
                    y = 1f;
                    break;
                case PivotPoint.LeftCenter:
                    x = 0f;
                    y = 0.5f;
                    break;
                case PivotPoint.RightCenter:
                    x = 1f;
                    y = 0.5f;
                    break;
                default:
                    break;
            }
            
            rt.pivot =  rt.anchorMin =  rt.anchorMax = new Vector2(x, y);
        }

        public static Vector2 GetExternalStickPivot(RectTransform target, PivotPoint mode, MotionMode modePosition)
        {
            float x = 0.0f, y = 0.0f;
            float tX = target.GetWidth();
            float tY = target.GetHeight();

            //float log = 0.0f;
            //log = targetRealParent.sizeDelta.x / target.sizeDelta.x;
            //bool isBiggerThanParent = log < 1f;

            switch (mode)
            {
                case PivotPoint.None:
                    break;
                case PivotPoint.Center:
                    break;
                case PivotPoint.BottomCenter:
                    y = -tY;
                    break;
                case PivotPoint.BottomLeft:
                    switch (modePosition)
                    {
                        case MotionMode.Horizontally:
                            x = -tX;
                            break;
                        case MotionMode.Vertically:
                            y = -tY;
                            break;
                        case MotionMode.Diagonally:
                            x = -tX;
                            y = -tY;
                            break;
                        default:
                            break;
                    }
                    break;
                case PivotPoint.BottomRight:
                    switch (modePosition)
                    {
                        case MotionMode.Horizontally:
                            x = tX;
                            break;
                        case MotionMode.Vertically:
                            y = -tY;
                            break;
                        case MotionMode.Diagonally:
                            x = tX;
                            y = -tY;
                            break;
                        default:
                            break;
                    }
                    break;
                case PivotPoint.TopCenter:
                    x = 0.0f;
                    y = tY;
                    break;
                case PivotPoint.TopLeft:
                    switch (modePosition)
                    {
                        case MotionMode.Horizontally:
                            x = -tX;
                            break;
                        case MotionMode.Vertically:
                            y = tY;
                            break;
                        case MotionMode.Diagonally:
                            x = -tX;
                            y = tY;
                            break;
                        default:
                            break;
                    }
                    break;
                case PivotPoint.TopRight:
                    switch (modePosition)
                    {
                        case MotionMode.Horizontally:
                            x = tX;
                            break;
                        case MotionMode.Vertically:
                            y = tY;
                                break;
                        case MotionMode.Diagonally:
                            x = tX;
                            y = tY;
                            break;
                        default:
                            break;
                    }
                    break;
                case PivotPoint.LeftCenter:
                    x = -tX;
                    y = 0.0f;
                    break;
                case PivotPoint.RightCenter:
                    x = tX;
                    y = 0.0f;
                    break;
                default:
                    break;
            }

            return new Vector2(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pivotMode"></param>
        /// <param name="target"></param>
        /// <param name="targetParent"></param>
        /// <returns>Initial position relative to parent size delta</returns>
        public static Vector2 GetInitialParentPercentagePos(PivotPoint pivotMode, RectTransform target, RectTransform targetParent)
        {
            Vector2 _parentSize = CalcUtils.GetPositiveRealVector2(targetParent.RealSize());
            Vector2 _targetSize = CalcUtils.GetPositiveRealVector2(target.RealSize());
            Vector2 _targetPos = target.anchoredPosition;

            float allWidth = _parentSize.x - _targetSize.x;
            float allHeight = _parentSize.y - _targetSize.y;

            switch (pivotMode)
            {
                case PivotPoint.None:
                    break;
                case PivotPoint.Center:
                default:
                    _targetPos = (_parentSize - _targetSize) / 2f;
                    break;
                case PivotPoint.BottomLeft:
                case PivotPoint.BottomRight:
                case PivotPoint.TopLeft:
                case PivotPoint.TopRight:
                    _targetPos.x = allWidth;
                    _targetPos.y = allHeight;
                    break;
                case PivotPoint.BottomCenter:
                case PivotPoint.TopCenter:
                    _targetPos.x = allWidth / 2f;
                    _targetPos.y = allHeight;
                    break;
                case PivotPoint.LeftCenter:
                case PivotPoint.RightCenter:
                    _targetPos.x = allWidth;
                    _targetPos.y = allHeight / 2f;
                    break;
                
            }

            return CalcUtils.GetNoNanVector2Position(_targetPos);
        }

        /// <summary>
        /// x min max = Vector4 x,y
        /// <para>y min max = Vector4 z,w</para> 
        /// </summary>
        /// <returns>Vector4 min max values based on pivot point</returns>
        public static Vector4 GetStartPositionMinMaxValues(PivotPoint pivotMode)
        {
            Vector4 mmvals = new Vector4(-100f, 100f, -100f, 100f);

            #region Iterate Modes

            switch (pivotMode)
            {
                case PivotPoint.None:
                case PivotPoint.Center:
                default:
                    mmvals = new Vector4(-100f, 100f, -100f, 100f);
                    break;
                case PivotPoint.BottomCenter:
                    mmvals = new Vector4(-100f, 100f, 0, 100f);
                    break;
                case PivotPoint.BottomLeft:
                    mmvals = new Vector4(0f, 100f, 0f, 100f);
                    break;
                case PivotPoint.BottomRight:
                    mmvals = new Vector4(-100f, 0f, 0f, 100f);
                    break;
                case PivotPoint.TopCenter:
                    mmvals = new Vector4(-100f, 100f, -100f, 0f);
                    break;
                case PivotPoint.TopLeft:
                    mmvals = new Vector4(0f, 100f, -100f, 0f);
                    break;
                case PivotPoint.TopRight:
                    mmvals = new Vector4(-100f, 0f, -100f, 0f);
                    break;
                case PivotPoint.LeftCenter:
                    mmvals = new Vector4(0f, 100f, -100f, 100f);
                    break;
                case PivotPoint.RightCenter:
                    mmvals = new Vector4(-100f, 0f, -100f, 100f);
                    break;
            }

            #endregion

            return mmvals;
        }

        /// <summary>
        /// is pivot at any corner
        /// </summary>
        /// <param name="pivotMode"></param>
        /// <returns></returns>
        public static bool IsDiagonallyPivoted(PivotPoint pivotMode)
        {
            return !pivotMode.ToString().ToLower().Contains("center") && !pivotMode.ToString().ToLower().Contains("none");
        }
        public static bool IsPivotCenterOrNone(PivotPoint pivotMode)
        {
            return pivotMode == PivotPoint.Center || pivotMode == PivotPoint.None;
        }
        public static bool IsPivotCenterOrNone(int pivotMode)
        {
            return pivotMode == (int)PivotPoint.Center || pivotMode == (int)PivotPoint.None;
        }
        public static bool IsPivotCenter(PivotPoint pivotMode)
        {
            return pivotMode == PivotPoint.Center;
        }
        public static bool IsPivotCenter(int pivotMode)
        {
            return pivotMode == (int)PivotPoint.Center;
        }
        public static bool IsPivotNone(PivotPoint pivotMode)
        {
            return pivotMode == PivotPoint.None;
        }
        public static bool IsPivotVerticalCenter(PivotPoint pivotMode)
        {
            return pivotMode == PivotPoint.BottomCenter || pivotMode == PivotPoint.TopCenter;
        }
        public static bool IsPivotVerticalCenter(int pivotModeIndex)
        {
            return pivotModeIndex == (int)PivotPoint.BottomCenter || pivotModeIndex == (int)PivotPoint.TopCenter;
        }
        public static bool IsPivotHorizontalCenter(PivotPoint pivotMode)
        {
            return pivotMode == PivotPoint.LeftCenter || pivotMode == PivotPoint.RightCenter;
        }
        public static bool IsPivotHorizontalCenter(int pivotModeIndex)
        {
            return pivotModeIndex == (int)PivotPoint.LeftCenter || pivotModeIndex == (int)PivotPoint.RightCenter;
        }

        #endregion
        
        public static bool IsObjNull(System.Object t) { return System.Object.ReferenceEquals(t, null); } //t is null;

    }
}
