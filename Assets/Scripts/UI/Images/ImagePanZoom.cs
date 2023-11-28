//Diadrasis ©2023 - Stathis Georgiou
using UnityEngine;

namespace Diadrasis.Rethymno
{
    //FIRST OPTION
    //ZOOM WITH 2 FINGERS
    //PAN WITH 1 FINGERs
    public class ImagePanZoom : MonoBehaviour
    {

        [Space(2)]
        [Header("Settings")]
        // Content need set Anchor Presets [Middle & Center]
        [SerializeField] private GameObject Content;

        [Space(10)]
        [SerializeField] private float MinZoom = 0.5f;
        [SerializeField] private float MaxZoom = 3f;
       /* [SerializeField] */private float ZoomSpeed = -0.002f;
       /* [SerializeField] */private float MoveSpeed = 1f;

        [Space(10)]
        /*[SerializeField]*/ [Range(1, 10)] private int SmoothMove = 6;
        /*[SerializeField]*/ [Range(1, 10)] private int SmoothScale = 6;

        [Space(10)]
        // Can be zero values
        /*[SerializeField]*/ private float MinPermissibleHeightOffset;
        /*[SerializeField]*/ private float MaxPermissibleHeightOffset;
        /*[SerializeField]*/ private float MinPermissibleWidthOffset;
        /*[SerializeField]*/ private float MaxPermissibleWidthOffset;

        private RectTransform ContentRect;
        private Vector2 ContentSizeDelta;

        private Vector2 TargetScale;
        private Vector2 TargetPosition;

        private float moveMagnitude;
        private float scaleMagnitude;

        private float oldMoveMagnitude;
        private float oldScaleMagnitude;
        private float startScaleMagnitude;
        private float newScaleMagnitude;

        private Vector2 ContentScaleOnStart;
        private Vector2 ContentPositionOnStart;

        private Vector2 startPostiton;
        private Vector2 newPosition;

        private Vector2 stepMove;
        private Vector3 stepScale;

        private int avalibleMoveSteps;
        private int avalibleScaleSteps;

        Vector2 fingerPosition1;
        Vector2 fingerPosition2;

        private bool _isPinching, _isPaning;
        private bool active, panactive;

        private void Awake()
        {
            Input.multiTouchEnabled = true;
        }

        void Start()
        {
            ContentRect = Content.GetComponent<RectTransform>();
            ContentSizeDelta = ContentRect.sizeDelta;
        }

        public void ResetZoom()
        {
            ContentRect.localScale = Vector3.one;
            ContentRect.anchoredPosition = Vector2.zero;
        }

        private void Update()
        {
            if (!Content.transform.parent.gameObject.activeSelf) return;

            if (Input.touchCount > 0)
            {

                if (Input.touches[0].tapCount == 2)
                {
                    ResetZoom();
                    return;
                }

                if (Input.touchCount == 1)
                {
                    panactive = true;
                }
                else if (Input.touchCount == 2)
                {
                    active = true;
                }

            }
            else
            {
                if (panactive)
                {
                    panactive = false;
                    TryOutDangerZone();
                    _isPaning = false;
                }
                if (active)
                {
                    active = false;
                    TryOutDangerZone();
                    _isPinching = false;
                }
                TryOutDangerZone();
            }
        }

        private void FixedUpdate()
        {
            TransformContent();

            if (Input.touchCount == 1)
            {
                if (!panactive) return;

                fingerPosition1 = Input.touches[0].position;

                if (!_isPaning)
                {
                    _isPaning = true;
                    OnPanStart();
                }
                OnPan();
            }


            else if (Input.touchCount == 2)
            {
                if (!active) return;

                fingerPosition1 = Input.touches[0].position;
                fingerPosition2 = Input.touches[1].position;

                if (!_isPinching)
                {
                    _isPinching = true;
                    OnPinchStart();
                }
                OnPinch();
            }
        }

        void OnPanStart()
        {
            startPostiton = fingerPosition1;
            ContentPositionOnStart = ContentRect.anchoredPosition;
        }

        private void OnPinchStart()
        {
            startScaleMagnitude = (fingerPosition1 - fingerPosition2).magnitude;
            ContentScaleOnStart = ContentRect.localScale;
        }

        void OnPan()
        {
            newPosition = fingerPosition1;// GetMidlePosition(fingerPosition1, fingerPosition2);
            oldMoveMagnitude = moveMagnitude;
            moveMagnitude = (newPosition - startPostiton).magnitude;

            if (Mathf.Abs(moveMagnitude - oldMoveMagnitude) > 1)
            {
                CalculateMoveContent();
            }
        }

        private void OnPinch()
        {
            newPosition = GetMidlePosition(fingerPosition1, fingerPosition2);

            oldScaleMagnitude = scaleMagnitude;

            newScaleMagnitude = (fingerPosition1 - fingerPosition2).magnitude;
            scaleMagnitude = startScaleMagnitude - newScaleMagnitude;

            if (Mathf.Abs(scaleMagnitude - oldScaleMagnitude) > 1)
            {
                CalculateScaleContent();
            }
        }

