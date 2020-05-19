using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimeDisplay : MonoBehaviour
{
    TextMeshProUGUI text;

    void OnEnable() {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (TimeTrialManager.instance != null) {
            var time = TimeTrialManager.instance.Timer;
            
            TimeSpan t = TimeSpan.FromSeconds(time);
            string display = string.Format("{0:D2}:{1:D2}.{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
            text.text = display;
        }
    }
}
