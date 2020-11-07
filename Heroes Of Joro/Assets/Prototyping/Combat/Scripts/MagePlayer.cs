using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagePlayer : MonoBehaviour
{
    #region Editor Fields

    // Contains the camera and the 3D model
    [SerializeField] private ThirdPersonMovementScript _physicalPlayer;
    
    // Contains the projectile objects for each spell
    [SerializeField] private SpellBase[] _spells;

    #endregion

    #region Private Data

    // Index into the spell array so we can instaniate and fire the spell
    private int _selectedSpellIndex = -1;

    #endregion

    #region Functions



    #endregion
}
