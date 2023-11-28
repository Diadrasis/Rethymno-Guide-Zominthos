//StaGe Games ©2022
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaGeGames.BestFit 
{

	public class ScrollBarVisibility : MonoBehaviour
	{
		
		void Start()
		{
			
		}

		void Update()
		{
			
		}

        //private bool hScrollingNeeded
        //{
        //    get
        //    {
        //        if (Application.isPlaying)
        //            return m_ContentBounds.size.x > m_ViewBounds.size.x + 0.01f;
        //        return true;
        //    }
        //}
        //private bool vScrollingNeeded
        //{
        //    get
        //    {
        //        if (Application.isPlaying)
        //            return m_ContentBounds.size.y > m_ViewBounds.size.y + 0.01f;
        //        return true;
        //    }
        //}

        //private readonly Vector3[] m_Corners = new Vector3[4];
        //private Bounds GetBounds()
        //{
        //    if (m_Content == null)
        //        return new Bounds();
        //    m_Content.GetWorldCorners(m_Corners);
        //    var viewWorldToLocalMatrix = viewRect.worldToLocalMatrix;
        //    return InternalGetBounds(m_Corners, ref viewWorldToLocalMatrix);
        //}

        //void UpdateOneScrollbarVisibility(bool xScrollingNeeded, bool xAxisEnabled, ScrollbarVisibility scrollbarVisibility, Scrollbar scrollbar)
        //{
        //    m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
        //    m_ContentBounds = GetBounds();
        //    if (scrollbar)
        //    {
        //        if (scrollbarVisibility == ScrollbarVisibility.Permanent)
        //        {
        //            if (scrollbar.gameObject.activeSelf != xAxisEnabled)
        //                scrollbar.gameObject.SetActive(xAxisEnabled);
        //        }
        //        else
        //        {
        //            if (scrollbar.gameObject.activeSelf != xScrollingNeeded)
        //                scrollbar.gameObject.SetActive(xScrollingNeeded);
        //        }
        //    }
        //}


    }

}
