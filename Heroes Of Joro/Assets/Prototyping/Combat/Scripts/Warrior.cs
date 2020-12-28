using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warrior : PlayerBase
{
    #region Editor Fields

    // Contains the camera and the 3D model
    [SerializeField] private ThirdPersonMovementScript _physicalPlayer;

    // Contains the sword object
    [SerializeField] private SwordBase _sword;

    // Data
    [SerializeField] private float _maxHealth = 200.0f;

    // UI
    [SerializeField] private Slider _healthBar; 

    #endregion

    #region Private Data

    // Store current health
    private float _health;

    #endregion

    #region Functions

    private void Start()
    {
        Type = PlayerType.Warrior;

        _health = _maxHealth;

        enabled = false;
    }

    private void Update()
    {
        // Update bars
        if (_healthBar)
        {
            _healthBar.value = _health / _maxHealth;
        }

        // Check inputs
        // Left click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SwingSword();
        }

    }

    private void SwingSword()
    {
        if (!_sword._swinging)
        {
            // This will trigger it in the sword function
            _sword._swinging = true;
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
