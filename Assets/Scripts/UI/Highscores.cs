using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;

public class Highscores : MonoBehaviour
{
    public TextMeshProUGUI bestLapText;
    public TextMeshProUGUI bestTimeText;

    public GameObject mainScreen;
    public GameObject highScores;

    TimeTrialData stats;

    // Start is called before the first frame update
    void Start()
    {
        LoadData();

        float bestLap = Mathf.Infinity;
        float bestTime = Mathf.Infinity;

        foreach (var stat in stats.races) {
            if (stat.bestLap < bestLap) bestLap = stat.bestLap;
            if (stat.time < bestTime) bestTime = stat.time;
        }

        if (bestLap == Mathf.Infinity) {
            bestLapText.text = "--";
        } else {
            bestLapText.text = TimeToString(bestLap);
        }

        if (bestTime == Mathf.Infinity) {
            bestTimeText.text = "--";
        } else {
            bestTimeText.text = TimeToString(bestTime);
        }
    }

    public void ShowHighscores() {
        mainScreen.SetActive(false);
        highScores.SetActive(true);
    }

    public void ShowMainScreen() {
        mainScreen.SetActive(true);
        highScores.SetActive(false);
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

    string TimeToString(float time) {
        TimeSpan t = TimeSpan.FromSeconds(time);
        string display = string.Format("{0:D2}:{1:D2}.{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
        return display;
    }
}
