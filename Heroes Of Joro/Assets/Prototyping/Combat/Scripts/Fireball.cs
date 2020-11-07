using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Fireball : SpellBase
{
    #region Editor Fields

    [SerializeField] private ParticleSystem _particleVfx;
    [SerializeField] private ParticleSystem _explosionVfx;
    [SerializeField] private Collider _collisionZone;
    [SerializeField] private Rigidbody _rigidBody;

    #endregion

    #region Private Data

    #endregion

    #region Functions

    private void Start()
    {
        _rigidBody.velocity = transform.forward * 20.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
