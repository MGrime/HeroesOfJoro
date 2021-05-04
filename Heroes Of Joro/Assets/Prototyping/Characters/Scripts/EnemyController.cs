using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    #region EDITOR FIELDS

    // Controls how much the enemy can see in front
    [SerializeField] private float _lookRadius = 10.0f;

    // Controls the granularity of the rotation
    [SerializeField] private float _smoothRotation = 5.0f;

    // Tracking variables for objects in the world used by the enemy
    [SerializeField] private GameObject _patrolPoint = null;

    // Standard variables to control enemy stats
    [SerializeField] private int _enemyHealth = 100;
    [SerializeField] private float _setAttackTime = 2.0f;

    // Tracking variables for other parts of the enemy object this script needs
    [SerializeField] private Animator _enemyAnimator = null;
    [SerializeField] private PickupBase _dropPickup = null;
    [SerializeField] private EnemyType _enemyType = EnemyType.MutantOne;

    #endregion

    #region PUBLIC ACCESSORS

    public GameObject PatrolPoint => _patrolPoint;

    #endregion

    #region PRIVATE DATA

    // Same controller shared between enemies
    private enum EnemyType
    {
        MutantOne = 0,
        MutantTwo = 1,
    }
    
    // Variables to track how long between state changes and the object that is our current target
    private Transform _target;
    private float _attackTime;
    private float _lookTime;

    // Constants of how long to wait after finishing a state. Different based on if looking around or not
    private const float BufferTime = 3.0f;
    private const float WaitTime = 5.0f;

    // Variables to store information needed for navigation
    private NavMeshAgent _agent;
    private Vector3 _startingPosition;
    private Vector3 _lastPPoint;

    // State variables to store what the enemy should be doing and if we have recently taken damage
    private bool _lookAround;
    private int _playerDamage;
    private bool _playerChased;

    // Variables to track distances to key important things for the enemy to know about
    private const float MinDistance = 3.0f;
    private float _distanceToPlayer;
    private float _distanceToStart;
    private float _distanceToPoint;
    
    #endregion

    #region Functions

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _startingPosition = transform.position;
        _attackTime = _setAttackTime;

    }

    public void Enable()
    {
        enabled = true;
        _patrolPoint.transform.position += new Vector3(0.0f,0.0f,5.0f);
        _target = PlayerManager._instance.ActivePlayer.transform;
        

    }

    // Update is called once per frame
    void Update()
    {
        _target = PlayerManager._instance.ActivePlayer.transform; 
        _distanceToPlayer = Vector3.Distance(_target.position, transform.position);
        _distanceToStart = Vector3.Distance(_startingPosition, transform.position);
        _distanceToPoint = Vector3.Distance(_patrolPoint.transform.position, transform.position);

        _attackTime -= Time.deltaTime;
        _lookTime -= Time.deltaTime;
        if (_distanceToPlayer <= _lookRadius)
        {
            _lookAround = false;
            _playerChased = true;
            _agent.SetDestination(_target.position);
             //Face the target
             FaceTarget();
            //Start following the player
            if (_enemyType == EnemyType.MutantOne)
            {
                if (_distanceToPlayer <= _agent.stoppingDistance)
                {
                    _enemyAnimator.SetBool("Attacking", true);
                    _enemyAnimator.SetBool("isWalking", false);

                }
                else
                {
                    _enemyAnimator.SetBool("Attacking", false);
                    _enemyAnimator.SetBool("isWalking", true);
                }
            }
            else if (_enemyType == EnemyType.MutantTwo)
            {
                StartRunning();
                if (_distanceToPlayer <= _agent.stoppingDistance)
                {
                    StopRunning();
                    _enemyAnimator.SetBool("isAttacking", true);
                }
                else
                {
                    _enemyAnimator.SetBool("isAttacking", false);
                    StartRunning();
                }
            }
            
            
        }
        else //Patrol
        {
            Patrol();

        }
    }



    #endregion

    #region Enemy Behaviour Functions
    void Patrol()
    {
        if (_enemyType == EnemyType.MutantTwo) StopRunning();
        if (_playerChased)
        {
            _agent.SetDestination(_lastPPoint);
            _enemyAnimator.SetBool("Attacking", false);
            _enemyAnimator.SetBool("isWalking", true);
            _playerChased = false;
        }
        else
        {
            //Return to starting position
            if (_distanceToStart <= MinDistance)//Go to patrol point
            {
                _lastPPoint = _patrolPoint.transform.position;
                if (LookAround()) _agent.SetDestination(_patrolPoint.transform.position);
            }

            if (_distanceToPoint <= MinDistance)//Go to start point
            {
                _lastPPoint = _startingPosition;
                if (LookAround()) _agent.SetDestination(_startingPosition);

            }
        }
    }

   
    bool LookAround()
    {
        if (!_lookAround && _lookTime <= 0.0f)
        {
            _enemyAnimator.SetBool("isWalking", false);
            _lookAround = true;
            _lookTime = WaitTime;
            return false;
        }
        if (_lookAround && _lookTime <= 0.0f)
        {
            _enemyAnimator.SetBool("isWalking", true);
            _lookAround = false;
            _lookTime = BufferTime;
            return true;
        }
        return false;

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is hit!");
            other.gameObject.SendMessage("ReceiveDamage", 5);
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime*_smoothRotation);
    }
    void StartRunning()
    {
        _enemyAnimator.SetBool("isRunning", true);
        _enemyAnimator.SetBool("isWalking", false);
        _agent.speed = 5.0f;
    }

    void StopRunning()
    {
        _enemyAnimator.SetBool("isRunning", false);
        _agent.speed = 2.0f;
    }
    
    
    public void SetDamage(int damage)
    {
        _playerDamage = damage;

        // Flash the enemy red when they get hit
        Renderer[] rendererObjects = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rendererObjects)
        {
            r.material.color = Color.red;
        }
        StartCoroutine(UndoDamageFlash());


    }
    public void ReduceHealth()
    {
        if (_enemyHealth > 0)
        {
            _enemyHealth -= _playerDamage;
            if (_enemyHealth <= 0)
            {
                // Spawn a range of coin pickups
                int dropAmount = Random.Range(1, 6);

                for (int i = 0; i < dropAmount; ++i)
                {
                    GameObject coin = Instantiate(_dropPickup.gameObject, transform.position + new Vector3(0.0f,2.0f), Quaternion.identity);
                    coin.transform.SetParent(transform.parent);
                }

                Destroy(gameObject);
            }
        }
    }

    public IEnumerator UndoDamageFlash()
    {
        yield return new WaitForSeconds(0.2f);

        // Now after a second set back to wait
        Renderer[] rendererObjects = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rendererObjects)
        {
            r.material.color = Color.white;
        }

    }

   
    #endregion 
    #region Enemy Vision
    //Visually see with gizmos the look radius of the enemy
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _lookRadius);
    }
    #endregion
}
