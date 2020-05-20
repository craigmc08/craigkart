using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI preambleText;
    public float displayLapTimeFor = 1f;
    public float displayFlashFrequency = 1f;
    public Color displayColor1;
    public Color displayColor2;

    TextMeshProUGUI text;

    void OnEnable() {
        text = GetComponent<TextMeshProUGUI>();
    }

    int lastCompletedLap = 0;
    bool flashing = false;

    void Update() {
        if (TimeTrialManager.instance != null) {
            if (TimeTrialManager.instance.highestCompletedLap > lastCompletedLap) {
                FlashLapTime();
                lastCompletedLap = TimeTrialManager.instance.highestCompletedLap;
            } else if (!flashing) {
                var time = TimeTrialManager.instance.Timer;
                text.text = TimeToString(time);
            }
        }
    }

    void FlashLapTime() {
        flashing = true;

        var mngr = TimeTrialManager.instance;
        var time = mngr.lapTimes[mngr.highestCompletedLap - 1];

        text.text = TimeToString(time);
        preambleText.text = "LAP TIME";

        var flashAnim = LeanTween.value(gameObject, UpdateColor, 0, displayLapTimeFor, displayLapTimeFor);
        flashAnim.setOnComplete(AfterFlashLapTime);
    }

    void UpdateColor(float t) {
        float v = 1 - Mathf.Cos(t * displayFlashFrequency * Mathf.PI * 2);

        Color col = Color.Lerp(displayColor1, displayColor2, v);
        text.color = col;
        preambleText.color = col;
    }

    void AfterFlashLapTime() {
        preambleText.text = "TIME";
        flashing = false;
        text.color = displayColor1;
        preambleText.color = displayColor1;
    }

    string TimeToString(float time) {
        TimeSpan t = TimeSpan.FromSeconds(time);
        string display = string.Format("{0:D2}:{1:D2}.{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
        return display;
    }
}
