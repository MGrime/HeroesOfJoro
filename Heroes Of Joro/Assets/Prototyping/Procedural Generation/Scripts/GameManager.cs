using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _loadingImage;
    [SerializeField] private AudioSource _loadingMusic;

    [SerializeField] private AudioSource _dungeonMusic;

    #endregion

    #region Functions

    private void Start()
    {
        // Load prefs
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            _loadingMusic.volume = PlayerPrefs.GetFloat("MusicVolume");
            _dungeonMusic.volume = PlayerPrefs.GetFloat("MusicVolume");
        }

        if (_loadingMusic)
        {
            _loadingMusic.Play();
        }

    }

    public void LoadingFinished()
    {
        if (_loadingImage)
        {
            _loadingImage.SetActive(false);
        }

        if (_loadingMusic)
        {
            _loadingMusic.Stop();
        }

        if (_dungeonMusic)
        {
            _dungeonMusic.Play();
        }

    }

    #endregion
}
