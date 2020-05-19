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

[System.Serializable]
public class Checkpoint
{
    static Color keyColor = Color.red;
    static Color color = Color.blue;

    [SerializeField] Vector3 center = Vector2.zero;
    [SerializeField] Vector2 size = new Vector2(1, 1);
    [SerializeField] float angle = 0;
    [SerializeField] bool isKeyCheckpoint = false;
    bool draw = true;
    bool overrideDrawColor = false;
    Color drawOverrideColor = Color.black;

    public Vector3 scale = Vector3.one;

    public Checkpoint(Vector3 center, Vector2 size, float angle, bool isKeyCheckpoint) {
        this.center = center;
        this.size = size;
        this.angle = angle;
        this.isKeyCheckpoint = isKeyCheckpoint;
    }

    public Corners GetCorners()
    {
        float rads = angle * Mathf.Deg2Rad;
        Vector3 tangent = UVector;
        Vector3 bitangent = VVector;
        Vector3 left = Center - Width * tangent / 2;
        Vector3 right = Center + Width * tangent / 2;
        Vector3 top = Height * bitangent / 2;
        Vector3 bottom = -Height * bitangent / 2;
        return new Corners
        {
            TopLeft = left + top,
            TopRight = right + top,
            BottomRight = right + bottom,
            BottomLeft = left + bottom
        };
    }

    public void DrawGizmos()
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

    // math comes from distance-to-rectangle-notes.txt
    public float Distance(Vector3 Q) {
        Vector3 P = Vector3.ProjectOnPlane(Q - Center, Normal);
        float lxl = Mathf.Abs(P.x * Mathf.Cos(Rads) - P.z * Mathf.Sin(Rads));
        float lyl = Mathf.Abs(P.y);
        float dp = 0;
        if (lyl <= Height / 2 && lxl <= Width / 2) dp = 0;
        else if (lyl <= Height / 2) dp = lxl - Width / 2;
        else if (lxl <= Width / 2) dp = lyl - Height / 2;
        else {
            float dx = lxl - Width / 2;
            float dy = lyl - Height / 2;
            dp = Mathf.Sqrt(dx * dx + dy * dy);
        }
        return Mathf.Sqrt((Q - Center - P).sqrMagnitude + dp*dp);
    }

    public Vector3 Center { get => new Vector3(center.x * scale.x, center.y * scale.y, center.z * scale.z); }
    
    public float Width { get {
        var z = Mathf.Sin(Rads) * size.x * scale.z;
        var x = Mathf.Cos(Rads) * size.x * scale.x;

        return Mathf.Sqrt(z * z + x * x);
    } }
    public float Height { get => size.y * scale.y; }
    public float Angle { get => angle; }
    public float Rads { get => angle * Mathf.Deg2Rad; }
    public bool IsKeyCheckpoint { get => isKeyCheckpoint; }

    public Corners Corners { get => GetCorners(); }

    public Vector3 UVector { get => new Vector3(-Mathf.Cos(Rads), 0, Mathf.Sin(Rads)); }
    public Vector3 VVector { get => Vector3.up; }
    public Vector3 Normal { get => new Vector3(Mathf.Sin(Rads), 0, Mathf.Cos(Rads)); }

    public bool Draw { get => draw; set { draw = value; } }
    public bool OverrideDrawColor { get => overrideDrawColor; set { overrideDrawColor = value; } }
    public Color DrawOverrideColor { get => drawOverrideColor; set { drawOverrideColor = value; } }
}
