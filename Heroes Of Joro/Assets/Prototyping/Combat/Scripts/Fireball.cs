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

        _particleVfx.Play(false);
    }

    private IEnumerator DestroySpellCheck()
    {
        while (true)
        {
            if (!_explosionVfx.isPlaying)
            {
                DestroyImmediate(gameObject);
                yield break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" && other.tag != "Spell")
        {
            _particleVfx.Stop();

            // Kill all momentum
            _rigidBody.velocity = Vector3.zero;

            _explosionVfx.Play(false);

            // Set off destroy check
            StartCoroutine("DestroySpellCheck");
        }
    }

    #endregion
}
