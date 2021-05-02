using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBetter : PlayerBase
{
    #region Editor Fields

    // Bow object that actually fires the arrow
    [SerializeField] private BowBase _bow;

    // Animator to control draw animations manually
    [SerializeField] private Animator _animator;

    [SerializeField] private AudioSource _bowDrawSound;
    private bool _manualPlayingTracker;

    [SerializeField] private AudioSource _bowShootSound;

    public float BowSpeed
    {
        get => _bow.HoldTime;
        set => _bow.HoldTime = value;
    }

    #endregion

    #region Private Data

    // Track how long we have been holding the mouse button and if we are currently charging
    private float   m_TimeHeld;

    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        Type = PlayerType.Archer;

        m_TimeHeld = 0.0f;

        // The sound clip is .8 seconds.
        // We need to match it to the current bow speed
        float extendRatio = _bow.HoldTime / 0.8f;

        // This will sync length to be the same
        _bowDrawSound.pitch = extendRatio / 10.0f;
        _manualPlayingTracker = false;

        // Set volume
        float volume = PlayerPrefs.GetFloat("SoundEffectVolume", 1.0f);

        _bowDrawSound.volume = volume;
        _bowShootSound.volume = volume;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        // Left click hold to change fire
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!_manualPlayingTracker)
            {
                _bowDrawSound.Play();

                // Unity play will reset when clip finished if player holds longer than max bow time
                _manualPlayingTracker = true;
            }
            // Increase upto the bow max
            if (m_TimeHeld < _bow.HoldTime)
            {
                m_TimeHeld += Time.deltaTime;
            }

            // Trigger animation after a fractional delay to stop gltichy spam
            if (m_TimeHeld > _bow.HoldTime / 10.0f)
            {
                if (!_animator.GetBool("isDrawn"))
                {
                    _animator.SetBool("isDrawn", true);
                }
            }
        }
        else
        {
            // Only check when key released
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (_bowDrawSound.isPlaying)
                {
                    _bowDrawSound.Stop();
                }

                _manualPlayingTracker = false;

                if (m_TimeHeld > _bow.HoldTime / 10.0f)
                {
                    // Switch out the animation
                    if (_animator.GetBool("isDrawn"))
                    {
                        _animator.SetBool("isDrawn", false);
                    }

                    if (!_bowShootSound.isPlaying)
                    {
                        _bowShootSound.Play();
                    }

                    // Fire
                    //_bow.ChargedFire(m_TimeHeld);

                }
                // Reset
                m_TimeHeld = 0.0f;
            }
        }
    }
}
