using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


// This is a projectile spell
// It does damage on hit
public class Fireball : SpellBase
{
    #region Editor Fields

    [SerializeField] private ParticleSystem _particleVfx = null;
    [SerializeField] private ParticleSystem _explosionVfx = null;
    [SerializeField] private Rigidbody _rigidBody = null;
    [SerializeField] private AudioSource _sound = null;

    #endregion

    #region Private Data

    private static float _effectVolume = -1.0f;

    #endregion

    #region Functions

    private void Start()
    {
        // This spell for 50 on impact 
        SetDamage(50);

        // Flies straight forward
        _rigidBody.velocity = transform.forward * 20.0f;

        // Start the effect (which makes basically all the looks)
        _particleVfx.Play(false);

        // If the volume hasnt been set it based on the saved value
        if (Mathf.Approximately(_effectVolume, -1.0f))
        {
            _effectVolume = PlayerPrefs.GetFloat("SoundEffectVolume",1.0f);
        }

        _sound.volume = _effectVolume;

        _sound.Play();
    }

    // Set off on explosion
    private IEnumerator DestroySpellCheck()
    {
        while (true)
        {
            // When it is done playing
            if (!_explosionVfx.isPlaying)
            {
                // Destroy and stop this thread
                DestroyImmediate(gameObject);
                yield break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only destory on non-player & non-spell collision
        if (other.tag != "Player" && other.tag != "Spell")
        {
            // Stop the effect
            _particleVfx.Stop();

            // Kill all momentum
            _rigidBody.velocity = Vector3.zero;

            // Play the on hit explosion vfx
            _explosionVfx.Play(false);

            // Send correct damage messaged based on type 
            if (other.name == "EnemyControls")
            {
                other.gameObject.SendMessage("SetDamage", GetDamage());
                other.gameObject.SendMessage("ReduceHealth");
            }
            if (other.name == "Face(Clone)")
            {
                other.gameObject.SendMessage("ReduceProjectileHealth", GetDamage());
            }

            // Set off destroy check. Using a co-routine to allow explosion vfx to finish
            StartCoroutine("DestroySpellCheck");
        }
    }

    #endregion
}
