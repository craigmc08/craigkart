using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[AddComponentMenu("UI/Blur Panel")]
public class BlurPanel : Image
{
    public bool animate;
    public float time = 0.5f;
    public float delay = 0f;
    public Vector2 blurRange = new Vector2(0, 0.01f);

    CanvasGroup canvasGroup;

    protected override void Reset() {
        base.Reset();
        color = Color.black * 0.1f;
    }

    protected override void Awake() {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        
        if (Application.isPlaying) {
            if (animate) {
                UpdateBlur(0);
                LeanTween.value(gameObject, UpdateBlur, 0, 1, time).setDelay(delay);
            } else {
                UpdateBlur(1);
            }
        }
    }

    void UpdateBlur(float t) {
        material.SetFloat("_Size", Mathf.Lerp(blurRange.x, blurRange.y, t));
        canvasGroup.alpha = t;
    }
}
