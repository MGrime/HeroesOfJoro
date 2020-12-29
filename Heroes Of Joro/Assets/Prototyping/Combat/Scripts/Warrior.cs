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

    // UI
    [SerializeField] private Slider _healthBar; 

    #endregion

    #region Functions

    private void Start()
    {
        Type = PlayerType.Warrior;

        Health = MaxHealth;

        enabled = false;
    }

    private void Update()
    {
        // Update bars
        if (_healthBar)
        {
            _healthBar.value = Health / MaxHealth;
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

    #endregion
}
