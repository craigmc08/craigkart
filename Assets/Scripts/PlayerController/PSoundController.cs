using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Rigidbody), typeof(PDriverController))]
public class PSoundController : MonoBehaviour
{
    AudioSource audioSource;
    PDriverController controller;
    Rigidbody rb;

    [Range(0, 1)]
    public float maxVolume = 1;
    public float minPitch = 0.5f;
    public float maxPitch = 1f;

    [Range(0,1)]
    public float engineSmoothing = 0.8f;

    public float overspeedPitch = 2f;

    public float timeForShift = 2f;

    float timeNearMax = 0;

    float previousFraction = 0;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<PDriverController>();
        rb = GetComponent<Rigidbody>();
    }
    void Update() {
        if (!controller.runPhysics) {
            if (audioSource.isPlaying) audioSource.Stop();
            return;
        } else if (!audioSource.isPlaying) {
            audioSource.volume = 0;
            audioSource.Play();
        }

        var speed = Vector3.Project(rb.velocity, controller.KartForward).magnitude;
        var speedFraction = speed / controller.maxForwardSpeed;

        // var additionalFraction = timeNearMax / timeForShift;
        // var additionalPitch = Mathf.LerpUnclamped(maxPitch, overspeedPitch, additionalFraction) - maxPitch;
        float additionalPitch = 0;

        var actualFraction = Mathf.Lerp(speedFraction, previousFraction, engineSmoothing);
        
        audioSource.volume = Mathf.Lerp(0, maxVolume, speedFraction);
        audioSource.pitch = Mathf.LerpUnclamped(minPitch, maxPitch, actualFraction) + additionalPitch;

        // if (speedFraction > 0.96f) {
        //     timeNearMax += Time.deltaTime;
        // } else if (timeNearMax > Time.deltaTime) {
        //     timeNearMax -= Time.deltaTime;
        // }

        // if (timeNearMax >= timeForShift + Random.Range(-0.3f, 0.3f)) {
        //     timeNearMax = Random.Range(-timeForShift, 0);
        // }

        previousFraction = actualFraction;
    }
}
