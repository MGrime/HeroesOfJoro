using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Acts as a way to store spells in a list
// All extra functionality comes in children classes
// Healing spells can inherit and ignore
// TODO: Other functionality that can go here include the UI icon
public class SpellBase : MonoBehaviour
{
    [SerializeField] private float _damage = 0.0f;
    public void SetDamage(float newDamage)
    {
        _damage = newDamage;
    }

    public float GetDamage()
    {
        return _damage;
    }
}