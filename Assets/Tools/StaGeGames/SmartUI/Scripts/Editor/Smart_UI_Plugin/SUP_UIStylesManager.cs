//StaGe Games ©2022
using StaGeGames.BestFit.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StaGeGames.BestFit.EditorSpace
{

	public static class SUP_UIStylesManager
	{

		public static Texture2D iconPivotNone, iconPivotCenter, iconPivotLeft, iconPivotTop, iconPivotBottom, iconPivotRight, iconPivotTopLeft, iconPivotTopRight, iconPivotBottomLeft, iconPivotBottomRight;
        public static Texture2D iconStickExternal, iconStickInternal, iconStickVertically, iconStickHorizontally, iconStickDiagonally;
        public static Texture2D iconEmpty;
        public static Texture2D iconMovable, iconStatic;
        public static Texture2D iconVisible, iconInVisible;
        public static Texture2D iconShow, iconHide;

        public static GUIContent contPivotNone, contPivotCenter, contPivotLeftCenter, contPivotRightCenter, contPivotTopCenter, contPivotTopLeft, contPivotTopRight, contPivotBottomCenter, contBottomLeft, contPivotBottomRight;
        public static GUIContent contStickExternal, contStickInternal, contStickHorizontally, contStickVertically, contStickDiagonally;
        public static GUIContent contEmpty;
        public static GUIContent contMovable, contStatic;
        public static GUIContent contVisible, contInVisible;
        public static GUIContent contShow, contHide;

        public static GUIContent[] pivotContentA;
        public static GUIContent[] pivotContentB;
        public static GUIContent[] pivotContentC;
        public static EnumUtils.PivotPoint[] PivotColA, PivotColB, PivotColC;

        public static GUIContent[] stickModeContentA;
        public static EnumUtils.StickMode[] StickModeColA;

        public static GUIContent[] stickPositionContentA;
        public static EnumUtils.MotionMode[] StickPositionColA;

        static SUP_UIStylesManager()
        {
			// Find to location of the SUPlugin Asset Folder (as users may have moved it)
			var tmproAssetFolderPath = BF_EditorUtils.packageRelativePath;

			if (EditorGUIUtility.isProSkin)
            {
                iconPivotLeft = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-left-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotTop = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-top-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotTopLeft = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-top-left.psd", typeof(Texture2D)) as Texture2D;
                iconPivotTopRight = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-top-right.psd", typeof(Texture2D)) as Texture2D;
                iconPivotBottom = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-bottom-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotBottomLeft = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-bottom-left.psd", typeof(Texture2D)) as Texture2D;
                iconPivotBottomRight = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-bottom-right.psd", typeof(Texture2D)) as Texture2D;
                iconPivotRight = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-right-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotCenter = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotNone = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-none.psd", typeof(Texture2D)) as Texture2D;

                iconStickInternal = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-internal.psd", typeof(Texture2D)) as Texture2D;
                iconStickExternal = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-external.psd", typeof(Texture2D)) as Texture2D;
                iconStickHorizontally = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-position-horizontally-letter.psd", typeof(Texture2D)) as Texture2D;
                iconStickVertically = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-position-vertcally-letter.psd", typeof(Texture2D)) as Texture2D;
                iconStickDiagonally = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-position-diagonally-letter.psd", typeof(Texture2D)) as Texture2D;

                iconEmpty = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/empty-selection.psd", typeof(Texture2D)) as Texture2D;

                iconMovable = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/movable.psd", typeof(Texture2D)) as Texture2D;
                iconStatic = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/static.psd", typeof(Texture2D)) as Texture2D;

                iconVisible = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/visible.psd", typeof(Texture2D)) as Texture2D;
                iconInVisible = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/inVisible.psd", typeof(Texture2D)) as Texture2D;

                iconShow = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/move-in.psd", typeof(Texture2D)) as Texture2D;
                iconHide = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/move-out.psd", typeof(Texture2D)) as Texture2D;
            }
            else
            {
                iconPivotLeft = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-left-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotTop = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-top-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotTopLeft = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-top-left.psd", typeof(Texture2D)) as Texture2D;
                iconPivotTopRight = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-top-right.psd", typeof(Texture2D)) as Texture2D;
                iconPivotBottom = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-bottom-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotBottomLeft = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-bottom-left.psd", typeof(Texture2D)) as Texture2D;
                iconPivotBottomRight = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-bottom-right.psd", typeof(Texture2D)) as Texture2D;
                iconPivotRight = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-right-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotCenter = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-center.psd", typeof(Texture2D)) as Texture2D;
                iconPivotNone = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/pivot-none.psd", typeof(Texture2D)) as Texture2D;

                iconStickInternal = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-internal.psd", typeof(Texture2D)) as Texture2D;
                iconStickExternal = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-external.psd", typeof(Texture2D)) as Texture2D;
                iconStickHorizontally = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-position-horizontally-letter.psd", typeof(Texture2D)) as Texture2D;
                iconStickVertically = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-position-vertcally-letter.psd", typeof(Texture2D)) as Texture2D;
                iconStickDiagonally = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/stick-position-diagonally-letter.psd", typeof(Texture2D)) as Texture2D;

                iconEmpty = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/empty-selection.psd", typeof(Texture2D)) as Texture2D;

                iconMovable = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/movable.psd", typeof(Texture2D)) as Texture2D;
                iconStatic = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/static.psd", typeof(Texture2D)) as Texture2D;

                iconVisible = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/visible.psd", typeof(Texture2D)) as Texture2D;
                iconInVisible = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/inVisible.psd", typeof(Texture2D)) as Texture2D;

                iconShow = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/move-in.psd", typeof(Texture2D)) as Texture2D;
                iconHide = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/Editor Resources/Textures/move-out.psd", typeof(Texture2D)) as Texture2D;
            }

            contPivotLeftCenter = new GUIContent(iconPivotLeft, "Left Center");
            contPivotRightCenter = new GUIContent(iconPivotRight, "Right Center");
            contPivotBottomCenter = new GUIContent(iconPivotBottom, "Bottom Center");
            contBottomLeft = new GUIContent(iconPivotBottomLeft, "Bottom Left");
            contPivotBottomRight = new GUIContent(iconPivotBottomRight, "Bottom Right");
            contPivotTopCenter = new GUIContent(iconPivotTop, "Top Center");
            contPivotTopLeft = new GUIContent(iconPivotTopLeft, "Top Left");
            contPivotTopRight = new GUIContent(iconPivotTopRight, "Top Right");
            contPivotCenter = new GUIContent(iconPivotCenter, "Center");
            contPivotNone = new GUIContent(iconPivotNone, "None");

            pivotContentA = new[]
            {
                contPivotTopLeft,
                contPivotLeftCenter,
                contBottomLeft
            };

            PivotColA = new[] { EnumUtils.PivotPoint.TopLeft, EnumUtils.PivotPoint.LeftCenter, EnumUtils.PivotPoint.BottomLeft };

            pivotContentB = new[]
            {
                contPivotTopCenter,
                contPivotCenter,
                contPivotBottomCenter
            };

            PivotColB = new[] { EnumUtils.PivotPoint.TopCenter, EnumUtils.PivotPoint.Center, EnumUtils.PivotPoint.BottomCenter };

            pivotContentC = new[]
            {
                contPivotTopRight,
                contPivotRightCenter,
                contPivotBottomRight
            };

            PivotColC = new[] { EnumUtils.PivotPoint.TopRight, EnumUtils.PivotPoint.RightCenter, EnumUtils.PivotPoint.BottomRight };


            contStickInternal = new GUIContent(iconStickInternal, "Internal");
            contStickExternal = new GUIContent(iconStickExternal, "External");

            stickModeContentA = new[]
            {
                contStickInternal,
                contStickExternal
            };

            StickModeColA = new[] { EnumUtils.StickMode.Internal, EnumUtils.StickMode.External };

            contStickHorizontally = new GUIContent(iconStickHorizontally, "Horizontally");
            contStickVertically = new GUIContent(iconStickVertically, "Vertically");
            contStickDiagonally = new GUIContent(iconStickDiagonally, "Diagonally");

            stickPositionContentA = new[]
            {
                contStickHorizontally,
                contStickVertically,
                contStickDiagonally
            };

            StickPositionColA = new[] { EnumUtils.MotionMode.Horizontally, EnumUtils.MotionMode.Vertically, EnumUtils.MotionMode.Diagonally };


            contEmpty = new GUIContent(iconEmpty, "");

            contMovable = new GUIContent(iconMovable, "Is Movable");
            contStatic = new GUIContent(iconStatic, "Is Static");

            contVisible = new GUIContent(iconVisible, "Visible");
            contInVisible = new GUIContent(iconInVisible, "Invisible");


            contShow = new GUIContent(iconShow, "Show");
            contHide = new GUIContent(iconHide, "Hide");

        }

        public static GUIContent GuiTextureMovable(bool val)
        {
            return val ? contMovable : contStatic;
        }

        public static GUIContent GuiTextureVisible(bool val)
        {
            return val ? contVisible : contInVisible;
        }

        public static GUIContent GuiTextureShowHide(bool val)
        {
            return !val ? contShow : contHide;
        }

        public static GUIContent GuiTextureStickModeSelected(EnumUtils.StickMode stickMode)
        {
            switch (stickMode)
            {
                case EnumUtils.StickMode.Internal:
                default:
                    return contStickInternal;
                case EnumUtils.StickMode.External:
                    return contStickExternal;
            }
        }

        public static GUIContent GuiTextureStickPositionSelected(EnumUtils.MotionMode motionMode)
        {
            switch (motionMode)
            {
                case EnumUtils.MotionMode.Horizontally:
                default:
                    return contStickHorizontally;
                case EnumUtils.MotionMode.Vertically:
                    return contStickVertically;
                case EnumUtils.MotionMode.Diagonally:
                    return contStickDiagonally;
            }
        }

        public static GUIContent GuiTextureMotionDirectionSelected(EnumUtils.MotionMode motionMode)
        {
            switch (motionMode)
            {
                case EnumUtils.MotionMode.Horizontally:
                default:
                    return contStickHorizontally;
                case EnumUtils.MotionMode.Vertically:
                    return contStickVertically;
                case EnumUtils.MotionMode.Diagonally:
                    return contStickDiagonally;
            }
        }

        public static GUIContent GuiTexSelected(EnumUtils.PivotPoint pivotMode)
        {
            switch (pivotMode)
            {
                case EnumUtils.PivotPoint.None:
                default:
                    return contPivotNone;
                case EnumUtils.PivotPoint.Center:
                    return contPivotCenter;
                case EnumUtils.PivotPoint.BottomCenter:
                    return contPivotBottomCenter;
                case EnumUtils.PivotPoint.BottomLeft:
                    return contBottomLeft;
                case EnumUtils.PivotPoint.BottomRight:
                    return contPivotBottomRight;
                case EnumUtils.PivotPoint.TopCenter:
                    return contPivotTopCenter;
                case EnumUtils.PivotPoint.TopLeft:
                    return contPivotTopLeft;
                case EnumUtils.PivotPoint.TopRight:
                    return contPivotTopRight;
                case EnumUtils.PivotPoint.LeftCenter:
                    return contPivotLeftCenter;
                case EnumUtils.PivotPoint.RightCenter:
                    return contPivotRightCenter;

            }
        }


    }

}
