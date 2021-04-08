using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBase : MonoBehaviour
{
    #region Data

    // Storing this helps with manager code
    public enum PlayerType
    {
        Mage,
        Warrior,
        Archer
    }
    private PlayerType _type;
    public PlayerType Type
    {
        get => _type;
        set => _type = value;
    }

    // Common things between all player types here
    private float _health;
    public float Health
    {
        get => _health;
        set => _health = value;
    }

    [SerializeField] private float _maxHealth;
    public float MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    // UI
    [SerializeField] private Slider _healthBar;
    public Slider HealthBar
    {
        get => _healthBar;
        set => _healthBar = value;
    }

    private GameManager _manager;

    // Store a reference to the player manager
    private PlayerManager _playerManager;

    #endregion

    #region Functions

    protected virtual void Start()
    {
        _manager = null;
        _manager = GameObject.FindObjectOfType<GameManager>();

        _playerManager = null;
        _playerManager = FindObjectOfType<PlayerManager>();

        // This is called after the higher up variants have set their type
        // So we can check type here
        switch (Type)
        {
            case PlayerType.Warrior:
                if (PlayerPrefs.HasKey("Warrior_MaxHealth"))
                {
                    _maxHealth = PlayerPrefs.GetFloat("Warrior_MaxHealth");
                }
                break;
            case PlayerType.Archer:
                if (PlayerPrefs.HasKey("Archer_MaxHealth"))
                {
                    _maxHealth = PlayerPrefs.GetFloat("Archer_MaxHealth");
                }
                break;
            case PlayerType.Mage:
                if (PlayerPrefs.HasKey("Mage_MaxHealth"))
                {
                    _maxHealth = PlayerPrefs.GetFloat("Mage_MaxHealth");
                }
                break;
        }

        _health = _maxHealth;
    }

    protected virtual void Update()
    {
        // Update bars
        if (_healthBar)
        {
            _healthBar.value = _health / _maxHealth;
        }
    }

    // Message functions
    public void ReceiveDamage(int damage)
    {
        if (_health > 0)
        {
            _health -= damage;
            Debug.Log("Damage received:" + damage);

            if (_health <= 0)
            {
                // Let player manager know the player has died
                _playerManager.PlayerDied();
            }

        }
    }
    public void PickupHealth()
    {
        if (_health < _maxHealth - 50.0f)
        {
            _health += 50.0f;
        }
        else
        {
            _health = _maxHealth;
        }
    }

    public void PickupCoin()
    {
        if (_manager)
        {
            _manager.AddCoin();
            Debug.Log("Picked up coin! Coin count: " + _manager.Coins);
        }
    }

    #endregion



}
