using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    public Vector3[] controlPoints;

    public int gizmoResPerPoint = 10;

    public Vector3 GetPoint(float t)
    {
        float regionSize = 1 / ((float)controlPoints.Length);
        int region = (int)(t / regionSize);
        if (region == controlPoints.Length) region--;
        float regionT = t / regionSize - region;
        return Vector3.Lerp(controlPoints[region], controlPoints[(region + 1) % controlPoints.Length], regionT);
    }

    void OnDrawGizmos()
    {
        if (gizmoResPerPoint < 1 || controlPoints.Length < 1) return;
        float step = 1 / ((float)gizmoResPerPoint * controlPoints.Length);
        Gizmos.color = Color.cyan;
        for (float t = 0; t + step < 1; t += step)
        {
            Gizmos.DrawLine(GetPoint(t), GetPoint(t + step));
        }
        Gizmos.DrawLine(GetPoint(1 - step), GetPoint(1));
    }
}
