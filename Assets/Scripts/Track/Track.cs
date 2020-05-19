using UnityEngine;

[ExecuteInEditMode]
public class Track : MonoBehaviour {
    public TrackCheckpoints trackCheckpoints;
    
    void OnDrawGizmos() {
        if (trackCheckpoints != null) trackCheckpoints.DrawGizmos();
    }

    private void OnDisable() {
        trackCheckpoints.CleanUpCheckpoints();
    }

    void Update() {
        foreach (var c in trackCheckpoints.GetCheckpoints()) {
            c.scale = transform.localScale;
        }
    }
}
