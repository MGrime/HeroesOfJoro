using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Editor Fields

    #region Music Fields

    [SerializeField] [Range(0.0f, 1.0f)] private float _musicVolume;
    [SerializeField] [Range(0.0f, 1.0f)] private float _soundEffectVolume;

    [SerializeField] private AudioSource _titleMusic;

    [SerializeField] private AudioSource _click;

    #endregion

    #region Scene Fields

    [SerializeField] private string _mainGameScene;

    #endregion

    #region UI Fields

    [SerializeField] private GameObject _titleUI;

    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effectsSlider;

    #endregion


    #endregion

    #region Private Functions

    private void Start()
    {
        // Load prefs
        _musicVolume = PlayerPrefs.GetFloat("MusicVolume",1.0f);
        _musicSlider.value = _musicVolume;

        _soundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume",1.0f); 
        _effectsSlider.value = _soundEffectVolume;

        _titleMusic.volume = _musicVolume;
        _click.volume = _soundEffectVolume;

        _titleMusic.Play();
    }

    #endregion

    #region Public Functions

    public void PlayClick()
    {
        _click.Play();
    }

    public void LoadGame()
    {
        // Set return reason to -1 to signify in hub we have just started session
        PlayerPrefs.SetInt("ReturnReason",-1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(_mainGameScene);
    }

    public void LoadTitleUI()
    {
        // Save changes
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        PlayerPrefs.SetFloat("SoundEffectVolume",_soundEffectVolume);
        PlayerPrefs.Save();

        // Disable settings UI
        _settingsUI.SetActive(false);

        // Load title UI
        _titleUI.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetMusicVolume(Single value)
    {
        _musicVolume = value;

        _titleMusic.volume = _musicVolume;
    }

    public void SetEffectsVolume(Single value)
    {
        _soundEffectVolume = value;

        _click.volume = _soundEffectVolume;

        if (!_click.isPlaying)
        {
            PlayClick();
        }
    }

    public void LoadSettingsUI()
    {
        // Disable title UI
        _titleUI.SetActive(false);

        // Load Settings UI
        _settingsUI.SetActive(true);
    }

    #endregion


}
