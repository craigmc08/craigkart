using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UITweenIn : MonoBehaviour
{
    public float duration = 0.2f;
    public float delay = 0f;

    RectTransform rectTransform;

    void OnEnable() {
        rectTransform = GetComponent<RectTransform>();    
    }

    public void AnimateIn() {
        UpdateScale(0);
        LeanTween.value(gameObject, UpdateScale, 0, 1, duration).setDelay(delay).setEaseOutBack();
    }

    void UpdateScale(float t) {
        rectTransform.localScale = new Vector3(t, t, t);
    }
}
