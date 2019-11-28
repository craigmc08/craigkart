using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Surface))]
public class SurfaceEditor : Editor
{
    Surface inst;

    void OnEnable()
    {
        inst = (Surface)target;
    }

    public override void OnInspectorGUI()
    {
        inst.surfaceType = (Surface.SurfaceType)EditorGUILayout.EnumPopup("Surface Type", inst.surfaceType);
        inst.driveType = (Surface.DriveType)EditorGUILayout.EnumFlagsField("Drive Type(s)", inst.driveType);
    }
}
