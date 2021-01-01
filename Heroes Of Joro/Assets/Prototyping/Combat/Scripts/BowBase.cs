using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowBase : MonoBehaviour
{
    #region Editor Fields

    // Arrow to fire.
    [SerializeField] private Arrow _arrow;

    // damages
    [SerializeField] private float _fastDamage;
    [SerializeField] private float _chargedBaseDamage;

    // Max hold for this bow
    [SerializeField] private float _holdTime;

    #endregion

    #region Code Fields

    public float HoldTime
    {
        get => _holdTime;
        set => _holdTime = value;
    }


    #endregion


    #region Functions

    public void FastFire()
    {
        Debug.Log("Fast");
    }

    public void ChargedFire(float heldTime)
    {
        Debug.Log("Charged");
    }

    #endregion
}
