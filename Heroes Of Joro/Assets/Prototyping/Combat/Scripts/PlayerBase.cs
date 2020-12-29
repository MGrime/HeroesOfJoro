using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    #region Data

    // Storing this helps with manager code
    public enum PlayerType
    {
        Mage,
        Warrior
    }
    private PlayerType _type;
    public PlayerType Type
    {
        get => _type;
        set => _type = value;
    }

    // Common things between all player types here
    private float _health;
    public float Health
    {
        get => _health;
        set => _health = value;
    }

    [SerializeField] private float _maxHealth;
    public float MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    #endregion

    #region Functions
    // Message functions
    public void ReceiveDamage(int damage)
    {
        if (_health > 0)
        {
            _health -= damage;
            Debug.Log("Damage received:" + damage);

        }
        else if (_health <= 0)
        {
            //Destroy(gameObject);
            //TO DO: change this so it sends a message to GameOver function in the GameManager class
        }
    }
    public void PickupHealth()
    {
        if (_health < _maxHealth - 50.0f)
        {
            _health += 50.0f;
        }
        else
        {
            _health = _maxHealth;
        }
    }
    #endregion



}
