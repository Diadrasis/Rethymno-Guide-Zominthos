//StaGe Games ©2022
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace StaGeGames.BestFit
{
    [AddComponentMenu("UI/BestFit/Smart GridLayout")]
    [RequireComponent(typeof(BestFitter))]
    [DisallowMultipleComponent]
    public class SmartGridLayout : LayoutGroup
    {
        public enum Alignment
        {
            Horizontal,
            Vertical
        }

        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns,
            FixedBoth
        }

        public Alignment alignment;

        [Space]
        public FitType fitType;

        [Min(1)]
        public int columns;

        [Min(1)]
        public int rows;

        [Space]
        [Min(0)]
        public Vector2 spacing;
        public Vector2 cellSize;

        public bool fitX = true;
        public bool fitY = true;
        public bool NudgeLastItemsOver;

        public float parentWidth, parentHeight;

        public override void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputHorizontal();

            float sqrRt = Mathf.Sqrt(transform.childCount);
            parentWidth = Mathf.Abs(rectTransform.rect.width);
            parentHeight = Mathf.Abs(rectTransform.rect.height);

            if (float.IsNaN(parentWidth) || float.IsNaN(parentHeight))
            {
                Debug.LogWarning("Warning! Avoid using too small values...");
                spacing = Vector2.one;
                return;
            }
            if(parentWidth <= spacing.x || parentHeight <= spacing.y)
            {
                Debug.LogWarning("Warning! Avoid using too small values...Spacing is bigger than parents size! Resetting Spacing");
                spacing = Vector2.one;
                return;
            }

            float cellWidth;
            float cellHeight;

            switch (fitType)
            {
                case FitType.Uniform:
                default:
                    rows = Mathf.CeilToInt(sqrRt);
                    columns = Mathf.CeilToInt(sqrRt);
                    rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                    columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                    break;
                case FitType.Width:
                    rows = Mathf.CeilToInt(sqrRt);
                    columns = Mathf.CeilToInt(sqrRt);
                    rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                    break;
                case FitType.Height:
                    rows = Mathf.CeilToInt(sqrRt);
                    columns = Mathf.CeilToInt(sqrRt);
                    columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                    break;
                case FitType.FixedRows:
                    columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                    break;
                case FitType.FixedColumns:
                    rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                    break;
                case FitType.FixedBoth:
                    break;

            }

            switch (alignment)
            {
                case Alignment.Horizontal:
                    cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * (columns - 1)) - (padding.left / (float)columns) - (padding.right / (float)columns);
                    cellHeight = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * (rows - 1)) - (padding.top / (float)rows) - (padding.bottom / (float)rows);
                    break;
                case Alignment.Vertical:
                    cellHeight = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * (columns - 1)) - (padding.left / (float)columns) - (padding.right / (float)columns);
                    cellWidth = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * (rows - 1)) - (padding.top / (float)rows) - (padding.bottom / (float)rows);
                    break;
                default:
                    cellHeight = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * (columns - 1)) - (padding.left / (float)columns) - (padding.right / (float)columns);
                    cellWidth = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * (rows - 1)) - (padding.top / (float)rows) - (padding.bottom / (float)rows);
                    break;
            }

            cellSize.x = fitX ? (cellWidth <= 0 ? cellSize.x : cellWidth) : cellSize.x;
            cellSize.y = fitY ? (cellHeight <= 0 ? cellSize.y : cellHeight) : cellSize.y;

            if (float.IsNaN(cellSize.x) || float.IsNaN(cellSize.y))
            {
                Debug.LogWarning("Warning! Avoid using too small values...");
                return;
            }

            int columnCount = 0;
            int rowCount = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                var item = rectChildren[i];
                float posX;
                float posY;
                float xLastItemOffset = 0;
                
                switch (alignment)
                {
                    case Alignment.Horizontal:
                        rowCount = i / columns;
                        columnCount = i % columns;
                        if (NudgeLastItemsOver && rowCount == (rectChildren.Count / columns)) { xLastItemOffset = (cellSize.x + padding.left) / 2; }
                        break;
                    case Alignment.Vertical:
                        rowCount = i / rows;
                        columnCount = i % rows;
                        if (NudgeLastItemsOver && rowCount == (rectChildren.Count / rows)) { xLastItemOffset = (cellSize.x + padding.left) / 2; }
                        break;
                    default:
                        rowCount = i / rows;
                        columnCount = i % rows;
                        if (NudgeLastItemsOver && rowCount == (rectChildren.Count / rows)) { xLastItemOffset = (cellSize.x + padding.left) / 2; }
                        break;
                }

                posX = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left + xLastItemOffset;
                posY = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                switch (m_ChildAlignment)
                {
                    case TextAnchor.UpperLeft:
                    default:
                        //No need to change posX;
                        //No need to change posY;
                        break;
                    case TextAnchor.UpperCenter:
                        //Center posX
                        posX += 0.5f * (parentWidth + (spacing.x + padding.left + padding.left) - (columns * (cellSize.x + spacing.x + padding.left))); 
                        //No need to change posY;
                        break;
                    case TextAnchor.UpperRight:
                        //Flip posX to go bottom-up
                        posX = -posX + parentWidth - cellSize.x; 
                        //No need to change posY;
                        break;
                    case TextAnchor.MiddleLeft:
                        //No need to change posX;
                        //Center posY
                        posY += (0.5f * (parentHeight + (spacing.y + padding.top + padding.top) - (rows * (cellSize.y + spacing.y + padding.top))));
                        break;
                    case TextAnchor.MiddleCenter:
                        //Center posX
                        posX += 0.5f * (parentWidth + (spacing.x + padding.left + padding.left) - (columns * (cellSize.x + spacing.x + padding.left)));
                         //Center posY
                        posY += 0.5f * (parentHeight + (spacing.y + padding.top + padding.top) - (rows * (cellSize.y + spacing.y + padding.top)));
                        break;
                    case TextAnchor.MiddleRight:
                        //Flip posX to go bottom-up
                        posX = -posX + parentWidth - cellSize.x;
                        //Center posY
                        posY += 0.5f * (parentHeight + (spacing.y + padding.top + padding.top) - (rows * (cellSize.y + spacing.y + padding.top)));
                        break;
                    case TextAnchor.LowerLeft:
                        //No need to change posX;
                        //Flip posY to go Right to Left
                        posY = -posY + parentHeight - cellSize.y;
                        break;
                    case TextAnchor.LowerCenter:
                        //Center posX
                        posX += (0.5f * (parentWidth + (spacing.x + padding.left + padding.left) - (columns * (cellSize.x + spacing.x + padding.left)))); 
                        //Flip posY to go Right to Left
                        posY = -posY + parentHeight - cellSize.y; 
                        break;
                    case TextAnchor.LowerRight:
                        //Flip posX to go bottom-up
                        posX = -posX + parentWidth - cellSize.x;
                        //Flip posY to go Right to Left
                        posY = -posY + parentHeight - cellSize.y; 
                        break;

                }

                SetChildAlongAxis(item, 0, posX, cellSize.x);
                SetChildAlongAxis(item, 1, posY, cellSize.y);
            }
        }

        public override void SetLayoutHorizontal() { }

        public override void SetLayoutVertical() { }

//#if UNITY_EDITOR
//        private new void Reset()
//        {
//            if (!GetComponent<MaskableGraphic>())
//            {
//                if (EditorUtility.DisplayDialog("Invalid operation.", SmartEditorParams.msgRemoveSmartUIComponent, "OK"))
//                {
                    
//                }
//                DestroyImmediate(this);
//            }
//        }
//#endif

    }

}
