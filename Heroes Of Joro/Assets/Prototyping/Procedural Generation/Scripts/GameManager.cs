using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _loadingImage;
    [SerializeField] private AudioSource _loadingMusic;

    [SerializeField] private AudioSource _dungeonMusic;
    [SerializeField] private Texture2D _cursorTexture;

    [SerializeField] private GameObject[] _dungeons;

    public CursorMode _cursorMode;
    public Vector2 _hotSpot = Vector2.zero;
    #endregion

    #region Functions
    /*private void OnMouseEnter()
    {
        Cursor.SetCursor(_cursorTexture, _hotSpot, _cursorMode);
    }
    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, _cursorMode);
    }*/
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
        _cursorMode = CursorMode.ForceSoftware;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        // Enable the correct dungeon
        if (_dungeons.Length != 0)
        {
            _dungeons[PlayerPrefs.GetInt("TargetDungeon")].SetActive(true);
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

        _sessionCoins = 0;

    }

    public void StopMusic()
    {

        if (_loadingMusic)
        {
            _loadingMusic.Stop();
        }

        if (_dungeonMusic)
        {
            _dungeonMusic.Stop();
        }
    }

    private int _sessionCoins;
    public int Coins
    {
        get => _sessionCoins;
        set => _sessionCoins = value;
    }

    public void AddCoin()
    {
        _sessionCoins++;
    }

    public void SaveCoins()
    {
        if (PlayerPrefs.HasKey("Coins"))
        {
            var coins = PlayerPrefs.GetInt("Coins");

            coins += _sessionCoins;

            PlayerPrefs.SetInt("Coins",coins);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetInt("Coins",_sessionCoins);
            PlayerPrefs.Save();
        }
    }

    #endregion
}
