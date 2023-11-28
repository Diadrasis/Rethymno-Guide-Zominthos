//StaGe Games ©2022
using StaGeGames.BestFit.Extens;
using UnityEngine;

namespace StaGeGames.BestFit.Utils
{

    #region options params
    public class BestFitTextureOptions
    {
        public EnumUtils.PivotPoint pivotPoint;
        public bool envelopeParent;
        public bool fitInParent;
        public bool fitInParentIgnoreWidth;
        public bool fitInParentShouldReduce;
        public float fitInParentReducePercentageValue;
        public float textureSizePercentageScaleValue = 100f;
        public bool lockWidth;
        public bool lockHeight;
    }

    public class BestFitParentOptions
    {
        public EnumUtils.PivotPoint pivotPoint;
        public float percentageScale = 100f;
        /// <summary>
        /// set this to false if you want to individually scale width and/or height
        /// </summary>
        public bool scaleBothAxes = true;
        public float percentageScaleWidth = 100f;
        /// <summary>
        /// set this to true if you want static width
        /// </summary>
        public bool isStaticWidth;
        public float staticWidth = 100f;
        public float percentageScaleHeight = 100f;
        /// <summary>
        /// set this to true if you want static height
        /// </summary>
        public bool isStaticHeight;
        public float staticHeight = 100f;
        /// <summary>
        /// set this to true if you want to decrease width
        /// </summary>
        public bool reduceFinalWidth;
        /// <summary>
        /// set this to true if you want to decrease height
        /// </summary>
        public bool reduceFinalHeight;
        /// <summary>
        /// set this to true if you want to decrease width with static values
        /// </summary>
        public bool isReduceWidthStatic;
        /// <summary>
        /// set this to true if you want to decrease height with static values
        /// </summary>
        public bool isReduceHeightStatic;
        public float reducedWidthStaticValue;
        public float reducedHeightStaticValue;
        public float reducedWidthPercentageValue;
        public float reducedHeightPercentageValue;
    }

    #endregion

    public class FitUtils
    {

        public static void BestFitParentSize(RectTransform rect, BestFitParentOptions options, bool resetInitialPosition = false)
        {
            if (!rect) return;
            //in case there is already attached
            BestFitter fitter = rect.gameObject.GetComponent<BestFitter>();
            //else add new fitter
            if (fitter == null) fitter = rect.gameObject.AddComponent<BestFitter>();
            //abort for any other reason that we can not add smart fitter
            if (!fitter) return;
            SetParentOptions(fitter, options);
            if (resetInitialPosition) fitter.ResetInitialPosition();
            fitter.Init();
        }

        public static void SetParentOptions(BestFitter fitter, BestFitParentOptions options)
        {
            if (!fitter) return;
            fitter.pivotMode = options.pivotPoint;
            fitter.resizeMode = EnumUtils.ResizeMode.AdaptParentSize;
            fitter.fParentSizePercentageScale = options.percentageScale;
            fitter.bParentSizeScaleBothAxes = options.scaleBothAxes;
            fitter.fParentSizePercentageScaleWidth = options.percentageScaleWidth;
            fitter.fParentSizePercentageScaleHeight = options.percentageScaleHeight;
            //reduce width
            fitter.bReduceFinalWidth = options.reduceFinalWidth;
            fitter.fReducedWidthPercentageValue = options.reducedWidthPercentageValue;
            fitter.bReduceWidthStatic = options.isReduceWidthStatic;
            fitter.fReducedWidthStaticValue = options.reducedWidthStaticValue;
            //reduce height
            fitter.bReduceFinalHeight = options.reduceFinalHeight;
            fitter.bReduceHeightStatic = options.isReduceHeightStatic;
            fitter.fReducedHeightStaticValue = options.reducedHeightStaticValue;
            fitter.fReducedHeightPercentageValue = options.reducedHeightPercentageValue;
        }

