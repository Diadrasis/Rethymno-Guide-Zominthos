using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Spinaloga
{
    public class ScrollRectOffVertical : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        public ScrollRect sr, parentSR;
        public bool isVerticalDrag;

        //private void Start()
        //{
        //    parentSR = transform.parent.parent.parent.GetComponent<ScrollRect>();
        //    sr = GetComponent<ScrollRect>();
        //}

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector2 dragDelta = eventData.delta;
            isVerticalDrag = Mathf.Abs(dragDelta.y) > Mathf.Abs(dragDelta.x);
            if (isVerticalDrag)
            {
                sr.enabled = false;
                parentSR.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            parentSR.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isVerticalDrag)
            {
                parentSR.OnEndDrag(eventData);
            }
            isVerticalDrag = false;
            sr.enabled = true;
        }
    }

}
