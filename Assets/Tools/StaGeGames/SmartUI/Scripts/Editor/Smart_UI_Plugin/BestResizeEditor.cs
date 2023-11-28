//StaGe Games ©2021
using StaGeGames.BestFit.Extens;
using StaGeGames.BestFit.Utils;
using UnityEditor;
using UnityEngine;
using EditorUtils = StaGeGames.BestFit.EditorSpace.BF_EditorUtils;

namespace StaGeGames.BestFit.EditorSpace
{
    [CustomEditor(typeof(BestResize), true), CanEditMultipleObjects]
    public class BestResizeEditor : BestResizeBaseEditor
    {
        private SmartMotion myMotion;
        private bool hasUnnecessaryMotionScript = false;
        private bool allowInactive;
        private bool showReduceSizePanel;
        private bool showTexturePreviewPanel;

        private GUIContent guiContentPivotSelected;
        private bool isPivotSelectionVisible;
        private GUIContent guiContentStickModeSelected;
        private bool isStickModeSelectionVisible;
        private GUIContent guiContentStickPositionSelected;
        private bool isStickPositionSelectionVisible;
        private GUIContent guiContentIsMovable, guiContentIsVisible, guiContentShowHide;
        private GUIContent guiContentMotionDirectionSelected;
        private bool isMotionDirectionSelectionVisible;

        protected override void OnEnable()
        {
            base.OnEnable();

            InitializeValues();

            CheckIsMovable();

            ForceRefreshTarget();
        }

        private void SaveChangesIfAny(SerializedObject so)
        {
            if (GUI.changed)
            {
                if (m_BestResizeTarget == null) return;
                ForceRefreshTarget(); Debug.Log("ForceRefreshTarget 1");
                so.ApplyModifiedProperties();

                Debug.Log("SaveChangesIfAny");
            }
        }

        //private void ShowSettingsTabs()
        //{
        //    ResetColors();

        //    settingsTab = GUILayout.Toolbar(settingsTab, new string[] { "Resize Options", "Position Options" });//, (GUIStyle)"FoldoutPreDrop", GUILayout.Height(20f));

        //    switch (settingsTab)
        //    {
        //        case 0:
        //            ShowResizeOptions();
        //            break;
        //        case 1:
        //            ShowPositionOptions();
        //            break;
        //        default:
        //            ShowResizeOptions();
        //            break;
        //    }
        //}

        protected override void ShowResizeOptions()
        {
            #region RESIZE SETTINGS

            EditorGUILayout.Space();

            BeginCheck();
            EditorGUILayout.PropertyField(m_resizeModeProp, BF_EditorParams.gc_ResizeModeLabel);
            if (EndCheck())  m_HavePropertiesChanged = true;

            EditorGUILayout.Space();

            switch (m_resizeModeProp.enumValueIndex)
            {
                case (int)EnumUtils.ResizeMode.None:
                default:
                    break;
                case (int)EnumUtils.ResizeMode.Static:
                    DrawAdaptStaticSize();
                    break;
                case (int)EnumUtils.ResizeMode.AdaptParentSize:
                    DrawAdaptParentSize();
                    break;
                case (int)EnumUtils.ResizeMode.AdaptTextureSize:
                    DrawAdaptTextureSize();
                    break;
                case (int)EnumUtils.ResizeMode.AdaptMockRatio:
                    DrawAdaptMockRatio();
                    break;
                case (int)EnumUtils.ResizeMode.AdaptChildSize:
                    DrawAdaptChildSize();
                    break;
            }

            EditorGUILayout.Space();

            #endregion
        }

