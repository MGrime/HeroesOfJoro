using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private float _lookRadius = 10.0f;
    [SerializeField] private float _smoothRotation = 5.0f;
    [SerializeField] public GameObject _patrolPoint;
    [SerializeField] public GameObject _enemyProjectile;
    [SerializeField] public int _enemyHealth = 100;
    [SerializeField] private float _setAttackTime = 2.0f;
    [SerializeField] private Animator _enemyAnimator;

    #endregion
    #region Private Data
    private Transform _target;
    private float _attackTime;
    public float _lookTime;
    private NavMeshAgent _agent;
    private Vector3 _startingPosition;
    public bool _switchPoints;
    public bool _lookAround = false;
    private int _playerDamage;


    //Will change them to private later need them there for debugging
    public float distanceToPlayer;
    public float distanceToStart;
    public float distanceToPoint;
    private const float _minDistance = 3.0f;
    #endregion
    #region Functions
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _startingPosition = transform.position;
        _attackTime = _setAttackTime;
        //enabled = false;
        //_target = PlayerManager._instance.PlayerTracker.transform;

    }

    public void Enable()
    {
        enabled = true;
        _patrolPoint.transform.position += new Vector3(0.0f,0.0f,5.0f);
        _switchPoints = true;
        _target = PlayerManager._instance.PlayerTracker.transform;
        

    }

    // Update is called once per frame
    void Update()
    {
       
        _target = PlayerManager._instance.PlayerTracker.transform; 
        distanceToPlayer = Vector3.Distance(_target.position, transform.position);
        distanceToStart = Vector3.Distance(_startingPosition, transform.position);
        distanceToPoint = Vector3.Distance(_patrolPoint.transform.position, transform.position);

        _attackTime -= Time.deltaTime;
        _lookTime -= Time.deltaTime;
        if (distanceToPlayer <= _lookRadius)
        {
            //Start following the player
            _agent.SetDestination(_target.position);
            _switchPoints = true;
            //
            if (distanceToPlayer <= 5.0f)
            {
                if (_attackTime <= 0.0f)
                {  
                    Instantiate(_enemyProjectile, transform.position, Quaternion.identity);
                    _attackTime = _setAttackTime;
                }

                //Face the target
                FaceTarget();
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
        //Return to starting position
        if (distanceToStart <= _minDistance)//Go to patrol point
        {
            if(LookAround())_agent.SetDestination(_patrolPoint.transform.position);
        }
        
        if (distanceToPoint <= _minDistance)//Go to start point
        {
            if(LookAround())_agent.SetDestination(_startingPosition);
        }
    }
    bool LookAround()
    {
        if (!_lookAround)
        {
            //_enemyAnimator.SetBool("isWalking", false);
            _lookTime = 2.0f;
            _lookAround = true;
            return false;
        }
        else if (_lookTime <= 0.0f)
        {
           // _enemyAnimator.SetBool("isWalking", true);
            _lookAround = false;
            return true;
        }
        return false;

    }
    void FaceTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime*_smoothRotation);
    }

    public void SetDamage(int damage)
    {
        _playerDamage = damage;

        // Flash the enemy red when they get hit
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
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
            if (_enemyHealth <= 0)  Destroy(gameObject);
        }
    }

    public IEnumerator UndoDamageFlash()
    {
        yield return new WaitForSeconds(0.2f);

        // Now after a second set back to wait
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
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
