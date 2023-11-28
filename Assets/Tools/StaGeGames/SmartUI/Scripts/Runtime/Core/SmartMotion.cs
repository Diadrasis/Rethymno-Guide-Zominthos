//StaGe Games ©2022
using StaGeGames.BestFit.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace StaGeGames.BestFit
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(BestResize))]
    [Serializable]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class SmartMotion : MonoBehaviour
    {
        [SerializeField]
        public Button[] showButtons, hideButtons, toggleVisibilityButtons;

        [Header("Pivot point")]
        // [HideInInspector]
        [SerializeField]
        public EnumUtils.PivotPoint pivotMode = EnumUtils.PivotPoint.None;
        [Header("Target RectTransform To Move - If empty then nothing happens.")]
        [SerializeField]
        public RectTransform targetRect;

        [Header("True: Calculate Speed - False: Use Custom move speed.")]//, order =1)]
        [SerializeField]
        public bool isAutoSpeed = true;
        [SerializeField]
        public float autoSpeedMultiplier = 1f;

        [HideInInspector]
        [Header("Custom Move Speed")]
        [SerializeField]
        public float moveSpeedCustom = 250f;

        [Header("Custom Space StartPos")]
        [SerializeField]
        public float spaceHorizontal = 0f;
        [Header("Custom Space Corner Up or Down Pos")]
        [SerializeField]
        public float spacevertical = 0f;

        // [HideInInspector]
        [SerializeField]
        public EnumUtils.MotionMode motionMode = EnumUtils.MotionMode.Horizontally;
        [SerializeField]
        Vector2 panelInitPosition, panelHiddenPosition;

        [Space]
        [SerializeField]
        public bool isVisible, isVisibleAtStart;
        [Space]
        [SerializeField]
        public bool showHideInstantly, hideGraphicAtPivotCenter = false;
        [Space]
        [SerializeField]
        public bool IsAutoHideOn;
        [SerializeField]
        public float hideTime = 5f;
        [SerializeField]
        public bool isHiding;

        [SerializeField]
        private bool initCompleted;

        [HideInInspector]
        [SerializeField]
        public UnityEvent OnHideStart, OnHideComplete, OnShowStart, OnShowComplete, OnShowPercentageStart, OnShowPercentageComplete;

#if UNITY_EDITOR
        [HideInInspector]
        [SerializeField]
        public bool isInEditorAlive;
#endif

        [SerializeField]
        BestResize smartResize;

        private void Awake()
        {
            if (showButtons != null && showButtons.Length > 0) { foreach (Button btn in showButtons) btn.onClick.AddListener(ShowPanel); }
            if (hideButtons != null && hideButtons.Length > 0) { foreach (Button btn in hideButtons) btn.onClick.AddListener(HidePanel); }
            if (toggleVisibilityButtons != null && toggleVisibilityButtons.Length > 0) { foreach (Button btn in toggleVisibilityButtons) btn.onClick.AddListener(TogglePanelAppearance); }
        }


        /// <summary>
        /// Init panel
        /// </summary>
        public void Init()
        {
//#if UNITY_EDITOR
//            Debug.LogWarning("SMART MOTION INIT");
//#endif

            if (smartResize == null) smartResize = GetComponent<BestResize>();
            if (smartResize == null)
            {
                Debug.LogError("Missing SmartResize!");
                return;
            }

            targetRect = smartResize.rectTarget;

            if (targetRect == null)
            {
                Debug.LogError("Missing target rect !");
                return;
            }

            bool isNowVisible = isVisible;
            //if (isNowVisible) HidePanelInstantly();

            pivotMode = smartResize.pivotMode;
            motionMode = smartResize.motionMode;

            panelInitPosition = Vector2.zero;// targetRect.anchoredPosition;
            panelHiddenPosition = HidePosition();
            CalculateSpeed();

            isVisibleAtStart = smartResize.isVisibleOnStart;
            if (!isVisibleAtStart)
            {
                if (!isNowVisible)
                {
                    isVisible = false;
                    //if (HideIfPivotCenter()) ShowGraphic(false);
                    targetRect.anchoredPosition = panelHiddenPosition;
                    if (OnHideStart != null) OnHideStart?.Invoke();
                }
            }
            else
            {
                isVisible = true;
                // ShowGraphic(true);
                targetRect.anchoredPosition = panelInitPosition;
            }

            initCompleted = true;



#if UNITY_EDITOR
            PrepareEventsForEditorUse();

            if (!gameObject.GetComponent<SmartControl>())
            {
                SmartControl smartControl = gameObject.AddComponent<SmartControl>();
                smartControl.smartMotion = this;
                smartControl.smartResize = gameObject.GetComponent<BestResize>();
            }
#endif

            if (!isVisible && isNowVisible)//restore view
            {
                //targetRect.anchoredPosition = panelInitPosition;
                ShowPanel();
            }

        }

        bool HideIfPivotCenter()
        {
            return hideGraphicAtPivotCenter && pivotMode == EnumUtils.PivotPoint.Center;
        }

        /// <summary>
        /// Show-Hide Panel
        /// </summary>
        public void TogglePanelAppearance()
        {
            CancelInvoke();
            if (targetRect.anchoredPosition == panelHiddenPosition) { ShowPanel(); }
            else { /*if (transform.parent.parent.gameObject.activeSelf)*/ HidePanel(); }
        }


        private RawImage _rawImage;
        private Image _image;

        void ShowGraphic(bool val)
        {
            //Debug.LogWarning("ShowGraphic = " + val);
            if (_image == null) _image = GetComponent<Image>();
            if (_rawImage == null) _rawImage = GetComponent<RawImage>();

            if (_image)
            {
                _image.enabled = val;
                _image.raycastTarget = val;
            }

            if (_rawImage)
            {
                _rawImage.enabled = val;
                _rawImage.raycastTarget = val;
            }
        }

        /// <summary>
        /// Hide panel
        /// </summary>
        public void HidePanel()
        {
            CancelInvoke();
            if (!InitRequest()) return;
            if (!gameObject.activeInHierarchy)
            {
                //if (Application.isEditor) Debug.LogWarning("Panel is inactive!");
                return;
            }
            isHiding = true;
            if (showHideInstantly)
            {
                if (OnHideStart != null) OnHideStart?.Invoke();
                //if (HideIfPivotCenter()) { ShowGraphic(false); }
                targetRect.anchoredPosition = panelHiddenPosition;
                isVisible = false;
                isHiding = false;
                if (OnHideComplete != null) OnHideComplete?.Invoke();
            }
            else
            {
                //if (HideIfPivotCenter()) { ShowGraphic(false); }
                StartCoroutine(HideLerpPanel());
            }
        }

        public void HidePanelInstantly()
        {
            CancelInvoke();
            if (!InitRequest()) return;
            if (!gameObject.activeInHierarchy)
            {
                //if (Application.isEditor) Debug.LogWarning("Panel is inactive!");
                return;
            }
            isHiding = true;
            if (OnHideStart != null) OnHideStart?.Invoke();
            //if (HideIfPivotCenter()) { ShowGraphic(false); }
            targetRect.anchoredPosition = panelHiddenPosition;
            isVisible = false;
            isHiding = false;
            if (OnHideComplete != null) OnHideComplete?.Invoke();
        }


        IEnumerator HideLerpPanel()
        {
            if (OnHideStart != null) OnHideStart?.Invoke();
            yield return new WaitForEndOfFrame();
            while (Vector2.Distance(targetRect.anchoredPosition, panelHiddenPosition) > 10f)
            {
                targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, panelHiddenPosition, Time.smoothDeltaTime * moveSpeedCustom);
                yield return null;
            }
            targetRect.anchoredPosition = panelHiddenPosition;
            isVisible = false;
            isHiding = false;
            if (OnHideComplete != null) OnHideComplete?.Invoke();
            yield break;
        }

        /// <summary>
        /// show panel
        /// </summary>
        public void ShowPanel()
        {
            if (!InitRequest()) return;
            if (!gameObject.activeInHierarchy)
            {
                //if (Application.isEditor) Debug.LogWarning("Panel is inactive!");
                return;
            }

            if (IsAutoHideOn)
            {
                CancelInvoke();
                Invoke(nameof(HidePanel), hideTime);
            }

            if (showHideInstantly)
            {
                if (OnShowStart != null) OnShowStart?.Invoke();
                //ShowGraphic(true);
                targetRect.anchoredPosition = panelInitPosition;
                isVisible = true;
                if (OnShowComplete != null) OnShowComplete?.Invoke();
            }
            else
            {
                // Debug.Log("ShowLerpPanel 1");
                // ShowGraphic(true);
                StartCoroutine(ShowLerpPanel());
            }
        }

        IEnumerator ShowLerpPanel()
        {
            //Debug.Log("ShowLerpPanel 2");

            if (OnShowStart != null) OnShowStart?.Invoke();

            yield return new WaitForEndOfFrame();
            while (Vector2.Distance(targetRect.anchoredPosition, panelInitPosition) > 10f)
            {
                targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, panelInitPosition, Time.smoothDeltaTime * moveSpeedCustom);
                yield return null;
            }
            targetRect.anchoredPosition = panelInitPosition;
            isVisible = true;
            if (OnShowComplete != null) OnShowComplete?.Invoke();
            yield break;
        }

        /// <summary>
        /// in seconds
        /// </summary>
        /// <param name="travelTime"></param>
        public void ShowPanelWithTime(float travelTime)
        {
            if (!InitRequest()) return;
            if (!gameObject.activeInHierarchy)
            {
                if (Application.isEditor) Debug.LogWarning("Panel is inactive!");
                return;
            }

            if (IsAutoHideOn)
            {
                CancelInvoke();
                Invoke(nameof(HidePanel), hideTime);
            }

            if (travelTime <= 0)
            {
                if (OnShowStart != null) OnShowStart?.Invoke();
                targetRect.anchoredPosition = panelInitPosition;
                isVisible = true;
                if (OnShowComplete != null) OnShowComplete?.Invoke();
            }
            else
            {
                StartCoroutine(ShowPanelInTime(Mathf.Abs(travelTime)));
            }
        }

        IEnumerator ShowPanelInTime(float travelTime)
        {
            if (OnShowStart != null) OnShowStart?.Invoke();

            yield return new WaitForEndOfFrame();

            var currentPos = targetRect.anchoredPosition;
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / travelTime;
                targetRect.anchoredPosition = Vector3.Lerp(currentPos, panelInitPosition, t);
                yield return null;
            }

            targetRect.anchoredPosition = panelInitPosition;
            isVisible = true;
            if (OnShowComplete != null) OnShowComplete?.Invoke();
            yield break;
        }

        public float GetPanelHeight() { return panelHiddenPosition.y; }

        /// <summary>
        /// show panel with percent
        /// </summary>
        public void ShowPanelWithPercent(float percentage)
        {
            if (!InitRequest()) return;
            if (!gameObject.activeInHierarchy)
            {
                if (Application.isEditor) Debug.LogWarning("Panel is inactive!");
                return;
            }

            if (IsAutoHideOn)
            {
                CancelInvoke();
                Invoke(nameof(HidePanel), hideTime);
            }

            if (showHideInstantly)
            {
                if (OnShowPercentageStart != null) OnShowPercentageStart?.Invoke();
                //ShowGraphic(true);
                targetRect.anchoredPosition = panelInitPosition;
                isVisible = true;
                if (OnShowPercentageComplete != null) OnShowPercentageComplete?.Invoke();
            }
            else
            {
                // Debug.Log("ShowLerpPanel 1");
                // ShowGraphic(true);
                StartCoroutine(ShowPanelPercentage(percentage));
            }
        }

        IEnumerator ShowPanelPercentage(float percentage)
        {
            //Debug.Log("ShowLerpPanel 2");

            if (OnShowStart != null) OnShowStart?.Invoke();
            float percent_val = Mathf.Abs((100 - percentage) / 100f);
            Vector2 finalPos = panelHiddenPosition * percent_val;

            yield return new WaitForEndOfFrame();
            while (Vector2.Distance(targetRect.anchoredPosition, finalPos) > 10f)
            {
                targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, finalPos, Time.smoothDeltaTime * moveSpeedCustom);
                yield return null;
            }
            targetRect.anchoredPosition = finalPos;
            isVisible = true;
            if (OnShowPercentageComplete != null) OnShowPercentageComplete?.Invoke();
            yield break;
        }


        public void ShowPanelWithPercentInTime(float percentage, float travelTime)
        {
            if (!InitRequest()) return;
            if (!gameObject.activeInHierarchy)
            {
                if (Application.isEditor) Debug.LogWarning("Panel is inactive!");
                return;
            }

            if (IsAutoHideOn)
            {
                CancelInvoke();
                Invoke(nameof(HidePanel), hideTime);
            }

            if (travelTime <= 0)
            {
                if (OnShowPercentageStart != null) OnShowPercentageStart?.Invoke();
                targetRect.anchoredPosition = panelInitPosition;
                isVisible = true;
                if (OnShowPercentageComplete != null) OnShowPercentageComplete?.Invoke();
            }
            else
            {
                StartCoroutine(ShowPanelPercentageWithTime(percentage, travelTime));
            }
        }

        IEnumerator ShowPanelPercentageWithTime(float percentage, float travelTime)
        {
            if (OnShowStart != null) OnShowStart?.Invoke();
            float percent_val = Mathf.Abs((100 - percentage) / 100f);
            Vector2 finalPos = panelHiddenPosition * percent_val;

            yield return new WaitForEndOfFrame();

            var currentPos = targetRect.anchoredPosition;
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / travelTime;
                targetRect.anchoredPosition = Vector3.Lerp(currentPos, finalPos, t);
                yield return null;
            }

            targetRect.anchoredPosition = finalPos;
            isVisible = true;
            if (OnShowPercentageComplete != null) OnShowPercentageComplete?.Invoke();
            yield break;
        }


        public void ResetAutoHideTimeIfUserInteractsWithPanel()
        {
            if (!isVisibleAtStart) return;
            if (IsAutoHideOn)
            {
                CancelInvoke("HidePanel");
                Invoke(nameof(HidePanel), hideTime);
            }
        }


        /// <summary>
        /// https://gamedev.stackexchange.com/questions/157642/moving-a-2d-object-along-circular-arc-between-two-points
        /// </summary>
        /// <param name="mover"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="radius"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        IEnumerator FollowArc(Transform mover, Vector2 start, Vector2 end,
            float radius, // Set this to negative if you want to flip the arc.
        float duration)
        {

            Vector2 difference = end - start;
            float span = difference.magnitude;

            // Override the radius if it's too small to bridge the points.
            float absRadius = Mathf.Abs(radius);
            if (span > 2f * absRadius)
                radius = absRadius = span / 2f;

            Vector2 perpendicular = new Vector2(difference.y, -difference.x) / span;
            perpendicular *= Mathf.Sign(radius) * Mathf.Sqrt(radius * radius - span * span / 4f);

            Vector2 center = start + difference / 2f + perpendicular;

            Vector2 toStart = start - center;
            float startAngle = Mathf.Atan2(toStart.y, toStart.x);

            Vector2 toEnd = end - center;
            float endAngle = Mathf.Atan2(toEnd.y, toEnd.x);

            // Choose the smaller of two angles separating the start & end
            float travel = (endAngle - startAngle + 5f * Mathf.PI) % (2f * Mathf.PI) - Mathf.PI;

            float progress = 0f;
            do
            {
                float angle = startAngle + progress * travel;
                mover.position = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * absRadius;
                progress += Time.deltaTime / duration;
                yield return null;
            } while (progress < 1f);

            mover.position = end;
        }

        void CalculateSpeed()
        {
            //auto set speed from width
            if (isAutoSpeed)
            {
                Vector2 pos = panelHiddenPosition;// HidePosition();
                if (pos.x != 0)
                {
                    moveSpeedCustom = Mathf.Abs(3f * pos.x);// + (pos.x / 2f));
                }
                else if (pos.y != 0)
                {
                    moveSpeedCustom = Mathf.Abs(3f * pos.y);// + (pos.y / 2f));
                }

                moveSpeedCustom = moveSpeedCustom * autoSpeedMultiplier;
            }
            else
            {
                if (moveSpeedCustom < 0f) moveSpeedCustom = Mathf.Abs(moveSpeedCustom);
            }
        }


        Vector2 HidePosition()
        {
            Vector2 hidePos = targetRect.anchoredPosition;

            switch (pivotMode)
            {
                case EnumUtils.PivotPoint.None:
                    break;
                case EnumUtils.PivotPoint.Center:
                    break;
                case EnumUtils.PivotPoint.BottomCenter:
                    hidePos = new Vector2(0f, -targetRect.rect.height);
                    panelInitPosition.y = panelInitPosition.x + spaceHorizontal;
                    break;
                case EnumUtils.PivotPoint.BottomLeft:
                    switch (motionMode)
                    {
                        case EnumUtils.MotionMode.Horizontally:
                            hidePos = new Vector2(-targetRect.rect.width, spacevertical);
                            panelInitPosition.x = panelInitPosition.x + spaceHorizontal;
                            panelInitPosition.y = panelInitPosition.y + spacevertical;
                            break;
                        case EnumUtils.MotionMode.Vertically:
                            hidePos = new Vector2(0f, -targetRect.rect.height);
                            panelInitPosition.x = panelInitPosition.x + spaceHorizontal;
                            panelInitPosition.y = panelInitPosition.y + spacevertical;
                            break;
                        case EnumUtils.MotionMode.Diagonally:
                            hidePos = new Vector2(-targetRect.rect.width, -targetRect.rect.height);
                            panelInitPosition.x = panelInitPosition.x + spaceHorizontal;
                            panelInitPosition.y = panelInitPosition.y + spacevertical;
                            break;
                        default:
                            hidePos = new Vector2(-targetRect.rect.width, spacevertical);
                            panelInitPosition.x = panelInitPosition.x + spaceHorizontal;
                            panelInitPosition.y = panelInitPosition.y + spacevertical;
                            break;
                    }
                    break;
                case EnumUtils.PivotPoint.BottomRight:
                    switch (motionMode)
                    {
                        case EnumUtils.MotionMode.Horizontally:
                            hidePos = new Vector2(targetRect.rect.width, spacevertical);
                            panelInitPosition.x = panelInitPosition.x - spaceHorizontal;
                            panelInitPosition.y = panelInitPosition.y + spacevertical;
                            break;
                        case EnumUtils.MotionMode.Vertically:
                            hidePos = new Vector2(0f, -targetRect.rect.height);
                            break;
                        case EnumUtils.MotionMode.Diagonally:
                            hidePos = new Vector2(targetRect.rect.width, -targetRect.rect.height);
                            break;
                        default:
                            hidePos = new Vector2(targetRect.rect.width, spacevertical);
                            panelInitPosition.x = panelInitPosition.x - spaceHorizontal;
                            panelInitPosition.y = panelInitPosition.y + spacevertical;
                            break;
                    }
                    break;
                case EnumUtils.PivotPoint.TopCenter:
                    hidePos = new Vector2(0f, targetRect.rect.height);
                    panelInitPosition.y = panelInitPosition.x - spaceHorizontal;
                    break;
                case EnumUtils.PivotPoint.TopLeft:
                    switch (motionMode)
                    {
                        case EnumUtils.MotionMode.Horizontally:
                            hidePos = new Vector2(-targetRect.rect.width, -spacevertical);
                            break;
                        case EnumUtils.MotionMode.Vertically:
                            hidePos = new Vector2(0f, targetRect.rect.height);
                            break;
                        case EnumUtils.MotionMode.Diagonally:
                            hidePos = new Vector2(-targetRect.rect.width, targetRect.rect.height);
                            break;
                        default:
                            hidePos = new Vector2(-targetRect.rect.width, -spacevertical);
                            break;
                    }
                    panelInitPosition.x = panelInitPosition.x + spaceHorizontal;
                    panelInitPosition.y = panelInitPosition.y - spacevertical;
                    break;
                case EnumUtils.PivotPoint.TopRight:
                    switch (motionMode)
                    {
                        case EnumUtils.MotionMode.Horizontally:
                            hidePos = new Vector2(targetRect.rect.width, -spacevertical);
                            break;
                        case EnumUtils.MotionMode.Vertically:
                            hidePos = new Vector2(0f, targetRect.rect.height);
                            break;
                        case EnumUtils.MotionMode.Diagonally:
                            hidePos = new Vector2(targetRect.rect.width, targetRect.rect.height);
                            break;
                        default:
                            hidePos = new Vector2(targetRect.rect.width, -spacevertical);

                            break;
                    }
                    panelInitPosition.x = panelInitPosition.x - spaceHorizontal;
                    panelInitPosition.y = panelInitPosition.y - spacevertical;
                    break;
                case EnumUtils.PivotPoint.LeftCenter:
                    hidePos = new Vector2(-targetRect.rect.width, 0f);
                    panelInitPosition.x = panelInitPosition.x + spaceHorizontal;
                    break;
                case EnumUtils.PivotPoint.RightCenter:
                    hidePos = new Vector2(targetRect.rect.width, 0f);
                    panelInitPosition.x = panelInitPosition.x - spaceHorizontal;
                    break;
                default:
                    break;
            }

            return hidePos;
        }

        private bool InitRequest()
        {
            if (initCompleted) return true;
            //SmartResize autoFit = GetComponent<SmartResize>();
            if (!smartResize) return false;
            smartResize.Init();
            return true;
        }

        private void PrepareEventsForEditorUse()
        {
            if (OnHideComplete != null && OnHideComplete.GetPersistentEventCount() > 0) OnHideComplete.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
            if (OnHideStart != null && OnHideStart.GetPersistentEventCount() > 0) OnHideStart.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
            if (OnShowComplete != null && OnShowComplete.GetPersistentEventCount() > 0) OnShowComplete.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
            if (OnShowStart != null && OnShowStart.GetPersistentEventCount() > 0) OnShowStart.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
        }

#if UNITY_EDITOR

        private void Reset()
        {
            //if (!GetComponent<CanvasRenderer>())
            //{
            //    Debug.LogWarning("Can't add \"SmartMotion\" to non-UI objects");
            //    DestroyImmediate(this);
            //    return;
            //}
            if (gameObject.GetComponents<SmartMotion>().Length > 1)
            {
                Debug.LogWarning("Can't add more than one \"SmartMotion\" to " + gameObject.name);
                DestroyImmediate(this);
            }
        }

        private void OnDestroy()
        {
            //if (GetComponent<SmartControl>() != null)
            //{
            //    DestroyImmediate(GetComponent<SmartControl>());
            //}
        }

#endif

    }

}
