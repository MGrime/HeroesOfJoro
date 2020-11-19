using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


// This is a projectile spell
// It does damage on hit


public class Fireball : SpellBase
{
    #region Editor Fields

    [SerializeField] private ParticleSystem _particleVfx;
    [SerializeField] private ParticleSystem _explosionVfx;
    [SerializeField] private Collider _collisionZone;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private AudioSource _sound;
    #endregion

    #region Private Data

    private static float _effectVolume = -1.0f;

    #endregion

    #region Functions

    private void Start()
    {
        SetDamage(50);

        _rigidBody.velocity = transform.forward * 20.0f;

        _particleVfx.Play(false);

        if (Mathf.Approximately(_effectVolume, -1.0f))
        {
            if (PlayerPrefs.HasKey("SoundEffectVolume"))
            {
                _effectVolume = PlayerPrefs.GetFloat("SoundEffectVolume");
            }
        }

        _sound.volume = _effectVolume;

        _sound.Play();
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

            // Send damage message here!
            Debug.Log("Sent damage to " + other.name);
            if (other.name == "EnemyControls")
            {
                other.gameObject.SendMessage("SetDamage", GetDamage());
                other.gameObject.SendMessage("ReduceHealth");
            }
            // Set off destroy check
            StartCoroutine("DestroySpellCheck");
        }
    }

    public void PlaySound()
    {
        _sound.Play();
    }

    #endregion
}
