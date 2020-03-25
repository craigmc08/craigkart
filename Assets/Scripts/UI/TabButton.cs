using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    private Image background;

    public Image Background {
        get { return background; }
    }

    void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    void Update()
    {
        
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
}
