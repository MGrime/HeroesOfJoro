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
    private void Start()
    {
        // Start at the base of the spell array
        if (_spells.Length > 0)
        {
            _selectedSpellIndex = 0;
        }
    }

    private void Update()
    {
        // TODO: USE THE NEW INPUT SYSTEM TO SUPPORT ALL CONTROL TYPES
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireActiveSpell();
        }
    }

    private void FireActiveSpell()
    {
        // Create a new spell object
        GameObject newSpellObject = Instantiate(_spells[_selectedSpellIndex].gameObject,_physicalPlayer.transform.position,_physicalPlayer.transform.rotation);

        // This spell script controls the behaviour. Thats the beauty of an extra level of abstraction
    }



    #endregion
}
