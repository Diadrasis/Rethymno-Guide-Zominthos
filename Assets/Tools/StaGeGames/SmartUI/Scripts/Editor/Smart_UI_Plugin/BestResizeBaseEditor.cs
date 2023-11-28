//StaGe Games ©2022
using StaGeGames.BestFit.Extens;
using StaGeGames.BestFit.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace StaGeGames.BestFit.EditorSpace
{
    public abstract class BestResizeBaseEditor : Editor
    {
        protected SerializedProperty m_rectTargetProp;
        protected SerializedProperty m_rectParentProp;
        protected SerializedProperty m_rectRealParentProp;
        protected SerializedProperty m_rectChildProp;

        protected SerializedProperty m_isMovableProp;
        protected SerializedProperty m_motionModeProp;

        protected SerializedProperty m_smartMotionProp;
        protected SerializedProperty m_smartMotionIsVisibleProp;

        protected SerializedProperty m_pivotModeProp;
        protected SerializedProperty m_stickModeProp;
        protected SerializedProperty m_stickPositionProp;

        protected SerializedProperty m_resizeModeProp;

        protected SerializedProperty m_IgnoreFitWidthProp;
        protected SerializedProperty m_IgnoreFitHeightProp;

        //parent resize
        protected SerializedProperty m_fParentSizePercentageScaleProp;
        protected SerializedProperty m_bParentScaleLockAxesProp;
        protected SerializedProperty m_fParentSizePercentageScaleWidthProp;
        protected SerializedProperty m_fParentSizePercentageScaleHeightProp;

        protected SerializedProperty m_bIsStaticParentSizeWidthProp;
        protected SerializedProperty m_fStaticParentSizeWidthProp;
        protected SerializedProperty m_bIsStaticParentSizeHeightProp;
        protected SerializedProperty m_fStaticParentSizeHeightProp;

        protected SerializedProperty m_bReduceFinalWidthProp;
        protected SerializedProperty m_bReduceFinalHeightProp;
        protected SerializedProperty m_bReduceWidthStaticProp;
        protected SerializedProperty m_fReducedWidthStaticValueProp;
        protected SerializedProperty m_bReduceHeightStaticProp;
        protected SerializedProperty m_fReducedHeightStaticValueProp;
        protected SerializedProperty m_fReducedWidthPercentageValueProp;
        protected SerializedProperty m_fReducedHeightPercentageValueProp;

        //tetxure size
        protected SerializedProperty m_fTextureSizePercentageScaleValueProp;
        protected SerializedProperty m_bLockTextureSizeXProp;
        protected SerializedProperty m_bLockTextureSizeYProp;
        protected SerializedProperty m_bTextureFitInParentProp;
        protected SerializedProperty m_bTextureEnvelopeParentProp;
        protected SerializedProperty m_bTextureFitInParentIgnoreWidthProp;
        protected SerializedProperty m_bTextureFitInParentShouldReduceProp;
        protected SerializedProperty m_bTextureFitInParentShouldReduceStaticProp;
        protected SerializedProperty m_fTextureFitInParentReduceStaticValueProp;
        protected SerializedProperty m_fTextureFitInParentReducePercentageValueProp;

        //Mock ratio
        protected SerializedProperty m_mockRatioModeProp;
        protected SerializedProperty m_nMockRatioModeIndexProp;
        protected SerializedProperty m_bMockRatioIsPortraitProp;
        protected SerializedProperty m_fMockAspectRatioProp;
        protected SerializedProperty m_nMockRatioWidthProp;
        protected SerializedProperty m_nMockRatioHeightProp;
        protected SerializedProperty m_bMockRatioFitInParentProp;
        protected SerializedProperty m_bMockRatioFitInParentIgnoreWidthProp;
        protected SerializedProperty m_bMockRatioFitInParentShouldReduceProp;
        protected SerializedProperty m_bMockRatioFitInParentShouldReduceStaticProp;
        protected SerializedProperty m_fMockRatioFitInParentReduceStaticValueProp;
        protected SerializedProperty m_fMockRatioFitInParentReducePercentageValueProp;

        //children size
        protected SerializedProperty m_childHorizontalProp;
        protected SerializedProperty m_childVerticalProp;
        protected SerializedProperty m_childWidthIncreaseValueProp;
        protected SerializedProperty m_childHeightIncreaseValueProp;
        protected SerializedProperty m_childSizeMaxSizeLimitToParentSizeProp;
        protected SerializedProperty m_childSizeMaxWidthProp;
        protected SerializedProperty m_childSizeMaxHeightProp;
        protected SerializedProperty m_childSizeMinWidthProp;
        protected SerializedProperty m_childSizeMinHeightProp;

        //static resize
        protected SerializedProperty m_StaticWidthProp, m_StaticHeightProp;

        //initial position

        protected SerializedProperty m_InitialStaticPosProp;
        protected SerializedProperty m_InitialPositionModeProp;
        protected SerializedProperty m_initVectorSelfValuesProp;
        protected SerializedProperty m_initVectorParentValuesProp;

        protected BestResize m_BestResizeTarget;

        protected SerializedProperty m_updateFitProp;
        protected SerializedProperty m_useCanvasAsParentProp;


        protected Color oldColor;
        protected bool hasErrors;

        protected static bool showMotionSettings = true;
        protected static bool showResizeSettings = true;
        protected static bool showExtraSettings;

        protected bool m_HavePropertiesChanged;



        protected virtual void OnEnable()
        {
            m_rectTargetProp = serializedObject.FindProperty("m_rectTarget");
            m_rectParentProp = serializedObject.FindProperty("m_rectParent");
            m_rectRealParentProp = serializedObject.FindProperty("m_rectRealParent");
            m_rectChildProp = serializedObject.FindProperty("m_rectChild");

            //motion
            m_isMovableProp = serializedObject.FindProperty("m_isMovable");
            m_motionModeProp = serializedObject.FindProperty("m_motionMode");
            m_smartMotionProp = serializedObject.FindProperty("m_smartMotion");

            m_pivotModeProp = serializedObject.FindProperty("m_pivotMode");
            m_stickModeProp = serializedObject.FindProperty("m_stickMode");
            m_stickPositionProp = serializedObject.FindProperty("m_stickPosition");

            //resize
            m_resizeModeProp = serializedObject.FindProperty("m_resizeMode");

            //m_IgnoreFitWidthProp
            m_IgnoreFitWidthProp = serializedObject.FindProperty("m_IgnoreFitWidth");
            m_IgnoreFitHeightProp = serializedObject.FindProperty("m_IgnoreFitHeight");

            //parent resize
            m_fParentSizePercentageScaleProp = serializedObject.FindProperty("m_fParentSizePercentageScale");
            m_bParentScaleLockAxesProp = serializedObject.FindProperty("m_bParentScaleLockAxes");
            m_fParentSizePercentageScaleWidthProp = serializedObject.FindProperty("m_fParentSizePercentageScaleWidth");
            m_fParentSizePercentageScaleHeightProp = serializedObject.FindProperty("m_fParentSizePercentageScaleHeight");

            m_bIsStaticParentSizeWidthProp = serializedObject.FindProperty("m_bIsStaticParentSizeWidth");
            m_fStaticParentSizeWidthProp = serializedObject.FindProperty("m_fStaticParentSizeWidth");
            m_bIsStaticParentSizeHeightProp = serializedObject.FindProperty("m_bIsStaticParentSizeHeight");
            m_fStaticParentSizeHeightProp = serializedObject.FindProperty("m_fStaticParentSizeHeight");

            m_bReduceFinalWidthProp = serializedObject.FindProperty("m_bReduceFinalWidth");
            m_bReduceFinalHeightProp = serializedObject.FindProperty("m_bReduceFinalHeight");
            m_bReduceWidthStaticProp = serializedObject.FindProperty("m_bReduceWidthStatic");
            m_fReducedWidthStaticValueProp = serializedObject.FindProperty("m_fReducedWidthStaticValue");
            m_bReduceHeightStaticProp = serializedObject.FindProperty("m_bReduceHeightStatic");
            m_fReducedHeightStaticValueProp = serializedObject.FindProperty("m_fReducedHeightStaticValue");
            m_fReducedWidthPercentageValueProp = serializedObject.FindProperty("m_fReducedWidthPercentageValue");
            m_fReducedHeightPercentageValueProp = serializedObject.FindProperty("m_fReducedHeightPercentageValue");

            //tetxure size
            m_fTextureSizePercentageScaleValueProp = serializedObject.FindProperty("m_fTextureSizePercentageScaleValue");
            m_bLockTextureSizeXProp = serializedObject.FindProperty("m_bLockTextureSizeX");
            m_bLockTextureSizeYProp = serializedObject.FindProperty("m_bLockTextureSizeY");
            m_bTextureFitInParentProp = serializedObject.FindProperty("m_bTextureFitInParent");
            m_bTextureEnvelopeParentProp = serializedObject.FindProperty("m_bTextureEnvelopeParent");
            m_bTextureFitInParentIgnoreWidthProp = serializedObject.FindProperty("m_bTextureFitInParentIgnoreWidth");
            m_bTextureFitInParentShouldReduceProp = serializedObject.FindProperty("m_bTextureFitInParentShouldReduce");
            m_bTextureFitInParentShouldReduceStaticProp = serializedObject.FindProperty("m_bTextureFitInParentShouldReduceStatic");
            //not in use
            m_fTextureFitInParentReduceStaticValueProp = serializedObject.FindProperty("m_fTextureFitInParentReduceStaticValue");
            m_fTextureFitInParentReducePercentageValueProp = serializedObject.FindProperty("m_fTextureFitInParentReducePercentageValue");

            //Mock ratio
            m_mockRatioModeProp = serializedObject.FindProperty("m_mockRatioMode");
            m_nMockRatioModeIndexProp = serializedObject.FindProperty("m_nMockRatioModeIndex");
            m_bMockRatioIsPortraitProp = serializedObject.FindProperty("m_bMockRatioIsPortrait");
            m_fMockAspectRatioProp = serializedObject.FindProperty("m_fMockAspectRatio");
            m_nMockRatioWidthProp = serializedObject.FindProperty("m_nMockRatioWidth");
            m_nMockRatioHeightProp = serializedObject.FindProperty("m_nMockRatioHeight");
            m_bMockRatioFitInParentProp = serializedObject.FindProperty("m_bMockRatioFitInParent");
            m_bMockRatioFitInParentIgnoreWidthProp = serializedObject.FindProperty("m_bMockRatioFitInParentIgnoreWidth");
            m_bMockRatioFitInParentShouldReduceProp = serializedObject.FindProperty("m_bMockRatioFitInParentShouldReduce");
            m_bMockRatioFitInParentShouldReduceStaticProp = serializedObject.FindProperty("m_bMockRatioFitInParentShouldReduceStatic");
            m_fMockRatioFitInParentReduceStaticValueProp = serializedObject.FindProperty("m_fMockRatioFitInParentReduceStaticValue");
            m_fMockRatioFitInParentReducePercentageValueProp = serializedObject.FindProperty("m_fMockRatioFitInParentReducePercentageValue");

            //children size
            m_childHorizontalProp = serializedObject.FindProperty("m_childHorizontal");
            m_childVerticalProp = serializedObject.FindProperty("m_childVertical");
            m_childHeightIncreaseValueProp = serializedObject.FindProperty("m_childHeightIncreaseValue");
            m_childWidthIncreaseValueProp = serializedObject.FindProperty("m_childWidthIncreaseValue");
            m_childSizeMaxSizeLimitToParentSizeProp = serializedObject.FindProperty("m_childSizeMaxSizeLimitToParentSize");
            m_childSizeMaxWidthProp = serializedObject.FindProperty("m_childSizeMaxWidth");
            m_childSizeMaxHeightProp = serializedObject.FindProperty("m_childSizeMaxHeight");
            m_childSizeMinWidthProp = serializedObject.FindProperty("m_childSizeMinWidth");
            m_childSizeMinHeightProp = serializedObject.FindProperty("m_childSizeMinHeight");

            //static resize
            m_StaticWidthProp = serializedObject.FindProperty("m_staticWidth");
            m_StaticHeightProp = serializedObject.FindProperty("m_staticHeight");

            //initial position
            m_InitialStaticPosProp = serializedObject.FindProperty("m_InitialStaticPos");
            m_InitialPositionModeProp = serializedObject.FindProperty("m_InitialPositionMode");
            m_initVectorSelfValuesProp = serializedObject.FindProperty("m_initVectorSelfValues");
            m_initVectorParentValuesProp = serializedObject.FindProperty("m_initVectorParentValues");

            //update fittter
            m_updateFitProp = serializedObject.FindProperty("m_updateFit");
            //use Canvas as parent [if real parent has adapt child size]
            m_useCanvasAsParentProp = serializedObject.FindProperty("m_useCanvasAsParent");

            m_BestResizeTarget = (BestResize)target;

            // Initialize the Event Listener for Undo Events.
            Undo.undoRedoPerformed += OnUndoRedo;
        }        
        protected virtual void OnDisable()
        {
            if (Undo.undoRedoPerformed != null)
                Undo.undoRedoPerformed -= OnUndoRedo;
        }

        protected void GetSmartMotion()
        {
            m_smartMotionProp = serializedObject.FindProperty("m_smartMotion");
            SerializedObject propObj = new SerializedObject(m_smartMotionProp.objectReferenceValue);
            m_smartMotionIsVisibleProp = propObj.FindProperty("isVisible");
        }

        public override void OnInspectorGUI()
        {
            if (m_BestResizeTarget == null) return;

            // Make sure Multi selection only includes SmartResizes.
            if (IsSelectionTypesMixed()) return;

            serializedObject.Update(); 

            InitializeStyles();

            ShowPlayModeWarning();

           // GetAllRectTargets();

            hasErrors = CheckForErrors();

            if (hasErrors)
            {
                EditorGUILayout.Space(20f);
                return;
            }
            EditorGUILayout.Space(5f);

            EditorGUILayout.PropertyField(m_updateFitProp, BF_EditorParams.gc_UpdateFitLabel);
            
            GUILayout.Space(10f);

            ShowGUI(m_BestResizeTarget != null && m_BestResizeTarget.hasChildren && m_BestResizeTarget.resizeMode != EnumUtils.ResizeMode.AdaptChildSize 
                && (m_BestResizeTarget.resizeMode == EnumUtils.ResizeMode.AdaptParentSize || m_BestResizeTarget.resizeMode == EnumUtils.ResizeMode.AdaptMockRatio));
            //best fit children
            if (GUILayout.Button(BF_EditorParams.gc_BestFitChildrenButtonLabel, GUILayout.Width(150f)))
            {
                FitUtils.BestFitChildren(m_BestResizeTarget);
                m_HavePropertiesChanged = true;
            }
            ShowGUI(true);

            GUILayout.Space(10f);

            EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());
            showMotionSettings = GUILayout.Toggle(showMotionSettings, "Motion Settings", "foldout"); //"toolbarbutton");// "foldout");
            EditorGUILayout.EndVertical();

            if (showMotionSettings)
            {                
                DrawMotionMode();
                DrawPivotStickyOptions();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());
            showResizeSettings = GUILayout.Toggle(showResizeSettings, "Resize Settings", "foldout");
            EditorGUILayout.EndVertical();

            if (showResizeSettings)
            {
                EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());
                if (m_BestResizeTarget.rectParent != m_BestResizeTarget.rectCanvas)
                {
                    EditorGUILayout.PropertyField(m_useCanvasAsParentProp, BF_EditorParams.gc_UseCanvasAsParentLabel);
                }
                GUILayout.BeginHorizontal();
                BeginCheck();
                EditorGUILayout.PropertyField(m_IgnoreFitWidthProp, BF_EditorParams.gc_IgnoreFitWidthLabel);
                EditorGUILayout.PropertyField(m_IgnoreFitHeightProp, BF_EditorParams.gc_IgnoreFitHeightLabel);
                if (EndCheck()) m_HavePropertiesChanged = true;
                GUILayout.EndHorizontal();

                ShowResizeOptions();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());
            showExtraSettings = GUILayout.Toggle(showExtraSettings, "Extra Settings", "foldout");
            EditorGUILayout.EndVertical();

            if (showExtraSettings) ShowExtraSettings();

            EditorGUILayout.Space();

            ShowButtonRefreshSmartResize();

            EditorGUILayout.Space(20f);
            SerializedProperty OnResizeComplete = serializedObject.FindProperty("OnResizeComplete"); // <-- UnityEvent
            EditorGUILayout.PropertyField(OnResizeComplete);

            if (Event.current.type == EventType.MouseDown)
            {
               // Debug.Log(GUI.GetNameOfFocusedControl());
                GUI.FocusControl(null);
                ClosePopUpPanels();
            }

            KeepSmartControlScriptFoldOut();

            //CheckRecordStatus();

            //EditorGUILayout.Space();

            if (serializedObject.ApplyModifiedProperties() || m_HavePropertiesChanged)
            {
                m_HavePropertiesChanged = false;
                EditorUtility.SetDirty(target);
                //ForceRefreshTarget(); Debug.Log("ApplyModifiedProperties");
                
                BF_EditorActions.ApplySmartResizeTo(Selection.gameObjects);
            }
        }

        private void DrawPivotStickyOptions()
        {
            if (m_BestResizeTarget == null) return;

            GUILayout.BeginHorizontal();

            GUI.backgroundColor = EditorGUIUtility.isProSkin ? oldColor : Color.gray;

            EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox(), GUILayout.MaxWidth(130f));
            DrawPivotMode();
            GUILayout.Space(5f);
            GUILayout.EndVertical();

            if (!m_isMovableProp.boolValue)
            {
                bool isGuiVisible = !EnumUtils.IsPivotCenterOrNone(m_BestResizeTarget.pivotMode);

                ShowGUI(isGuiVisible);
                EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox(), GUILayout.MaxWidth(130f));
                DrawStickMode(isGuiVisible);
                GUILayout.Space(5f);
                GUILayout.EndVertical();

                isGuiVisible = isGuiVisible && EnumUtils.IsDiagonallyPivoted(m_BestResizeTarget.pivotMode) && m_BestResizeTarget.stickMode == EnumUtils.StickMode.External;

                ShowGUI(isGuiVisible);
                EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox(), GUILayout.MaxWidth(130f));
                DrawStickPosition(isGuiVisible);
                GUILayout.Space(5f);
                GUILayout.EndVertical();
                ShowGUI(true);

            }
            else
            {
                bool isGuiVisible = EnumUtils.IsDiagonallyPivoted(m_BestResizeTarget.pivotMode);

                ShowGUI(isGuiVisible);
                EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox(), GUILayout.MaxWidth(130f));
                DrawMotionDirection(isGuiVisible);
                GUILayout.Space(5f);
                GUILayout.EndVertical();
                ShowGUI(true);
            }

            //if (Event.current.type == EventType.MouseDown)
            //{
            //    ClosePopUpPanels(-1);
            //}

            

            ResetColors();
            GUILayout.EndHorizontal();
        }                     

        private void InitializeStyles()
        {
            oldColor = GUI.backgroundColor;//black
            //SmartEditorParams.StaGeLabel();
            ResetColors();
        }

        public void ResetColors()
        {
            GUI.contentColor = Color.white; GUI.backgroundColor = oldColor;
        }
        
        private void ShowWarningWindow(string title, string msg)
        {
            if (EditorUtility.DisplayDialog(title, msg, "OK")) { }
        }

        #region Checks

        //private void CheckRecordStatus()
        //{
        //    if (!EditorApplication.isPlaying) { Undo.RecordObject(m_SmartResizeTarget, "SmartResize"); }
        //    else { Undo.undoRedoPerformed -= m_SmartResizeTarget.Init; }
        //}

        private bool CheckRectTargets()
        {
            if (m_BestResizeTarget == null) return true;

            if (PrefabUtility.IsPartOfPrefabAsset(m_BestResizeTarget))
            {
                BF_EditorParams.ShowHelpBox("Editing prefab is not allowed! Create a new instance in the scene, custimize and save it as new prefab.", 15f, MessageType.Warning, 2f);
                return true;
            }
            else
            {
                if (m_BestResizeTarget.rectParent == null)
                {
                    BF_EditorParams.ShowHelpBox(BF_EditorParams.msgTargetParentNull, 15f, MessageType.Error);
                    return true;
                }
                else
                {
                    if (m_BestResizeTarget.enabled == false)
                    {
                        GUI.contentColor = Color.cyan;
                        if (GUILayout.Button("ENABLE SCRIPT")) m_BestResizeTarget.enabled = true;
                        GUI.contentColor = Color.white;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasParentError()
        {
            if (m_BestResizeTarget == null) return true;

            if (m_resizeModeProp.enumValueIndex == (int)EnumUtils.ResizeMode.AdaptChildSize) return false;
            if (m_BestResizeTarget.rectParent == null)
            {
                GUI.contentColor = Color.yellow;
                BF_EditorParams.ShowHelpBox(BF_EditorParams.msgTargetParentNotReal, 15, MessageType.Warning);
                ResetColors();
                GUI.backgroundColor = BF_EditorUtils.ColorButtonsImportant(oldColor);
                if (GUILayout.Button("Fix Parent", BF_EditorUtils.StyleWarningButton()))
                {
                    if (m_BestResizeTarget.rectLastParent)
                    {
                        if (m_BestResizeTarget.rectLastParent.root.GetComponent<Canvas>())
                        {
                            m_BestResizeTarget.rectTarget.SetParent(m_BestResizeTarget.rectLastParent);
                            ForceRefreshTarget();
                        }
                    }
                }
                ResetColors();
                return true;
            }
            return false;
        }

        private bool HasScaleError()
        {
            if (m_BestResizeTarget == null) return true;
            //check root
            //check parents
            if (HasParentScaleError(out Transform _parent))
            {
                GUI.contentColor = Color.yellow;
                BF_EditorParams.ShowHelpBox("Parent " + _parent.name + " has wrong scale and final result will be affected.", 15, MessageType.Error);
                ResetColors();
                GUI.backgroundColor = BF_EditorUtils.ColorButtonsImportant(oldColor);
                if (GUILayout.Button("Fix Scale", BF_EditorUtils.StyleWarningButton()))
                {
                    _parent.localScale = _parent.localScale.x < _parent.localScale.y ? Vector3.one * _parent.localScale.x : Vector3.one * _parent.localScale.y;
                    ForceRefreshTarget();
                }

                ResetColors();
                GUILayout.Space(5f);
                return true;
            }
            Vector3 scal = m_BestResizeTarget.transform.localScale;
            if (scal.x == scal.y) return false;
            GUI.contentColor = Color.yellow;
            BF_EditorParams.ShowHelpBox(BF_EditorParams.msgTargetScaleNotOne, 15, MessageType.Error);
            ResetColors();
            GUI.backgroundColor = BF_EditorUtils.ColorButtonsImportant(oldColor);
            if (GUILayout.Button("Fix Scale", BF_EditorUtils.StyleWarningButton()))
            {
                m_BestResizeTarget.transform.localScale = Vector3.one;
                ForceRefreshTarget();
            }
            ResetColors();
            return true;
        }

        private bool HasParentScaleError(out Transform _parent)
        {
            _parent = null;
            if (m_BestResizeTarget == null) return true;

            _parent = m_BestResizeTarget.transform;
            if (!m_BestResizeTarget) return false;
            for (int i = 0; i < 1000; i++)
            {
                _parent = _parent.parent;
                if (_parent == null) break;
                if(_parent.localScale.x != _parent.localScale.y) return true;
            }
            return false;
        }

        private bool CheckRotation()
        {
            if (m_BestResizeTarget == null) return false;

            if (m_BestResizeTarget.transform.localEulerAngles == Vector3.zero) return false;
            //GUI.contentColor = Color.yellow;
            //SmartEditorParams.ShowHelpBox(SmartEditorParams.msgTargetScaleNotOne, 15, MessageType.Warning);
           // ResetColors();
            GUI.backgroundColor = BF_EditorUtils.ColorButtonsImportant(oldColor);
            if (GUILayout.Button("Fix Rotation", BF_EditorUtils.StyleWarningButton()))
            {
                m_BestResizeTarget.transform.localEulerAngles = Vector3.zero;
                m_BestResizeTarget.Init();
            }
            ResetColors();
            return true;
        }

        private bool CheckForErrors()
        {
            if (m_BestResizeTarget == null) return true;

            if (m_BestResizeTarget.gameObject.GetComponents<BestResize>().Length > 1)
            {
                ShowWarningWindow("[ ! ] " + m_BestResizeTarget.gameObject.name, BF_EditorParams.msgSmartResizeOnce);
                DestroyImmediate(m_BestResizeTarget);
                m_BestResizeTarget = null;
                return true;
            }
            if (HasParentError()) return true;
            if (CheckRectTargets()) return true;
            if (HasScaleError()) return true;
            if (CheckRotation()) return true;


            if (!m_BestResizeTarget.GetComponent<MaskableGraphic>())
            {
                if (!m_BestResizeTarget.rectTarget.IsCanvasChild())
                {
                    ShowWarningWindow("Invalid operation.", BF_EditorParams.msgRemoveSmartUIComponent);
                    DestroyImmediate(m_BestResizeTarget);
                    m_BestResizeTarget = null;
                    return true;
                }
            }

            if (m_BestResizeTarget.HasTargetAspectRatioFitter())
            {
                BF_EditorParams.ShowHelpBox("Aspect Ratio Fitter detected and has been Disabled", 15f, MessageType.Error, 2f);
                ResetColors();
                GUI.backgroundColor = BF_EditorUtils.ColorButtonsError(oldColor);
                if (GUILayout.Button("Remove Aspect Ratio Fitter", BF_EditorUtils.StyleErrorButton())) m_BestResizeTarget.RemoveAspectRatioFitter();
                ResetColors();
                return true;
            }

            if (m_BestResizeTarget.HasTargetContentSizeFitter(m_BestResizeTarget.rectParent))
            {
                BF_EditorParams.ShowHelpBox("Content Size Fitter detected on parent and has been Disabled", 15f, MessageType.Error, 2f);
                ResetColors();
                GUI.backgroundColor = BF_EditorUtils.ColorButtonsError(oldColor);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove Content Size Fitter", BF_EditorUtils.StyleErrorButton())) m_BestResizeTarget.RemoveContentSizeFitter(m_BestResizeTarget.rectParent);
                if (GUILayout.Button("Use Canvas as Parent", BF_EditorUtils.StyleWarningButton())) m_BestResizeTarget.UseCanvasAsParent();
                GUILayout.EndHorizontal();
                ResetColors();
                return true;
            }
            
            if (m_BestResizeTarget.HasTargetContentSizeFitter(m_BestResizeTarget.rectTarget))
            {
                BF_EditorParams.ShowHelpBox("Content Size Fitter detected and has been Disabled", 15f, MessageType.Error, 2f);
                ResetColors();
                GUI.backgroundColor = BF_EditorUtils.ColorButtonsError(oldColor);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove Content Size Fitter", BF_EditorUtils.StyleErrorButton())) m_BestResizeTarget.RemoveContentSizeFitter(m_BestResizeTarget.rectTarget);
                if (GUILayout.Button("Use Canvas as Parent", BF_EditorUtils.StyleWarningButton())) m_BestResizeTarget.UseCanvasAsParent();
                GUILayout.EndHorizontal();
                ResetColors();
                return true;
            }

            if (m_BestResizeTarget.HasTargetLayoutElement())
            {
                BF_EditorParams.ShowHelpBox("Layout Element detected and has been Disabled", 15f, MessageType.Error, 2f);
                ResetColors();
                GUI.backgroundColor = BF_EditorUtils.ColorButtonsError(oldColor);
                if (GUILayout.Button("Remove Layout Element", BF_EditorUtils.StyleErrorButton())) m_BestResizeTarget.RemoveLayoutElement();
                ResetColors();
                return true;
            }

            return false;
        }

        #endregion

        protected void ShowGUI(bool val) { GUI.enabled = val; }
        protected void BeginCheck() { EditorGUI.BeginChangeCheck(); }
        protected bool EndCheck() { return EditorGUI.EndChangeCheck(); }

        /// <summary>
        /// clamps between 0f - 10000f
        /// </summary>
        protected void DrawFloatStaticValue(SerializedProperty prop, GUIContent cont = null, float resetVal = 0f)
        {
            GUILayout.BeginHorizontal();

            BeginCheck();
            EditorGUILayout.PropertyField(prop, cont == null ? BF_EditorParams.gc_StaticValueLabel : cont, BF_EditorUtils.GLO_Numb(EditorGUIUtility.labelWidth));
            if (EndCheck()) prop.floatValue = prop.floatValue.ClampMe();

            if (prop.floatValue != resetVal)
            {
                if (BF_EditorUtils.IsButtonReset())
                {
                    prop.floatValue = resetVal;
                    m_HavePropertiesChanged = true;
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// clamps between 0 - _maxVal
        /// </summary>
        protected void DrawFloatPercentageValue(SerializedProperty prop, float resetVal, float _maxVal, GUIContent cont = null)
        {
            GUILayout.BeginHorizontal();

            BeginCheck();
            EditorGUILayout.PropertyField(prop, cont == null ? BF_EditorParams.gc_PercentageValueLabel : cont, BF_EditorUtils.GLO_Numb(EditorGUIUtility.labelWidth));
            if (EndCheck()) prop.floatValue = prop.floatValue.ClampMeCustom(0f, _maxVal);

            if (prop.floatValue != resetVal)
            {
                if (BF_EditorUtils.IsButtonReset())
                {
                    prop.floatValue = resetVal;
                    m_HavePropertiesChanged = true;
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// clamps between _minVal - _maxVal
        /// </summary>
        protected void DrawFloatCustomValue(SerializedProperty prop, float resetVal, float _minVal, float _maxVal, GUIContent cont = null)
        {
            GUILayout.BeginHorizontal();

            BeginCheck();
            EditorGUILayout.PropertyField(prop, cont == null ? BF_EditorParams.gc_PercentageValueLabel : cont, BF_EditorUtils.GLO_Numb(EditorGUIUtility.labelWidth));
            if (EndCheck()) prop.floatValue = prop.floatValue.ClampMeCustom(_minVal, _maxVal);

            if (prop.floatValue != resetVal)
            {
                if (BF_EditorUtils.IsButtonReset())
                {
                    prop.floatValue = resetVal;
                    m_HavePropertiesChanged = true;
                }
            }
            GUILayout.EndHorizontal();
        }

        protected void DrawIntegerCustomValue(SerializedProperty prop, int resetVal, int _minVal, int _maxVal, GUIContent cont = null)
        {
            GUILayout.BeginHorizontal();

            BeginCheck();
            EditorGUILayout.PropertyField(prop, cont == null ? BF_EditorParams.gc_PercentageValueLabel : cont, BF_EditorUtils.GLO_Numb(EditorGUIUtility.labelWidth));
            if (EndCheck()) prop.intValue = prop.intValue.ClampMeCustom(_minVal, _maxVal);

            if (prop.intValue != resetVal)
            {
                if (BF_EditorUtils.IsButtonReset())
                {
                    prop.intValue = resetVal;
                    m_HavePropertiesChanged = true;
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ShowPlayModeWarning()
        {
            if (Application.isEditor)
            {
                if (EditorApplication.isPlaying)
                {
                    GUI.contentColor = Color.yellow;
                    BF_EditorParams.ShowHelpBox(BF_EditorParams.msgPlayMode, 15f, MessageType.Warning);
                    ResetColors();
                }
            }
        }

        protected Vector2 InitialStaticPos { get { return m_BestResizeTarget.initialStaticPos; } set { m_BestResizeTarget.initialStaticPos = value; } }

        protected void DrawSizeInfo()
        {
            EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());
            BF_EditorParams.ShowLabel(string.Format("Parent size: {0} x {1}", m_BestResizeTarget.parentSize.x, m_BestResizeTarget.parentSize.y), 0f, Color.white);
            GUILayout.Space(-15);
            BF_EditorParams.ShowLabel(string.Format("Target size: {0} x {1}", m_BestResizeTarget.targetSize.x, m_BestResizeTarget.targetSize.y), 0f, Color.cyan);
            if (m_IgnoreFitWidthProp.boolValue) { GUILayout.Space(-15); BF_EditorParams.ShowLabel("Width is being ignored", 0f, Color.yellow); }
            if (m_IgnoreFitHeightProp.boolValue) { GUILayout.Space(-15); BF_EditorParams.ShowLabel("Height is being ignored", 0f, Color.yellow); }
            GUILayout.EndVertical();
        }

        protected abstract bool IsSelectionTypesMixed();
        protected abstract void OnUndoRedo();
        protected abstract void DrawMotionMode();
        protected abstract void DrawPivotMode();
        protected abstract void DrawStickMode(bool isVisible);
        protected abstract void DrawStickPosition(bool isVisible);
        protected abstract void DrawMotionDirection(bool isVisible);
        protected abstract void ForceRefreshTarget();
        protected abstract void ClosePopUpPanels(int val = -1);
        protected abstract void ShowResizeOptions();
        protected abstract void DrawAdaptParentSize();
        protected abstract void DrawAdaptChildSize();
        protected abstract void DrawAdaptTextureSize();
        protected abstract void DrawAdaptMockRatio();
        protected abstract void DrawAdaptStaticSize();
        protected abstract void ShowExtraSettings();
        protected abstract void KeepSmartControlScriptFoldOut();
        protected abstract void ShowButtonRefreshSmartResize();

        protected abstract void CheckIsMovable();

    }

}
