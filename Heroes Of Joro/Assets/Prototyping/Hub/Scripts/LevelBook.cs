using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBook : MonoBehaviour
{
    [SerializeField] private GameObject _overlayCanvas;
    // Extracted from overlay canvas as this is a specific one scene setup
    private Text _playerTypeText;

    private Text _coinText;
    private int _coins;

    private Text _healthText;


    private PlayerManager _playerManager;
    private int _levelingPlayerIndex;   // The player we are editing

    private bool _isActive = false;

    #region Level scaling config

    [SerializeField] private int _healthUpgradeCost = 5;

    #endregion

    [SerializeField] private AudioSource _errorSound;

    public void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _levelingPlayerIndex = 0;

        // Extract UI elements
        // Texts
        foreach (var text in _overlayCanvas.GetComponentsInChildren<Text>())
        {
            if (text.name == "Current Character Text")
            {
                _playerTypeText = text;
            }
            else if (text.name == "Coin Text")
            {
                _coinText = text;

                if (PlayerPrefs.HasKey("Coins"))
                {
                    _coins = PlayerPrefs.GetInt("Coins");
                    _coinText.text = _coins.ToString();
                    
                }
                else
                {
                    PlayerPrefs.SetInt("Coins",0);
                    _coins = 0;
                    _coinText.text = "0";
                    PlayerPrefs.Save();

                }
            }
            else if (text.name == "Health Text")
            {
                _healthText = text;
            }
        }

        UpdateStatsText();
    }
    public void Update()
    {
        // If the user presses button 
        if (Input.GetKeyDown(KeyCode.F) && !_isActive)
        {
            float distance = Vector3.Distance(_playerManager.ActivePlayer.transform.position, transform.position);

            // Within 5 units
            if (distance <= 5.0f)
            {
                Debug.Log("Activated Book!");
                // Open the overlay
                _overlayCanvas.SetActive(true);

                // Force disable movement
                _playerManager.ActivePlayer.GetComponent<ThirdPersonMovementScript>().enabled = false;
                _playerManager.SetCameraRotation(false);
                // Deactive book interaction
                _isActive = true;
            }
        }
    }

    // Region the ui functions for ease of reading.
    // All functions here called by buttons
    #region UI Functions

    public void CloseBook()
    {
        // Close the overlay
        _overlayCanvas.SetActive(false);

        // Force enable movement
        _playerManager.ActivePlayer.GetComponent<ThirdPersonMovementScript>().enabled = true;
        _playerManager.SetCameraRotation(true);


        // activate book interaction
        _isActive = false;

        SaveUpgrades();
    }

    // Move to next player in list
    public void CycleForwardPlayer()
    {
        if (_levelingPlayerIndex + 1 == _playerManager.PlayerTrackers.Length)
        {
            _levelingPlayerIndex = 0;
        }
        else
        {
            _levelingPlayerIndex++;
        }
        UpdateStatsText();
        
    }

    // Move to next player
    public void CycleBackwardPlayer()
    {
        if (_levelingPlayerIndex - 1 == 0)
        {
            _levelingPlayerIndex = _playerManager.PlayerTrackers.Length - 1;
        }
        else
        {
            _levelingPlayerIndex--;
        }
        UpdateStatsText();
    }

    public void IncreaseHealth()
    {
        if (_coins - _healthUpgradeCost < 0)
        {
            if (_errorSound)
            {
                _errorSound.Play();
            }
        }
        else
        {
            _playerManager.PlayerTrackers[_levelingPlayerIndex].MaxHealth += 10.0f;
            _coins -= _healthUpgradeCost;
        }

        UpdateStatsText();
    }

    #endregion

    #region UI Helpers

    public void UpdateStatsText()
    {
        if (_playerTypeText)
        {
            _playerTypeText.text = _playerManager.PlayerTrackers[_levelingPlayerIndex].Type.ToString();
        }

        if (_healthText)
        {
            _healthText.text = "Health:     " + _playerManager.PlayerTrackers[_levelingPlayerIndex].MaxHealth;
        }

        if (_coinText)
        {
            _coinText.text = _coins.ToString();
        }
    }

    public void SaveUpgrades()
    {
        // Save new coin value
        PlayerPrefs.SetInt("Coins",_coins);

        foreach (var player in _playerManager.PlayerTrackers)
        {
            // Read and save warrior stats
            if (player.Type == PlayerBase.PlayerType.Warrior)
            {
                PlayerPrefs.SetFloat("Warrior_MaxHealth", player.MaxHealth);
            }
            // Read and save archer stats
            if (player.Type == PlayerBase.PlayerType.Archer)
            {
                PlayerPrefs.SetFloat("Archer_MaxHealth", player.MaxHealth);
            }
            // Read and save mage stats
            if (player.Type == PlayerBase.PlayerType.Mage)
            {
                PlayerPrefs.SetFloat("Mage_MaxHealth", player.MaxHealth);
            }
        }


        PlayerPrefs.Save();
    }

    #endregion
}
