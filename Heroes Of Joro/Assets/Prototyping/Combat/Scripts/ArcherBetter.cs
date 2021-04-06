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

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Left click hold to change fire
        if (Input.GetKey(KeyCode.Mouse0))
        {
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
                if (m_TimeHeld > _bow.HoldTime / 10.0f)
                {
                    // Switch out the animation
                    if (_animator.GetBool("isDrawn"))
                    {
                        _animator.SetBool("isDrawn", false);
                    }

                    // Fire
                    _bow.ChargedFire(m_TimeHeld);

                }
                // Reset
                m_TimeHeld = 0.0f;
            }
        }
    }
}
