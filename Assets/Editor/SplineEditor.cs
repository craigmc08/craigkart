using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    Spline instance;

    protected virtual void OnSceneGUI()
    {
        //int controlId = EditorGUIUtility.GetControlID("Free2DMoveHandle".GetHashCode(), FocusType.Keyboard);
        //switch (Event.current.type)
        //{
        //    case EventType.MouseDown:
        //        break;
        //    case EventType.MouseUp:
        //        break;
        //    case EventType.MouseDrag:
        //        break;
        //    case EventType.Repaint:
        //        break;
        //    case EventType.Layout:
        //        break;
        //}
        //instance = (Spline)target;
        //for (int i = 0; i < instance.controlPoints.Length; i++)
        //{
        //    EditorGUI.BeginChangeCheck();
        //    Vector3 newPointPos = Handles.PositionHandle(instance.controlPoints[i], Quaternion.identity);
        //    if (EditorGUI.EndChangeCheck())
        //    {
        //        Undo.RecordObject(instance, "Move Curve Point");
        //        instance.controlPoints[i] = newPointPos;
        //    }
        //}
    }
}
