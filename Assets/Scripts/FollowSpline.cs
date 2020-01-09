using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSpline : MonoBehaviour
{
    public Spline spline;
    public float duration;

    public bool autorun = false;
    public bool loop = false;

    float startTime = 0;
    bool running = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Clamp01((Time.time - startTime) / duration);

        if (!running || (t >= 1 && !loop)) return;

        transform.position = spline.GetPoint(t);
        if (t >= 1 && loop) Reset();
    }

    public void Reset()
    {
        running = true;
        startTime = Time.time;
    }
    public void Stop()
    {
        running = false;
    }
    public void Play()
    {
        running = true;
        startTime = Time.time;
    }
}
