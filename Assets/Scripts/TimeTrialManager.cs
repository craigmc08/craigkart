using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class TimeTrialManager : MonoBehaviour
{
    public static TimeTrialManager instance;

    [Header("Settings")]
    public RaceProgress playerProgress;
    public PDriverController playerController;
    public float timeUntilLoadMenu = 5f;

    [Header("Sounds")]
    public AudioClip countdownSound;
    public AudioClip lapCountSound;
    public AudioClip finishSound;

    [Header("Other Things")]
    public bool raceInProgress = false;
    public bool finishedRace = false;
    public int lap = 1;
    
    public float startAt = 0;
    public float finishedTime = 0;

    public float finishedAt = 0;

    AudioSource audioSource;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        startAt = Time.time + 3f;
        playerController.controllable = false;
        playerController.runPhysics = false;

        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(countdownSound);
    }

    private void Update() {
        if (finishedRace && (Time.time - finishedAt >= timeUntilLoadMenu)) {
            SceneManager.LoadSceneAsync("Menu");
            return;
        }

        if (!raceInProgress && Time.time < startAt + Time.deltaTime) {
            if (Time.time >= startAt) {
                raceInProgress = true;
                playerController.controllable = true;
                playerController.runPhysics = true;
            }

            return;
        }

        var lastLap = lap;
        var lastFinished = finishedRace;

        lap = playerProgress.laps + 1; // Player progress starts with 0 laps
        if (lap < 1) lap = 1;

        if (lap > 3 && !finishedRace) {
            finishedTime = Timer;
            finishedAt = Time.time;
            raceInProgress = false; // End race
            finishedRace = true;
            playerController.controllable = false;
            // Don't stop physics, let player slow down to a stop

            lap = 3;
        }

        if (finishedRace && !lastFinished) {
            audioSource.PlayOneShot(finishSound);
        } else if (lastLap != lap) {
            audioSource.PlayOneShot(lapCountSound);
        }
    }

    public float Timer {
        get {
            if (finishedRace) return finishedTime;
            if (!raceInProgress) return 0;
            else return Time.time - startAt;
        }
    }

    public float TimeToStart {
        get {
            return startAt - Time.time;
        }
    }
}
