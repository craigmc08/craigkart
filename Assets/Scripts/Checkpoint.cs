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

public class Checkpoint : MonoBehaviour
{
    static Color keyColor = Color.red;
    static Color color = Color.blue;

    public Vector2 size;
    public float angle;
    public bool isKeyCheckpoint = false;
    public bool draw = false;
    public bool overrideDrawColor = false;
    public Color drawOverrideColor = Color.black;

    public Corners GetCorners()
    {
        float rads = angle * Mathf.Deg2Rad;
        Vector3 tangent = UVector;
        Vector3 bitangent = VVector;
        Vector3 left = transform.position - size.x * tangent / 2;
        Vector3 right = transform.position + size.x * tangent / 2;
        Vector3 top = size.y * bitangent / 2;
        Vector3 bottom = -size.y * bitangent / 2;
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
        if (!draw) return;

        Vector3[] corners = GetCorners().AsArray;
        Gizmos.color = isKeyCheckpoint ? keyColor : color;
        if (overrideDrawColor) Gizmos.color = drawOverrideColor;
        Color c = Gizmos.color;
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % corners.Length]);
            verts.Add(corners[i]);
        }
        Gizmos.DrawRay(new Ray(Center, Normal));

        int[] tris = new int[] { 0,1,2, 0,2,3, 0,2,1, 0,3,2 };
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        Gizmos.color = new Color(c.r, c.g, c.b, 0.4f);
        Gizmos.DrawMesh(mesh);
    }

    // math comes form distance-to-rectangle-notes.txt
    public float Distance(Vector3 Q) {
        Vector3 P = Vector3.ProjectOnPlane(Q - Center, Normal);
        // Gizmos.DrawSphere(P + Center, 0.2f);
        float i = P.x;
        float j = P.y;
        float k = P.z;
        float lxl = Mathf.Abs(i * Mathf.Cos(Rads) - k * Mathf.Sin(Rads));
        float lyl = Mathf.Abs(j);
        // Gizmos.DrawLine(Center, lxl * UVector + Center);
        // Gizmos.DrawLine(Center, lyl * VVector + Center);
        float dp = 0;
        if (lyl <= Height / 2 && lxl <= Width / 2) dp = 0;
        else if (lyl <= Height / 2) dp = lxl - Width / 2;
        else if (lxl <= Width / 2) dp = lyl - Height / 2;
        else {
            float dx = lxl - Width / 2;
            float dy = lyl - Height / 2;
            dp = Mathf.Sqrt(dx * dx + dy * dy);
        }
        // Gizmos.DrawLine(Q, P + Center);
        return Mathf.Sqrt((Q - Center - P).sqrMagnitude + dp*dp);
    }

    public Vector3 Center { get => transform.position; }
    public float Width { get => size.x; }
    public float Height { get => size.y; }
    public float Angle { get => angle; }
    public float Rads { get => angle * Mathf.Deg2Rad; }
    public bool IsKeyCheckpoint { get => isKeyCheckpoint; }

    public Corners Corners { get => GetCorners(); }

    public Vector3 UVector { get => new Vector3(-Mathf.Cos(Rads), 0, Mathf.Sin(Rads)); }
    public Vector3 VVector { get => Vector3.up; }
    public Vector3 Normal { get => new Vector3(Mathf.Sin(Rads), 0, Mathf.Cos(Rads)); }
}