        public static void BestFitTexture(RectTransform rect, BestFitTextureOptions options, bool resetInitialPosition = false)
        {
            if (!rect) return;
            //in case there is already attached
            BestFitter fitter = rect.gameObject.GetComponent<BestFitter>();
            //else add new fitter
            if (fitter == null) fitter = rect.gameObject.AddComponent<BestFitter>();
            //abort for any other reason that we can not add fitter
            if (!fitter) return;

            if (!fitter.HasTargetTexure())
            {
                Debug.LogWarningFormat("{0} rect has null Texture. Aborting best fit....", rect.name);
                fitter.enabled = false;
                return;
            }
            fitter.pivotMode = options.pivotPoint;
            fitter.resizeMode = Utils.EnumUtils.ResizeMode.AdaptTextureSize;
            fitter.bTextureEnvelopeParent = options.envelopeParent;
            fitter.bTextureFitInParent = options.fitInParent;
            fitter.bTextureFitInParentIgnoreWidth = options.fitInParentIgnoreWidth;
            fitter.bTextureFitInParentShouldReduce = options.fitInParentShouldReduce;
            fitter.fTextureFitInParentReducePercentageValue = options.fitInParentReducePercentageValue;
            fitter.fTextureSizePercentageScaleValue = options.textureSizePercentageScaleValue;
            fitter.bLockTextureSizeX = options.lockWidth;
            fitter.bLockTextureSizeY = options.lockHeight;

            if (resetInitialPosition) fitter.ResetInitialPosition();

            fitter.Init();
        }

        /// <summary>
        /// best fits if parent keeps the same aspect ratio (ex. 16/9)
        /// <para>keep position: keeps children relative position if parent size changed</para>
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="keepPosition"></param>
        public static void BestFitKeepAspectRatio(RectTransform rect, bool keepPosition = true)
        {
            if (rect == null) return;
            if (rect.root == null || rect.root.GetComponent<Canvas>() == null) return;
            RectTransform m_rtParent = rect.parent != null ? rect.parent.GetComponent<RectTransform>() : null;
            if ( m_rtParent == null) return;

            //fix anchors and pivot
            rect.AnchorsToCenter();
            rect.SetMyPivot(new Vector2(0.5f, 0.5f));
            //get position
            Vector2 m_pos = CalcUtils.GetNoNanVector2Position(rect.anchoredPosition);
            //get size
            Vector2 m_size = rect.RealSize();
            //get parent size
            Vector2 parentSize = m_rtParent.RealSize();
            //get percentage vector size relative to parent
            Vector2 relativeSize = (m_size * 100f) / parentSize;
            //get largest result (width or height)
            bool isbiggerX = relativeSize.x > relativeSize.y;
            //store percentage
            float sizePercentage = isbiggerX ? relativeSize.x : relativeSize.y;
            //calculate aspect ratio
            float log = m_size.x / m_size.y;
            //in case there is already attached
            BestFitter fitter = rect.gameObject.GetComponent<BestFitter>();
            //else add new fitter
            if (fitter == null) fitter = rect.gameObject.AddComponent<BestFitter>();
            //abort for any other reason that we can not add fitter
            if (!fitter) return;
            fitter.pivotMode = EnumUtils.PivotPoint.None;
            //we need to adapt mock ratio resize mode, to keep our size same across any resolution (responsive)
            fitter.resizeMode = Utils.EnumUtils.ResizeMode.AdaptMockRatio;
            //apply previous calculated aspect ratio as custom ratio (float number)
            fitter.mockRatioMode = Utils.EnumUtils.MockRatioMode.CustomRatio;
            fitter.fMockAspectRatio = log;
            //size is relative to parent so we must fit in parent and reduce size by previous calculated percentage (sizePercentage)
            fitter.bMockRatioFitInParent = true;
            fitter.bMockRatioFitInParentShouldReduce = true;
            fitter.fMockRatioFitInParentReducePercentageValue = 100f - sizePercentage;
            if (keepPosition)
            {
                //now we need to set relative position from center of parent
                fitter.initialPositionMode = EnumUtils.InitialPositionMode.ParentSizePercentage;
                Vector2 diaf = EnumUtils.GetInitialParentPercentagePos(EnumUtils.PivotPoint.Center, rect, m_rtParent);
                //initial parent percentage position on x axis
                float x = (100f * m_pos.x) / diaf.x;
                //initial parent percentage position on y axis
                float y = (100f * m_pos.y) / diaf.y;
                //store those values for position calculation
                fitter.initVectorParentValues = new Vector2(x, y);
                //set pivot to be at center of parent as all calculations are relative to this point
                fitter.pivotMode = EnumUtils.PivotPoint.Center;
            }
            //apply changes
            fitter.Init();
        }

