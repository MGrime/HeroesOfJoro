using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to control enemy damage dealing
public class EnemyAttack : MonoBehaviour
{
    #region EDITOR FIELDS

    // Variables to tweak damage dealing rage
    [SerializeField] private float _projectileSpeed=10.0f;
    [SerializeField] private float _projectileLife = 1.0f;
    [SerializeField] private float _distanceTo = 1.0f;
    [SerializeField] private float _distanceAway = 2.0f;

    // Variables to control effectiveness of enemy attack
    [SerializeField] private int _enemyDamage = 5;

    // Tracks duration of enemy attack
    [SerializeField] private float _timeAlive = 3.0f;

    // Shows how fast the enemy things the player is away
    [SerializeField] private float _distanceToPlayer;

    // Shows if the enemy is close enough to deal damage
    [SerializeField] private bool _dealDamage = true;

    #endregion

    #region PRIVATE DATA

    // Private tracking for the player
    private Transform _target;
    private Vector3 _targetPos;

    #endregion

    #region FUNCTIONS

    void Start()
    {
        _target = PlayerManager._instance.ActivePlayer.transform;
        _targetPos = _target.position + new Vector3(0,2,0);

        GetComponent<Rigidbody>().AddTorque(new Vector3(0.0f, 40.0f));
    }

    // Update is called once per frame
    void Update()
    {
        // Update the target in case player has changed
        _target = PlayerManager._instance.ActivePlayer.transform;

        // Calculate the distance to the player
        _distanceToPlayer = Vector3.Distance(_target.position, _targetPos);

        // Update our position to move towards the player
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _projectileSpeed * Time.deltaTime);

        // Check if we are wishing damage range
        _dealDamage = !(_distanceToPlayer < _distanceTo);

        // Decrease time alive
        _timeAlive -= Time.deltaTime;
        if (_timeAlive <= 0.0f) DestroyProjectile();
       

    }

    // Called when the something gets close enough to collide
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision entered!");

        // Check if the collision is player
        if (other.CompareTag("Player"))
        {
            // If we are within range
            if (_dealDamage )
            {
                // Deal damage
                AttackPlayer();
            }
        }
    }
    
    // Wrapper to send damage to player
    public void AttackPlayer()
    {
        // Messaging allows easy code, everything other than the player will ignore it
       // _target.gameObject.SendMessage("ReceiveDamage", _enemyDamage);
    }

    // Called when player hits the enemy's projectile 
    void ReduceProjectileHealth(int amount)
    {
        _projectileLife -= amount;
        if (_projectileLife <= 0)
        {
            DestroyProjectile();
        }
    }

    // Removes this game object
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    #endregion
}
