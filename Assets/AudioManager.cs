using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioClip clip;

    [Header("Sliders")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private const string MasterVolumeKey = "masterVolume";
    private const string MusicVolumeKey = "musicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    float masterVolume => PlayerPrefs.GetFloat(MasterVolumeKey, 1);
    float musicVolume => PlayerPrefs.GetFloat(MusicVolumeKey, 1);
    float sfxVolume => PlayerPrefs.GetFloat(SFXVolumeKey, 1);
    float PercentToDecibels(float volume) => Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20; // Ensure volume is not zero for Log10

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

    public void SetVolume(float volume, string playerPrefsKey, string mixerParameter)
    {
        mixer.SetFloat(mixerParameter, PercentToDecibels(volume));
        PlayerPrefs.SetFloat(playerPrefsKey, volume);
    }

}