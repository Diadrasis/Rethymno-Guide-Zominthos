//StaGe Games ©2021
using StaGeGames.BestFit.Utils;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace StaGeGames.BestFit.EditorSpace
{

	public static class BF_EditorParams
	{

		#region smart resize editor static fields

		public static GUIContent gc_InitialPositionLabel(int val) 
		{
            switch (val)
            {
				case 0:
				default:
					return new GUIContent("Initial Position", "Any custom values");
				case 1:
					return new GUIContent("Initial Position", "Maximum values equals target's size delta");
				case 2:
					return new GUIContent("Initial Position", "Maximum values equals parent's size delta\nKeeps target into parent's rect");
			}
        }

		public static readonly GUIContent gc_UpdateFitLabel = new GUIContent("Update Fit", "Should always check to fit properly? Enable this if parent size changes during application lifetime");
		public static readonly GUIContent gc_UseCanvasAsParentLabel = new GUIContent("Use Canvas as parent", "Mainly if real parent has adapt child size");
		public static readonly GUIContent gc_BestFitChildrenButtonLabel = new GUIContent("Best Fit Children", "Keeps children relative position and size");
		public static readonly GUIContent gc_StaticWidthLabel = new GUIContent("Width Static", "Set width size.");
		public static readonly GUIContent gc_StaticHeightLabel = new GUIContent("Height Static", "Set height size.");
		public static readonly GUIContent gc_ResizeModeLabel = new GUIContent("Size Mode");
		public static readonly GUIContent gc_LockAxesLabel = new GUIContent("Both Sides");
		public static readonly GUIContent gc_LockAxisXLabel = new GUIContent("Lock Width");
		public static readonly GUIContent gc_LockAxisYLabel = new GUIContent("Lock Height");
		public static readonly GUIContent gc_IgnoreFitWidthLabel = new GUIContent("Ignore Fit Width", "If true, Width can be freely adjusted");
		public static readonly GUIContent gc_IgnoreFitHeightLabel = new GUIContent("Ignore Fit Height", "If true, Height can be freely adjusted");
		public static readonly GUIContent gc_PercentageScaleLabel = new GUIContent("Percentage Size [%]");
		public static readonly GUIContent gc_PercentageScaleWidthLabel = new GUIContent("Width Size [%]");
		public static readonly GUIContent gc_PercentageScaleHeightLabel = new GUIContent("Height Size [%]");
		public static readonly GUIContent gc_ReduceWidthLabel = new GUIContent("Decrease Width");
		public static readonly GUIContent gc_ReduceHeightLabel = new GUIContent("Decrease Height");
		public static readonly GUIContent gc_IncreaseWidthLabel = new GUIContent("Increase Width");
		public static readonly GUIContent gc_IncreaseHeightLabel = new GUIContent("Increase Height");
		public static readonly GUIContent gc_IsStaticLabel = new GUIContent("Is Static");
		public static readonly GUIContent gc_StaticValueLabel = new GUIContent("[Static] value");
		public static readonly GUIContent gc_PercentageValueLabel = new GUIContent("[%] decrease value");
		public static readonly GUIContent gc_EnvelopeParentLabel = new GUIContent("Envelope Parent");
		public static readonly GUIContent gc_FitInParentLabel = new GUIContent("Fit in Parent");
		public static readonly GUIContent gc_FitInParentLabelAlwaysTrueLabel = new GUIContent("Fit in Parent [Always True]");
		public static readonly GUIContent gc_IgnoreWidthLabel = new GUIContent("Ignore Width", infoFitParentIgnoreWidth);
		public static readonly GUIContent gc_FitInParentReduceSizeLabel = new GUIContent("Reduce Size", infoFitParentShouldReduce);
		public static readonly GUIContent gc_PercentageTextureScaleValueLabel = new GUIContent("Texture Scale [%]");
		
		public static readonly GUIContent gc_MockRatioModeLabel = new GUIContent("Mock Ratio Mode");
		public static readonly string gc_AspectRatioPresetsLabel = "Aspect Ratio Presets";
		public static readonly GUIContent gc_IsPortraitLabel = new GUIContent("Is Portrait");
		public static readonly GUIContent gc_IsReversedLabel = new GUIContent("Reverse", "Portrait >>[]<< Landscape");
		public static readonly GUIContent gc_CustomAspectRatioLabel = new GUIContent("Custom Aspect Ratio");
		public static readonly GUIContent gc_PrefferedWidthLabel = new GUIContent("Preffered Width");
		public static readonly GUIContent gc_PrefferedHeightLabel = new GUIContent("Preffered Height");
		public static readonly GUIContent gc_ReduceSizeLabel = new GUIContent("Decrease Size");

		public static readonly GUIContent gc_AlignHorizontalLabel = new GUIContent("Horizontal");
		public static readonly GUIContent gc_AlignVerticalLabel = new GUIContent("Vertical");
		public static readonly GUIContent gc_MaxSizeKeepParentSizeLabel = new GUIContent("Max Size to Parent's Size", "Limits max size to equals parent size.");
		public static readonly GUIContent gc_MaxWidthLabel = new GUIContent("Max Width", "Limits width to max value. If value is 0 then it is ignored");
		public static readonly GUIContent gc_MaxHeightLabel = new GUIContent("Max Height", "Limits height to max value. If value is 0 then it is ignored");
		public static readonly GUIContent gc_MinWidthLabel = new GUIContent("Min Width", "Limits width to min value. Value can not be greater than max width and smaller than 10 units");
		public static readonly GUIContent gc_MinHeightLabel = new GUIContent("Min Height", "Limits height to min value. Value can not be greater than max height and smaller than 10 units");
		#endregion

		#region Warnings - Infos - Tooltips

		public const string msgSmartResizeWarning = "Affects any item with SmartResize script on it.";
		public const string msgSmartResizeInactiveWarning = "Affects any item with SmartResize script on it.\nEven if the item is disabled in hierarchy.";
		public const string msgRecordInitialPosition = "Move target in scene view to automatically save start position";

		
		public const string msgTargetParentNull = "Target Parent is Null: Smart Resize is affected by this RectTransform";
		public const string msgTargetParentNotReal = "Target is NOT under Canvas. Set target as child of a canvas.";
		public const string msgTargetScaleNotOne = "Target scale is NOT Vector3 one, fix scale!";
		public const string msgInvalidOperationTitle = "Invalid operation.";
		public const string msgImageSpriteNullDesc = "Texture is Null!\nAssign a texture first.";
		public const string infoFitParentIgnoreWidth = "Fit height to parent's height but ignores width";
		public const string infoMockRatioFitParentIgnoreWidth = "Keeps height of targetRect same with parentRect's height but width is relative to height of Custom aspect ratio. This means that targetRect's width could be bigger or smaller than parentRect's width.";
		public const string infoFitParentShouldReduce = "Keeps aspect ratio and adds space distance from parent's size. Width and/or height, whichever is closest to the parent size.";

		public const string msgPlayMode = "Editing won't be saved while in play mode";

		public const string msgRemoveSmartUIComponent = "SmartUI components are only allowed on GameObjects with a <Graphic> component.\n\n[SmartUI] component has been removed!";
		public const string msgSmartResizeOnce = "Can't add more than one [SmartResize] component.\n\n[SmartResize] has been removed!";

		public const string msgUVRectWarning = "Texture coordinates [UVRect] values are not taking into account for size calculations!";

		public const string colButtonImportantPro = "#000000";//"#FF5400"; //
		public const string colButtonImportant = "#C98B00";// "#850015";

		public static void ShowHelpBox(string msg, float leftSpace, MessageType messageType, float height = 1.5f)
        {
			EditorGUI.HelpBox(RectInfoSingleLine(leftSpace, height), msg, messageType);
		}

		public static void ShowShadowLabel(string msg, float height = 1.5f)
        {
			EditorGUI.DropShadowLabel(RectInfoSingleLine(0f, height), msg);
		}

		public static void ShowLabel(string msg, float leftSpace, Color col, float height = 1.5f)
		{
			GUI.color = col;
			EditorGUI.LabelField(RectInfoSingleLine(leftSpace, height), msg);
			GUI.color = Color.white;
		}			

		#endregion

		#region Rects

		public static Rect RectInfoSingleLine(float leftSpace, float height = 1.5f)
        {
			height = Mathf.Clamp(height, 1f, 100f);
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * height);
			rect.x += leftSpace;
			rect.width -= leftSpace;
			return rect;
		}

		public static Rect CustomRect(float leftSpace, float width = 5f, float height = 1.5f)
		{
			height = Mathf.Clamp(height, 1f, 100f);
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * height);
			rect.x += leftSpace;
			rect.width = width;
			rect.width -= leftSpace;
			return rect;
		}

		#endregion

		#region Aspect Ratio

		public static readonly string[] aspectRatios = new string[] { "16:9", "16:10", "18:9", "21:9", "32:9", "6:5", "5:4", "4:3", "3:2", "1:1" };

        private static int GreatestCommonDivisor(int a, int b)
        {
            return (b == 0) ? a : GreatestCommonDivisor(b, a % b);
        }

        public static string GetAspectRatio(int width, int height)
        {
            int gcd = GreatestCommonDivisor(width, height);
            return width > height ? string.Format("{0}/{1}", width / gcd, height / gcd) : string.Format("{0}/{1}", height / gcd, width / gcd);
        }

        public static Vector2 GetRatioFromFloatingPointNumber(float ar)
        {
			int w = 0, h = 0;
			float minVal = Mathf.Infinity;

			for (int n = 1; n < 20; ++n)
			{
				int m = (int)(ar * n + 0.5); //rounding
				float calc = Mathf.Abs(ar - (float)m / n); //nearest value
				if (minVal > calc)
				{
					minVal = calc;
					if (calc < 0.01) { w = m; h = n; }
				}
			}
			return new Vector2(w, h);
		}


        public static string GetStringRatioFromFloatingPointNumber(float ar)
        {
            int w = 0, h = 0;
            float minVal = Mathf.Infinity;

            for (int n = 1; n < 50; ++n)
            {
                int m = (int)(ar * n + 0.5); //rounding
                float calc = Mathf.Abs(ar - (float)m / n); //nearest value
                if (minVal > calc)
                {
                    minVal = calc;
                    if (calc < 0.01) { w = m; h = n; }
                }
            }
            return string.Format("{0}/{1}", w, h);
        }

		#endregion

		public static readonly string smtitle = "wqk8Y29sb3I9IzAwQTc5RD5TdGE8L2NvbG9yPjxjb2xvcj0jQkUxRDJFPkdlPC9jb2xvcj4gPGNvbG9yPSNGNzk0MUQ+R2FtZXM8L2NvbG9yPiAyMDIyIC0gPGNvbG9yPSMwMDZEQUI+aHR0cHM6Ly9zdGFnZWdhbWVzLmV1LzwvY29sb3I+";

		public static string DataFormat(string val)
		{
			byte[] data = Convert.FromBase64String(val);
			return Encoding.UTF8.GetString(data);
		}

		public static void StaGeLabel()
		{
			GUI.backgroundColor = BF_EditorUtils.HexColor("#161616", Color.gray);
			string s = DataFormat(smtitle);
			if (GUILayout.Button(s, BF_EditorUtils.GUIStyleLogo())) { Application.OpenURL("http://stagegames.eu"); }
		}


	}

}
