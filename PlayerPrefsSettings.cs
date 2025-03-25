using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Globalization;

public class PlayerPrefsSettings : MonoBehaviour
{
    public static PlayerPrefsSettings instance { get; private set; }
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Settings in the scene. Destroying the newest one.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    [Header("InteractSprites")]
    public Image keyboardInteract;
    public Image gamepadInteract;

    [Header("GameObjects")]
    public GameObject onOffToggle;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject keyboardBindsPanel;
    [SerializeField] private GameObject controllerPanel;
    [SerializeField] private GameObject waitForInputPanel;
    [SerializeField] private CameraShaker cameraShaker;
    [SerializeField] private TabGroup tabGroup;

    [Header("Selected Buttons")]
    public MainMenuScript mainMenuScript;
    public GameObject mainMenuSettingsButton;
    public PauseMenuScript pauseMenuScript;
    [SerializeField] private GameObject pauseMenuSettingsButton;
    [SerializeField] private GameObject settingsFirstButton;

    [Header("Sound/Volume")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float _masterVolume = 1f;
    [SerializeField] private float _musicVolume = 1f;
    [SerializeField] private float _sFXVolume = 1f;
    [SerializeField] private float _dialogVolume = 1f;
    [SerializeField] private float _uiVolume = 1f;
    public List<string> audioPreferences;

    [Header("Volume Toggles")]
    [SerializeField] private Toggle masterToggle;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sFXToggle;
    [SerializeField] private Toggle dialogToggle;
    [SerializeField] private Toggle uiToggle;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sFXSlider;
    [SerializeField] private Slider dialogSlider;
    [SerializeField] private Slider uiSlider;

    [Header("Monitor")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullScreenToggle;
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    [SerializeField] private string currentRefreshRate;
    [SerializeField] private int currentResolutionIndex = 0;
    [SerializeField] private int defaultResolutionIndex;
    [SerializeField] private int fullScreen = 1;

    [Header("Other Toggles")]
    [SerializeField] private Toggle screenShakeToggle;
    [SerializeField] private Toggle zoomOutToggle;
    public int screenShake = 1;
    public int zoomOut = 1;
    //[SerializeField] private Toggle gamepadEnabledToggle;
    //[SerializeField] private int gamepadEnabledInt;

    [Header("FPS")]
    [SerializeField] private float targetFPS;
    [SerializeField] private Slider fpsSlider;
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private int vSyncCount;
    [SerializeField] private Toggle vSyncToggle;

    [Header("Animation")]
    [SerializeField] private Animator thisAnimator;

    [Header("Other")]
    public bool rebindingActive;

    private void Start()
    {
        Debug.Log("Starting setup");
        ResolutionLoop();
        LoadPreferences();
        audioPanel.SetActive(false);
        keyboardBindsPanel.SetActive(false);
        controllerPanel.SetActive(false);
        onOffToggle.SetActive(false);
        SliderListeners();
        ToggleListeners();
        SetFPS();
        Debug.Log("Setup finished");
    }

    private void ResolutionLoop()
    {
        Debug.Log("Sorting resolutions");
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio.ToString();

        for (int i = 0; i < resolutions.Length; i++) //Loops screen resolution settings and adds your monitors current refreshRate resolutions to a filtered list
        {
            if (resolutions[i].refreshRateRatio.ToString() == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        filteredResolutions.Reverse();

        List<string> options = new List<string>();

        for (int i = 0; i < filteredResolutions.Count; i++) //Uses the filtered list to display multiple resolution options with the current screen refreshRateRatio, list goes from highest value on top -> lowest value on the bottom
        {
            string resolutionOption = filteredResolutions[i].width + " x " + filteredResolutions[i].height; // + " @ " + filteredResolutions[i].refreshRateRatio + " Hz"
            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
                defaultResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        Debug.Log("Resolutions sorted");
    }

    //Settings to PlayerPrefs
    public void SavePreferences()
    {
        PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", _sFXVolume);
        PlayerPrefs.SetFloat("DialogVolume", _dialogVolume);
        PlayerPrefs.SetFloat("UIVolume", _uiVolume);
        PlayerPrefs.SetInt("CurrentResolution", currentResolutionIndex);
        PlayerPrefs.SetInt("FullScreen", fullScreen);
        PlayerPrefs.SetInt("ScreenShake", screenShake);
        PlayerPrefs.SetInt("ZoomOut", zoomOut);
        PlayerPrefs.SetFloat("targetFPS", targetFPS);
        PlayerPrefs.SetInt("VSync", vSyncCount);
        PlayerPrefs.Save();
        Debug.Log("Preferences saved");
    }

    public void LoadPreferences()
    {
        Debug.Log("Loading preferences");

        if (PlayerPrefs.HasKey("MasterVolume")) //if preferences has key named MasterVolume, use it
        {
            _masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("MasterVolume", 1f); //If preferences does not have a key, make one named MasterVolume and give it a default value of 1f, then use it
            _masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        }
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", 1f);
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            _sFXVolume = PlayerPrefs.GetFloat("SFXVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("SFXVolume", 1f);
            _sFXVolume = PlayerPrefs.GetFloat("SFXVolume");
        }
        if (PlayerPrefs.HasKey("DialogVolume"))
        {
            _dialogVolume = PlayerPrefs.GetFloat("DialogVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("DialogVolume", 1f);
            _dialogVolume = PlayerPrefs.GetFloat("DialogVolume");
        }
        if (PlayerPrefs.HasKey("UIVolume"))
        {
            _uiVolume = PlayerPrefs.GetFloat("UIVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("UIVolume", 1f);
            _uiVolume = PlayerPrefs.GetFloat("UIVolume");
        }

        if (PlayerPrefs.HasKey("targetFPS"))
        {
            targetFPS = PlayerPrefs.GetFloat("targetFPS");
        }
        else
        {
            float number = float.Parse(currentRefreshRate, CultureInfo.InvariantCulture);
            PlayerPrefs.SetFloat("targetFPS", number);
            targetFPS = PlayerPrefs.GetFloat("targetFPS");
        }

        GetSliderValue();
        GetToggleValue();

        if (PlayerPrefs.HasKey("CurrentResolution"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("CurrentResolution");
        }
        else
        {
            PlayerPrefs.SetInt("CurrentResolution", currentResolutionIndex);
            currentResolutionIndex = PlayerPrefs.GetInt("CurrentResolution");
        }

        SetResolution(currentResolutionIndex);

        if (PlayerPrefs.HasKey("FullScreen"))
        {
            fullScreen = PlayerPrefs.GetInt("FullScreen");
        }
        else
        {
            PlayerPrefs.SetInt("FullScreen", 1);
            fullScreen = PlayerPrefs.GetInt("FullScreen");
        }

        if (fullScreen == 1)  //Booleans cannot be stored to playprefs so need to go around with an integer
        {
            SetFullscreen(true);
        }
        else
        {
            SetFullscreen(false);
        }

        if (PlayerPrefs.HasKey("ScreenShake"))
        {
            screenShake = PlayerPrefs.GetInt("ScreenShake");
        }
        else
        {
            PlayerPrefs.SetInt("ScreenShake", 1);
            screenShake = PlayerPrefs.GetInt("ScreenShake");
        }

        if (screenShake == 1)  //Booleans cannot be stored to playprefs so need to go around with an integer
        {
            SetScreenShake(true);
        }
        else
        {
            SetScreenShake(false);
        }

        if (PlayerPrefs.HasKey("ZoomOut"))
        {
            zoomOut = PlayerPrefs.GetInt("ZoomOut");
        }
        else
        {
            PlayerPrefs.SetInt("ZoomOut", 1);
            zoomOut = PlayerPrefs.GetInt("ZoomOut");
        }

        if (zoomOut == 1)  //Booleans cannot be stored to playprefs so need to go around with an integer
        {
            SetZoomOut(true);
        }
        else
        {
            SetZoomOut(false);
        }

        if (PlayerPrefs.HasKey("VSync"))
        {
            vSyncCount = PlayerPrefs.GetInt("VSync");
        }
        else
        {
            PlayerPrefs.SetInt("VSync", 1);
            vSyncCount = PlayerPrefs.GetInt("VSync");
        }

        if(vSyncCount == 1)
        {
            SetVSync(true);
        }
        else
        {
            SetVSync(false);
        }

        Debug.Log("Preferences loaded");
    }

    public void DefaultPreferences()
    {
        if(audioPanel.activeSelf)
        {
            foreach(string key in audioPreferences)
            {
                PlayerPrefs.DeleteKey(key);
            }

            LoadPreferences();

            Debug.Log("Audio preferences defaulted");
        }
        else
        {
            PlayerPrefs.DeleteKey("CurrentResolution");
            PlayerPrefs.DeleteKey("FullScreen");
            PlayerPrefs.DeleteKey("ScreenShake");
            PlayerPrefs.DeleteKey("ZoomOut");
            PlayerPrefs.DeleteKey("targetFPS");
            PlayerPrefs.DeleteKey("VSync");

            LoadPreferences();

            SetResolution(defaultResolutionIndex);
            resolutionDropdown.value = defaultResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            SetFullscreen(true);
            SetScreenShake(true);
            SetZoomOut(true);
            SetVSync(true);

            Debug.Log("General preferences defaulted");
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Debug.Log("Fullscreen: " + isFullscreen);

        if (isFullscreen) { fullScreen = 1; }
        else { fullScreen = 0; }
        Screen.fullScreen = isFullscreen;
        fullScreenToggle.isOn = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Debug.Log("Resolution index: " + resolutionIndex);

        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        currentResolutionIndex = resolutionIndex;
    }

    public void SetScreenShake(bool enableShake)
    {
        if (enableShake) { screenShake = 1; }
        else {  screenShake = 0; }
        screenShakeToggle.isOn = enableShake;
    }

    public void SetZoomOut(bool enableZoom)
    {
        if (enableZoom) { zoomOut = 1; }
        else { zoomOut = 0; }
        zoomOutToggle.isOn = enableZoom;
    }
    public void SetVSync(bool enableVSync)
    {
        if (enableVSync) { vSyncCount = 1; }
        else { vSyncCount = 0; }
        vSyncToggle.isOn = enableVSync;
        QualitySettings.vSyncCount = vSyncCount;
    }

    public void SetMasterSliderValue(float sliderValue)
    {
        _masterVolume = sliderValue;
    }
    public void SetMusicSliderValue(float sliderValue)
    {
        _musicVolume = sliderValue;
    }
    public void SetSFXSliderValue(float sliderValue)
    {
        _sFXVolume = sliderValue;
    }
    public void SetDialogSliderValue(float sliderValue)
    {
        _dialogVolume = sliderValue;
    }
    public void SetUISliderValue(float sliderValue)
    {
        _uiVolume = sliderValue;
    }
    public void SetFPSSliderValue(float sliderValue)
    {
        targetFPS = sliderValue;
        fpsText.text = targetFPS.ToString();
        SetFPS();
    }

    public void GetSliderValue()
    {
        masterSlider.value = _masterVolume;
        musicSlider.value = _musicVolume;
        sFXSlider.value = _sFXVolume;
        dialogSlider.value = _dialogVolume;
        uiSlider.value = _uiVolume;
        fpsSlider.value = targetFPS;
    }

    public void GetToggleValue()
    {
        if (_masterVolume == masterSlider.minValue)
        {
            masterToggle.isOn = false;
        }
        else
        {
            masterToggle.isOn = true;
        }

        if (_musicVolume == musicSlider.minValue)
        {
            musicToggle.isOn = false;
        }
        else
        {
            musicToggle.isOn = true;
        }

        if (_sFXVolume == sFXSlider.minValue)
        {
            sFXToggle.isOn = false;
        }
        else
        {
            sFXToggle.isOn = true;
        }

        if (_dialogVolume == dialogSlider.minValue)
        {
            dialogToggle.isOn = false;
        }
        else
        {
            dialogToggle.isOn = true;
        }

        if (_uiVolume == uiSlider.minValue)
        {
            uiToggle.isOn = false;
        }
        else
        {
            uiToggle.isOn = true;
        }
    }

    public void SetInactive()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            mainMenuScript.menuButtons.SetActive(true);
        }
        else
        {
            pauseMenuScript.buttons.SetActive(true);
            pauseMenuScript.enabled = true;
        }
        onOffToggle.SetActive(false);
        SetSelectedButton();
    }

    public void ToggleSettings()
    {
        if (onOffToggle.activeSelf)
        {
            SetInactive();
        }
        else
        {
            onOffToggle.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            tabGroup.selectedTab = tabGroup.defaultTab;
            tabGroup.OnTabSelected(tabGroup.defaultTab);

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                if (InputTypeManager.instance.usingGamepad)
                {
                    EventSystem.current.SetSelectedGameObject(settingsFirstButton);
                }
            }
            else
            {
                if (InputTypeManager.instance.usingGamepad)
                {
                    EventSystem.current.SetSelectedGameObject(settingsFirstButton);
                }
            }
        }
    }

    private void SetSelectedButton()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {            
            if (InputTypeManager.instance.usingGamepad)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuSettingsButton);
            }
        }
        else
        {
            if (InputTypeManager.instance.usingGamepad)
            {
                EventSystem.current.SetSelectedGameObject(pauseMenuScript.settingsButton);
            }
        }
    }

    public void CameraShaker(bool enableShake)
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }
        else
        {
            if(cameraShaker == null)
            {
                cameraShaker = FindObjectOfType<CameraShaker>();
            }
            cameraShaker.shakeActive = enableShake;
        }
    }

    public void SetFPS()
    {
        Application.targetFrameRate = Convert.ToInt32(targetFPS);
    }

    private void ToggleListeners()
    {
        masterToggle.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        musicToggle.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        sFXToggle.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        dialogToggle.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        uiToggle.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        fullScreenToggle.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        screenShakeToggle.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        zoomOutToggle.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        vSyncToggle.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
    }

    private void SliderListeners()
    {
        masterSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
        uiSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
    }

    private void OnSliderValueChange()
    {
        SoundManager.instance.PlayUIClip(SoundManager.instance.click, 0.5f);
    }
}
