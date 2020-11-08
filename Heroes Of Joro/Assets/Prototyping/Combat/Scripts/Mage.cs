﻿using UnityEngine;
using UnityEngine.UI;

public class Mage : MonoBehaviour
{
    #region Editor Fields

    // Contains the camera and the 3D model
    [SerializeField] private ThirdPersonMovementScript _physicalPlayer;
    
    // Contains the projectile objects for each spell
    [SerializeField] private SpellBase[] _spells;

    // Contains the targeting sprite for targeting spells
    [SerializeField] private SpriteRenderer _reticle;

    // How long between spells
    [SerializeField] private float _castCooldown = 2.0f;

    [SerializeField] private float _maxHealth = 100.0f;

    [SerializeField] private float _maxMana = 100.0f;

    #endregion

    #region Private Data

    // Index into the spell array so we can instaniate and fire the spell
    private int _selectedSpellIndex = -1;

    // Control mana fill rate
    private float _manaRefillTimer;

    // Store current health/mana
    private float _health;
    private float _mana;

    private Slider _healthBar;
    private Slider _manaBar;
    private Text _activeSpell;

    #endregion

    #region Functions
    private void Start()
    {
        // Start at the base of the spell array
        if (_spells.Length > 0)
        {
            _selectedSpellIndex = 0;
        }

        _health = _maxHealth;
        _mana = _maxMana;

        // Will enable on game start
        enabled = false;

        // Disable until we know targeting spell equiped
        _reticle.enabled = false;

        // Find is okay on start
        _healthBar = GameObject.Find("Health Bar").GetComponent<Slider>();
        _manaBar = GameObject.Find("Mana Bar").GetComponent<Slider>();
        _activeSpell = GameObject.Find("Active Spell").GetComponent<Text>();
    }

    private void Update()
    {

        // Switch spell index with wheel
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)  // Forward
        {
            // Add 1 / loop

            ++_selectedSpellIndex;

            if (_selectedSpellIndex == _spells.Length)
            {
                _selectedSpellIndex = 0;
            }

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f) // Backwards
        {
            // Take 1 / loop

            --_selectedSpellIndex;

            if (_selectedSpellIndex == -1)
            {
                _selectedSpellIndex = _spells.Length - 1;
            }

        }
        

        // Mana refill
        if (_mana < _maxMana)
        {
            // Refill the timer
            _manaRefillTimer += Time.deltaTime;
            // 3 per second
            if (_manaRefillTimer > 1.0f)
            {
                _manaRefillTimer = 0.0f;

                _mana += 3.0f;
            }
        }
            
        // Update bars
        _healthBar.value = _health / _maxHealth;
        _manaBar.value = _mana / _maxMana;

        // Create a new spell object
        if (_selectedSpellIndex != -1)  // Catch incorrect prefab
        {
            // Get type
            SpellBase selectedSpell = _spells[_selectedSpellIndex];
            _activeSpell.text = "Spell: " + selectedSpell.name;


            if (selectedSpell.GetSpellType() == SpellBase.SpellType.Targeting)
            {
                _reticle.enabled = true;
            }
            else
            {
                _reticle.enabled = false;
            }

            // Need to have enough mana
            if (_mana >= selectedSpell.GetCost() && Input.GetKeyDown(KeyCode.Mouse0))
            {
                // Check type
                if (selectedSpell.GetSpellType() == SpellBase.SpellType.Projectile)
                {
                    FireProjectileSpell();
                }
                if (selectedSpell.GetSpellType() == SpellBase.SpellType.Targeting)
                {
                    // TODO: ADD REAL TARGETING. FOR NOW JUST PLACING 2 UNITS IN FRONT OF PLAYER
                    FireTargetingSpell();
                }

                _mana -= selectedSpell.GetCost();
            }
        }
        

    }

    private void FireProjectileSpell()
    {
        // Create a new spell object
        GameObject newSpellObject = Instantiate(_spells[_selectedSpellIndex].gameObject,_physicalPlayer.transform.position,_physicalPlayer.transform.rotation);

        // This spell script controls the behaviour. Thats the beauty of an extra level of abstraction
    }

    private void FireTargetingSpell()
    {
        // Create spell object at reticle position
        GameObject newSpellObject = Instantiate(_spells[_selectedSpellIndex].gameObject, _reticle.gameObject.transform.position, Quaternion.identity);
    }



    #endregion
}
