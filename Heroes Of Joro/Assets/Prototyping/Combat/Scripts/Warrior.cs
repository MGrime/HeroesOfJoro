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

    private float _swingTimer;

    #endregion

    #region Functions

    protected override void Start()
    {
        Type = PlayerType.Warrior;

        base.Start();

        _swingTimer = 0.0f;

    }

    // This fixes the ui thing. i have no idea what the real cause of the bug is
    protected void Awake()
    {
        Type = PlayerType.Warrior;

    }

    protected override void Update()
    {
        base.Update();

        // Check inputs
        // Left click
        if (Input.GetMouseButtonDown(0))
        {
            SwingSword();
        }

        if (_sword._swinging)
        {
            _swingTimer += Time.deltaTime;

            if (_swingTimer >= 1.0f)
            {
                _swingTimer = 0.0f;
                _sword._swinging = false;

                Debug.Log("Sword not swinging!");
            }
        }

    }

    private void SwingSword()
    {
        if (!_sword._swinging)
        {
            // This will trigger it in the sword function
            _sword._swinging = true;

            Debug.Log("Sword swinging!");
        }
    }

    #endregion
}