        private void TransformContent()
        {
            if (avalibleScaleSteps > 0)
            {
                ContentRect.localScale += stepScale * (avalibleScaleSteps / 2f);
                avalibleScaleSteps--;
            }

            if (avalibleMoveSteps > 0)
            {
                ContentRect.anchoredPosition += stepMove * (avalibleMoveSteps / 2f);
                avalibleMoveSteps--;
            }
        }

        private void CalculateScaleContent()
        {
            float tempScale = Mathf.Clamp(ContentScaleOnStart.x + (scaleMagnitude * ZoomSpeed), MinZoom, MaxZoom);

            TargetScale = new Vector2(tempScale, tempScale);

            stepScale = (TargetScale - (Vector2)ContentRect.localScale) / SmoothScale;
            avalibleScaleSteps = SmoothScale;
        }

        private void CalculateMoveContent()
        {
            Vector2 localScale = ContentRect.localScale;
            float maxMoveOffsetX = Mathf.Abs((ContentSizeDelta.x * localScale.x) - ContentSizeDelta.x) / 2 + MaxPermissibleWidthOffset;
            float maxMoveOffsetY = Mathf.Abs((ContentSizeDelta.y * localScale.y) - ContentSizeDelta.y) / 2 + MaxPermissibleHeightOffset;
            float offcetMoveX = newPosition.x - startPostiton.x;
            float offcetMoveY = newPosition.y - startPostiton.y;

            TargetPosition = new Vector2(
                Mathf.Clamp(ContentPositionOnStart.x + (offcetMoveX * MoveSpeed), -maxMoveOffsetX, maxMoveOffsetX),
                Mathf.Clamp(ContentPositionOnStart.y + (offcetMoveY * MoveSpeed), -maxMoveOffsetY, maxMoveOffsetY));

            stepMove = (TargetPosition - ContentRect.anchoredPosition) / SmoothMove;
            avalibleMoveSteps = SmoothMove;
        }

        private void TryOutDangerZone()
        {
            Vector2 localScale = ContentRect.localScale;
            float maxMoveOffsetX = Mathf.Abs((ContentSizeDelta.x * localScale.x) - ContentSizeDelta.x) / 2 + MinPermissibleWidthOffset;
            float maxMoveOffsetY = Mathf.Abs((ContentSizeDelta.y * localScale.y) - ContentSizeDelta.y) / 2 + MinPermissibleHeightOffset;

            TargetPosition = new Vector2(
                Mathf.Clamp(TargetPosition.x, -maxMoveOffsetX, maxMoveOffsetX),
                Mathf.Clamp(TargetPosition.y, -maxMoveOffsetY, maxMoveOffsetY));

            stepMove = (TargetPosition - ContentRect.anchoredPosition) / SmoothMove;
            avalibleMoveSteps = SmoothMove;
        }


        private Vector2 GetMidlePosition(Vector2 pos1, Vector2 pos2)
        {
            return Vector2.Lerp(pos1, pos2, 0.5f);
        }

    }

