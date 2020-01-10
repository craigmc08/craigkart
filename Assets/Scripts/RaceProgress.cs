using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceProgress : MonoBehaviour
{
    public Checkpoint[] checkpoints;
    public Checkpoint[] keyCheckpoints;

    public int laps = 0;
    public int currentKeyCheckpoint = 0;
    public int currentCheckpoint = 0;
    public float lapProgress = 0;

    public float collisionRadius;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var checkpoint in checkpoints) {
            checkpoint.draw = false;
        }
    }

    int inKeyCheckpoint = -1;
    int inCheckpoint = -1;

    // Update is called once per frame
    void Update()
    {
        if (inKeyCheckpoint == -1) {
            for (int i = 0; i < keyCheckpoints.Length; i++) {
                if (!KeyCheckpointLoaded(i)) continue;
                Intersection intrscn = SphereIntersectingCheckpoint(transform.position, collisionRadius, keyCheckpoints[i]);
                if (intrscn != Intersection.None) {
                    inKeyCheckpoint = i;
                    if (intrscn == Intersection.Back) OnKeyCheckpointForward(inKeyCheckpoint);
                    else OnKeyCheckpointBackward(inKeyCheckpoint);
                }
            }
        } else {
            Intersection intrscn = SphereIntersectingCheckpoint(transform.position, collisionRadius, keyCheckpoints[inKeyCheckpoint]);
            if (intrscn == Intersection.None) {
                inKeyCheckpoint = -1;
            }
        }

        if (inCheckpoint == -1) {
            int keyCount = -1;
            for (int i = 0; i < checkpoints.Length; i++) {
                if (checkpoints[i].isKeyCheckpoint) {
                    keyCount++;
                }
                bool loaded = checkpoints[i].isKeyCheckpoint ? KeyCheckpointLoaded(keyCount) : CheckpointLoaded(keyCount);
                if (!loaded) continue;
                Intersection intrscn = SphereIntersectingCheckpoint(transform.position, collisionRadius, checkpoints[i]);
                if (intrscn != Intersection.None) {
                    inCheckpoint = i;
                    if (intrscn == Intersection.Back) OnCheckpointForward(inCheckpoint);
                    else OnCheckpointBackward(inCheckpoint);
                }
            }
        } else {
            if (SphereIntersectingCheckpoint(transform.position, collisionRadius, checkpoints[inCheckpoint]) == Intersection.None) {
                inCheckpoint = -1;
            }
        }
        
        // Compute progress
        float distToCurrent = checkpoints[currentCheckpoint].Distance(transform.position);
        float distToNext = checkpoints[Math.mod(currentCheckpoint + 1, checkpoints.Length)].Distance(transform.position);
        
        float m = (float)currentCheckpoint;
        float n = (float)checkpoints.Length;
        
        float segmentProgress = distToCurrent / (distToCurrent + distToNext);
        lapProgress = (m + segmentProgress) / n;
    }

    void OnCheckpointForward(int index) {
        currentCheckpoint = index;
    }

    void OnCheckpointBackward(int index) {
        currentCheckpoint = Math.mod(index - 1, checkpoints.Length);
    }

    int previousKeyCheckpoint = -1;
    void OnKeyCheckpointForward(int index) {
        // Debug.Log("passed through key checkpoint " + index + " forwards");
        previousKeyCheckpoint = currentKeyCheckpoint;

        if (index == 0 && previousKeyCheckpoint == keyCheckpoints.Length - 1) laps++;
        currentKeyCheckpoint = index;
    }

    void OnKeyCheckpointBackward(int index) {
        // Debug.Log("passed through key checkpoint " + index + " backwards");
        previousKeyCheckpoint = currentKeyCheckpoint;

        if (index == 0 && previousKeyCheckpoint == 0) laps--;
        currentKeyCheckpoint = Math.mod(index - 1, keyCheckpoints.Length);
    }

    private void OnDrawGizmos()
    {
        int keyCount = -1;
        for (int i = 0; i < checkpoints.Length; i++) {
            if (checkpoints[i].isKeyCheckpoint) {
                keyCount++;
            }
            bool loaded = checkpoints[i].isKeyCheckpoint ? KeyCheckpointLoaded(keyCount) : CheckpointLoaded(keyCount);
            checkpoints[i].draw = loaded;
            checkpoints[i].overrideDrawColor = (checkpoints[i].isKeyCheckpoint && keyCount == currentKeyCheckpoint) || i == currentCheckpoint;
            checkpoints[i].drawOverrideColor = checkpoints[i].isKeyCheckpoint ? Color.yellow : Color.green;
        }

        if (inKeyCheckpoint != -1) Gizmos.color = Color.red;
        else if (inCheckpoint != -1) Gizmos.color = Color.blue;
        else Gizmos.color = Color.black;
        
        Gizmos.DrawSphere(transform.position, collisionRadius);

        // Gizmos.color = Color.magenta;
        // float distToCurrent = checkpoints[currentCheckpoint].Distance(transform.position);
        // Gizmos.color = Color.green;
        // float distToNext = checkpoints[Math.mod(currentCheckpoint + 1, checkpoints.Length)].Distance(transform.position);
        // Debug.Log(distToCurrent + ", " + distToNext);
    }

    public enum Intersection
    {
        Back,
        None,
        Front
    };
    public Intersection SphereIntersectingCheckpoint(Vector3 point, float radius, Checkpoint checkpoint)
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

        // u ⊥ v and pd is linearly dependent with u and v.
        // this is a shortcut to find a linear combination of u and v to get pd
        float uCoord = Vector3.Dot(pd, u);
        float vCoord = Vector3.Dot(pd, v);

        if (Mathf.Abs(uCoord) <= checkpoint.Width / 2 && Mathf.Abs(vCoord) <= checkpoint.Height / 2)
        {
            return Vector3.Dot(n, d) >= 0 ? Intersection.Front : Intersection.Back;
        }
        return Intersection.None;
    }

    public bool KeyCheckpointLoaded(int id) {
        return id == currentKeyCheckpoint || Math.mod(id - 1, keyCheckpoints.Length) == currentKeyCheckpoint || Math.mod(id + 1, keyCheckpoints.Length) == currentKeyCheckpoint;
    }
    public bool CheckpointLoaded(int followsKey) {
        return followsKey == currentKeyCheckpoint || Math.mod(followsKey + 1, keyCheckpoints.Length) == currentKeyCheckpoint;
    }

    public float Progress { get => laps + lapProgress; }
    public float LapProgress { get => lapProgress; }
    public float KeyCheckpoint { get => currentKeyCheckpoint; }
    public Checkpoint[] LoadedKeyCheckpoints { get => new Checkpoint[]
    {
        keyCheckpoints[(currentKeyCheckpoint - 1) % keyCheckpoints.Length],
        keyCheckpoints[(currentKeyCheckpoint)],
        keyCheckpoints[(currentKeyCheckpoint + 1) % keyCheckpoints.Length]
    };
    }
}
