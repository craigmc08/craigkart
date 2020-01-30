using UnityEngine;

public class Track : MonoBehaviour {
    public TrackCheckpoints trackCheckpoints;
    
    void OnDrawGizmos() {
        if (trackCheckpoints != null) trackCheckpoints.DrawGizmos();
    }

    private void OnDisable() {
        trackCheckpoints.CleanUpCheckpoints();
    }
}
