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

    #endregion

    #region Functions

    override protected void Start()
    {
        Type = PlayerType.Warrior;

        base.Start();

    }

    // This fixes the ui thing. i have no idea what the real cause of the bug is
    protected void Awake()
    {
        Type = PlayerType.Warrior;

    }

    override protected void Update()
    {
        base.Update();

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
