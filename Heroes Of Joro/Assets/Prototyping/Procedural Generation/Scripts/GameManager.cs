using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _loadingImage;
    [SerializeField] private AudioSource _loadingMusic;

    [SerializeField] private AudioSource _dungeonMusic;
    [SerializeField] private Texture2D _cursorTexture;

    [SerializeField] private GameObject[] _dungeons;

    // Store the UI text output
    [SerializeField] private Text _coinCounterText;
    private int _coinsAtStart;

    public CursorMode _cursorMode;
    public Vector2 _hotSpot = Vector2.zero;

    public enum DungeonEndReason
    {
        Lost = 0,
        Won = 1
    };

    #endregion

    #region Functions
    private void OnMouseEnter()
    {
        Cursor.SetCursor(_cursorTexture, _hotSpot, _cursorMode);
    }
    
    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, _cursorMode);
    }
    private void Start()
    {
        // Load prefs
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            if (_loadingMusic)
            {
                _loadingMusic.volume = PlayerPrefs.GetFloat("MusicVolume");
            }

            if (_dungeonMusic)
            {
                _dungeonMusic.volume = PlayerPrefs.GetFloat("MusicVolume");
            }
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

        // Check for coins
        if (PlayerPrefs.HasKey("Coins"))
        {
            _coinsAtStart = PlayerPrefs.GetInt("Coins");
            if (_coinCounterText)
            {
                _coinCounterText.text = _sessionCoins.ToString() + '(' + _coinsAtStart + ')';
            }
        }
        // Get component on the same game object that stores the player references
        var playerManager = GetComponentInParent<PlayerManager>();

        // got the reference
        if (playerManager)
        {
            // Get the array of player trackers
            var playerArray = playerManager.PlayerTrackers;

            foreach (var player in playerArray)
            {
                // Switch off the type
                switch (player.Type)
                {
                    case PlayerBase.PlayerType.Mage:
                    {
                        // convert to type
                        var mage = (Mage) player;

                        // Read in values, default values are the standards 
                        var maxHealth = PlayerPrefs.GetFloat("Mage_MaxHealth", mage.MaxHealth);
                        var specialUpgradeCount = PlayerPrefs.GetInt("Mage_SpecialUpgradeCount", 0);

                        // Update values
                        mage.MaxHealth = maxHealth;
                        mage.MaxMana += 20 * specialUpgradeCount;

                        // Print to check
                        Debug.Log("Mage health: " + mage.MaxHealth + " Mana: " + mage.MaxMana);

                        break;
                    }
                    case PlayerBase.PlayerType.Warrior:
                    {
                        // Convert to type
                        var warrior = (Warrior) player;

                        // Read in values, default values are the standards
                        var maxHealth = PlayerPrefs.GetFloat("Warrior_MaxHealth", warrior.MaxHealth);
                        var specialUpgradeCount = PlayerPrefs.GetInt("Warrior_SpecialUpgradeCount", 0);

                        // Update values
                        warrior.MaxHealth = maxHealth;
                        warrior.SwordDamage += 5 * specialUpgradeCount;

                        // Print to check
                        Debug.Log("Warrior health: " + warrior.MaxHealth + " Sword: " + warrior.SwordDamage);

                        break;
                    }
                    case PlayerBase.PlayerType.Archer:
                    {
                        // Convert to type
                        var archer = (Archer) player;

                        // Read in values, default values are the standarsd
                        var maxHealth = PlayerPrefs.GetFloat("Archer_MaxHealth", archer.MaxHealth);
                        var specialUpgradeCount = PlayerPrefs.GetInt("Archer_SpecialUpgradeCount", 0);

                        // Update values
                        archer.MaxHealth = maxHealth;
                        archer.BowSpeed -= 0.15f * specialUpgradeCount;

                        // Print to check
                        Debug.Log("Archer health: " + archer.MaxHealth + " Bow speed: " + archer.BowSpeed);

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

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

        // Update coin UI
        if (_coinCounterText)
        {
            _coinCounterText.text = _sessionCoins.ToString() + '(' + _coinsAtStart + ')';
        }
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
