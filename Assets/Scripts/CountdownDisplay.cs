using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownDisplay : MonoBehaviour
{
    public TextMeshProUGUI text;
    public RectTransform slider;

    int lastTime = 100;

    public float animationLength = 0.2f;
    public float visibleLength = 0.2f;
    public float fadeOutLength = 0.6f;
    public float verticalOffset = -20;

    bool finishedCountdown = false;

    bool lastFinishedState = false;

    void Start() {
        UpdateTransform(1);
        UpdateOpacity(0);
    }

    void Update()
    {
        if (TimeTrialManager.instance != null) {
            bool finishedNow = TimeTrialManager.instance.finishedRace && !lastFinishedState;
            lastFinishedState = TimeTrialManager.instance.finishedRace;

            if (finishedCountdown && !finishedNow) {
                return;
            }

            int newTime = Mathf.FloorToInt(TimeTrialManager.instance.TimeToStart);
            if (Mathf.FloorToInt(newTime) < Mathf.FloorToInt(lastTime) || finishedNow) {
                UpdateTransform(1);

                lastTime = newTime;

                var newText = (newTime + 1).ToString();
                if (finishedNow) {
                    newText = "FINISHED";
                } else if (newTime < 0) {
                    newText = "GO";
                    finishedCountdown = true;
                }
                text.text = newText; 

                var opacityAnim = LeanTween.value(gameObject, UpdateOpacity, 0, 1, animationLength);
                opacityAnim.setOnComplete(FinishOpacity);
                LeanTween.value(gameObject, UpdateTransform, 1, 0, animationLength)
                    .setEaseOutBack();
            }
        }
    }

    void UpdateOpacity(float opacity) {
        Color oldColor = text.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, opacity);
        text.color = newColor;
    }

    void FinishOpacity() {
        LeanTween.value(gameObject, UpdateOpacity, 1, 0, fadeOutLength).setDelay(visibleLength);
    }

    void UpdateTransform(float t) {
        slider.anchoredPosition = new Vector2(0, t * verticalOffset);
    }
}
