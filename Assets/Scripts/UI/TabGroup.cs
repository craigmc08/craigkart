using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TabStyle {
    public Color fontColor;
    public Color tabColor;
}

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public TabStyle tabIdle;
    public TabStyle tabHover;
    public TabStyle tabActive;
    public List<GameObject> tabs;
    public TabButton defaultTab;
    
    private TabButton selectedTab;

    public TabButton SelectedTab {
        get { return selectedTab; }
    }

    void Start() {
        OnTabSelected(defaultTab);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            int i = selectedTab.transform.GetSiblingIndex();
            if (i > 0) {
                OnTabSelected(tabButtons[i - 1]);
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            int i = selectedTab.transform.GetSiblingIndex();
            if (i < transform.childCount - 1) {
                OnTabSelected(tabButtons[i + 1]);
            }
        }
    }

    public void OnTabEnter(TabButton button) {
        ResetTabs();
        if (button != selectedTab) {
            button.Style(tabHover);
        }
    }

    public void OnTabExit(TabButton button) {
        ResetTabs();
        if (button != selectedTab) {
            button.Style(tabIdle);
        }
    }

    public void OnTabSelected(TabButton button) {
        if (selectedTab != null) {
            selectedTab.Deselect();
        }

        selectedTab = button;
        selectedTab.Select();

        ResetTabs();
        button.Style(tabActive);
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < tabs.Count; i++) {
            tabs[i].SetActive(i == index);
        }
    }

    public void ResetTabs() {
        foreach (TabButton button in tabButtons) {
            if (button == selectedTab) { continue; }
            button.Style(tabIdle);
        }
    }
}
