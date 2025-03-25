using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    private SaveSlot[] saveSlots;

    [Header("Menu Buttons and Panels")]
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject confirmationPopUp;
    [SerializeField] private GameObject confirmBackButton; 
    [SerializeField] private GameObject characterScreenBackButton;
    [SerializeField] private GameObject characterScreen;
    [SerializeField] private GameObject previouslyPressedButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject loadGameButton;

    [Header("Text fields")]
    [SerializeField] private TextMeshProUGUI dataInfoText;

    [Space(10)]

    [SerializeField] private bool isLoadingGame = false;
    [SerializeField] private string currentProfileId;
    [SerializeField] private MainMenuScript mainMenuScript;
    [SerializeField] private CharacterSelection characterSelection;

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    private void Update()
    {
        if(InputTypeManager.instance.inputActions.UI.Cancel.WasPressedThisFrame() && !mainMenuScript.loadingScreen.activeSelf)
        {
            if(confirmationPopUp.activeSelf)
            {
                mainMenuScript.ToggleEnabled(confirmationPopUp);
                EnableMenuButtons();
                PreviouslyPressedSelected();
            }
            else
            {
                mainMenuScript.ToggleEnabled(gameObject);
                SetSelectedMainMenu();
            }
        }
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        currentProfileId = saveSlot.GetProfileId();
        DisableMenuButtons();
        DataPersistenceManager.instance.ChangeSelectedProfileId(currentProfileId); // Updates the profile id and loads the file from that id

        if (!isLoadingGame)
        {
            if (!DataPersistenceManager.instance.HasGameData())
            {
                CharacterSelection();
            }
            else
            {
                dataInfoText.text = "Saved data will be deleted!\r\n\r\n" + saveSlot.playerClass + " - Level " + saveSlot.currentLevel + "\r\n\r\nAre you sure?";
                ConfirmationPopUp();
            }
        }
        else
        {
            mainMenuScript.TriggerLoadingAnim();
        }
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        gameObject.SetActive(true);
        this.isLoadingGame = isLoadingGame;
        // Load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        // Loop through each save slot in the UI and set the content
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.GetStats(profileData);
            saveSlot.SetData(profileData);
            if (profileData == null & isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        backButton.GetComponent<Button>().interactable = false;
    }

    public void EnableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(true);
        }
        backButton.GetComponent<Button>().interactable = true;
    }

    public void ConfirmationPopUp()
    {
        confirmationPopUp.SetActive(true);
        SetSelected(confirmBackButton);
    }

    public void DeleteSavedFile()
    {
        DataPersistenceManager.instance.DeleteProfileData(currentProfileId);
        EnableMenuButtons();
        ActivateMenu(isLoadingGame);
    }

    private void SetSelected(GameObject objectToSelect)
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (InputTypeManager.instance.usingGamepad)
        {
            EventSystem.current.SetSelectedGameObject(objectToSelect);
        }
    }

    public void PreviouslyPressedSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (InputTypeManager.instance.usingGamepad)
        {
            EventSystem.current.SetSelectedGameObject(previouslyPressedButton);
        }
    }

    public void SetPreviouslyPressedButton(GameObject thisObject)
    {
        previouslyPressedButton = thisObject;
    }

    public void SetSelectedMainMenu()
    {
        mainMenuScript.menuButtons.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

        if (InputTypeManager.instance.usingGamepad)
        {
            if (isLoadingGame)
            {
                EventSystem.current.SetSelectedGameObject(loadGameButton);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(newGameButton);
            }
        }
    }

    public void CharacterSelection()
    {
        characterSelection.RefreshCharacterScreen();
        characterScreen.SetActive(true);
        SetSelected(characterScreenBackButton);
        gameObject.SetActive(false);
    }

    public void CloseCharacterSelection()
    {
        characterScreen.SetActive(false);
        EnableMenuButtons();
        ActivateMenu(isLoadingGame);
        PreviouslyPressedSelected();
    }
}
