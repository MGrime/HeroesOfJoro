using System;
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

    private Text _specialText;

    private PlayerManager _playerManager;
    private int _levelingPlayerIndex;   // The player we are editing

    private bool _isActive = false;

    // Store count of how many upgrades have been applied to each player


    #region Level scaling config

    // Can upgrade infinitely
    [SerializeField] private int _healthUpgradeCost = 5;

    // Base costs
    [SerializeField] private int _mageManaUpgradeCost = 10;
    [SerializeField] private int _warriorDamageUpgradeCost = 7;
    [SerializeField] private int _archerChargeSpeedUpgradeCost = 15;

    // Get more expensive over time
    [SerializeField] private float _scalingCostIncrease = 1.5f;

    private int _mageManaUpgradeCount = 0;
    private int _warriorDamageUpgradeCount = 0;
    private int _archerChargeSpeedUpgradeCount = 0;

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
            else if (text.name == "Special Upgrade Text")
            {
                _specialText = text;
            }
        }


        // Read in pref values
        _mageManaUpgradeCount = PlayerPrefs.GetInt("Mage_SpecialUpgradeCount", 0);
        _archerChargeSpeedUpgradeCount = PlayerPrefs.GetInt("Archer_SpecialUpgradeCount", 0);
        _warriorDamageUpgradeCount = PlayerPrefs.GetInt("Warrior_SpecialUpgradeCount", 0);

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

    public void IncreaseSpecial()
    {
        ref var player = ref _playerManager.PlayerTrackers[_levelingPlayerIndex];

        switch (player.Type)
        {
            case PlayerBase.PlayerType.Mage:
                if (_coins - Convert.ToInt32(_mageManaUpgradeCost * _scalingCostIncrease) < 0)
                {
                    if (_errorSound)
                    {
                        _errorSound.Play();
                    }
                }
                else
                {
                    _mageManaUpgradeCount++;
                    _coins -= Convert.ToInt32(_mageManaUpgradeCost * _scalingCostIncrease);
                }
                break;
            case PlayerBase.PlayerType.Warrior:
                if (_coins - Convert.ToInt32(_warriorDamageUpgradeCost * _scalingCostIncrease) < 0)
                {
                    if (_errorSound)
                    {
                        _errorSound.Play();
                    }
                }
                else
                {
                    _warriorDamageUpgradeCount++;
                    _coins -= Convert.ToInt32(_warriorDamageUpgradeCost * _scalingCostIncrease);
                }
                break;
            case PlayerBase.PlayerType.Archer:
                if (_coins - Convert.ToInt32(_archerChargeSpeedUpgradeCost * _scalingCostIncrease) < 0)
                {
                    if (_errorSound)
                    {
                        _errorSound.Play();
                    }
                }
                else
                {
                    _archerChargeSpeedUpgradeCount++;
                    _coins -= Convert.ToInt32(_archerChargeSpeedUpgradeCost * _scalingCostIncrease);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion

    #region UI Helpers

    public void UpdateStatsText()
    {
        ref var player = ref _playerManager.PlayerTrackers[_levelingPlayerIndex];

        if (_playerTypeText)
        {
            _playerTypeText.text = player.Type.ToString();
        }

        if (_healthText)
        {
            _healthText.text = "Health:     " + player.MaxHealth;
        }

        if (_coinText)
        {
            _coinText.text = _coins.ToString();
        }

        if (_specialText)
        {
            switch (player.Type)
            {
                case PlayerBase.PlayerType.Mage:
                    var mana = Convert.ToInt32(((Mage) player).MaxMana + 20 * _mageManaUpgradeCount);
                    _specialText.text = "Mana: " + mana.ToString();
                    break;
                case PlayerBase.PlayerType.Warrior:
                    var damage = Convert.ToInt32(((Warrior) player).SwordDamage + 5 * _warriorDamageUpgradeCount);
                    _specialText.text = "Damage: " + damage.ToString();
                    break;
                case PlayerBase.PlayerType.Archer:
                    var speed = Convert.ToInt32(
                        ((ArcherBetter) player).BowSpeed - 0.15f * _archerChargeSpeedUpgradeCount);
                    _specialText.text = "Bow Speed (Secs): " + speed.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                PlayerPrefs.SetInt("Warrior_SpecialUpgradeCount",_warriorDamageUpgradeCount);
            }
            // Read and save archer stats
            if (player.Type == PlayerBase.PlayerType.Archer)
            {
                PlayerPrefs.SetFloat("Archer_MaxHealth", player.MaxHealth);
                PlayerPrefs.SetInt("Archer_SpecialUpgradeCount", _archerChargeSpeedUpgradeCount);
            }
            // Read and save mage stats
            if (player.Type == PlayerBase.PlayerType.Mage)
            {
                PlayerPrefs.SetFloat("Mage_MaxHealth", player.MaxHealth);
                PlayerPrefs.SetInt("Mage_SpecialUpgradeCount", _mageManaUpgradeCount);
            }
        }


        PlayerPrefs.Save();
    }

    #endregion
}
