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

    #endregion

    #region Functions

    protected virtual void Start()
    {
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

        }
        else if (_health <= 0)
        {
            //Destroy(gameObject);
            //TO DO: change this so it sends a message to GameOver function in the GameManager class
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
    #endregion



}
