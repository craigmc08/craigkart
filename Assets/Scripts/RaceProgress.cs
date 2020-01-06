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
        bool intersecting = SphereIntersectingCheckpoint(testSphere.position, testSphereRadius, testCheckpoint);

        Gizmos.color = intersecting ? Color.green : Color.red;
        Gizmos.DrawSphere(testSphere.position, testSphereRadius);
    }

    public bool SphereIntersectingCheckpoint(Vector3 point, float radius, CheckpointInfo checkpoint)
    {
        // Test if the point is within radius units of the plane formed by the corners of the checkpoint
        Vector3[] corners = checkpoint.GetCorners();
        Vector3 p = corners[0];
        Vector3 u = (corners[0] - corners[1]).normalized;
        Vector3 v = (corners[0] - corners[2]).normalized;
        Vector3 n = Vector3.Cross(u, v);

        Vector3 d = Vector3.Project(point - p, n);
        if (d.sqrMagnitude > radius * radius) return false;

        // Test if the coordinates in u,v space are within the bounds of the checkpoint
        Vector3 pointOnPlane = point - d;
        Vector3 pd = pointOnPlane - p;
        float uCoord = 0;
        float vCoord = 0;
        if (u.y * v.x != u.x * v.y && v.y != 0)
        {
            uCoord = (v.x * pd.y - v.y * pd.x) / (u.y * v.x - u.x * v.y);
            vCoord = (u.x * pd.y - u.y * pd.x) / (u.x * v.y - u.y * v.x);
        } else
        {
            // Invalid solution
            uCoord = -1000;
            vCoord = -1000;
        }

        Debug.Log(pd + ", " + uCoord + ", " + vCoord);
        return uCoord >= 0 && uCoord <= checkpoint.Width && vCoord >= 0 && vCoord <= checkpoint.Height;
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
