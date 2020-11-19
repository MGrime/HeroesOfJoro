using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Acts as a way to store spells in a list
// All extra functionality comes in children classes
// Healing spells can inherit and ignore
// TODO: Other functionality that can go here include the UI icon

// This may seem pointless as we can use unity's component system to do this kind of thing with GameObjects
// But having our own code abstraction allows the children class to implement their movement/detection/behaviour
// without having to go through unity. See line 48 of Mage.cs
public class SpellBase : MonoBehaviour
{
    public enum SpellType
    {
        Projectile = 0,
        Targeting = 1,
        Effect = 2
    }

    [SerializeField] private SpellType _spellType = SpellType.Projectile;

    public void SetSpellType(SpellType type)
    {
        _spellType = type;
    }

    public SpellType GetSpellType()
    {
        return _spellType;
    }


    [SerializeField] private int _damage = 0;
    public void SetDamage(int newDamage)
    {
        _damage = newDamage;
    }

    public int GetDamage()
    {
        return _damage;
    }

    [SerializeField] private float _manaCost;

    public void SetCost(float newCost)
    {
        _manaCost = newCost;
    }

    public float GetCost()
    {
        return _manaCost;
    }
}