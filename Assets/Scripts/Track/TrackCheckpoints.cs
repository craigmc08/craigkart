using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Track/TrackCheckpoints", order = 1)]
public class TrackCheckpoints : ScriptableObject {
    [SerializeField] Checkpoint[] checkpoints = new Checkpoint[0];

    public void DrawGizmos() {
        foreach (var checkpoint in checkpoints) checkpoint.DrawGizmos();
    }

    public Checkpoint[] GetCheckpoints() {
        return checkpoints;
    }
    public Checkpoint[] GetKeyCheckpoints() {
        List<Checkpoint> keyCheckpoints = new List<Checkpoint>();
        foreach (var checkpoint in checkpoints) {
            if (checkpoint.IsKeyCheckpoint) keyCheckpoints.Add(checkpoint);
        }
        return keyCheckpoints.ToArray();
    }

    public void CleanUpCheckpoints() {
        foreach (var checkpoint in checkpoints) {
            checkpoint.OverrideDrawColor = false;
            checkpoint.Draw = true;
        }
    }
}
