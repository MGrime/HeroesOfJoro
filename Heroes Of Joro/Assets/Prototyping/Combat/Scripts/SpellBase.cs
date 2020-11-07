using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Acts as a way to store spells in a list
// All extra functionality comes in children classes
// Healing spells can inherit and ignore
// TODO: Other functionality that can go here include the UI icon

// This may seem pointless as we can use unity's component system to do this kind of thing with GameObjects
// But having our own code abstraction allows the children class to implement their movement/detection/behaviour
// without having to go through unity. See line 48 of MagePlayer.cs
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