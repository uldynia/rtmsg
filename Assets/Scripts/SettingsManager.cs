using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioClip clip;

    [Header("Button")]
    [SerializeField] Button forfeitButton;

    [Header("Sliders")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private const string MasterVolumeKey = "masterVolume";
    private const string MusicVolumeKey = "musicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    float PercentToDecibels(float volume) => Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;

    private void Start()
    {
        InitializeSlider(masterSlider, MasterVolumeKey, "masterVolume");
        InitializeSlider(musicSlider, MusicVolumeKey, "musicVolume");
        InitializeSlider(sfxSlider, SFXVolumeKey, "SFXVolume");
    }

    private void InitializeSlider(Slider slider, string playerPrefsKey, string mixerParameter)
    {
        if (slider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(playerPrefsKey, 1);
            slider.value = savedVolume;
            mixer.SetFloat(mixerParameter, PercentToDecibels(savedVolume));
            slider.onValueChanged.AddListener((value) => SetVolume(value, playerPrefsKey, mixerParameter));
        }
        else
        {
            Debug.LogError($"Slider for {playerPrefsKey} not assigned in the Inspector!");
        }
    }
    private void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Title":
                forfeitButton.gameObject.SetActive(false);
                break;
            case "Lobby":
                forfeitButton.gameObject.SetActive(true);
                forfeitButton.onClick.RemoveAllListeners();
                forfeitButton.GetComponentInChildren<TextMeshProUGUI>().text = "LEAVE";
                forfeitButton.onClick.AddListener(() => {
                    NetworkManager.singleton.StopHost();
                    SceneManager.LoadScene("Title");
                });
                break;
            case "Game":
                forfeitButton.onClick.RemoveAllListeners();
                forfeitButton.GetComponentInChildren<TextMeshProUGUI>().text = "FORFEIT";
                forfeitButton.onClick.AddListener(() => {
                    PlayerController.localPlayer.Forfeit();
                });
                break;
        }
    }
    public void SetVolume(float volume, string playerPrefsKey, string mixerParameter)
    {
        mixer.SetFloat(mixerParameter, PercentToDecibels(volume));
        PlayerPrefs.SetFloat(playerPrefsKey, volume);
    }
}