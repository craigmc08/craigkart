using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointInfo : MonoBehaviour
{
    static Color keyColor = Color.red;
    static Color color = Color.blue;

    public Vector2 size;
    public float angle;
    public bool isKeyCheckpoint;

    public Vector3[] GetCorners()
    {
        float rads = angle * Mathf.Deg2Rad;
        Vector3 tangent = new Vector3(Mathf.Sin(rads), 0, Mathf.Cos(rads));
        Vector3 left = transform.position - size.x * tangent / 2;
        Vector3 right = transform.position + size.x * tangent / 2;
        Vector3 top = size.y * Vector3.up / 2;
        Vector3 bottom = -size.y * Vector3.up / 2;
        return new Vector3[]
        {
            left + top, right + top, right + bottom, left + bottom
        };
    }

    void OnDrawGizmos()
    {
        Vector3[] corners = GetCorners();
        Gizmos.color = isKeyCheckpoint ? keyColor : color;
        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % corners.Length]);
        }
    }

    public Vector3 Center { get => transform.position; }
    public float Width { get => size.x; }
    public float Height { get => size.y; }
    public float Angle { get => angle; }
    public float Rads { get => angle * Mathf.Deg2Rad; }
    public bool IsKeyCheckpoint { get => isKeyCheckpoint; }
}
