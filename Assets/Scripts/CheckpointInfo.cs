using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Corners {
    public Vector3 TopLeft;
    public Vector3 TopRight;
    public Vector3 BottomRight;
    public Vector3 BottomLeft;

    public Vector3[] AsArray { get => new Vector3[] { TopLeft, TopRight, BottomRight, BottomLeft }; }
}

public class CheckpointInfo : MonoBehaviour
{
    static Color keyColor = Color.red;
    static Color color = Color.blue;

    public Vector2 size;
    public float angle;
    public bool isKeyCheckpoint;

    public Corners GetCorners()
    {
        float rads = angle * Mathf.Deg2Rad;
        Vector3 tangent = new Vector3(Mathf.Sin(rads), 0, Mathf.Cos(rads));
        Vector3 left = transform.position - size.x * tangent / 2;
        Vector3 right = transform.position + size.x * tangent / 2;
        Vector3 top = size.y * Vector3.up / 2;
        Vector3 bottom = -size.y * Vector3.up / 2;
        return new Corners
        {
            TopLeft = left + top,
            TopRight = right + top,
            BottomRight = right + bottom,
            BottomLeft = left + bottom
        };
    }

    void OnDrawGizmos()
    {
        Vector3[] corners = GetCorners().AsArray;
        Gizmos.color = isKeyCheckpoint ? keyColor : color;
        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % corners.Length]);
        }
        Gizmos.DrawRay(new Ray(Center, Normal));
    }

    public Vector3 Center { get => transform.position; }
    public float Width { get => size.x; }
    public float Height { get => size.y; }
    public float Angle { get => angle; }
    public float Rads { get => angle * Mathf.Deg2Rad; }
    public bool IsKeyCheckpoint { get => isKeyCheckpoint; }

    public Corners Corners { get => GetCorners(); }

    public Vector3 UVector { get => Corners.TopLeft - Corners.TopRight; }
    public Vector3 VVector { get => Corners.BottomLeft - Corners.TopLeft; }
    public Vector3 Normal { get => Vector3.Cross(UVector, VVector); }
}
