using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : PlayerBase
{
    #region Editor Fields

    // Contains the bow object
    [SerializeField] private BowBase _bow;

    #endregion

    #region Private Data

    private bool m_HoldingFire = false;
    private float m_TimeHeld = 0.0f;

    #endregion

    // Start is called before the first frame update
    override protected void Start()
    {
        Type = PlayerType.Archer;

        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        // Only one type of attack per frame
        bool firing = false;

        // Left click will fire arrows fast

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _bow.FastFire();
            firing = true;
        }

        // Right click is slow but charged shot
        // Whilst held
        if (Input.GetKey(KeyCode.Mouse1))
        {
            // While we have been holding it
            if (m_HoldingFire && m_TimeHeld <= _bow.HoldTime)
            {
                // Add to time
                m_TimeHeld += Time.deltaTime;

                Debug.Log(m_TimeHeld);
            }
            else if (!m_HoldingFire)
            {
                // Set it to true and init timer
                m_TimeHeld = 0.0f;
                m_HoldingFire = true;
            }
        }
        // Not held OR we have held max time
        if (!Input.GetKey(KeyCode.Mouse1) || m_TimeHeld >= _bow.HoldTime)
        {
            // Check if we have been holding it and now let go
            if (m_HoldingFire)
            {
                _bow.ChargedFire(m_TimeHeld);

                // Reset bool
                m_HoldingFire = false;
                m_TimeHeld = 0.0f;
            }
        }

        base.Update();
    }
}
