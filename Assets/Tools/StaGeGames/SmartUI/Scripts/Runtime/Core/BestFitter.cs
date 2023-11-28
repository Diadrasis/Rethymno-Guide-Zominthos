//StaGe Games ©2022
using StaGeGames.BestFit.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StaGeGames.BestFit 
{
    [AddComponentMenu("UI/BestFit/Best Fitter")]
	public class BestFitter : BestResize
	{

        protected override void Start()
		{
            base.Start();
		}

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void SetPivot(EnumUtils.PivotPoint pivotPoint)
        {
            m_pivotMode = pivotPoint;
        }

        public void SetInitPos(EnumUtils.InitialPositionMode positionMode, Vector2 pos)
        {
            m_InitialPositionMode = positionMode;

            switch (positionMode)
            {
                case EnumUtils.InitialPositionMode.Static:
                default:
                    initialStaticPos = pos;
                    break;
                case EnumUtils.InitialPositionMode.SelfSizePercentage:
                    initVectorSelfValues = pos;
                    break;
                case EnumUtils.InitialPositionMode.ParentSizePercentage:
                    initVectorParentValues = pos;
                    break;
            }
        }

        /// <example>
        ///    private void TestFunction()
        ///    {
        ///        BestFitParentOptions options = new BestFitParentOptions
        ///        {
        ///            scaleBothAxes = false,
        ///            percentageScaleWidth = 60f,
        ///            isStaticHeight = true,
        ///            staticHeight = 120f,
        ///            reduceFinalWidth = true,
        ///            reducedWidthPercentageValue = 10f
        ///        };
        ///        this.AdaptParentSize(options);
        ///    }
        /// The above method will resize the rect transform to size
        /// width = (60% - 10% reduce)54% of parent width , height = 120
        /// </example>
        public void AdaptParentSize(BestFitParentOptions options)
        {
            FitUtils.SetParentOptions(this, options);
            Init();
        }



    }

}
