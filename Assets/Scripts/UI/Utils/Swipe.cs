//Diadrasis ©2023 - Stathis Georgiou
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Diadrasis.Rethymno.EnumsHolder;

namespace Diadrasis.Rethymno
{

    public class Swipe : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Button btnTarget;
        public DragDirection dragDirectionInvoker;

        Vector2 _lastPosition = Vector2.zero;

        //public EventHolder.Vector2D OnSwipeStart = new EventHolder.Vector2D();
        //public EventHolder.Vector2D OnSwipeEnd = new EventHolder.Vector2D();
        //public EventHolder.Vector2D OnSwipe = new EventHolder.Vector2D();

        [Space]
        public UnityEvent OnSwipeLeft;
        public UnityEvent OnSwipeRight;
        public UnityEvent OnSwipeUP;
        public UnityEvent OnSwipeDown;
        public UnityEvent OnSwipeReset;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _lastPosition = eventData.position;
            //OnSwipeStart.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //OnSwipeEnd.Invoke(eventData.position);

            Vector2 direction = eventData.position - _lastPosition;

            //Debug.Log(direction.sqrMagnitude);
            //Debug.Log(direction);

            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                if(direction.y < 0)
                {
                    OnSwipeDown?.Invoke();

                    if(dragDirectionInvoker == DragDirection.Down)//(directionTarget == -11)
                    {
                        if (btnTarget && btnTarget.gameObject.activeSelf) btnTarget.onClick.Invoke();
                    }
                }
                else
                {
                    OnSwipeUP?.Invoke();

                    if (dragDirectionInvoker == DragDirection.Up)//(directionTarget == 11)
                    {
                        if(btnTarget && btnTarget.gameObject.activeSelf) btnTarget.onClick.Invoke();
                    }
                }
            }
            else
            {
                if (direction.x < 0)
                {
                    OnSwipeLeft?.Invoke();

                    if (dragDirectionInvoker == DragDirection.Left)//(directionTarget == -11)
                    {
                        if (btnTarget) btnTarget.onClick.Invoke();
                    }
                }
                else
                {
                    OnSwipeRight?.Invoke();

                    if (dragDirectionInvoker == DragDirection.Right)//(directionTarget == 11)
                    {
                        if (btnTarget) btnTarget.onClick.Invoke();
                    }
                }
            }

            Invoke(nameof(DelayInvoke),0.25f);
        }

        void DelayInvoke()
        {
            OnSwipeReset?.Invoke();
        }
        

        public void OnDrag(PointerEventData eventData)
        {
           // Vector2 direction = eventData.position - _lastPosition;
           // _lastPosition = eventData.position;

           // OnSwipe.Invoke(direction);
        }
    }

}
