using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI characterLevel;
    [SerializeField] private TextMeshProUGUI characterClass;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color enabledColor;
    [SerializeField] private TextMeshProUGUI emptySaveText;

    public int currentLevel;
    public string playerClass;

    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        if (data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            characterLevel.text = "Level " + currentLevel.ToString();
            characterClass.text = playerClass.ToString();
        }
    }

    public string GetProfileId()
    {
        return profileId;
    }

    public void SetInteractable(bool interactable)
    {
        if (!interactable)
        {
            saveSlotButton.interactable = false;
            emptySaveText.color = disabledColor;
        }
        else
        {
            saveSlotButton.interactable = true;
            emptySaveText.color = enabledColor;
        }
    }

    public void GetStats(GameData data)
    {
        if(data != null)
        {
            currentLevel = data.currentLevel;
            playerClass =  GameDataSO.CharacterClassDataMap[data.playerClass].className;
        }
    }
}
