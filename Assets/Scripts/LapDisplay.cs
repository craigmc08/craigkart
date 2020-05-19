using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LapDisplay : MonoBehaviour
{
    TextMeshProUGUI text;

    void OnEnable() {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (TimeTrialManager.instance != null) {
            text.text = TimeTrialManager.instance.lap.ToString();
        }
    }
}