        /// <summary>
        /// Keeps relative percentage size from parent 
        /// <para>keep position: keeps children relative position if parent size changed</para>
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="keepPosition"></param>
        public static void BestFitKeepParentRelativeSize(RectTransform rect, bool keepPosition = true)
        {
            if (rect == null) return;
            if (rect.root == null || rect.root.GetComponent<Canvas>() == null) return;
            RectTransform m_rtParent = rect.parent != null ? rect.parent.GetComponent<RectTransform>() : null;
            if (m_rtParent == null) return;

            //fix anchors and pivot
            rect.AnchorsToCenter();
            rect.SetMyPivot(new Vector2(0.5f, 0.5f));
            //get position
            Vector2 m_pos = CalcUtils.GetNoNanVector2Position(rect.anchoredPosition);
            //get size
            Vector2 m_size = rect.RealSize();
            //get parent size
            Vector2 parentSize = m_rtParent.RealSize();
            //get percentage vector size relative to parent
            Vector2 relativeSize = (m_size * 100f) / parentSize;

            //in case there is already attached
            BestFitter fitter = rect.gameObject.GetComponent<BestFitter>();
            //else add new fitter
            if (fitter == null) fitter = rect.gameObject.AddComponent<BestFitter>();
            //abort for any other reason that we can not add fitter
            if (!fitter) return;


            fitter.pivotMode = EnumUtils.PivotPoint.None;
            fitter.resizeMode = Utils.EnumUtils.ResizeMode.AdaptParentSize;
            fitter.bParentSizeScaleBothAxes = false;
            fitter.fParentSizePercentageScaleWidth = relativeSize.x;
            fitter.fParentSizePercentageScaleHeight = relativeSize.y;

            if (keepPosition)
            {
                //now we need to set relative position from center of parent
                fitter.initialPositionMode = EnumUtils.InitialPositionMode.ParentSizePercentage;
                Vector2 diaf = EnumUtils.GetInitialParentPercentagePos(EnumUtils.PivotPoint.Center, rect, m_rtParent);
                //initial parent percentage position on x axis
                float x = (100f * m_pos.x) / diaf.x;
                //initial parent percentage position on y axis
                float y = (100f * m_pos.y) / diaf.y;
                //store those values for position calculation
                fitter.initVectorParentValues = new Vector2(x, y);
                //set pivot to be at center of parent as all calculations are relative to this point
                fitter.pivotMode = EnumUtils.PivotPoint.Center;
            }
            //apply changes
            fitter.Init();
        }

        /// <summary>
        /// Keeps relative size if parent size responsively changed (keeps the same aspect ratio)
        /// <para> keep position: keeps children relative position </para>
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="keepPosition"></param>
        public static void BestFitChildren(RectTransform rect, bool keepPosition = true)
        {
            if (!rect) return;
            RectTransform[] rts = rect.GetTopChildren();
            foreach (RectTransform rt in rts)
            {
                BestFitKeepAspectRatio(rt, keepPosition);
            }
        }

        public static void BestFitChildren(BestResize fitter)
        {
            if (fitter == null || fitter.rectTarget == null) return;
            RectTransform[] rts = fitter.rectTarget.GetTopChildren();
            foreach (RectTransform rt in rts)
            {
                if (fitter.resizeMode == EnumUtils.ResizeMode.AdaptMockRatio)
                {
                    BestFitKeepAspectRatio(rt);
                }
                else if(fitter.resizeMode == EnumUtils.ResizeMode.AdaptParentSize)
                {
                    BestFitKeepParentRelativeSize(rt);
                }
            }
        }


        #region Texture

        /// <summary>
        /// Resize texture to fit in preffered dimensions 
        /// <para>If keepWidth is true then only height is calulated to fit. In some cases behaves as envelop parent</para>
        /// </summary>
        public static Vector2 FitRectInContainer(Vector2 rectSize, Vector2 prefferedSize, bool ignoreWidth = false)
        {
            float logY = prefferedSize.y / rectSize.y;
            //resize
            float Y = rectSize.y * logY;
            float X = rectSize.x * logY;

            if (!ignoreWidth)
            {
                if (X > prefferedSize.x)
                {
                    float logX = prefferedSize.x / X;
                    //resize to fit
                    Y *= logX;
                    X *= logX;
                }
            }
            return new Vector2(X, Y);
        }

        /// <summary>
		/// Resize texture to fit in preffered dimensions 
		/// </summary>
		public static Vector2 TextureEnvelopeContainer(Vector2 textureSize, Vector2 prefferedSize)
        {
            float logY = prefferedSize.y / textureSize.y;
            //resize
            float Y = textureSize.y * logY;
            float X = textureSize.x * logY;

            if (X < prefferedSize.x)
            {
                float logX = prefferedSize.x / X;
                //resize to fit
                Y *= logX;
                X *= logX;
            }

            return new Vector2(X, Y);
        }


        #endregion

    }

}
