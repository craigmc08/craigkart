using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public TextMeshProUGUI  text;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    private Image background;

    void Awake()
    {
        background = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        tabGroup.OnTabExit(this);
    }

    public void Select() {
        if (onTabSelected != null) {
            onTabSelected.Invoke();
        }
    }

    public void Deselect() {
        if (onTabDeselected != null) {
            onTabDeselected.Invoke();
        }
    }

    public void Style(TabStyle style) {
        background.color = style.tabColor;
        text.color = style.fontColor;
    }
}
