using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _projectileSpeed=10.0f;
    [SerializeField] private float _distanceTo = 1.0f;
    [SerializeField] private float _distanceAway = 2.0f;

    [SerializeField] public int _enemyDamage = 5;
    [SerializeField] public float _timeAlive = 3.0f;
    [SerializeField] public float distanceToPlayer;

    private Transform _target;
    private Vector3 _targetPos;
    public bool _dealDamage = true;
    void Start()
    {
        _target = PlayerManager._instance.PlayerTracker.transform;
        _targetPos = _target.position;

        GetComponent<Rigidbody>().AddTorque(new Vector3(0.0f, 40.0f));
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(_target.position, _targetPos);
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _projectileSpeed * Time.deltaTime);
        if (distanceToPlayer < _distanceTo) _dealDamage = false;        
        if (distanceToPlayer > _distanceAway) _dealDamage = true;
        _timeAlive -= Time.deltaTime;
        if (_timeAlive <= 0.0f) DestroyProjectile();
       

    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision entered!");

        if (other.transform.name == "Player")
        {
            //TODO: NON Projectile attack
            //On Simple collision with enemy hurt the player every two seconds
            //For complex sword/claw attack I'll implement the following:
            //1. Attach a collider to the claw/sword
            //2. Set attack time for the enemy
            //3. Hurt player when the sword/claw collider hits him
            if (_dealDamage )
            {
                AttackPlayer();
            }
        }
    }
    public void AttackPlayer()
    {
        _target.gameObject.SendMessage("ReceiveDamage", _enemyDamage);
    }
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
