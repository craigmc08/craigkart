using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FlowType {
        AllManual,
        AutoRows,
        AutoColumns,
        AutoBothSquare,
        AutoBothMinColumns,
        AutoBothMinRows,
    }

    public FlowType flow;
    public int rows;
    public int columns;
    public float spacing;

    public override void CalculateLayoutInputHorizontal() {
        base.CalculateLayoutInputHorizontal();

        int r = rows;
        int c = columns;

        if (flow != FlowType.AllManual) {
            int childCount = transform.childCount;
            if (flow == FlowType.AutoRows) {
                if (c == 0) return;
                r = Mathf.CeilToInt((float)childCount / c);
            } else if (flow == FlowType.AutoColumns) {
                if (r == 0) return;
                c = Mathf.CeilToInt((float)childCount / r);
            } else {
                int asSquareSize = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
                if (flow == FlowType.AutoBothSquare) {
                    r = asSquareSize;
                    c = asSquareSize;
                } else {
                    int minorSize = asSquareSize;
                    int majorSize = asSquareSize;
                    while ((minorSize - 1) * majorSize >= childCount) {
                        minorSize--;
                    }
                    if (flow == FlowType.AutoBothMinRows) {
                        r = minorSize;
                        c = majorSize;
                    } else {
                        r = majorSize;
                        c = minorSize;
                    }
                }
            }
        }

        if (r == 0 || c == 0) return;

        float parentWidth = rectTransform.rect.width - padding.left - padding.right - spacing * (c - 1);
        float parentHeight = rectTransform.rect.height - padding.top - padding.bottom - spacing * (r - 1);

        float cellWidth = parentWidth / (float)c;
        float cellHeight = parentHeight / (float)r;

        Vector2 size = new Vector2(cellWidth, cellHeight);
        
        for (int i = 0; i < rectChildren.Count; i++) {
            int rowIndex = i / c;
            int columnIndex = i % c;

            var item = rectChildren[i];

            float xPos = (size.x + spacing) * columnIndex + padding.left;
            float yPos = (size.y + spacing) * rowIndex + padding.top;

            SetChildAlongAxis(item, 0, xPos, size.x);
            SetChildAlongAxis(item, 1, yPos, size.y);
        }
    }

    public override void CalculateLayoutInputVertical() {

    }

    public override void SetLayoutHorizontal() {

    }

    public override void SetLayoutVertical() {

    }
}
