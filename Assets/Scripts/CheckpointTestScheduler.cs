using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FollowSpline))]
public class CheckpointTestScheduler : MonoBehaviour
{
    public float waitToStart = 1;

    float loadedAt = 0;
    bool started = false;

    void Start() {
        loadedAt = Time.time;
    }

    void Update() {
        if (!started && loadedAt + waitToStart < Time.time) {
            GetComponent<FollowSpline>().Play();
            started = true;
        }
    }
}
