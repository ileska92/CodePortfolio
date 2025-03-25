using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PauseMenuScript : MonoBehaviour
{
    [Header("GameObjects/Buttons")]
    public GameObject buttons;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject continueButton;
    public GameObject settingsButton;
    [SerializeField] private GameObject keyGuideButton;
    [SerializeField] private GameObject keyGuide;
    [SerializeField] private TransitionScript transitionScript;

    private float buttonVolume = 0.5f;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        transitionScript = FindObjectOfType<TransitionScript>();
        PlayerPrefsSettings.instance.pauseMenuScript = this;
    }

    private void OnEnable() 
    {
        inputActions = InputTypeManager.instance.inputActions;
    }

    void Update()
    {
        if (inputActions.Player.Pause.WasPressedThisFrame() && !GameManager.instance.health.playerIsDead)
        {
            Pause();
        }

        if (inputActions.UI.Cancel.WasPressedThisFrame() && GameManager.instance.gameIsPaused && !PlayerPrefsSettings.instance.onOffToggle.activeSelf)
        {
            ContinueGame();
        }

        if(inputActions.UI.Cancel.WasPressedThisFrame() && GameManager.instance.activeUIPanel != null && !PlayerPrefsSettings.instance.onOffToggle.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(null); //Sends Deselect to disable tooltip
            GameManager.instance.activeUIPanel.gameObject.SetActive(false);
        }
    }

    public void Pause()
    {
        GameManager.instance.gameIsPaused = true;
        InputTypeManager.instance.PauseGameInput(GameManager.instance.gameIsPaused);
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        if (InputTypeManager.instance.usingGamepad)
        {
            EventSystem.current.SetSelectedGameObject(continueButton);
        }
    }

    public void ContinueGame()
    {
        GameManager.instance.gameIsPaused = false;
        InputTypeManager.instance.PauseGameInput(GameManager.instance.gameIsPaused);
        if (SceneManager.GetActiveScene().name == "CityScene")
        {
            InputTypeManager.instance.CityInput(); 
        }
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        EventSystem.current.SetSelectedGameObject(null);
        InputTypeManager.instance.LoadRebinds();
    }

    public void ToggleSettings()
    {
        buttons.SetActive(false);
        PlayerPrefsSettings.instance.ToggleSettings();
        gameObject.GetComponent<PauseMenuScript>().enabled = false;
    }

    public void MainMenu()
    {
        pauseMenu.SetActive(false);
        SoundManager.instance.FadeOutMusic();
        DataPersistenceManager.instance.SaveGame();
        Time.timeScale = 1;
        GameManager.instance.DestroyGameManager();
        transitionScript.StartCoroutine(transitionScript.LoadingScene(0));
    }

    public void QuitGame()
    { 
        Application.Quit(); //Game is saved OnApplicationQuit in DataPersistenceManager
    }

    public void PlayClickSound()
    {
        SoundManager.instance.PlayUIClip(SoundManager.instance.click, buttonVolume);
    }

    public void SetSelected(GameObject objectToSelect)
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (InputTypeManager.instance.usingGamepad)
        {
            EventSystem.current.SetSelectedGameObject(objectToSelect);
        }
    }

    public void ToggleEnabled(GameObject objectToToggle)
    {
        if (objectToToggle.activeSelf)
        {
            objectToToggle.SetActive(false);
        }
        else
        {
            objectToToggle.SetActive(true);
        }
    }
}
