//StaGe Games ©2022
using StaGeGames.BestFit.Extens;
using StaGeGames.BestFit.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StaGeGames.BestFit
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class BestResize : UIBehaviour
    {
              
        #region Variables

        #region Rects

        /// <summary>
        /// NEVER NULL
        /// <para>The target RectTransform</para>
        /// </summary>
        public RectTransform rectTarget
        {
            get { return EnumUtils.IsObjNull(m_rectTarget) || m_rectTarget != GetComponent<RectTransform>() ? GetComponent<RectTransform>() : m_rectTarget; }
            set { m_rectTarget = value; }
        }
        [SerializeField]
        protected RectTransform m_rectTarget;
        private RectTransform m_rect;//backup check, usually when copy componets performed

        public Vector2 targetPosition
        {
            get { return rectTarget.anchoredPosition; }
            set { rectTarget.anchoredPosition = value; }
        }

        public Vector2 targetSize
        {
            get { return CalcUtils.GetPositiveRealVector2(rectTarget.RealSize()); }
        }               

        /// <summary>
        /// NEVER NULL
        /// <para>The parent RectTransform</para>
        /// </summary>
        public RectTransform rectParent
        {
            get 
            {
                return EnumUtils.IsObjNull(m_rectParent) ? !EnumUtils.IsObjNull(rectTarget.parent) ? rectTarget.parent.GetComponent<RectTransform>() : null : m_rectParent; 
            }
            set { m_rectParent = value; }
        }
        [SerializeField]
        protected RectTransform m_rectParent;

        public RectTransform rectCanvas
        {
            get
            {
                return m_rectCanvas == null ? rectTarget.GetRootCanvasRectTransform() : m_rectCanvas;

                //return m_rectCanvas == null ? rectTarget.root.GetComponent<Canvas>() ? m_rectTarget.root.GetComponent<RectTransform>() : null : m_rectCanvas;
            }
            set { m_rectCanvas = value; }
        }
        protected RectTransform m_rectCanvas;

        /// <summary>
        /// use Canvas as parent for resize reference only - not applicable for pivot
        /// </summary>
        [SerializeField]
        protected bool m_useCanvasAsParent;

        public RectTransform rectLastParent
        {
            get { return m_rectLastParent; }
        }
        [SerializeField]
        protected RectTransform m_rectLastParent;

        protected RectTransform[] rectChildren
        {
            get { return rectTarget.GetTopChildren(); }
        }

        #endregion

        #region Children size fields
        [SerializeField]
        protected float m_childSizeMinWidth = 200f;
        [SerializeField]
        protected float m_childSizeMinHeight = 100f;
        [SerializeField]
        protected bool m_childSizeMaxSizeLimitToParentSize;
        [SerializeField]
        protected float m_childSizeMaxWidth;
        [SerializeField]
        protected float m_childSizeMaxHeight;
        [SerializeField]
        protected bool m_childHorizontal;
        [SerializeField]
        protected float m_childWidthIncreaseValue;
        [SerializeField]
        protected bool m_childVertical;
        [SerializeField]
        protected float m_childHeightIncreaseValue;

        public bool hasChildren { get { return m_hasChildren; } }
        [SerializeField]
        protected bool m_hasChildren;   

        #endregion

        #region Static size fields

        /// <summary>
        /// Keeps targetRect size static.
        /// </summary>
        public float staticWidth { get { return m_staticWidth; } set { m_staticWidth = value; } }
        public float staticHeight { get { return m_staticHeight; } set { m_staticHeight = value; } }
        [SerializeField]
        protected float m_staticWidth = 100f, m_staticHeight = 100f;


        #endregion              

        #region Parent fields
        [NonSerialized]
        protected BestFitter fitterParent;

        /// <summary>
        /// the size of parent rect transform
        /// </summary>
        public Vector2 parentSize { get { return m_parentSize; } set { m_parentSize = value; } }
        [SerializeField]
        protected Vector2 m_parentSize;
        /// <summary>
        /// store size to check if parent size has been changed
        /// </summary>
        private Vector2 parentSizeBefore { get { return m_parentSizeBefore; } set { m_parentSizeBefore = value; } }
        private Vector2 m_parentSizeBefore;
        /// <summary>
        /// [%] size relative to parent
        /// </summary>
        public float fParentSizePercentageScale { get { return m_fParentSizePercentageScale; } set { m_fParentSizePercentageScale = value; } }
        [SerializeField]
        protected float m_fParentSizePercentageScale = 100f;
        /// <summary>
        /// scale both axes?
        /// </summary>
        public bool bParentSizeScaleBothAxes { get { return m_bParentScaleLockAxes; } set { m_bParentScaleLockAxes = value; } }
        [SerializeField]
        protected bool m_bParentScaleLockAxes = true;
        /// <summary>
        /// [%] axis size relative to parent
        /// </summary>
        public float fParentSizePercentageScaleWidth { get { return m_fParentSizePercentageScaleWidth; } set { m_fParentSizePercentageScaleWidth = value; } }
        [SerializeField]
        protected float m_fParentSizePercentageScaleWidth = 100f;


        [SerializeField]
        protected bool m_bIsStaticParentSizeWidth;
        [SerializeField]
        protected float m_fStaticParentSizeWidth = 100f;

        /// <summary>
        /// [%] axis size relative to parent
        /// </summary>
        public float fParentSizePercentageScaleHeight { get { return m_fParentSizePercentageScaleHeight; } set { m_fParentSizePercentageScaleHeight = value; } }
        [SerializeField]
        protected float m_fParentSizePercentageScaleHeight = 100f;

        [SerializeField]
        protected bool m_bIsStaticParentSizeHeight;
        [SerializeField]
        protected float m_fStaticParentSizeHeight = 100f;

        /// <summary>
        /// enables size reduce
        /// </summary>
        public bool bReduceFinalWidth { get { return m_bReduceFinalWidth; } set { m_bReduceFinalWidth = value; } }
        [SerializeField]
        protected bool m_bReduceFinalWidth;
        public bool bReduceFinalHeight { get { return m_bReduceFinalHeight; } set { m_bReduceFinalHeight = value; } }
        [SerializeField]
        protected bool m_bReduceFinalHeight;
        /// <summary>
        /// if true static reduces final size
        /// <para>if false [%] reduces final size</para>
        /// </summary>
        public bool bReduceWidthStatic { get { return m_bReduceWidthStatic; } set { m_bReduceWidthStatic = value; } }
        [SerializeField]
        protected bool m_bReduceWidthStatic;
        public bool bReduceHeightStatic { get { return m_bReduceHeightStatic; } set { m_bReduceHeightStatic = value; } }
        [SerializeField]
        protected bool m_bReduceHeightStatic;
        /// <summary>
        /// static reduce final size, result depends on pivot mode
        /// </summary>
        public float fReducedWidthStaticValue { get { return m_fReducedWidthStaticValue; } set { m_fReducedWidthStaticValue = value; } }
        [SerializeField]
        protected float m_fReducedWidthStaticValue;
        public float fReducedHeightStaticValue { get { return m_fReducedHeightStaticValue; } set { m_fReducedHeightStaticValue = value; } }
        [SerializeField]
        protected float m_fReducedHeightStaticValue;
        /// <summary>
        /// [%] reduce final size, result depends on pivot mode
        /// </summary>
        public float fReducedWidthPercentageValue { get { return m_fReducedWidthPercentageValue; } set { m_fReducedWidthPercentageValue = value; } }
        [SerializeField]
        protected float m_fReducedWidthPercentageValue;
        public float fReducedHeightPercentageValue { get { return m_fReducedHeightPercentageValue; } set { m_fReducedHeightPercentageValue = value; } }
        [SerializeField]
        protected float m_fReducedHeightPercentageValue;
       
        #endregion

        #region Texture fields

        /// <summary>
        /// does this rect target has Image element?
        /// </summary>
        protected Image targetImage
        {
            get { return m_targetImage == null ? m_targetImage = rectTarget.GetComponent<Image>() : m_targetImage; }
        }
        private Image m_targetImage;
        /// <summary>
        /// does this rect target has RawImage element?
        /// </summary>
        public RawImage targetRawImage
        {
            get { return m_targetRawImage == null ? m_targetRawImage = rectTarget.GetComponent<RawImage>() : m_targetRawImage; }
        }
        private RawImage m_targetRawImage;
        /// <summary>
        /// the size of texture
        /// </summary>
        public Vector2 textureSize { get { return m_textureSize; } }
        protected Vector2 m_textureSize;
        /// <summary>
        /// scale texture size
        /// </summary>
        public float fTextureSizePercentageScaleValue { get { return m_fTextureSizePercentageScaleValue; } set { m_fTextureSizePercentageScaleValue = value; } }
        [SerializeField]
        protected float m_fTextureSizePercentageScaleValue = 100f;
        /// <summary>
        /// if true then this axis is locked and can't be controlled 
        /// <para>by percentage scale [m_fTextureSizePercentageScaleValue]</para>
        /// </summary>
        public bool bLockTextureSizeX { get { return m_bLockTextureSizeX; } set { m_bLockTextureSizeX = value; } }
        [SerializeField]
        protected bool m_bLockTextureSizeX;
        /// <summary>
        /// if true then this axis is locked and can't be controlled 
        /// <para>by percentage scale [m_fTextureSizePercentageScaleValue]</para>
        /// </summary>
        public bool bLockTextureSizeY { get { return m_bLockTextureSizeY; } set { m_bLockTextureSizeY = value; } }
        [SerializeField]
        protected bool m_bLockTextureSizeY;
        /// <summary>
        /// fit texture in parent's dimensions
        /// </summary>
        public bool bTextureFitInParent { get { return m_bTextureFitInParent; } set { m_bTextureFitInParent = value; } }
        [SerializeField]
        protected bool m_bTextureFitInParent;
        /// <summary>
        /// texture envelope parent
        /// </summary>
        public bool bTextureEnvelopeParent { get { return m_bTextureEnvelopeParent; } set { m_bTextureEnvelopeParent = value; } }
        [SerializeField]
        protected bool m_bTextureEnvelopeParent;
        /// <summary>
        /// If true then texture height == parents height
        /// <para>Texture width is ignored in fit parent calculations</para>
        ///<para>This means that texture's width could be bigger or smaller than parent's width</para>
        ///<para>If texture's width is bigger then performs like envelope parent</para>
        /// </summary>
        public bool bTextureFitInParentIgnoreWidth { get { return m_bTextureFitInParentIgnoreWidth; } set { m_bTextureFitInParentIgnoreWidth = value; } }
        [SerializeField]
        protected bool m_bTextureFitInParentIgnoreWidth;
        /// <summary>
        /// if true reduces final size
        /// </summary>
        public bool bTextureFitInParentShouldReduce { get { return m_bTextureFitInParentShouldReduce; } set { m_bTextureFitInParentShouldReduce = value; } }
        [SerializeField]
        protected bool m_bTextureFitInParentShouldReduce;
        /// <summary>
        /// if true static reduces final size
        /// <para>if false [%] reduces final size</para>
        /// </summary>
        public bool bTextureFitInParentShouldReduceStatic { get { return m_bTextureFitInParentShouldReduceStatic; } set { m_bTextureFitInParentShouldReduceStatic = value; } }
        [SerializeField]
        protected bool m_bTextureFitInParentShouldReduceStatic;
        /// <summary>
        /// static reduces final height, result depends on pivot mode
        /// </summary>
        public float fTextureFitInParentReduceStaticValue { get { return m_fTextureFitInParentReduceStaticValue; } set { m_fTextureFitInParentReduceStaticValue = value; } }
        [SerializeField]
        protected float m_fTextureFitInParentReduceStaticValue;
        /// <summary>
        /// [%] reduces final height, result depends on pivot mode
        /// </summary>
        public float fTextureFitInParentReducePercentageValue { get { return m_fTextureFitInParentReducePercentageValue; } set { m_fTextureFitInParentReducePercentageValue = value; } }
        [SerializeField]
        protected float m_fTextureFitInParentReducePercentageValue;

        #endregion

        #region Mock ratio

        public EnumUtils.MockRatioMode mockRatioMode { get { return m_mockRatioMode; } set { m_mockRatioMode = value; } }
        [SerializeField]
        protected EnumUtils.MockRatioMode m_mockRatioMode = EnumUtils.MockRatioMode.SelectPreset;

        private readonly float[] mockRatios = new float[] { 1.777f, 1.6f, 2f, 2.333f, 3.555f, 1.2f, 1.25f, 1.333f, 1.5f, 1 };
        public int nMockRatioModeIndex { get { return m_nMockRatioModeIndex; } set { m_nMockRatioModeIndex = value; } }
        [SerializeField]
        protected int m_nMockRatioModeIndex = 0;

        public bool bMockRatioIsPortrait { get { return m_bMockRatioIsPortrait; } set { m_bMockRatioIsPortrait = value; } }
        [SerializeField]
        protected bool m_bMockRatioIsPortrait;

        //proportional relationship between the width and height of any rect
        /// <summary>
        /// values {min = 0.01, max = 1000}
        /// </summary>
        public float fMockAspectRatio { get { return m_fMockAspectRatio; } set { m_fMockAspectRatio = value; } }
        [SerializeField]
        protected float m_fMockAspectRatio = 1.777f;

        /// <summary>
        /// values {min = 10, max = 4096}
        /// </summary>
        public int nMockRatioWidth { get { return m_nMockRatioWidth; } set { m_nMockRatioWidth = value; } }
        [SerializeField]
        protected int m_nMockRatioWidth = 1920;

        /// <summary>
        /// values {min = 10, max = 4096}
        /// </summary>
        public int nMockRatioHeight { get { return m_nMockRatioHeight; } set { m_nMockRatioHeight = value; } }
        [SerializeField]
        protected int m_nMockRatioHeight = 1080;

        /// <summary>
        /// fit in parent, always true
        /// </summary>
        public bool bMockRatioFitInParent { get { return m_bMockRatioFitInParent; } set { m_bMockRatioFitInParent = value; } }
        [SerializeField]
        protected bool m_bMockRatioFitInParent = true;
        /// <summary>
        /// If true then MockRatio height == parents height
        /// <para>MockRatio width is ignored in fit parent calculations</para>
        ///<para>This means that MockRatio's width could be bigger or smaller than parent's width</para>
        /// </summary>
        public bool bMockRatioFitInParentIgnoreWidth { get { return m_bMockRatioFitInParentIgnoreWidth; } set { m_bMockRatioFitInParentIgnoreWidth = value; } }
        [SerializeField]
        protected bool m_bMockRatioFitInParentIgnoreWidth;
        /// <summary>
        /// if true reduces final size
        /// </summary>
        public bool bMockRatioFitInParentShouldReduce { get { return m_bMockRatioFitInParentShouldReduce; } set { m_bMockRatioFitInParentShouldReduce = value; } }
        [SerializeField]
        protected bool m_bMockRatioFitInParentShouldReduce;
        /// <summary>
        /// if true static reduces final size
        /// <para>if false [%] reduces final size</para>
        /// </summary>
        public bool bMockRatioFitInParentShouldReduceStatic { get { return m_bMockRatioFitInParentShouldReduceStatic; } set { m_bMockRatioFitInParentShouldReduceStatic = value; } }
        [SerializeField]
        protected bool m_bMockRatioFitInParentShouldReduceStatic;
        /// <summary>
        /// static reduces final height, result depends on pivot mode
        /// </summary>
        public float fMockRatioFitInParentReduceStaticValue { get { return m_fMockRatioFitInParentReduceStaticValue; } set { m_fMockRatioFitInParentReduceStaticValue = value; } }
        [SerializeField]
        protected float m_fMockRatioFitInParentReduceStaticValue;
        /// <summary>
        /// [%] reduces final height, result depends on pivot mode
        /// </summary>
        public float fMockRatioFitInParentReducePercentageValue { get { return m_fMockRatioFitInParentReducePercentageValue; } set { m_fMockRatioFitInParentReducePercentageValue = value; } }
        [SerializeField]
        protected float m_fMockRatioFitInParentReducePercentageValue = 0f;

        #endregion

        #region Initial Position fields

        /// <summary>
        /// Initial Position Offset static
        /// </summary>
        public Vector2 initialStaticPos { get { return m_InitialStaticPos; } set { m_InitialStaticPos = value; } }
        [SerializeField]
        protected Vector2 m_InitialStaticPos;// X, m_initialPosY;
        /// <summary>
        /// Initial Position Offset Mode
        /// <para>Static values, Self or Parent size percentage values</para>
        /// </summary>
        public EnumUtils.InitialPositionMode initialPositionMode { get { return m_InitialPositionMode; } set { m_InitialPositionMode = value; } }
        [SerializeField]
        protected EnumUtils.InitialPositionMode m_InitialPositionMode = EnumUtils.InitialPositionMode.Static;
        /// <summary>
        /// Apply Initial Position Offset with target size delta percentage calculations
        /// </summary>
        public Vector2 initVectorSelfValues { get { return m_initVectorSelfValues; } set { m_initVectorSelfValues = value; } }
        [SerializeField]
        protected Vector2 m_initVectorSelfValues;
        /// <summary>
        /// Apply Initial Position Offset with parent size delta percentage calculations.
        /// <para>Depends from pivot mode</para>
        /// </summary>
        public Vector2 initVectorParentValues { get { return m_initVectorParentValues; } set { m_initVectorParentValues = value; } }
        [SerializeField]
        protected Vector2 m_initVectorParentValues;

        #endregion

        #region Motion fields

        /// <summary>
        /// is target RectTransform moving?
        /// </summary>
        public bool isMovable { get { return m_isMovable; } set { m_isMovable = value; } }
        [SerializeField]
        protected bool m_isMovable;
        /// <summary>
        /// should moving target be visible at start?
        /// </summary>
        public bool isVisibleOnStart { get { return m_isVisibleOnStart; } set { m_isVisibleOnStart = value; } }
        [SerializeField]
        protected bool m_isVisibleOnStart;
        /// <summary>
        /// moving direction from pivot point
        /// </summary>
        public EnumUtils.MotionMode motionMode { get { return m_motionMode; } set { m_motionMode = value; } }
        [SerializeField]
        protected EnumUtils.MotionMode m_motionMode = EnumUtils.MotionMode.Free;

        #endregion

        public bool updateFit { get { return m_updateFit; } set { m_updateFit = value; } }
        [SerializeField]
        protected bool m_updateFit = true;

        [SerializeField]
        protected bool m_IgnoreFitWidth;
        [SerializeField]
        protected bool m_IgnoreFitHeight;

        public bool IgnoreFitHeight { get { return m_IgnoreFitHeight; } set { m_IgnoreFitHeight = value; } }

        /// <summary>
        /// Rotations, size, and scale modifications occur around the pivot
        /// </summary>
        public EnumUtils.PivotPoint pivotMode { get { return m_pivotMode; } set { m_pivotMode = value; } }
        [SerializeField]
        protected EnumUtils.PivotPoint m_pivotMode = EnumUtils.PivotPoint.None;
        /// <summary>
        /// stick in or out
        /// </summary>
        public EnumUtils.StickMode stickMode { get { return m_stickMode; } set { m_stickMode = value; } }
        [SerializeField]
        protected EnumUtils.StickMode m_stickMode = EnumUtils.StickMode.Free;
        /// <summary>
        /// stick positions
        /// </summary>
        public EnumUtils.MotionMode stickPosition { get { return m_stickPosition; } set { m_stickPosition = value; } }
        [SerializeField]
        protected EnumUtils.MotionMode m_stickPosition = EnumUtils.MotionMode.Free;
        public EnumUtils.ResizeMode resizeMode { get { return m_resizeMode; } set { m_resizeMode = value; } }
        [SerializeField]
        protected EnumUtils.ResizeMode m_resizeMode = EnumUtils.ResizeMode.None;

        /// <summary>
        /// Event which is raised when resizing complete
        /// </summary>
        public UnityEvent OnResizeComplete;

        #endregion

        public SmartMotion smartMotion
        {
            get { return m_smartMotion == null ? m_isMovable ? GetComponent<SmartMotion>() == null ? gameObject.AddComponent<SmartMotion>() : GetComponent<SmartMotion>() : m_smartMotion : m_smartMotion; }
            set { m_smartMotion = value; }
        }
        [SerializeField]
        protected SmartMotion m_smartMotion;

        protected HorizontalLayoutGroup horizontalLayout
        {
            get { return m_horizontalLayout == null ? rectTarget.GetComponent<HorizontalLayoutGroup>() : m_horizontalLayout; }
            set { m_horizontalLayout = value; }
        }
        [NonSerialized]
        private HorizontalLayoutGroup m_horizontalLayout;
        protected VerticalLayoutGroup verticalLayout
        {
            get { return m_verticalLayout == null ? rectTarget.GetComponent<VerticalLayoutGroup>() : m_verticalLayout; }
            set { m_verticalLayout = value; }
        }
        [NonSerialized]
        private VerticalLayoutGroup m_verticalLayout;
        public bool hasHorizontalLayoutGroup { get { return horizontalLayout != null && horizontalLayout.enabled; } }
        public bool hasVerticalLayoutGroup { get { return verticalLayout != null && verticalLayout.enabled; } }

        public void CheckComponetsChanges()
        {
            m_horizontalLayout = GetComponent<HorizontalLayoutGroup>();
            m_verticalLayout = GetComponent<VerticalLayoutGroup>();
        }

        protected void NotifyChilds()
        {
            if (rectChildren.Length <= 0) return;
            for (int i = 0; i < rectChildren.Length; i++)
            {
                if (rectChildren[i] == null || !rectChildren[i].gameObject.activeInHierarchy)
                    continue;
                rectChildren[i].BestFitInitDelayed();
            }
           // Init();
        }

        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
#pragma warning disable UNT0008 // Null propagation on Unity objects
                SmartScreenOrientationChecker.Instance?.Init();
#pragma warning restore UNT0008 // Null propagation on Unity objects
        }

        protected override void OnEnable() { base.OnEnable(); InitAtRuntime(); SmartScreenOrientationChecker.OnCreateRegister?.Invoke(this); }

        private void InitAtRuntime()
        {
            if (!TargetIsChild(out float waitTime)) { Init(); } else { Invoke(nameof(Init), waitTime); }
        }

        public void InitDelayed() { Invoke(nameof(Init), 0.1f); }

        public float GloballyInit()
        {
            if (!TargetIsChild(out float waitTime)) { Init(); return 0f; } else { Invoke(nameof(Init), waitTime); return waitTime; }
        }

        public bool IsParentSizeChanged()
        {
            if (rectParent == null) return false;
            m_parentSizeBefore = m_useCanvasAsParent ? CalcUtils.GetPositiveRealVector2(rectCanvas.RealSize()) : CalcUtils.GetPositiveRealVector2(rectParent.RealSize());
            return m_parentSizeBefore != m_parentSize && m_parentSizeBefore != Vector2.zero;
        }

        private bool GetTargets()
        {
            if (m_rect == null) m_rect = GetComponent<RectTransform>();
            if (rectTarget == null || rectTarget != m_rectTarget || !m_rect) rectTarget = GetComponent<RectTransform>();
            if (!rectTarget) return false;
            //rectChildren = rectTarget.GetTopChildren();
            if (m_rectCanvas == null)
            {
                rectCanvas = m_rectTarget.root.GetComponent<Canvas>() ? m_rectTarget.root.GetComponent<RectTransform>() : null;
            }
            if (rectTarget.parent != null)  rectParent = rectTarget.parent.GetComponent<RectTransform>();
            if (rectParent == null) { return false; }
            return true;
        }

        private void GetAllSizes()
        {
            m_parentSize = m_useCanvasAsParent ? CalcUtils.GetPositiveRealVector2(rectCanvas.RealSize()) : CalcUtils.GetPositiveRealVector2(rectParent.RealSize());
            m_parentSizeBefore = m_parentSize;    
            rectTarget.sizeDelta = GetFinalSize();
        }

        private bool isInitializing;

        public void Init()
        {
           // Debug.Log("INITIALIZING "+transform.name);

            if (/*!this.enabled ||*/ !m_updateFit) return;
            if (!GetTargets())  return;
            if (isInitializing) return;
            isInitializing = true;
            GetAllSizes();

            m_hasChildren = rectChildren.Length > 0;

            rectTarget.SetBestPivot(pivotMode);
            targetPosition = CalcUtils.GetNoNanVector2Position(CalculateInitialPosition());

            if (m_isMovable) smartMotion.Init();

            CancelInvoke();

            if (OnResizeComplete != null) OnResizeComplete?.Invoke();

            if (fitterParent)
            {
                if (fitterParent.resizeMode == EnumUtils.ResizeMode.AdaptChildSize)
                    fitterParent.Init();
            }

            if(hasChildren) NotifyChilds();

#if UNITY_EDITOR
            PrepareEventsForEditorUse();
#endif

            isInitializing = false;
        }

        #region Initial Position Calculations

        public void ResetInitialPosition()
        {
            switch (initialPositionMode)
            {
                case EnumUtils.InitialPositionMode.SelfSizePercentage:
                    m_initVectorSelfValues = Vector2.zero;
                    break;
                case EnumUtils.InitialPositionMode.ParentSizePercentage:
                    m_initVectorParentValues = Vector2.zero;
                    break;
                case EnumUtils.InitialPositionMode.Static:
                default:
                    m_InitialStaticPos = Vector2.zero;
                    break;
            }
        }

        private bool IsPivotNone(Vector2 v) 
        { 
            if (pivotMode == EnumUtils.PivotPoint.None) 
                if (v == Vector2.zero) return true;
            return false;
        }

        public void AnimateMe() { isAnimatingPosition = true; }
        public void AnimateMeStop() { isAnimatingPosition = false; }
        public bool isAnimatingPosition;
        private void Update()
        {
            if (isAnimatingPosition) targetPosition = CalculateInitialPosition();
        }

        public Vector2 CalculateInitialPosition()
        {
            //if (Application.isEditor) Debug.Log("CalculateInitialPosition "+ rectTarget.name);           

            Vector2 finalPos = targetPosition;
            if (IsPivotNone(m_initVectorSelfValues)) return finalPos;

            RectTransform rtParent = m_useCanvasAsParent ? m_rectCanvas : rectParent;            

            switch (initialPositionMode)
            {
                case EnumUtils.InitialPositionMode.SelfSizePercentage:

                    finalPos.x = rectTarget.GetWidth() * m_initVectorSelfValues.x / 100f;
                    finalPos.y = rectTarget.GetHeight() * m_initVectorSelfValues.y / 100f;

                    break;
                case EnumUtils.InitialPositionMode.ParentSizePercentage:

                    if (!rectParent)
                    {
                        if (Application.isEditor) Debug.LogWarningFormat("initPosWithPercentage: Target Parent is null for {0}", rectTarget.name);
                        return Vector2.zero;
                    }

                    Vector2 diaf = EnumUtils.GetInitialParentPercentagePos(pivotMode, rectTarget, rtParent);// rectParent);

                    finalPos.x = diaf.x * m_initVectorParentValues.x / 100f;
                    finalPos.y = diaf.y * m_initVectorParentValues.y / 100f;

                    break;
                case EnumUtils.InitialPositionMode.Static:
                default:
                    finalPos = m_InitialStaticPos;
                    break;
            }

            if (m_stickMode == EnumUtils.StickMode.External) finalPos += EnumUtils.GetExternalStickPivot(rectTarget, pivotMode, stickPosition);

            return CalcUtils.GetNoNanVector2Position(finalPos);
        }


        #endregion

        #region Width - Height Calculations

        public Vector2 GetFinalSize()
        {
            switch (m_resizeMode)
            {
                case EnumUtils.ResizeMode.None:
                    return rectTarget.RealSize();
                case EnumUtils.ResizeMode.Static:
                default:
                    Vector2 _staticSize = CalcUtils.GetPositiveVector2(new Vector2(m_staticWidth, m_staticHeight));
                    if (m_IgnoreFitWidth) _staticSize.x = rectTarget.GetWidth();
                    if (m_IgnoreFitHeight) _staticSize.y = rectTarget.GetHeight();
                    return _staticSize;
                case EnumUtils.ResizeMode.AdaptParentSize:
                    Vector2 _relSize = GetParentSizePercentage();
                    if (m_IgnoreFitWidth) _relSize.x = rectTarget.GetWidth();
                    if (m_IgnoreFitHeight) _relSize.y = rectTarget.GetHeight();
                    return _relSize;
                case EnumUtils.ResizeMode.AdaptTextureSize:
                    Vector2 _texSize = rectTarget.RealSize();
                    if (!GetTextureSize()) return _texSize; //new Vector2(100f, 100f);
                    if (m_bTextureEnvelopeParent) {  _texSize = GetResponsiveTextureEnvelopParent(); }
                    else if (m_bTextureFitInParent) { _texSize = GetResponsiveTextureSizeFitInParent(); }
                    else  {  _texSize = GetResponsivePercentageTextureSizeVector(); }
                    if (m_IgnoreFitWidth) _texSize.x = rectTarget.GetWidth();
                    if (m_IgnoreFitHeight) _texSize.y = rectTarget.GetHeight();
                    return _texSize;
                case EnumUtils.ResizeMode.AdaptMockRatio:
                    Vector2 _mockSize = GetMockRatioSize();
                    if (m_IgnoreFitWidth) _mockSize.x = rectTarget.GetWidth();
                    if (m_IgnoreFitHeight) _mockSize.y = rectTarget.GetHeight();
                    return _mockSize;
                case EnumUtils.ResizeMode.AdaptChildSize:
                    Vector2 _chSize = GetChildSize();
                    if (m_IgnoreFitWidth) _chSize.x = rectTarget.GetWidth();
                    if (m_IgnoreFitHeight) _chSize.y = rectTarget.GetHeight();
                    return _chSize;
            }
        }

        #region CHILD SIZE

        protected Vector2 GetChildSize()
        {
            if (rectChildren.Length <= 0) return targetSize;

            float w = rectChildren[0].GetWidth();// !m_childVertical ? rectChildren[0].GetWidth() : GetChildrenTotalWidth();
            float h = rectChildren[0].GetHeight();// !m_childHorizontal ? rectChildren[0].GetHeight() : GetChildrenTotalHeight();
            
            Vector2 finalSize = CalcUtils.GetPositiveRealVector2(new Vector2(w, h));

            float maxLimitWidth = m_childSizeMaxSizeLimitToParentSize ? parentSize.x : m_childSizeMaxWidth > 0 ? m_childSizeMaxWidth : 50000f;
            float maxLimitHeight = m_childSizeMaxSizeLimitToParentSize ? parentSize.y : m_childSizeMaxHeight > 0 ? m_childSizeMaxHeight : 50000f;

            //if (m_childSizeMaxSizeLimitToParentSize)
            //{
            //    finalSize.x = Mathf.Min(finalSize.x, parentSize.x);
            //    finalSize.y = Mathf.Min(finalSize.y, parentSize.y);
            //    //rectChildren[0].sizeDelta = new Vector2(Mathf.Min(rectChildren[0].GetWidth(), parentSize.x - m_childWidthIncreaseValue), Mathf.Min(rectChildren[0].GetHeight(), parentSize.y - m_childHeightIncreaseValue));
            //}
            //else
            //{
            //    if (m_childSizeMaxWidth > 0)
            //    {
            //        finalSize.x = Mathf.Min(finalSize.x, m_childSizeMaxWidth);
            //        //rectChildren[0].sizeDelta = new Vector2(Mathf.Min(rectChildren[0].GetWidth(), m_childSizeMaxWidth - m_childWidthIncreaseValue), rectChildren[0].GetHeight());
            //    }

            //    if (m_childSizeMaxHeight > 0)
            //    {
            //        finalSize.y = Mathf.Min(finalSize.y, m_childSizeMaxHeight);
            //        //rectChildren[0].sizeDelta = new Vector2(rectChildren[0].GetWidth(), Mathf.Min(rectChildren[0].GetHeight(), m_childSizeMaxHeight - m_childHeightIncreaseValue));
            //    }
            //}

            #region deprecated
            /*

            //Limits max size to equals parent size.
            //If both max width and max height are > 0 then this is ignored.
            //If max width is > 0 and height == 0 then max size will be max width, parent's height.
            //If max height is > 0 and width == 0 then max size will be parent's width, max height.
            bool isMaxSizeParentIgnored = m_childSizeMaxWidth > 0 && m_childSizeMaxHeight > 0;
            if (m_childSizeMaxSizeLimitToParentSize && !isMaxSizeParentIgnored)
            {
                if(m_childSizeMaxWidth > 0)
                {
                    finalSize.x = Mathf.Clamp(finalSize.x, 0f, m_childSizeMaxWidth);
                    finalSize.y = Mathf.Clamp(finalSize.y, 0f, parentSize.y);
                }
                else 
                if (m_childSizeMaxHeight > 0)
                {
                    finalSize.x = Mathf.Clamp(finalSize.x, 0f, parentSize.x);
                    finalSize.y = Mathf.Clamp(finalSize.y, 0f, m_childSizeMaxHeight);
                }
                else
                {
                    finalSize.x = Mathf.Clamp(finalSize.x, 0f, parentSize.x);
                    finalSize.y = Mathf.Clamp(finalSize.y, 0f, parentSize.y);
                }
            }
            else
            {
                if (m_childSizeMaxWidth > 0)
                {
                    finalSize.x = Mathf.Clamp(finalSize.x, 0f, m_childSizeMaxWidth);
                }
                else
                if (m_childSizeMaxHeight > 0)
                {
                    finalSize.y = Mathf.Clamp(finalSize.y, 0f, m_childSizeMaxHeight);
                }
            }
            */

            #endregion

            //finalSize.x = m_childSizeMaxSizeLimitToParentSize ? Mathf.Clamp(finalSize.x, m_childSizeMinWidth, parentSize.x) : Mathf.Clamp(finalSize.x, m_childSizeMinWidth, m_childSizeMinWidth);
            //finalSize.y = m_childSizeMaxSizeLimitToParentSize ? Mathf.Clamp(finalSize.y, m_childSizeMinHeight, parentSize.y) : Mathf.Clamp(finalSize.y, m_childSizeMinHeight, m_childSizeMinHeight);

            finalSize.x += m_childWidthIncreaseValue;
            finalSize.y += m_childHeightIncreaseValue;

            finalSize.x = Mathf.Clamp(finalSize.x, m_childSizeMinWidth, maxLimitWidth);
            finalSize.y = Mathf.Clamp(finalSize.y, m_childSizeMinHeight, maxLimitHeight);
            rectChildren[0].sizeDelta = new Vector2(finalSize.x - m_childWidthIncreaseValue, finalSize.y - m_childHeightIncreaseValue);


           

            return finalSize;
        }

        protected float GetChildrenTotalHeight()
        {
            if (hasVerticalLayoutGroup) return verticalLayout.preferredHeight;
            RectTransform[] chs = rectChildren;
            if (chs.Length <= 0) return rectTarget.GetHeight();
            float h = 0f;
            foreach (RectTransform rt in chs) h += rt.GetHeight();
            return h;
        }

        protected float GetChildrenTotalWidth()
        {
            if (hasHorizontalLayoutGroup) return horizontalLayout.preferredWidth;
            RectTransform[] chs = rectChildren;
            if (chs.Length <= 0) return rectTarget.GetWidth();
            float w = 0f;
            foreach (RectTransform rt in chs) w += rt.GetWidth();
            return w;
        }

        #endregion

        #region MOCK RATIO

        protected Vector2 GetMockRatioSize()
        {
            Vector2 newParentSize = CalcUtils.GetPositiveRealVector2(m_parentSize);

            //if (m_bMockRatioFitInParentShouldReduce)
            //{
            //    if (m_bMockRatioFitInParentShouldReduceStatic)
            //    {
            //        newParentSize.x -= Mathf.Abs(m_fMockRatioFitInParentReduceStaticValue);
            //        newParentSize.y -= Mathf.Abs(m_fMockRatioFitInParentReduceStaticValue);
            //    }
            //    else
            //    {
            //        if (m_fMockRatioFitInParentReducePercentageValue > 0)
            //        {
            //            float space = Mathf.Abs(m_fMockRatioFitInParentReducePercentageValue);

            //            float x = (space * newParentSize.x) / 100f;
            //            float y = (space * newParentSize.y) / 100f;
            //            newParentSize = new Vector2(x, y);

            //            //if (m_fReducedWidthPercentageValue > 0)
            //            //{
            //            //    float spaceX = Mathf.Abs(m_fReducedWidthPercentageValue);
            //            //    spaceX = currWidth * spaceX / 100f;
            //            //    currWidth -= spaceX;
            //            //}
            //        }
            //    }
            //}

            //Vector2 aspect = Vector2.one;
            bool isPortrait = false;

            float ratio;
            switch (mockRatioMode)
            {
                case EnumUtils.MockRatioMode.SelectPreset:
                default:
                    ratio = mockRatios[m_nMockRatioModeIndex];
                    isPortrait = m_bMockRatioIsPortrait;
                    break;
                case EnumUtils.MockRatioMode.CustomRatio:
                    ratio = m_fMockAspectRatio;
                    isPortrait = m_bMockRatioIsPortrait;
                    break;
                case EnumUtils.MockRatioMode.PrefferedDimensions:
                    ratio = (float)m_nMockRatioWidth / (float)m_nMockRatioHeight;
                    if (!m_bMockRatioFitInParent) return new Vector2(m_nMockRatioWidth, m_nMockRatioHeight);
                    break;
            }

            float _h = newParentSize.x / ratio;
            float _w = _h * ratio;
            Vector2 aspect = CalcUtils.GetPositiveRealVector2(new Vector2(_w, _h));

            if (isPortrait) aspect = new Vector2(_h, _w);

            Vector2 finalSize = FitUtils.FitRectInContainer(aspect, CalcUtils.GetPositiveRealVector2(newParentSize), m_bMockRatioFitInParentIgnoreWidth);

            if (m_bMockRatioFitInParentShouldReduce)
            {
                if (m_bMockRatioFitInParentShouldReduceStatic)
                {
                    finalSize.x -= Mathf.Abs(m_fMockRatioFitInParentReduceStaticValue);
                    finalSize.y -= Mathf.Abs(m_fMockRatioFitInParentReduceStaticValue);
                }
                else
                {
                    if (m_fMockRatioFitInParentReducePercentageValue > 0)
                    {
                        float spaceX = Mathf.Abs(m_fMockRatioFitInParentReducePercentageValue);
                        float spaceY = Mathf.Abs(m_fMockRatioFitInParentReducePercentageValue);

                        spaceX = finalSize.x * spaceX / 100f;
                        spaceY = finalSize.y * spaceY/ 100f;

                        finalSize.x -= spaceX;// (space * newParentSize.x) / 100f;
                        finalSize.y -= spaceY;//(space * newParentSize.y) / 100f;

                        //if (m_fReducedWidthPercentageValue > 0)
                        //{
                        //    float spaceX = Mathf.Abs(m_fReducedWidthPercentageValue);
                        //    spaceX = currWidth * spaceX / 100f;
                        //    currWidth -= spaceX;
                        //}
                    }
                }
            }

            return finalSize;
        }

        #endregion

        #region TEXTURE
        public bool HasTargetTexure()
        {
            if (targetImage)
            {
                if (targetImage.sprite != null) { return targetImage.sprite.texture != null; }
                if (targetImage.material != null) { return targetImage.material.mainTexture != null; }
                return false;
            }
            else if (targetRawImage)
            {
                if (targetRawImage.texture != null) return true;
                if (targetRawImage.material != null) { return targetRawImage.material.mainTexture != null; }
            }
            return false;
        }

        public Texture GetTexure()
        {
            if(Application.isEditor) Debug.Log("GetTexure");
            if (targetImage)//give priority to image texture
            {
                return targetImage.sprite ? targetImage.sprite.texture != null ? targetImage.sprite.texture
                    : null : targetImage.material != null ? targetImage.material.mainTexture != null
                    ? targetImage.material.mainTexture : null : null;
            }
            else if (targetRawImage)//give priority to rawImage texture
            {
                return targetRawImage.texture != null
                    ? targetRawImage.texture
                    : targetRawImage.material != null 
                    ? targetRawImage.material.mainTexture != null
                    ? targetRawImage.material.mainTexture
                    : null : null;
            }
            return null;
        }

        private bool GetTextureSize() 
        {
            if (m_resizeMode != EnumUtils.ResizeMode.AdaptTextureSize) return false;

            //if (Application.isEditor) Debug.LogWarningFormat("GetTextureSize for {0}", gameObject.name);
            if (!HasTargetTexure()) { PrintTextureError(); return false; }
            if (targetImage)
            {
                if (targetImage.sprite) 
                { 
                    if (targetImage.sprite.texture != null) 
                    {
                        CheckTextureSizeChanged(targetImage.sprite.TexureSize());//.texture.Size());
                        return true;
                    } 
                }
                else if (targetImage.material != null)
                { 
                    if(targetImage.material.mainTexture != null)
                    {
                        CheckTextureSizeChanged(targetImage.material.TexureSize());//.mainTexture.Size()) ;
                        return true;
                    }
                }
            }
            else if (targetRawImage)
            {
                if (targetRawImage.texture != null)
                {
                    CheckTextureSizeChanged(targetRawImage.texture.Size());
                    return true;
                }
                else if (targetRawImage.material != null) 
                { 
                    if(targetRawImage.material.mainTexture != null)
                    {
                        CheckTextureSizeChanged(targetRawImage.material.TexureSize());//.mainTexture.Size());
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckTextureSizeChanged(Vector2 texSize)
        {
            texSize = CalcUtils.GetPositiveRealVector2(texSize);
            if (texSize.HasZeroAxis()) { PrintTextureSizeError(); return false; }
            if (!texSize.EqualsTo(m_textureSize)) { m_textureSize = texSize; return true; }
            return false;
        }

        private Vector2 GetResponsivePercentageTextureSizeVector()
        {
            float x = m_bLockTextureSizeX ? m_textureSize.x : (m_textureSize.x * m_fTextureSizePercentageScaleValue) / 100f;
            float y = m_bLockTextureSizeY ? m_textureSize.y : (m_textureSize.y * m_fTextureSizePercentageScaleValue) / 100f;
            Vector2 newSize = CalcUtils.GetPositiveRealVector2(new Vector2(x, y));
            return newSize;
        }

        private Vector2 GetResponsiveTextureSizeFitInParent()
        {
            Vector2 newParentSize = CalcUtils.GetPositiveRealVector2(m_parentSize);
            if (m_bTextureFitInParentShouldReduce)
            {
                if (m_bTextureFitInParentShouldReduceStatic)
                {
                    newParentSize.x -= Mathf.Abs(m_fTextureFitInParentReduceStaticValue);
                    newParentSize.y -= Mathf.Abs(m_fTextureFitInParentReduceStaticValue);
                }
                else
                {
                    if (m_fTextureFitInParentReducePercentageValue > 0)
                    {
                        //float space = Mathf.Abs(m_fTextureFitInParentReducePercentageValue);

                        //float x = (space * newParentSize.x) / 100f;
                        //float y = (space * newParentSize.y) / 100f;
                        //newParentSize = new Vector2(x, y);

                        float spaceX = Mathf.Abs(m_fTextureFitInParentReducePercentageValue);
                        float spaceY = Mathf.Abs(m_fTextureFitInParentReducePercentageValue);

                        spaceX = newParentSize.x * spaceX / 100f;
                        spaceY = newParentSize.y * spaceY / 100f;

                        newParentSize.x -= spaceX;// (space * newParentSize.x) / 100f;
                        newParentSize.y -= spaceY;//(space * newParentSize.y) / 100f;

                    }
                }
            }
            Vector2 mySize = FitUtils.FitRectInContainer(m_textureSize, CalcUtils.GetPositiveRealVector2(newParentSize), m_bTextureFitInParentIgnoreWidth);
            
            return CalcUtils.GetPositiveRealVector2(new Vector2(mySize.x, mySize.y));
        }

        private Vector2 GetResponsiveTextureEnvelopParent()
        {
            return FitUtils.TextureEnvelopeContainer(m_textureSize, CalcUtils.GetPositiveRealVector2(m_parentSize));
        }

        private void PrintTextureError()
        {
            //if (Application.isEditor) Debug.LogWarningFormat("Texture is null for [ {0} ]", rectTarget.name);
        }
        private void PrintTextureSizeError()
        {
            if (Application.isEditor) Debug.LogWarningFormat("Texture Size Zero Axis Error for [ {0} ]", rectTarget.name);
        }

        #endregion

        #region PARENTING

        private Vector2 GetParentSizePercentage()
        {
           // m_parentSize = CalcUtils.GetPositiveRealVector2(rectParent.RealSize());
            m_parentSize = m_useCanvasAsParent ? CalcUtils.GetPositiveRealVector2(rectCanvas.RealSize()) : CalcUtils.GetPositiveRealVector2(rectParent.RealSize());

            //vParentSizeBefore = MathUtilities.GetPositiveRealVector2(vParentSize);

            float _scale = CalcUtils.GetPositiveRealFloat(m_fParentSizePercentageScale);

            Vector2 _vectorScale = m_bParentScaleLockAxes ? new Vector2(_scale, _scale)
                               : new Vector2(CalcUtils.GetPositiveRealFloat(m_fParentSizePercentageScaleWidth), 
                               CalcUtils.GetPositiveRealFloat(m_fParentSizePercentageScaleHeight));

            float finalWidth = m_bIsStaticParentSizeWidth && !m_bParentScaleLockAxes ? m_fStaticParentSizeWidth : (m_parentSize.x * _vectorScale.x) / 100f;
            float finalwHeight = m_bIsStaticParentSizeHeight && !m_bParentScaleLockAxes ? m_fStaticParentSizeHeight : (m_parentSize.y * _vectorScale.y) / 100f;

            //fix negative values and calculate reduce
            finalWidth = CalcUtils.GetPositiveRealFloat(CheckReducedWidth(finalWidth));
            finalwHeight = CalcUtils.GetPositiveRealFloat(CheckReducedHeight(finalwHeight));

            return new Vector2(finalWidth, finalwHeight);
        }

        private float CheckReducedWidth(float currWidth)
        {
            if (m_bReduceFinalWidth)
            {
                if (m_bReduceWidthStatic)
                {
                    if (m_fReducedWidthStaticValue > 0) currWidth -= m_fReducedWidthStaticValue;
                }
                else
                {
                    if (m_fReducedWidthPercentageValue > 0)
                    {
                        float spaceX = Mathf.Abs(m_fReducedWidthPercentageValue);
                        spaceX = currWidth * spaceX / 100f;
                        currWidth -= spaceX;
                    }
                }
            }
            return currWidth;
        }
        private float CheckReducedHeight(float currHeight)
        {
            if (m_bReduceFinalHeight)
            {
                if (m_bReduceHeightStatic)
                {
                    if (m_fReducedHeightStaticValue > 0) currHeight -= m_fReducedHeightStaticValue;
                }
                else
                {
                    if (m_fReducedHeightPercentageValue > 0)
                    {
                        float spaceY = Mathf.Abs(m_fReducedHeightPercentageValue);
                        spaceY = currHeight * spaceY / 100f;
                        currHeight -= spaceY;
                    }
                }
            }
            return currHeight;
        }

        #endregion

        #endregion

        #region Check Target Componets

        public bool HasTargetAspectRatioFitter()
        {
            if (rectTarget.TryGetComponent(out AspectRatioFitter ratioFitter))
            {
                ratioFitter.aspectMode = AspectRatioFitter.AspectMode.None; 
                ratioFitter.enabled = false; 
                return true; 
            }
            return false;
        }

        public void RemoveAspectRatioFitter()
        {
            if (rectTarget.TryGetComponent(out AspectRatioFitter ratioFitter))
            {
                if (Application.isEditor) { DestroyImmediate(ratioFitter); }
                else { Destroy(ratioFitter); }
            }
        }

        public bool HasTargetContentSizeFitter(RectTransform target)
        {
            if (target == null || m_useCanvasAsParent) return false;
            if (target.TryGetComponent(out ContentSizeFitter sizeFitter))
            {
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                sizeFitter.enabled = false;
                return true;
            }
            return false;
        }

        public void UseCanvasAsParent()
        {
            m_useCanvasAsParent = true;
        }

        public void RemoveContentSizeFitter(RectTransform target)
        {
            if (target == null) return;
            if (target.TryGetComponent(out ContentSizeFitter sizeFitter))
            {
                if (Application.isEditor) { DestroyImmediate(sizeFitter); }
                else { Destroy(sizeFitter); }
            }
        }

        public bool HasTargetLayoutElement()
        {
            if (rectTarget.TryGetComponent(out LayoutElement layout))
            {
                layout.ignoreLayout = true;
                layout.enabled = false;
                return true;
            }
            return false;
        }

        public void RemoveLayoutElement()
        {
            if (rectTarget.TryGetComponent(out LayoutElement layout))
            {
                if (Application.isEditor) { DestroyImmediate(layout); }
                else { Destroy(layout); }
            }
        }


        #endregion

        #region General Functions

        
        [ContextMenu("Find All Parents")]
        private void FindAllParents()
        {
            if (TargetIsChild(out float p)) { Debug.LogFormat("{0} has {1} parents with SmartResize", rectTarget.name, p * 10); }
        }

        private bool isRootSmartResize
        {
            get
            {
                Transform parent = transform.parent;
                if (parent != null)
                {
                    if (parent.GetComponent<Canvas>())
                        return true;
                }
                else
                {
                    return false;
                }
                return parent.GetComponent<BestResize>() == null;
            }
        }

        /// <summary>
        /// Init Delay Time
        /// <para>Returns delay time to Init to give parents priority to Init first</para>
        /// <para>Every parent adds 0.1 sec delay</para>
        /// </summary>
        /// <param name="timeWaitStep"></param>
        /// <returns></returns>
        private bool TargetIsChild(out float timeWaitStep)
        {
            timeWaitStep = 0f;
            int p = 0;
            if (!rectTarget || isRootSmartResize) return false;
            Transform tp = rectTarget;
            for (int i = 0; i < 1000; i++)
            {
                tp = tp.parent;
                if (tp == null) break;
                if (tp.GetComponent<BestResize>() != null) { p++; }
            }
            if (p <= 0) return false;
            // Debug.Log("has " + p + " parents");
            timeWaitStep = p * 0.1f;
            return true;
        }        

        #endregion

        protected override void OnDisable()
        {
            base.OnDisable();
            SmartScreenOrientationChecker.OnDisableUnregister?.Invoke(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            SmartScreenOrientationChecker.OnDestroyUnregister?.Invoke(this);
        }

        #region Internal functions

#if UNITY_EDITOR
        private void PrepareEventsForEditorUse()
        {
            if (OnResizeComplete != null && OnResizeComplete.GetPersistentEventCount() > 0) 
                OnResizeComplete.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
        }

        protected override void OnValidate() 
        { 
            base.OnValidate(); 
            //UnityEditor.EditorApplication.delayCall += _OnValidate;
            _OnValidate();
        }

        protected void _OnValidate()
        {
            if (this == null) return;
            //Debug.Log("OnValidate " + transform.name);
            if (m_rectTarget == null) GetTargets();
           
        }

        protected override void Reset()
        {
            
            if(this.transform.parent == null)
            {
                CancelInvoke();
                DestroyImmediate(this);
                return;
            }
            if (this.GetComponent<Canvas>())
            {
                if (this.GetComponent<Canvas>().isRootCanvas)
                {
                    CancelInvoke();
                    DestroyImmediate(this);
                    return;
                }
            }
            if (this.GetType().Name == Utils.CommonUtilities.baseclass)
            {
                CancelInvoke();
                DestroyImmediate(this);
                return;
            }
            Init();
        }
#endif

        //OnCanvasGroupChanged 
        //OnCanvasHierarchyChanged >> Called when the state of the parent Canvas is changed.

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (GetTargets()) NotifyChilds();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            m_rectParent = !EnumUtils.IsObjNull(rectTarget.parent) ? rectTarget.parent.GetComponent<RectTransform>() : null;
            if (!EnumUtils.IsObjNull(m_rectParent))
            {
                rectCanvas = m_rectTarget.root.GetComponent<Canvas>() ? m_rectTarget.root.GetComponent<RectTransform>() : null;
                if (m_useCanvasAsParent)
                {
                    m_rectParent = rectCanvas != null ? rectCanvas : m_rectTarget.root.GetComponent<RectTransform>();
                    m_rectLastParent = m_rectParent;//store last parent
                }
                else
                {
                    fitterParent = m_rectParent.GetComponent<BestFitter>(); //Debug.Log("Parent Fitter");
                    if (m_rectParent.root.GetComponent<Canvas>())//if last parent had root a canvas
                        m_rectLastParent = m_rectParent;//store last parent
                }
            }
            Init();
        }

        private void OnTransformChildrenChanged()
        {
            //Debug.Log("OnTransformChildrenChanged for " + transform.name);
            Invoke(nameof(Init), 0.05f);
        }

        #endregion

    }

}