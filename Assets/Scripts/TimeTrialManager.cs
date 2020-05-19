using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialManager : MonoBehaviour
{
    public static TimeTrialManager instance;

    public RaceProgress playerProgress;
    public PDriverController playerController;

    public bool raceInProgress = false;
    public bool finishedRace = false;
    public int lap = 1;
    
    public float startAt = 0;
    public float finishedTime = 0;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        startAt = Time.time + 3f;
        playerController.controllable = false;
        playerController.runPhysics = false;
    }

    private void Update() {
        if (!raceInProgress && Time.time < startAt + Time.deltaTime) {
            if (Time.time >= startAt) {
                raceInProgress = true;
                playerController.controllable = true;
                playerController.runPhysics = true;
            }

            return;
        }

        lap = playerProgress.laps + 1; // Player progress starts with 0 laps
        if (lap < 1) lap = 1;

        if (lap > 3) {
            finishedTime = Timer;
            raceInProgress = false; // End race
            finishedRace = true;
            playerController.controllable = false;
            // Don't stop physics, let player slow down to a stop
            lap = 3;
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
