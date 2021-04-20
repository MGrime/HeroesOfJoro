using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowBase : MonoBehaviour
{
    #region Editor Fields

    // Arrow to fire.
    [SerializeField] private GameObject _arrow;

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

        // Create arrow
        Arrow newArrow = Instantiate(_arrow, gameObject.transform.position, gameObject.transform.rotation).GetComponent<Arrow>();


        newArrow.Fire(_fastDamage);
    }

    public void ChargedFire(float heldTime)
    {
        Debug.Log("Charged");

        float percentage = _holdTime / heldTime;
        Transform ArrowPos =
            transform.Find("Hips").transform.Find("Spine").transform.Find("Spine1").transform.Find("Spine2").transform.Find("LeftShoulder").transform.Find("LeftArm").transform.Find("ArrowStartPos");
        // Create arrow
        Arrow newArrow = Instantiate(_arrow, ArrowPos.position, gameObject.transform.rotation).GetComponent<Arrow>();

        newArrow.Fire(_chargedBaseDamage * percentage);
    }
   
    #endregion
}
