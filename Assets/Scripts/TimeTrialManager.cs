using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

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

    public float[] lapTimes { get; private set; }
    public int highestCompletedLap { get; private set; }

    public static TimeTrialData stats = new TimeTrialData();

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

        playerController.GetComponent<PItemController>().GiveItem(new Items.TripleBoost());

        lapTimes = new float[3];

        LoadData();
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

        if (lap > lastLap) {
            highestCompletedLap = lastLap;
            audioSource.PlayOneShot(lapCountSound);

            // All initialized to zero, so the previous times sum
            // is the total of the lap times array.
            float previousLapSum = 0;
            for (int i = 0; i < lapTimes.Length; i++) {
                previousLapSum += lapTimes[i];
            }

            lapTimes[lastLap - 1] = Timer - previousLapSum;
        }

        if (finishedRace && !lastFinished) {
            float bestLap = lapTimes[0];
            for (int i = 1; i < lapTimes.Length; i++) {
                if (lapTimes[i] < bestLap) bestLap = lapTimes[i];
            }
            stats.races.Add(new RaceData(bestLap, finishedTime));
            
            SaveData();
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

    public void LoadData() {
        string destination = Application.persistentDataPath + "/timetrials.json";
        StreamReader file;

        if (File.Exists(destination)) file = new StreamReader(destination);
        else {
            stats = new TimeTrialData();
            return;
        }

        stats = JsonUtility.FromJson<TimeTrialData>(file.ReadToEnd());
        if (stats == null) stats = new TimeTrialData();
        file.Close();
    }

    public void SaveData() {
        string destination = Application.persistentDataPath + "/timetrials.json";
        StreamWriter file;

        file = new StreamWriter(destination);

        var output = JsonUtility.ToJson(stats);
        file.Write(output);
        file.Close();
    }
}

[System.Serializable]
public class TimeTrialData {
    public List<RaceData> races;

    public TimeTrialData() {
        races = new List<RaceData>();
    }
}

[System.Serializable]
public class RaceData {
    public float bestLap;
    public float time;

    public RaceData(float bestLap, float time) {
        this.bestLap = bestLap;
        this.time = time;
    }
}
