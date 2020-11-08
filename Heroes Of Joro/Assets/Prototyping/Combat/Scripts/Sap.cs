using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a targeting spell
// It applies low damage while the target is STOOD in the square

public class Sap : SpellBase
{
    #region Editor Fields

    [SerializeField] private ParticleSystem _particleVfx;
    [SerializeField] private Collider _collisionZone;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private uint _pulseCount;

    #endregion

    #region Private Data

    private uint _remainingPulses;

    private List<Collider> _inRangeEnemies;

    #endregion

    #region Functions
    private void Start()
    {
        // Set pulses
        _remainingPulses = _pulseCount;

        // Init list
        _inRangeEnemies = new List<Collider>();
    }

    private void Update()
    {
        // Check if pulses remaining
        if (_remainingPulses != 0)
        {
            // This means not started OR a pulse has just finished
            if (!_particleVfx.isPlaying)
            {
                _particleVfx.Play();
                --_remainingPulses;

                // Send damage to all eneimies in range
                foreach (Collider enemy in _inRangeEnemies)
                {
                    // TODO: SEND DAMAGE
                    if (enemy)  // Could be null even if destroyed
                    {
                        Debug.Log("Sent damage to " + enemy.name);
                    }
                }
            }
        }
        else
        {
            // Destroy
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Entering trigger adds this collider to the inrange enemies
        _inRangeEnemies.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        // Exiting removes them
        _inRangeEnemies.Remove(other);  

    }

    #endregion
}
