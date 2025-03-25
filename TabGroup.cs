using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class TabGroup : MonoBehaviour
{
    public TabButtons[] tabButtons;
    public GameObject[] panels;
    public TabButtons defaultTab;
    public Color activeTabColor;
    public Color idleTabColor;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public TabButtons selectedTab;
    [SerializeField] private GameObject[] gamepadSelectedObjects;

    [SerializeField] private int selectedTabIndex = 0;

    public void OnTabEnter(TabButtons button)
    {
        ResetTabs();
        if(selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(TabButtons button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButtons button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }

        if (selectedTab != button)
        {
            SoundManager.instance.PlayUIClip(SoundManager.instance.click, 0.5f);
        }

        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
        button.background.sprite = tabActive;
        button.GetComponentInChildren<TextMeshProUGUI>().color = activeTabColor;
        selectedTabIndex = button.transform.GetSiblingIndex();
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == selectedTabIndex)
            {
                panels[i].SetActive(true);
            }
            else
            {
                panels[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach(TabButtons button in tabButtons)
        {
            if(selectedTab != null && button == selectedTab) 
            {
                continue; 
            }
            button.background.sprite = tabIdle;
            button.GetComponentInChildren<TextMeshProUGUI>().color = idleTabColor;
        }
    }

    //These are for controller, Left and Right Bumper
    public void NextTab()
    {
        EventSystem.current.SetSelectedGameObject(null);
        selectedTabIndex++;
        if (selectedTabIndex >= tabButtons.Length)
        {
            selectedTabIndex = 0;
        }

        OnTabSelected(tabButtons[selectedTabIndex]);
        if(InputTypeManager.instance.usingGamepad)
        {
            EventSystem.current.SetSelectedGameObject(gamepadSelectedObjects[selectedTabIndex]);
        }
    }

    public void PreviousTab()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (selectedTabIndex == 0)
        {
            selectedTabIndex = tabButtons.Length - 1;
        }
        else
        {
            selectedTabIndex--;
        }

        OnTabSelected(tabButtons[selectedTabIndex]);
        if(InputTypeManager.instance.usingGamepad)
        {
            EventSystem.current.SetSelectedGameObject(gamepadSelectedObjects[selectedTabIndex]);
        }
    }
}