    //SECOND OPTION
    //ZOOM PAN WITH 2 FINGERS
    /*
	public class ImagePanZoom : MonoBehaviour
    {

        [Space(2)]
        [Header("Settings")]
        // Content need set Anchor Presets [Middle & Center]
        [SerializeField] private GameObject Content;

        [Space(10)]
        [SerializeField] private float MinZoom = 0.5f;
        [SerializeField] private float MaxZoom = 3f;
        [SerializeField] private float ZoomSpeed = -0.002f;
        [SerializeField] private float MoveSpeed = 1f;

        [Space(10)]
        [SerializeField] [Range(1, 10)] private int SmoothMove = 6;
        [SerializeField] [Range(1, 10)] private int SmoothScale = 6;

        [Space(10)]
        // Can be zero values
        [SerializeField] private float MinPermissibleHeightOffset;
        [SerializeField] private float MaxPermissibleHeightOffset;
        [SerializeField] private float MinPermissibleWidthOffset;
        [SerializeField] private float MaxPermissibleWidthOffset;

        private RectTransform ContentRect;
        private Vector2 ContentSizeDelta;

        private Vector2 TargetScale;
        private Vector2 TargetPosition;

        private float moveMagnitude;
        private float scaleMagnitude;

        private float oldMoveMagnitude;
        private float oldScaleMagnitude;
        private float startScaleMagnitude;
        private float newScaleMagnitude;

        private Vector2 ContentScaleOnStart;
        private Vector2 ContentPositionOnStart;

        private Vector2 startPostiton;
        private Vector2 newPosition;

        private Vector2 stepMove;
        private Vector3 stepScale;

        private int avalibleMoveSteps;
        private int avalibleScaleSteps;

        Vector2 fingerPosition1;
        Vector2 fingerPosition2;

        private bool _isPinching;
        private bool active;

        private void Awake()
        {
            Input.multiTouchEnabled = true;
        }

        void Start()
        {
            ContentRect = Content.GetComponent<RectTransform>();
            ContentSizeDelta = ContentRect.sizeDelta;
        }

        public void ResetZoom() 
        {
            ContentRect.localScale = Vector3.one;
            ContentRect.anchoredPosition = Vector2.zero;
        }

        private void Update()
        {
            if(Input.touchCount > 0) 
            {
                if(Input.touches[0].tapCount == 2)
                {
                    ResetZoom();
                    return;
                }
            }

            if (Input.touchCount == 2)
                active = true;
            else if (active)
            {
                active = false;
                TryOutDangerZone();
                _isPinching = false;
            }
        }

        private void FixedUpdate()
        {
            TransformContent();

            if (!active) return;
            if (Input.touchCount != 2) return;

            fingerPosition1 = Input.touches[0].position;
            fingerPosition2 = Input.touches[1].position;

            if (!_isPinching)
            {
                _isPinching = true;
                OnPinchStart();
            }
            OnPinch();
        }

        private void OnPinchStart()
        {
            startPostiton = GetMidlePosition(fingerPosition1, fingerPosition2);
            startScaleMagnitude = (fingerPosition1 - fingerPosition2).magnitude;
            ContentScaleOnStart = ContentRect.localScale;
            ContentPositionOnStart = ContentRect.anchoredPosition;
        }

        private void OnPinch()
        {
            newPosition = GetMidlePosition(fingerPosition1, fingerPosition2);

            oldMoveMagnitude = moveMagnitude;
            oldScaleMagnitude = scaleMagnitude;

            moveMagnitude = (newPosition - startPostiton).magnitude;

            newScaleMagnitude = (fingerPosition1 - fingerPosition2).magnitude;
            scaleMagnitude = startScaleMagnitude - newScaleMagnitude;


            if (Mathf.Abs(scaleMagnitude - oldScaleMagnitude) > 1 ||
                Mathf.Abs(moveMagnitude - oldMoveMagnitude) > 1)
            {
                CalculateScaleContent();
                CalculateMoveContent();
            }
        }

        private void TransformContent()
        {
            if (avalibleScaleSteps > 0)
            {
                ContentRect.localScale += stepScale * (avalibleScaleSteps / 2f);
                avalibleScaleSteps--;
            }

            if (avalibleMoveSteps > 0)
            {
                ContentRect.anchoredPosition += stepMove * (avalibleMoveSteps / 2f);
                avalibleMoveSteps--;
            }
        }

        private void CalculateScaleContent()
        {
            float tempScale = Mathf.Clamp(ContentScaleOnStart.x + (scaleMagnitude * ZoomSpeed), MinZoom, MaxZoom);

            TargetScale = new Vector2(tempScale, tempScale);

            stepScale = (TargetScale - (Vector2)ContentRect.localScale) / SmoothScale;
            avalibleScaleSteps = SmoothScale;
        }

        private void CalculateMoveContent()
        {
            Vector2 localScale = ContentRect.localScale;
            float maxMoveOffsetX = Mathf.Abs((ContentSizeDelta.x * localScale.x) - ContentSizeDelta.x) / 2 + MaxPermissibleWidthOffset;
            float maxMoveOffsetY = Mathf.Abs((ContentSizeDelta.y * localScale.y) - ContentSizeDelta.y) / 2 + MaxPermissibleHeightOffset;
            float offcetMoveX = newPosition.x - startPostiton.x;
            float offcetMoveY = newPosition.y - startPostiton.y;

            TargetPosition = new Vector2(
                Mathf.Clamp(ContentPositionOnStart.x + (offcetMoveX * MoveSpeed), -maxMoveOffsetX, maxMoveOffsetX),
                Mathf.Clamp(ContentPositionOnStart.y + (offcetMoveY * MoveSpeed), -maxMoveOffsetY, maxMoveOffsetY));

            stepMove = (TargetPosition - ContentRect.anchoredPosition) / SmoothMove;
            avalibleMoveSteps = SmoothMove;
        }

        private void TryOutDangerZone()
        {
            Vector2 localScale = ContentRect.localScale;
            float maxMoveOffsetX = Mathf.Abs((ContentSizeDelta.x * localScale.x) - ContentSizeDelta.x) / 2 + MinPermissibleWidthOffset;
            float maxMoveOffsetY = Mathf.Abs((ContentSizeDelta.y * localScale.y) - ContentSizeDelta.y) / 2 + MinPermissibleHeightOffset;

            TargetPosition = new Vector2(
                Mathf.Clamp(TargetPosition.x, -maxMoveOffsetX, maxMoveOffsetX),
                Mathf.Clamp(TargetPosition.y, -maxMoveOffsetY, maxMoveOffsetY));

            stepMove = (TargetPosition - ContentRect.anchoredPosition) / SmoothMove;
            avalibleMoveSteps = SmoothMove;
        }

        private Vector2 GetMidlePosition(Vector2 pos1, Vector2 pos2)
        {
            return Vector2.Lerp(pos1, pos2, 0.5f);
        }




    }

    */

}

