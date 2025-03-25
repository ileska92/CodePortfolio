using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartographerScript : MonoBehaviour, IInteractable, IDataPersistence, ILevelUnlock
{
    //General
    [SerializeField] private GameObject cartographerPanel;
    [SerializeField] private GameObject mapUnlockPanel;
    private int levelToUnlock;

    //Button stuff
    [SerializeField] private GameObject buttonSelected;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private Button buyButton;

    //MapSize Cost and other info
    [SerializeField] private TextMeshProUGUI goldCost;
    [SerializeField] private TextMeshProUGUI diamondCost;
    [SerializeField] private TextMeshProUGUI emeraldCost;
    [SerializeField] private MapSizesInfo currentlySelectedMap;
    private MapSizesInfo tempSelectedMap;
    [SerializeField] private SpriteStateSwap spriteStateSwap;
    [SerializeField] private Color disabledTextColor;
    [SerializeField] private Color enabledTextColor;
    [SerializeField] private MapSizesInfo[] mapSizes;
    [SerializeField] private UpdatePlayerShinies playerShinies;


    public void LoadData(GameData data)
    {
        GameManager.instance.selectedMap = data.selectedMap;
        GameManager.instance.mapSize = data.mapSize;
        GameManager.instance.mapUnlocks = data.mapUnlocks;
    }

    public void SaveData(GameData data)
    {
        data.selectedMap = GameManager.instance.selectedMap;
        data.mapSize = GameManager.instance.mapSize;
        data.mapUnlocks = GameManager.instance.mapUnlocks;
    }

    public void Interact()
    {
       if(cartographerPanel.activeSelf)
       {
            cartographerPanel.SetActive(false);
       }
       else
       {
            cartographerPanel.SetActive(true);
            EnableButtons();
       }

       if (InputTypeManager.instance.usingGamepad)
       {
           EventSystem.current.SetSelectedGameObject(buttonSelected);
       }
    }

    public void CheckUnlock(int level)
    {
        levelToUnlock = GameManager.instance.playerXP.cartographerUnlockLevel;
        if (levelToUnlock > level)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentlySelectedMap = mapSizes[GameManager.instance.selectedMap]; 
        InitializeMapUnlocks();
        SelectMap(currentlySelectedMap);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.playerController.targetInteractable = this;
            GameManager.instance.playerController.GetComponentInChildren<InteractIconScript>().EnableIcon();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.playerController.targetInteractable = null;
            GameManager.instance.playerController.GetComponentInChildren<InteractIconScript>().DisableIcon();
        }
    }


    private void InitializeMapUnlocks()
    {
        if(GameManager.instance.mapUnlocks.Count < mapSizes.Length) //If adding more mapsize choices updates the saved list to same size, default unlock bool in a new map = false
        {   
            int amountToAdd = mapSizes.Length - GameManager.instance.mapUnlocks.Count;
            for (int i = 0; i < amountToAdd; i++)
            {
                GameManager.instance.mapUnlocks.Add(false);
            }
            Debug.Log("Updated saved map list, added new maps: " + amountToAdd);
        }
        else
        {
            Debug.Log("No need to update saved map list");
        }

        for (int i = 0; i < mapSizes.Length; i++)
        {
            mapSizes[i].unlocked = GameManager.instance.mapUnlocks[i];
        }
    }

    private void SetButtonVisuals()
    {
        for (int i = 0; i < mapSizes.Length; i++)
        {
            if(!mapSizes[i].unlocked)
            {
                mapSizes[i].button.GetComponentInChildren<TextMeshProUGUI>().text = "Unlock";
                mapSizes[i].button.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                mapSizes[i].cost.SetActive(true);
            }
            else
            {
                mapSizes[i].cost.SetActive(false);

                if(mapSizes[i].selected)
                {
                    mapSizes[i].button.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
                    mapSizes[i].button.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
                }
                else
                {
                    mapSizes[i].button.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
                    mapSizes[i].button.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                }
            }
        }
    }

    public void MapButtonClicked(MapSizesInfo map)
    {
        if (map.unlocked)
        {
            SelectMap(map);
        }
        else
        {
            OpenMapUnlockPanel(map);
        }
    }

    private void SelectMap(MapSizesInfo map)
    {
        int i = Array.IndexOf(mapSizes, map);
        GameManager.instance.selectedMap = i;
        GameManager.instance.mapSize = map.mapSize.mapSize;
        
        foreach (MapSizesInfo mapInfo in mapSizes)
        {
            mapInfo.mapSizeFrame.SetActive(false);
            mapInfo.selected = false;

            if(mapInfo == map)
            {
                mapInfo.mapSizeFrame.SetActive(true);
                mapInfo.selected = true;
            }
        }

        SetButtonVisuals();
    }

    public void OpenMapUnlockPanel(MapSizesInfo map)
    {
        tempSelectedMap = map;
        goldCost.text = map.mapSize.goldCost.ToString();
        diamondCost.text = map.mapSize.diamondCost.ToString();
        emeraldCost.text = map.mapSize.emeraldCost.ToString();
        if(map.mapUnlockIcon != null)
        {
            map.mapUnlockIcon.SetActive(true);
        }

        if (GameManager.instance.treasureCounter.goldCounter >= map.mapSize.goldCost && GameManager.instance.treasureCounter.diamondCounter >= map.mapSize.diamondCost && GameManager.instance.treasureCounter.emeraldCounter >= map.mapSize.emeraldCost)
        {
            buyButton.interactable = true;
            buyButton.GetComponentInChildren<TextMeshProUGUI>().color = enabledTextColor;
        }
        else
        {
            buyButton.interactable = false;
            buyButton.GetComponentInChildren<TextMeshProUGUI>().color = disabledTextColor;
        }

        mapUnlockPanel.SetActive(true);
        DisableButtons();
        if (InputTypeManager.instance.usingGamepad)
        {
            EventSystem.current.SetSelectedGameObject(cancelButton);
        }
    }

    public void UnlockMapSize()
    {
        int i = Array.IndexOf(mapSizes, tempSelectedMap);
        GameManager.instance.mapUnlocks[i] = true;
        tempSelectedMap.unlocked = true;
        GameManager.instance.treasureCounter.goldCounter -= tempSelectedMap.mapSize.goldCost;
        GameManager.instance.treasureCounter.diamondCounter -= tempSelectedMap.mapSize.diamondCost;
        GameManager.instance.treasureCounter.emeraldCounter -= tempSelectedMap.mapSize.emeraldCost;
        playerShinies.UpdateShinies();
        SetButtonVisuals();
        SelectMap(tempSelectedMap);
        CloseMapUnlockPanel();
    }

    public void CloseMapUnlockPanel()
    {
        tempSelectedMap.mapUnlockIcon.SetActive(false);
        tempSelectedMap = null;
        mapUnlockPanel.SetActive(false);
        EnableButtons();

        if (InputTypeManager.instance.usingGamepad)
        {
            EventSystem.current.SetSelectedGameObject(buttonSelected);
        }
    }

    private void EnableButtons()
    {
        cartographerPanel.GetComponent<SetSelectedObject>().enabled = true;
        buttonSelected.GetComponent<Button>().interactable = true;
        buttonSelected.GetComponentInChildren<TextMeshProUGUI>().color = enabledTextColor;
        foreach (MapSizesInfo map in mapSizes)
        {
            map.button.interactable = true;
        }
        SetButtonVisuals();
    }

    private void DisableButtons()
    {
        cartographerPanel.GetComponent<SetSelectedObject>().enabled = false;
        buttonSelected.GetComponent<Button>().interactable = false;
        buttonSelected.GetComponentInChildren<TextMeshProUGUI>().color = disabledTextColor;
        foreach (MapSizesInfo map in mapSizes)
        {
            map.button.interactable = false;
            map.button.GetComponentInChildren<TextMeshProUGUI>().color = disabledTextColor;
        }
    }
}
