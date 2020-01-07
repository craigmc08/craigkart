using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceProgress : MonoBehaviour
{
    public CheckpointInfo[] checkpoints;
    public CheckpointInfo[] keyCheckpoints;

    int laps = 0;
    int currentKeyCheckpoint = 0;
    int currentCheckpoint = 0;
    float lapProgress = 0;

    public Transform testSphere;
    public float testSphereRadius;
    public CheckpointInfo testCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Intersection intersection = SphereIntersectingCheckpoint(testSphere.position, testSphereRadius, testCheckpoint);

        switch (intersection)
        {
            case Intersection.None: Gizmos.color = Color.red; break;
            case Intersection.Back: Gizmos.color = Color.blue; break;
            case Intersection.Front: Gizmos.color = Color.green; break;
        }
        Gizmos.DrawSphere(testSphere.position, testSphereRadius);
    }

    public enum Intersection
    {
        Back,
        None,
        Front
    };
    public Intersection SphereIntersectingCheckpoint(Vector3 point, float radius, CheckpointInfo checkpoint)
    {
        // Test if the point is within radius units of the plane formed by the corners of the checkpoint
        Corners corners = checkpoint.GetCorners();
        Vector3 p = checkpoint.Center;
        Vector3 u = checkpoint.UVector;
        Vector3 v = checkpoint.VVector;
        Vector3 n = checkpoint.Normal;

        Vector3 d = Vector3.Project(point - p, n);
        if (d.sqrMagnitude > radius * radius) return Intersection.None;

        // Test if the coordinates in u,v space are within the bounds of the checkpoint
        Vector3 pointOnPlane = point - d;
        Vector3 pd = pointOnPlane - p;

        if (u.y * v.x == u.x * v.y || v.y == 0) {
            // Invalid solution. This should be impossible, pd should be lineary dependent
            // on u and v because it is in the plane of u and v by its definition
            return Intersection.None;
        }

        float uCoord = (v.x * pd.y - v.y * pd.x) / (u.y * v.x - u.x * v.y);
        float vCoord = (u.x * pd.y - u.y * pd.x) / (u.x * v.y - u.y * v.x);

        if (Mathf.Abs(uCoord) <= checkpoint.Width / 2 && Mathf.Abs(vCoord) <= checkpoint.Height / 2)
        {
            return Vector3.Dot(n, d) >= 0 ? Intersection.Front : Intersection.Back;
        }
        return Intersection.None;
    }

    public float Progress { get => laps + lapProgress; }
    public float LapProgress { get => lapProgress; }
    public float KeyCheckpoint { get => currentKeyCheckpoint; }
    public CheckpointInfo[] LoadedKeyCheckpoints { get => new CheckpointInfo[]
    {
        keyCheckpoints[(currentKeyCheckpoint - 1) % keyCheckpoints.Length],
        keyCheckpoints[(currentKeyCheckpoint)],
        keyCheckpoints[(currentKeyCheckpoint + 1) % keyCheckpoints.Length]
    };
    }
}