        protected override void ShowButtonRefreshSmartResize()
        {
            GUILayout.BeginVertical("[Editor] Functions", "window");

            GUILayout.Space(10f);

            allowInactive = EditorGUILayout.Toggle("Include Inactive", allowInactive);

            GUILayout.Space(10f);

            GUI.backgroundColor = EditorGUIUtility.isProSkin ? oldColor : EditorUtils.HexColor("#575757", Color.gray);

            GUIStyle style = new GUIStyle(EditorStyles.textField);
            style.fontSize = 12;
            style.fontStyle = FontStyle.Normal;
            style.normal.textColor = allowInactive ? Color.cyan : EditorGUIUtility.isProSkin ? Color.white : Color.white;
            style.hover.textColor = Color.yellow;
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = 150;

            #region deprecated

            //GUILayout.BeginHorizontal();

            //if (EditorTools.EditorActions.GetChildSmartResizes(m_SmartResizeTarget.targetRect, allowInactive))
            //{
            //    if (GUILayout.Button("Resize Target Childs", style))
            //    {
            //        string msg = allowInactive ? EditorParams.msgSmartResizeInactiveWarning : EditorParams.msgSmartResizeWarning;
            //        if (EditorUtility.DisplayDialog("Apply Smart Resize to child items of this target", msg + "\n\nAre you sure?", "Apply", "Cancel"))
            //        {
            //            EditorTools.EditorActions.ApplyParentSmartResize(m_SmartResizeTarget.targetParentRect, allowInactive);
            //        }
            //    }
            //}
            //else
            //{
            //    if (GUILayout.Button("Resize Target", style))
            //    {
            //        ForceRefreshTarget();
            //    }
            //}

            //GUILayout.Space(10f);

            //if (GUILayout.Button("Resize Target Parent", style))
            //{
            //    string msg = allowInactive ? EditorParams.msgSmartResizeInactiveWarning : EditorParams.msgSmartResizeWarning;
            //    if (EditorUtility.DisplayDialog("Apply Smart Resize to child items of this Parent", msg + "\n\nAre you sure?", "Apply", "Cancel"))
            //    {
            //        EditorTools.EditorActions.ApplyParentSmartResize(m_SmartResizeTarget.targetParentRect, allowInactive);
            //    }
            //}

            //GUILayout.EndHorizontal();
            //GUILayout.Space(10f);

            #endregion

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh", style))
            {
                //ForceRefreshTarget();
                if (m_BestResizeTarget) m_BestResizeTarget.Init();
            }
            GUILayout.Space(10f);

            if (GUILayout.Button("Refresh root Canvas", style))
            {
                // string msg = allowInactive ? SmartEditorParams.msgSmartResizeInactiveWarning : SmartEditorParams.msgSmartResizeWarning;
                //  if (EditorUtility.DisplayDialog("Applies Smart Resize to ALL items of this Canvas", msg + "\n\nAre you sure?", "Apply", "Cancel"))
                //  {
                if (m_BestResizeTarget == null) return;
                Canvas canv = m_BestResizeTarget.rectTarget.transform.root.GetComponent<Canvas>();
                if (canv) EditorSpace.BF_EditorActions.ApplySmartResizeCanvas(canv, allowInactive);
                //  }
            }
            GUILayout.Space(10f);

            if (GUILayout.Button("Refresh All Canvases", style))
            {
                //string msg = allowInactive ? SmartEditorParams.msgSmartResizeInactiveWarning : SmartEditorParams.msgSmartResizeWarning;
                // if (EditorUtility.DisplayDialog("Apply Smart Resize to ALL items of every Canvas in the Scene", msg + "\n\nAre you sure?", "Apply", "Cancel"))
                //  {
                EditorSpace.BF_EditorActions.ApplySmartResizeAllCanvases(allowInactive);
                //  }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.EndVertical();

            ResetColors();
        }

        /// <summary>
        /// keeps SmartControl script expanded in inspector to visualize control panel in scene view
        /// </summary>
        protected override void KeepSmartControlScriptFoldOut()
        {
            if (m_BestResizeTarget == null) return;
            if (myMotion != null)
            {
                SmartControl smartControl = m_BestResizeTarget.GetComponent<SmartControl>();
                if (smartControl)
                    UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(smartControl, true);
            }
        }               

        // Method to handle multi object selection
        protected override bool IsSelectionTypesMixed()
        {
            GameObject[] objects = Selection.gameObjects;
            if (objects.Length > 1)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].GetComponent<BestResize>() == null)
                        return true;
                }
            }
            return false;
        }

        protected override void OnUndoRedo()
        {
           /* ForceRefreshTarget();*/ //Debug.Log("OnUndoRedo");
            BF_EditorActions.ApplySmartResizeTo(Selection.gameObjects);

            InitializeValues();
        }

        protected override void ForceRefreshTarget()
        {
            //Debug.Log("ForceRefreshTarget");
            //if (m_SmartResizeTarget) m_SmartResizeTarget.Init();
        }

        void InitializeValues()
        {
            //Debug.Log("InitializeValues");
            if (m_BestResizeTarget == null) return;

            showTexturePreviewPanel = false;
            
            guiContentPivotSelected = SUP_UIStylesManager.GuiTexSelected(m_BestResizeTarget.pivotMode);
            guiContentStickModeSelected = SUP_UIStylesManager.GuiTextureStickModeSelected(m_BestResizeTarget.stickMode);
            guiContentStickPositionSelected = SUP_UIStylesManager.GuiTextureStickPositionSelected(m_BestResizeTarget.stickPosition);

            guiContentIsMovable = SUP_UIStylesManager.GuiTextureMovable(m_isMovableProp.boolValue);// (m_SmartResizeTarget.isMovable);
            guiContentIsVisible = SUP_UIStylesManager.GuiTextureVisible(m_BestResizeTarget.isVisibleOnStart);
            if (m_BestResizeTarget.smartMotion)
            {
                guiContentShowHide = SUP_UIStylesManager.GuiTextureShowHide(m_BestResizeTarget.smartMotion.isVisible);
            }
            m_BestResizeTarget.CheckComponetsChanges();
        }

        #region Pivot Mode

        protected override void DrawPivotMode()
        {
            if (m_BestResizeTarget == null) return;

            EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), "Pivot Mode", BF_EditorUtils.StyleToolbarButton());
            GUILayout.BeginHorizontal(BF_EditorUtils.StyleSelection());

            if (GUILayout.Button(guiContentPivotSelected, BF_EditorUtils.GLO_Button))
            {
                ClosePopUpPanels(0);
                isPivotSelectionVisible = !isPivotSelectionVisible;
            }

            EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), m_BestResizeTarget.pivotMode.ToString());

            GUILayout.EndHorizontal();

            if (isPivotSelectionVisible)
            {
                GUILayout.BeginHorizontal();

                #region column A
                GUILayout.BeginVertical();
                for (int i = 0; i < 3; i++)
                {
                    if (SUP_UIStylesManager.PivotColA[i] == m_BestResizeTarget.pivotMode)
                    {
                        if (GUILayout.Button(new GUIContent(), BF_EditorUtils.GLO_Button))
                        {
                            isPivotSelectionVisible = false;
                            return;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(SUP_UIStylesManager.pivotContentA[i], BF_EditorUtils.GLO_Button))
                        {
                            SetPivot(SUP_UIStylesManager.PivotColA[i]);
                            return;
                        }
                    }
                }
                GUILayout.EndVertical();
                #endregion

                #region column B
                GUILayout.BeginVertical();
                for (int i = 0; i < 3; i++)
                {
                    if (SUP_UIStylesManager.PivotColB[i] == m_BestResizeTarget.pivotMode)
                    {
                        if (GUILayout.Button(new GUIContent(), BF_EditorUtils.GLO_Button))
                        {
                            isPivotSelectionVisible = false;
                            return;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(SUP_UIStylesManager.pivotContentB[i], BF_EditorUtils.GLO_Button))
                        {
                            SetPivot(SUP_UIStylesManager.PivotColB[i]);
                            return;
                        }
                    }
                }
                GUILayout.EndVertical();
                #endregion

                #region column C
                GUILayout.BeginVertical();
                for (int i = 0; i < 3; i++)
                {
                    if (SUP_UIStylesManager.PivotColC[i] == m_BestResizeTarget.pivotMode)
                    {
                        if (GUILayout.Button(new GUIContent(), BF_EditorUtils.GLO_Button))
                        {
                            isPivotSelectionVisible = false;
                            return;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(SUP_UIStylesManager.pivotContentC[i], BF_EditorUtils.GLO_Button))
                        {
                            SetPivot(SUP_UIStylesManager.PivotColC[i]);
                            return;
                        }
                    }
                }
                GUILayout.EndVertical();
                #endregion

                #region column D
                GUILayout.BeginVertical();
                if (m_BestResizeTarget.pivotMode == EnumUtils.PivotPoint.None)
                {
                    if (GUILayout.Button(new GUIContent(), BF_EditorUtils.GLO_Button))
                    {
                        isPivotSelectionVisible = false;
                        return;
                    }
                }
                else
                {
                    if (GUILayout.Button(SUP_UIStylesManager.contPivotNone, BF_EditorUtils.GLO_Button))
                    {
                        //m_SmartResizeTarget.pivotMode = SmartUtilities.PivotMode.None;
                        m_pivotModeProp.SetEnumValue(EnumUtils.PivotPoint.None);
                        guiContentPivotSelected = SUP_UIStylesManager.contPivotNone;
                        return;
                    }
                }
                GUILayout.EndVertical();
                #endregion

                GUILayout.EndHorizontal();

            }

        }

        private void SetPivot(EnumUtils.PivotPoint pivot)
        {
            ClosePopUpPanels(0);
            guiContentPivotSelected = SUP_UIStylesManager.GuiTexSelected(pivot);
            m_pivotModeProp.SetEnumValue(pivot);
        }

        #endregion

        #region Stick Mode
        protected override void DrawStickMode(bool isVisible)
        {
            if (m_BestResizeTarget == null) return;

            if (m_BestResizeTarget.stickMode == EnumUtils.StickMode.Free) m_BestResizeTarget.stickMode = EnumUtils.StickMode.Internal;
            guiContentStickModeSelected = isVisible ? SUP_UIStylesManager.GuiTextureStickModeSelected(m_BestResizeTarget.stickMode) : SUP_UIStylesManager.contEmpty;

            string boxStyle = "selectionRect";
            if (!isVisible)
            {
                if (m_BestResizeTarget.pivotMode == EnumUtils.PivotPoint.None)
                {
                    guiContentStickModeSelected = SUP_UIStylesManager.contEmpty;
                    m_BestResizeTarget.stickMode = EnumUtils.StickMode.Free;
                }
                else
                {
                    m_BestResizeTarget.stickMode = EnumUtils.StickMode.Internal;
                }
                boxStyle = "ProgressBarBack";
            }
            guiContentStickModeSelected = SUP_UIStylesManager.GuiTextureStickModeSelected(m_BestResizeTarget.stickMode);

            EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), "Sticky Mode", BF_EditorUtils.StyleToolbarButton());
            GUILayout.BeginHorizontal((GUIStyle)boxStyle);// "selectionRect");

            if (GUILayout.Button(guiContentStickModeSelected, BF_EditorUtils.GLO_Button))
            {
                ClosePopUpPanels(1);
                isStickModeSelectionVisible = !isStickModeSelectionVisible;
            }

            EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), m_BestResizeTarget.stickMode.ToString());

            GUILayout.EndHorizontal();

            if (isStickModeSelectionVisible)
            {
                GUILayout.BeginHorizontal();

                #region column A
                GUILayout.BeginHorizontal();
                for (int i = 0; i < SUP_UIStylesManager.stickModeContentA.Length; i++)
                {
                    if (SUP_UIStylesManager.StickModeColA[i] != m_BestResizeTarget.stickMode)
                    {
                        if (GUILayout.Button(SUP_UIStylesManager.stickModeContentA[i], BF_EditorUtils.GLO_Button))
                        {
                            SetStickMode(SUP_UIStylesManager.StickModeColA[i]);
                            return;
                        }
                        EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), SUP_UIStylesManager.StickModeColA[i].ToString());
                    }
                }
                GUILayout.EndHorizontal();
                #endregion


                //GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

            }
        }

        private void SetStickMode(EnumUtils.StickMode stickMode)
        {
            ClosePopUpPanels(1);
            guiContentStickModeSelected = SUP_UIStylesManager.GuiTextureStickModeSelected(stickMode);
            m_stickModeProp.SetEnumValue(stickMode);
        }

        #endregion

        #region Stick Position
        protected override void DrawStickPosition(bool isVisible)
        {
            int styleIndex = 1;
            if (isVisible)
            {
                if (m_stickPositionProp.enumValueIndex == (int)EnumUtils.MotionMode.Free) m_stickPositionProp.SetEnumValue(EnumUtils.MotionMode.Horizontally);
                guiContentStickPositionSelected = SUP_UIStylesManager.GuiTextureStickPositionSelected(m_BestResizeTarget.stickPosition);
            }
            else
            {
                if (m_BestResizeTarget.stickMode == EnumUtils.StickMode.Internal)
                {
                    guiContentStickPositionSelected = SUP_UIStylesManager.contEmpty;
                    m_stickPositionProp.SetEnumValue(EnumUtils.MotionMode.Free);
                    styleIndex = 2;
                }
                else
                {
                    if (EnumUtils.IsPivotHorizontalCenter(m_BestResizeTarget.pivotMode))
                    {
                        guiContentStickPositionSelected = SUP_UIStylesManager.GuiTextureStickPositionSelected(EnumUtils.MotionMode.Horizontally);
                        m_stickPositionProp.SetEnumValue(EnumUtils.MotionMode.Horizontally);
                        styleIndex = 3;
                    }
                    else if (EnumUtils.IsPivotVerticalCenter(m_BestResizeTarget.pivotMode))
                    {
                        guiContentStickPositionSelected = SUP_UIStylesManager.GuiTextureStickPositionSelected(EnumUtils.MotionMode.Vertically);
                        m_stickPositionProp.SetEnumValue(EnumUtils.MotionMode.Vertically);
                        styleIndex = 3;
                    }
                    else
                    {
                        guiContentStickPositionSelected = SUP_UIStylesManager.contEmpty;
                        m_stickPositionProp.SetEnumValue(EnumUtils.MotionMode.Free);
                        styleIndex = 2;
                    }
                }
            }

            EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), "Sticky Position", BF_EditorUtils.StyleToolbarButton());
            GUILayout.BeginHorizontal(BF_EditorUtils.StyleOptions(styleIndex));

            if (GUILayout.Button(guiContentStickPositionSelected, BF_EditorUtils.GLO_Button))
            {
                ClosePopUpPanels(2);
                isStickPositionSelectionVisible = !isStickPositionSelectionVisible;
            }

            EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), m_BestResizeTarget.stickPosition.ToString());

            GUILayout.EndHorizontal();


            if (isStickPositionSelectionVisible)
            {
                GUILayout.BeginVertical();

                #region column A
                //GUILayout.BeginHorizontal();
                for (int i = 0; i < SUP_UIStylesManager.stickPositionContentA.Length; i++)
                {
                    if (SUP_UIStylesManager.StickPositionColA[i] != m_BestResizeTarget.stickPosition)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(SUP_UIStylesManager.stickPositionContentA[i], BF_EditorUtils.GLO_Button))
                        {
                            SetStickPosition(SUP_UIStylesManager.StickPositionColA[i]);
                            return;
                        }
                        EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), SUP_UIStylesManager.StickPositionColA[i].ToString());
                        GUILayout.EndHorizontal();
                    }
                }
                //GUILayout.EndHorizontal();
                #endregion

                GUILayout.EndVertical();

            }
        }

        private void SetStickPosition(EnumUtils.MotionMode motionMode)
        {
            ClosePopUpPanels(2);
            guiContentStickPositionSelected = SUP_UIStylesManager.GuiTextureStickPositionSelected(motionMode);
            //m_SmartResizeTarget.stickPosition = motionMode;
            m_stickPositionProp.SetEnumValue(motionMode);
        }

        #endregion

        protected override void ClosePopUpPanels(int ignorePanel = -1)
        {
            if (ignorePanel != 0) isPivotSelectionVisible = false;
            if (ignorePanel != 1) isStickModeSelectionVisible = false;
            if (ignorePanel != 2) isStickPositionSelectionVisible = false;
            if (ignorePanel != 3) isMotionDirectionSelectionVisible = false;
        }

        #region Draw Motion Mode - Direction
        void DrawButtonRemoveMotionScript()
        {
            ResetColors();
            GUI.backgroundColor = BF_EditorUtils.ColorButtonsImportant(oldColor);
            if (GUILayout.Button("Remove Smart Motion?", BF_EditorUtils.StyleWarningButton()))
            {
                if (myMotion != null) DestroyImmediate(myMotion);
                hasUnnecessaryMotionScript = false;
                if (m_BestResizeTarget.GetComponent<SmartControl>()) DestroyImmediate(m_BestResizeTarget.GetComponent<SmartControl>());
            }
            ResetColors();
        }

        protected override void DrawMotionMode()
        {
            #region MOTION SETTINGS

            GUI.backgroundColor = EditorGUIUtility.isProSkin ? oldColor : Color.gray;

            EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());//, GUILayout.MinWidth(200f));

            bool useMotion = !EnumUtils.IsPivotCenter(m_pivotModeProp.enumValueIndex);

            if (!useMotion && m_isMovableProp.boolValue)
            {
                m_isMovableProp.boolValue = useMotion;
                CheckIsMovable();
            }
            ShowGUI(useMotion);

            GUILayout.BeginHorizontal(m_isMovableProp.boolValue ? BF_EditorUtils.StyleSelection() : GUIStyle.none);

            if (GUILayout.Button(guiContentIsMovable, BF_EditorUtils.GLO_Button))
            {
                ClosePopUpPanels(-1);
                m_isMovableProp.boolValue = !m_isMovableProp.boolValue;
                guiContentIsMovable = SUP_UIStylesManager.GuiTextureMovable(m_isMovableProp.boolValue);// (m_SmartResizeTarget.isMovable);
                CheckIsMovable();
            }

            string moveTitle = m_isMovableProp.boolValue ? "Movable" : "Static";
            EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), moveTitle);


            if (hasUnnecessaryMotionScript)
            {
                ShowGUI(true);
                DrawButtonRemoveMotionScript();
            }
            else
            {
                //ShowGUI(!SmartUtilities.IsPivotCenterOrNone(m_SmartResizeTarget.pivotMode));

                if (m_isMovableProp.boolValue)
                {
                    if (GUILayout.Button(guiContentIsVisible, BF_EditorUtils.GLO_Button))
                    {
                        ClosePopUpPanels();
                        m_BestResizeTarget.isVisibleOnStart = !m_BestResizeTarget.isVisibleOnStart;
                        guiContentIsVisible = SUP_UIStylesManager.GuiTextureVisible(m_BestResizeTarget.isVisibleOnStart);

                    }

                    moveTitle = m_BestResizeTarget.isVisibleOnStart ? "Visible\nat Start" : "Hidden\nat Start";
                    EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), moveTitle);

                    if (m_BestResizeTarget.smartMotion)
                    {
                        guiContentShowHide = SUP_UIStylesManager.GuiTextureShowHide(m_BestResizeTarget.smartMotion.isVisible);
                        if (GUILayout.Button(guiContentShowHide, BF_EditorUtils.GLO_Button))
                        {
                            ClosePopUpPanels();
                            m_BestResizeTarget.smartMotion.TogglePanelAppearance();
                        }
                        moveTitle = m_BestResizeTarget.smartMotion.isVisible ? "Hide" : "Show";
                        EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), moveTitle);
                    }
                }
            }

            GUILayout.Space(10f);
            GUILayout.EndHorizontal();

            //GUILayout.FlexibleSpace();
           // GUILayout.Space(5f);

            GUILayout.EndVertical();

            ResetColors();

            ShowGUI(true);

            #endregion
        }

        protected override void CheckIsMovable()
        {

            if (myMotion == null) myMotion = m_BestResizeTarget.GetComponent<SmartMotion>();

            if (!m_isMovableProp.boolValue)
            {
                if (myMotion != null)
                {
                    hasUnnecessaryMotionScript = true;
                    myMotion.isInEditorAlive = false;
                }
            }
            else
            {
                if (myMotion == null) myMotion = m_BestResizeTarget.gameObject.AddComponent<SmartMotion>();

                if (myMotion != null)
                {
                    myMotion.isInEditorAlive = true;
                }
                hasUnnecessaryMotionScript = false;
            }
        }

        /// <summary>
        /// <para>the motion direction of rect target</para>
        /// <para>is visible only if pivot is at any corner</para>
        /// </summary>
        /// <param name="isVisible"></param>
        protected override void DrawMotionDirection(bool isVisible)
        {
            int styleIndex = 1;
            if (isVisible)
            {
                if (m_motionModeProp.enumValueIndex == (int)EnumUtils.MotionMode.Free) m_motionModeProp.SetEnumValue(EnumUtils.MotionMode.Horizontally);
            }
            else
            {
                if (EnumUtils.IsPivotHorizontalCenter(m_pivotModeProp.enumValueIndex))
                {
                    m_motionModeProp.SetEnumValue(EnumUtils.MotionMode.Horizontally);
                    styleIndex = 3;
                }
                else if (EnumUtils.IsPivotVerticalCenter(m_pivotModeProp.enumValueIndex))
                {
                    m_motionModeProp.SetEnumValue(EnumUtils.MotionMode.Vertically);
                    styleIndex = 3;
                }
                else
                {
                    m_motionModeProp.SetEnumValue(EnumUtils.MotionMode.Free);
                    styleIndex = 2;
                }
            }
            guiContentMotionDirectionSelected = SUP_UIStylesManager.GuiTextureMotionDirectionSelected((EnumUtils.MotionMode)m_motionModeProp.enumValueIndex);

            EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), "Motion Direction", BF_EditorUtils.StyleToolbarButton());

            GUILayout.BeginHorizontal(BF_EditorUtils.StyleOptions(styleIndex));

            if (GUILayout.Button(guiContentMotionDirectionSelected, BF_EditorUtils.GLO_Button))
            {
                ClosePopUpPanels(3);
                isMotionDirectionSelectionVisible = !isMotionDirectionSelectionVisible;
            }

            EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), ((EnumUtils.MotionMode)m_motionModeProp.enumValueIndex).ToString());

            GUILayout.EndHorizontal();


            if (isMotionDirectionSelectionVisible)
            {
                #region column A
                GUILayout.BeginVertical();
                for (int i = 0; i < SUP_UIStylesManager.stickPositionContentA.Length; i++)
                {
                    if (SUP_UIStylesManager.StickPositionColA[i] != (EnumUtils.MotionMode)m_motionModeProp.enumValueIndex)
                    {
                        GUILayout.BeginHorizontal();
                        
                        if (GUILayout.Button(SUP_UIStylesManager.stickPositionContentA[i], BF_EditorUtils.GLO_Button))
                        {
                            SetMotionDirection(SUP_UIStylesManager.StickPositionColA[i]);
                            return;
                        }
                        EditorGUI.LabelField(BF_EditorParams.RectInfoSingleLine(0), SUP_UIStylesManager.StickPositionColA[i].ToString());
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
                #endregion

                if (Event.current.type == EventType.MouseDown)
                {
                    isMotionDirectionSelectionVisible = false;
                }
            }
        }

        private void SetMotionDirection(EnumUtils.MotionMode motionMode)
        {
            ClosePopUpPanels(3);
            guiContentMotionDirectionSelected = SUP_UIStylesManager.GuiTextureStickPositionSelected(motionMode);
            m_motionModeProp.SetEnumValue(motionMode);
        }

        #endregion
                

        #region Size Modes

        protected override void DrawAdaptParentSize()
        {
            #region AdaptParentRatio

            DrawSizeInfo();

            GUILayout.Space(10f);
            //lock axes
            EditorGUILayout.PropertyField(m_bParentScaleLockAxesProp, BF_EditorParams.gc_LockAxesLabel);
            ShowGUI(m_bParentScaleLockAxesProp.boolValue);
            //size [%]
            DrawFloatPercentageValue(m_fParentSizePercentageScaleProp, 100f, 1000f, BF_EditorParams.gc_PercentageScaleLabel);
            ShowGUI(!m_bParentScaleLockAxesProp.boolValue && !m_bIsStaticParentSizeWidthProp.boolValue);
            //width [%]
            DrawFloatPercentageValue(m_fParentSizePercentageScaleWidthProp, 100f,1000f, BF_EditorParams.gc_PercentageScaleWidthLabel);
            ShowGUI(!m_bParentScaleLockAxesProp.boolValue);
            //is static width
            EditorGUILayout.PropertyField(m_bIsStaticParentSizeWidthProp, BF_EditorParams.gc_IsStaticLabel);
            ShowGUI(!m_bParentScaleLockAxesProp.boolValue && m_bIsStaticParentSizeWidthProp.boolValue);
            //static width
            DrawFloatStaticValue(m_fStaticParentSizeWidthProp, BF_EditorParams.gc_StaticWidthLabel, 100f);
            GUILayout.Space(10f);
            ShowGUI(!m_bParentScaleLockAxesProp.boolValue && !m_bIsStaticParentSizeHeightProp.boolValue);
            //height [%]
            DrawFloatPercentageValue(m_fParentSizePercentageScaleHeightProp, 100f, 1000f, BF_EditorParams.gc_PercentageScaleHeightLabel);
            ShowGUI(!m_bParentScaleLockAxesProp.boolValue);
            //is static height
            EditorGUILayout.PropertyField(m_bIsStaticParentSizeHeightProp, BF_EditorParams.gc_IsStaticLabel);
            ShowGUI(!m_bParentScaleLockAxesProp.boolValue && m_bIsStaticParentSizeHeightProp.boolValue);
            //static height
            DrawFloatStaticValue(m_fStaticParentSizeHeightProp, BF_EditorParams.gc_StaticHeightLabel, 100f);
            ShowGUI(true);
            EditorGUILayout.Space();

            #region REDUCE SIZE
            //bool isUsingReduce = (m_bReduceFinalWidthProp.boolValue && (!m_fReducedWidthPercentageValueProp.floatValue.isZero()) && !m_fReducedWidthStaticValueProp.floatValue.isZero()) ||
            //                     (m_bReduceFinalHeightProp.boolValue && (!m_fReducedHeightPercentageValueProp.floatValue.isZero() && !m_fReducedHeightStaticValueProp.floatValue.isZero()));

            bool isUsingReduce = (m_bReduceFinalWidthProp.boolValue && 
                             (m_bReduceWidthStaticProp.boolValue ? !m_fReducedWidthStaticValueProp.floatValue.isZero() : !m_fReducedWidthPercentageValueProp.floatValue.isZero()))
                             || (m_bReduceFinalHeightProp.boolValue && 
                             ( m_bReduceHeightStaticProp.boolValue ? !m_fReducedHeightStaticValueProp.floatValue.isZero() : !m_fReducedHeightPercentageValueProp.floatValue.isZero()));

            GUI.contentColor = isUsingReduce ? Color.cyan : Color.white;
            string _titleToggleReduce = isUsingReduce ? "Reduce [Self] Size Options [In Use]" : "Reduce [Self] Size Options [Not in Use]";

            EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());
            showReduceSizePanel = GUILayout.Toggle(showReduceSizePanel, _titleToggleReduce, "foldout");
            EditorGUILayout.EndVertical();
            ResetColors();

            if (showReduceSizePanel)
            {
                #region SPACE WIDTH

                GUILayout.Space(10f);
                GUILayout.BeginVertical(BF_EditorUtils.StyleBox());// ("Decrease Size Options", "window");

                ShowGUI(false);
                if (m_bReduceFinalWidthProp.boolValue)
                {
                    if (m_bReduceWidthStaticProp.boolValue) { BF_EditorParams.ShowLabel("[ ! ] Final Width = Target width - Static value", 0f, Color.yellow); }
                    else { BF_EditorParams.ShowLabel("[ ! ] Final Width = Target width - [% Target width] value", 0f, Color.yellow); }
                }
                else { BF_EditorParams.ShowLabel("[ ! ] Final Width = Parent width * Percentage Ratio", 0f, Color.yellow); }
                ShowGUI(true);

                EditorGUILayout.PropertyField(m_bReduceFinalWidthProp, BF_EditorParams.gc_ReduceWidthLabel);

                //is static
                ShowGUI(m_bReduceFinalWidthProp.boolValue);
                EditorGUILayout.PropertyField(m_bReduceWidthStaticProp, BF_EditorParams.gc_IsStaticLabel);
                ShowGUI(m_bReduceWidthStaticProp.boolValue && m_bReduceFinalWidthProp.boolValue);

                DrawFloatStaticValue(m_fReducedWidthStaticValueProp);

                ShowGUI(!m_bReduceWidthStaticProp.boolValue && m_bReduceFinalWidthProp.boolValue);

                //% width
                DrawFloatPercentageValue(m_fReducedWidthPercentageValueProp, 0f, 100f);

                GUILayout.Space(10f);

                ShowGUI(false);
                if (m_bReduceFinalHeightProp.boolValue)
                {
                    if (m_bReduceHeightStaticProp.boolValue) { BF_EditorParams.ShowLabel("[ ! ] Final Height = Target height - Static value", 0f, Color.yellow); }
                    else { BF_EditorParams.ShowLabel("[ ! ] Final Height = Target height - [% Target height] value", 0f, Color.yellow); }
                }
                else { BF_EditorParams.ShowLabel("[ ! ] Final Height = Parent height * Percentage Ratio", 0f, Color.yellow); }
                ShowGUI(true);

                EditorGUILayout.PropertyField(m_bReduceFinalHeightProp, BF_EditorParams.gc_ReduceHeightLabel);

                //is static
                ShowGUI(m_bReduceFinalHeightProp.boolValue);
                EditorGUILayout.PropertyField(m_bReduceHeightStaticProp, BF_EditorParams.gc_IsStaticLabel);
                ShowGUI(m_bReduceHeightStaticProp.boolValue && m_bReduceFinalHeightProp.boolValue);
                //static height value
                DrawFloatStaticValue(m_fReducedHeightStaticValueProp);
                ShowGUI(!m_bReduceHeightStaticProp.boolValue && m_bReduceFinalHeightProp.boolValue);
                //% height value
                DrawFloatPercentageValue(m_fReducedHeightPercentageValueProp, 0f, 100f, BF_EditorParams.gc_PercentageValueLabel);
                ShowGUI(true);

                GUILayout.EndVertical();

                #endregion

                #region SPACE HEIGHT

                /*

                GUILayout.Space(10f);
                GUILayout.BeginVertical("Reduce Height Options", "window");

                ShowGUI(false);
                if (m_bReduceFinalHeightProp.boolValue)
                {
                    if (m_bReduceHeightStaticProp.boolValue) { SmartEditorParams.ShowLabel("[ ! ] Final Height = Target height - Static value", 0f, Color.yellow); }
                    else { SmartEditorParams.ShowLabel("[ ! ] Final Height = Target height - [% Target height] value", 0f, Color.yellow); }
                }
                else { SmartEditorParams.ShowLabel("[ ! ] Final Height = Parent height * Percentage Ratio", 0f, Color.yellow); }
                ShowGUI(true);

                EditorGUILayout.PropertyField(m_bReduceFinalHeightProp, SmartEditorParams.gc_ReduceHeightLabel);

                //is static
                ShowGUI(m_bReduceFinalHeightProp.boolValue);
                EditorGUILayout.PropertyField(m_bReduceHeightStaticProp, SmartEditorParams.gc_IsStaticLabel);
                ShowGUI(m_bReduceHeightStaticProp.boolValue && m_bReduceFinalHeightProp.boolValue);
                //static height value
                DrawFloatStaticValue(m_fReducedHeightStaticValueProp);
                ShowGUI(!m_bReduceHeightStaticProp.boolValue && m_bReduceFinalHeightProp.boolValue);
                //% height value
                DrawFloatPercentageValue(m_fReducedHeightPercentageValueProp, 0f, 100f, SmartEditorParams.gc_PercentageValueLabel);
                ShowGUI(true);
                GUILayout.EndVertical();

                */

                #endregion

            }

            ShowGUI(true);

            #endregion

            #endregion
        }
        
        protected override void DrawAdaptChildSize()
        {
            #region Child Size

            //Min Values
            BeginCheck();
            float maxvalue = m_childSizeMaxSizeLimitToParentSizeProp.boolValue ? m_BestResizeTarget.parentSize.x - 10f : m_childSizeMaxWidthProp.floatValue - 10f;
            DrawFloatCustomValue(m_childSizeMinWidthProp, 10f, 10f, maxvalue, BF_EditorParams.gc_MinWidthLabel);
            if (EndCheck())
            {
                m_childSizeMinWidthProp.floatValue = CalcUtils.GetPositiveFloat(Mathf.Clamp(m_childSizeMinWidthProp.floatValue, 1f, maxvalue));
            }
            BeginCheck();
            maxvalue = m_childSizeMaxSizeLimitToParentSizeProp.boolValue ? m_BestResizeTarget.parentSize.y - 10f : m_childSizeMaxHeightProp.floatValue - 10f;
            DrawFloatCustomValue(m_childSizeMinHeightProp, 10f, 10f, maxvalue, BF_EditorParams.gc_MinHeightLabel);
            if (EndCheck())
            {
                m_childSizeMinHeightProp.floatValue = CalcUtils.GetPositiveFloat(Mathf.Clamp(m_childSizeMinHeightProp.floatValue, 1f, maxvalue));
            }

            EditorGUILayout.Space(5f);

            EditorGUILayout.PropertyField(m_childSizeMaxSizeLimitToParentSizeProp, BF_EditorParams.gc_MaxSizeKeepParentSizeLabel);
            ShowGUI(!m_childSizeMaxSizeLimitToParentSizeProp.boolValue);

            //Max Values

            Vector2 maxSize = new Vector2(50000f, 50000f);
            //[Improvements] should we set canvas size as max params? or a % value as we dont know the actual size of device sceen size that this app will run
            //maxSize = m_BestResizeTarget.rectCanvas.RealSize();

            BeginCheck();
            float minvalue = Mathf.Min(0f, 15f + m_childSizeMinWidthProp.floatValue);
            DrawFloatCustomValue(m_childSizeMaxWidthProp, minvalue, minvalue, maxSize.x, BF_EditorParams.gc_MaxWidthLabel);
            if (EndCheck())
            {
                if (m_childSizeMaxWidthProp.floatValue != 0f)
                    m_childSizeMaxWidthProp.floatValue = CalcUtils.GetPositiveFloat(Mathf.Clamp(m_childSizeMaxWidthProp.floatValue, minvalue, maxSize.x));
            }
            BeginCheck();
            minvalue = Mathf.Min(0f, 15f + m_childSizeMinHeightProp.floatValue);
            DrawFloatCustomValue(m_childSizeMaxHeightProp, minvalue, minvalue, maxSize.y, BF_EditorParams.gc_MaxHeightLabel);
            if (EndCheck())
            {
                if (m_childSizeMaxHeightProp.floatValue != 0f)
                    m_childSizeMaxHeightProp.floatValue = CalcUtils.GetPositiveFloat(Mathf.Clamp(m_childSizeMaxHeightProp.floatValue, minvalue, maxSize.y));
            }

            //check layout groups

            ShowGUI(m_BestResizeTarget.hasHorizontalLayoutGroup);
            EditorGUILayout.PropertyField(m_childHorizontalProp, BF_EditorParams.gc_AlignHorizontalLabel);
            ShowGUI(m_BestResizeTarget.hasVerticalLayoutGroup);
            EditorGUILayout.PropertyField(m_childVerticalProp, BF_EditorParams.gc_AlignVerticalLabel);
            ShowGUI(true);

            DrawFloatPercentageValue(m_childWidthIncreaseValueProp, 0f, 5000f, BF_EditorParams.gc_IncreaseWidthLabel);
            DrawFloatPercentageValue(m_childHeightIncreaseValueProp, 0f, 5000f, BF_EditorParams.gc_IncreaseHeightLabel);

            #endregion
        }

        protected override void DrawAdaptTextureSize()
        {
            #region AdaptTextureRatio

            if (m_BestResizeTarget.HasTargetTexure())
            {
                //CHECK UVRect
                if (m_BestResizeTarget.targetRawImage)
                {
                    Rect rt = m_BestResizeTarget.targetRawImage.uvRect;
                    if (rt.position != Vector2.zero || rt.size != Vector2.one)
                    {
                        BF_EditorParams.ShowHelpBox(BF_EditorParams.msgUVRectWarning, 15f, MessageType.Warning);
                        ResetColors();
                        GUI.backgroundColor = BF_EditorUtils.ColorButtonsImportant(oldColor);
                        if (GUILayout.Button("Fix UVRect", BF_EditorUtils.StyleWarningButton()))
                        {
                            m_BestResizeTarget.targetRawImage.uvRect = new Rect(Vector2.zero, Vector2.one);
                        }
                        ResetColors();
                    }
                }

                EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());
                showTexturePreviewPanel = GUILayout.Toggle(showTexturePreviewPanel, "Preview Texture", "foldout");

                if (showTexturePreviewPanel)
                {
                    EditorGUILayout.BeginHorizontal();
                    float texW = m_BestResizeTarget.textureSize.x / 10f;
                    float texH = m_BestResizeTarget.textureSize.y / 10f;
                    Texture tex = m_BestResizeTarget.GetTexure();
                    GUILayout.Box(tex, GUILayout.Height(Mathf.Clamp(texH, 30, 70)), GUILayout.Width(Mathf.Clamp(texW, 30, 70)));

                    EditorGUILayout.BeginVertical();
                    BF_EditorParams.ShowLabel("[ " + tex.name + " ]", 0f, Color.white);
                    GUILayout.Space(-6);//closer
                    if (GUILayout.Button("Refresh", GUILayout.Width(70f))) { ForceRefreshTarget(); Debug.Log("ForceRefreshTarget 3"); }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                }

                BF_EditorParams.ShowLabel("Parent size: " + m_BestResizeTarget.parentSize.x + " x " + m_BestResizeTarget.parentSize.y, 0f, Color.white);
                GUILayout.Space(-15);//closer
                BF_EditorParams.ShowLabel("Texture size: " + m_BestResizeTarget.textureSize.x + " x " + m_BestResizeTarget.textureSize.y, 0f, Color.yellow);
                GUILayout.Space(-15);//closer
                BF_EditorParams.ShowLabel("Target size: " + m_BestResizeTarget.targetSize.x + " x " + m_BestResizeTarget.targetSize.y, 0f, Color.cyan);
                if (m_IgnoreFitWidthProp.boolValue) { GUILayout.Space(-15); BF_EditorParams.ShowLabel("Width is being ignored", 0f, Color.yellow); }
                if (m_IgnoreFitHeightProp.boolValue) { GUILayout.Space(-15); BF_EditorParams.ShowLabel("Height is being ignored", 0f, Color.yellow); }
                GUILayout.EndVertical();

                GUILayout.Space(10f);

                //envelope parent
                EditorGUILayout.PropertyField(m_bTextureEnvelopeParentProp, BF_EditorParams.gc_EnvelopeParentLabel);

                ShowGUI(!m_bTextureEnvelopeParentProp.boolValue);

                GUILayout.Space(10f);

                //fit in parent
                EditorGUILayout.PropertyField(m_bTextureFitInParentProp, BF_EditorParams.gc_FitInParentLabel);

                ShowGUI(m_bTextureFitInParentProp.boolValue && !m_bTextureEnvelopeParentProp.boolValue);

                //ignore width
                EditorGUILayout.PropertyField(m_bTextureFitInParentIgnoreWidthProp, BF_EditorParams.gc_IgnoreWidthLabel);
                //reduce size
                EditorGUILayout.PropertyField(m_bTextureFitInParentShouldReduceProp, BF_EditorParams.gc_FitInParentReduceSizeLabel);

                ShowGUI(m_bTextureFitInParentShouldReduceProp.boolValue && m_bTextureFitInParentProp.boolValue && !m_bTextureEnvelopeParentProp.boolValue);

                //is static
                //EditorGUILayout.PropertyField(m_bTextureFitInParentShouldReduceStaticProp, SmartEditorParams.gc_IsStaticLabel);

                ShowGUI(/*m_bTextureFitInParentShouldReduceStaticProp.boolValue && */m_bTextureFitInParentShouldReduceProp.boolValue && m_bTextureFitInParentProp.boolValue && !m_bTextureEnvelopeParentProp.boolValue);

                //static value
                //DrawFloatStaticValue(m_fTextureFitInParentReduceStaticValueProp);

                ShowGUI(/*!m_bTextureFitInParentShouldReduceStaticProp.boolValue && */m_bTextureFitInParentShouldReduceProp.boolValue && m_bTextureFitInParentProp.boolValue && !m_bTextureEnvelopeParentProp.boolValue);

                //% value
                DrawFloatPercentageValue(m_fTextureFitInParentReducePercentageValueProp, 0f, 100f);

                GUILayout.Space(10f);
                ShowGUI(!m_bTextureFitInParentProp.boolValue && !m_bTextureEnvelopeParentProp.boolValue);

                //lock axes
                EditorGUILayout.PropertyField(m_bLockTextureSizeXProp, BF_EditorParams.gc_LockAxisXLabel);
                EditorGUILayout.PropertyField(m_bLockTextureSizeYProp, BF_EditorParams.gc_LockAxisYLabel);

                if (!m_bTextureFitInParentProp.boolValue && !m_bTextureEnvelopeParentProp.boolValue) { ShowGUI(!m_bLockTextureSizeXProp.boolValue || !m_bLockTextureSizeYProp.boolValue); }

                //Percentage Texture Scale
                DrawFloatPercentageValue(m_fTextureSizePercentageScaleValueProp, 100f, 1000f, BF_EditorParams.gc_PercentageTextureScaleValueLabel);

                ShowGUI(true);

            }
            else
            {
                BF_EditorParams.ShowHelpBox(BF_EditorParams.msgImageSpriteNullDesc, 0f, MessageType.Error, 2f);
            }

            #endregion
        }

        protected override void DrawAdaptMockRatio()
        {
            #region ADAPT MOCK RATIO

            DrawSizeInfo();

            GUILayout.Space(10f);

            BeginCheck();
            EditorGUILayout.PropertyField(m_mockRatioModeProp, BF_EditorParams.gc_MockRatioModeLabel);
            if (EndCheck()) m_HavePropertiesChanged = true;

            switch (m_mockRatioModeProp.enumValueIndex)
            {
                case (int)EnumUtils.MockRatioMode.SelectPreset:
                default:
                    m_bMockRatioFitInParentProp.boolValue = true;
                    GUILayout.Space(5f);
                    //GUILayout.BeginHorizontal();
                    BF_EditorParams.ShowLabel(BF_EditorParams.gc_AspectRatioPresetsLabel, 0f, Color.white);
                    GUILayout.Space(-7f);
                    BeginCheck();
                    m_nMockRatioModeIndexProp.intValue = EditorGUILayout.Popup(m_BestResizeTarget.nMockRatioModeIndex, BF_EditorParams.aspectRatios);
                    if (EndCheck()) m_HavePropertiesChanged = true;
                    //GUILayout.EndHorizontal();
                    GUILayout.Space(5f);
                    BeginCheck();
                    EditorGUILayout.PropertyField(m_bMockRatioIsPortraitProp, BF_EditorParams.gc_IsPortraitLabel);
                    if (EndCheck()) m_HavePropertiesChanged = true;
                    break;
                case (int)EnumUtils.MockRatioMode.CustomRatio:
                    m_bMockRatioFitInParentProp.boolValue = true;
                    GUILayout.Space(10f);
                    DrawFloatCustomValue(m_fMockAspectRatioProp, 1.777f, 0.01f, 1000f, BF_EditorParams.gc_CustomAspectRatioLabel);
                    BeginCheck();
                    EditorGUILayout.PropertyField(m_bMockRatioIsPortraitProp, BF_EditorParams.gc_IsReversedLabel);
                    if (EndCheck()) m_HavePropertiesChanged = true;
                    break;
                case (int)EnumUtils.MockRatioMode.PrefferedDimensions:
                    GUILayout.Space(10f);
                    DrawIntegerCustomValue(m_nMockRatioWidthProp, 1920, 10, 4096, BF_EditorParams.gc_PrefferedWidthLabel);
                    DrawIntegerCustomValue(m_nMockRatioHeightProp, 1080, 10, 4096, BF_EditorParams.gc_PrefferedHeightLabel);
                    GUILayout.Space(5f);//closer
                    ShowGUI(false);
                    string _aspectRatioNow = BF_EditorParams.GetStringRatioFromFloatingPointNumber((float)m_BestResizeTarget.nMockRatioWidth / m_BestResizeTarget.nMockRatioHeight);
                    BF_EditorParams.ShowLabel("Aspect Ratio: " + _aspectRatioNow, 0f, Color.white);
                    ShowGUI(true);
                    break;

            }

            GUILayout.Space(10f);

            bool isPrefferedDimens = m_mockRatioModeProp.enumValueIndex == (int)EnumUtils.MockRatioMode.PrefferedDimensions;
            ShowGUI(isPrefferedDimens);//disable editing
            BeginCheck();
            EditorGUILayout.PropertyField(m_bMockRatioFitInParentProp, isPrefferedDimens ? BF_EditorParams.gc_FitInParentLabel : BF_EditorParams.gc_FitInParentLabelAlwaysTrueLabel);
            if (EndCheck()) m_HavePropertiesChanged = true;
            ShowGUI(m_bMockRatioFitInParentProp.boolValue);

            BeginCheck();
            EditorGUILayout.PropertyField(m_bMockRatioFitInParentIgnoreWidthProp, BF_EditorParams.gc_IgnoreWidthLabel);
            if (EndCheck()) m_HavePropertiesChanged = true;

            BeginCheck();
            EditorGUILayout.PropertyField(m_bMockRatioFitInParentShouldReduceProp, BF_EditorParams.gc_ReduceSizeLabel);
            if (EndCheck()) m_HavePropertiesChanged = true;

            ShowGUI(m_bMockRatioFitInParentProp.boolValue && m_bMockRatioFitInParentShouldReduceProp.boolValue);

            //BeginCheck();
            //EditorGUILayout.PropertyField(m_bMockRatioFitInParentShouldReduceStaticProp, SmartEditorParams.gc_IsStaticLabel);
            //if (EndCheck()) m_HavePropertiesChanged = true;

            ShowGUI(m_bMockRatioFitInParentProp.boolValue && m_bMockRatioFitInParentShouldReduceProp.boolValue /*&& m_bMockRatioFitInParentShouldReduceStaticProp.boolValue*/);

            //static value
           // DrawFloatCustomValue(m_fMockRatioFitInParentReduceStaticValueProp, 0f, 0f, 4096f, SmartEditorParams.gc_StaticValueLabel);

            ShowGUI(m_bMockRatioFitInParentProp.boolValue /*&& !m_bMockRatioFitInParentShouldReduceStaticProp.boolValue*/ && m_bMockRatioFitInParentShouldReduceProp.boolValue);
            //[%] value]
            DrawFloatCustomValue(m_fMockRatioFitInParentReducePercentageValueProp, 0f, 0f, 100f, BF_EditorParams.gc_PercentageValueLabel);

            ShowGUI(true);

            #endregion
        }
        
        protected override void DrawAdaptStaticSize()
        {
            if (m_BestResizeTarget == null) return;

            EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());
            BF_EditorParams.ShowLabel(string.Format("Parent size: {0} x {1}", m_BestResizeTarget.parentSize.x, m_BestResizeTarget.parentSize.y), 0f, Color.white);
            if (m_IgnoreFitWidthProp.boolValue) { GUILayout.Space(-15); BF_EditorParams.ShowLabel("Width is being ignored", 0f, Color.yellow); }
            if (m_IgnoreFitHeightProp.boolValue) { GUILayout.Space(-15); BF_EditorParams.ShowLabel("Height is being ignored", 0f, Color.yellow); }
            GUILayout.EndVertical();

            EditorGUILayout.Space();

            DrawFloatStaticValue(m_StaticWidthProp, BF_EditorParams.gc_StaticWidthLabel, 100f);
            DrawFloatStaticValue(m_StaticHeightProp, BF_EditorParams.gc_StaticHeightLabel, 100f);

            if (m_StaticWidthProp.floatValue != m_BestResizeTarget.targetSize.x || m_StaticHeightProp.floatValue != m_BestResizeTarget.targetSize.y)
            {
                EditorGUILayout.Space();
               // GUI.backgroundColor = Color.black;
                //GUI.contentColor = Editor. Color.cyan;
                if (GUILayout.Button("Copy size", BF_EditorUtils.StyleWarningButton()))
                {
                    m_StaticWidthProp.floatValue = m_BestResizeTarget.targetSize.x;
                    m_StaticHeightProp.floatValue = m_BestResizeTarget.targetSize.y;
                    m_HavePropertiesChanged = true;
                }
                ResetColors();
            }
        }

        #endregion

        private void ResetInitialPosition()
        {
            switch (m_InitialPositionModeProp.enumValueIndex)
            {
                case (int)EnumUtils.InitialPositionMode.SelfSizePercentage:
                    m_initVectorSelfValuesProp.vector2Value = Vector2.zero;
                    break;
                case (int)EnumUtils.InitialPositionMode.ParentSizePercentage:
                    m_initVectorParentValuesProp.vector2Value = Vector2.zero;
                    break;
                case (int)EnumUtils.InitialPositionMode.Static:
                default:
                    m_InitialStaticPosProp.vector2Value = Vector2.zero;
                    break;
            }
        }

        protected override void ShowExtraSettings()
        {
            if (m_BestResizeTarget == null) return;

            #region Initial position offset SETTINGS

            bool drawProperties = !m_isMovableProp.boolValue && m_pivotModeProp.enumValueIndex != (int)EnumUtils.PivotPoint.None;
            if (!drawProperties) ResetInitialPosition();

            ShowGUI(drawProperties);

            GUILayout.BeginVertical("Initial Position Offset", "window");

            EditorGUILayout.BeginVertical(BF_EditorUtils.StyleBox());

            EditorGUILayout.Space();
            GUIContent ttt = new GUIContent("Offset Mode", "Initial position offset mode selection");

            m_BestResizeTarget.initialPositionMode = (EnumUtils.InitialPositionMode)EditorGUILayout.EnumPopup(ttt, m_BestResizeTarget.initialPositionMode);

            EditorGUILayout.Space();

            int iVal = (int)m_BestResizeTarget.initialPositionMode;

            switch (m_InitialPositionModeProp.enumValueIndex)
            {
                case (int)EnumUtils.InitialPositionMode.SelfSizePercentage:

                    #region SelfSizePercentage

                    BeginCheck();
                    EditorGUILayout.PropertyField(m_initVectorSelfValuesProp, BF_EditorParams.gc_InitialPositionLabel(iVal));
                    if (EndCheck()) m_initVectorSelfValuesProp.vector2Value = CalcUtils.ClampVector2(m_initVectorSelfValuesProp.vector2Value, new Vector4(-100f, 100f, -100f, 100f));

                    EditorGUILayout.Space(5);

                    if (!m_initVectorSelfValuesProp.vector2Value.IsZero())
                    {
                        ResetColors();
                        GUI.backgroundColor = BF_EditorUtils.ColorButtonsImportant(oldColor);
                        if (GUILayout.Button("Reset Start Position", BF_EditorUtils.StyleWarningButton()))
                        {
                            ResetInitialPosition();
                            m_HavePropertiesChanged = true;
                        }
                        ResetColors();
                    }

                    #endregion

                    break;
                case (int)EnumUtils.InitialPositionMode.ParentSizePercentage:

                    #region ParentSizePercentage

                    BeginCheck();
                    EditorGUILayout.PropertyField(m_initVectorParentValuesProp, BF_EditorParams.gc_InitialPositionLabel(iVal));
                    if (EndCheck()) m_initVectorParentValuesProp.vector2Value = CalcUtils.ClampVector2(m_initVectorParentValuesProp.vector2Value, EnumUtils.GetStartPositionMinMaxValues(m_BestResizeTarget.pivotMode));

                    EditorGUILayout.Space(5);

                    if (!m_initVectorParentValuesProp.vector2Value.IsZero())
                    {
                        EditorGUILayout.Space(5f);
                        ResetColors();
                        GUI.backgroundColor = BF_EditorUtils.ColorButtonsImportant(oldColor);
                        if (GUILayout.Button("Reset Start Position", BF_EditorUtils.StyleWarningButton()))
                        {
                            ResetInitialPosition();
                            m_HavePropertiesChanged = true;
                        }
                        ResetColors();
                    }

                    #endregion

                    break;
                case (int)EnumUtils.InitialPositionMode.Static:
                default:

                    #region Static

                    BeginCheck();
                    EditorGUILayout.PropertyField(m_InitialStaticPosProp, BF_EditorParams.gc_InitialPositionLabel(iVal));
                    if (EndCheck()) m_InitialStaticPosProp.vector2Value = CalcUtils.ClampVector2(m_InitialStaticPosProp.vector2Value, new Vector4(-10000f, 10000f, - 10000f, 10000f));


                    EditorGUILayout.Space(5);

                    if (!m_InitialStaticPosProp.vector2Value.IsZero())
                    {
                        EditorGUILayout.Space(10f);
                        ResetColors();
                        GUI.backgroundColor = BF_EditorUtils.ColorButtonsImportant(oldColor);
                        if (GUILayout.Button("Reset Start Position", BF_EditorUtils.StyleWarningButton()))
                        {
                            ResetInitialPosition();
                            m_HavePropertiesChanged = true;
                        }
                        ResetColors();
                    }

                    #endregion

                    break;
            }

            EditorGUILayout.Space();


            EditorGUILayout.EndVertical();

            GUILayout.EndVertical();

            ShowGUI(true);

            #endregion
        }

       
    }
}