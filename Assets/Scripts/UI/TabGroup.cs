using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;
    public List<GameObject> panels;
    public TabButton selectedTab;

    public void Subscribe(TabButton button) {
        if(tabButtons == null) {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button) {
        ResetTabs();
        if (button != selectedTab) {
            button.Background.color = tabHover;
        }
    }

    public void OnTabExit(TabButton button) {
        ResetTabs();
        if (button != selectedTab) {
            button.Background.color = tabIdle;
        }
    }

    public void OnTabSelected(TabButton button) {
        if (selectedTab != null) {
            selectedTab.Deselect();
        }

        selectedTab = button;
        selectedTab.Select();
        
        ResetTabs();
        button.Background.color = tabActive;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < panels.Count; i++) {
            panels[i].SetActive(i == index);
        }
    }

    public void ResetTabs() {
        foreach (TabButton button in tabButtons) {
            if (button == selectedTab) { continue; }
            button.Background.color = tabIdle;
        }
    }
}
