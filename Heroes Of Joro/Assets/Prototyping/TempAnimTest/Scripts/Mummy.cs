using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Mummy : MonoBehaviour
{
    public static event Action Spawned;
    public static event Action Died;

    NavMeshAgent _navMeshAgent;
    Animator _animator;
    
    Coroutine _hitRoutine;
    int _health;

    [SerializeField] int _startingHealth = 2;
    [SerializeField] float _attackRange = 1.5f;

    bool Dead => _health <= 0;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Spawned?.Invoke();
    }

    public void StartWalking()
    {
        if (Dead)
            return;

        _navMeshAgent.enabled = true;
        Debug.Log("Starting Walk", gameObject);
        _animator.SetBool("Walk", true);
    }

    void Update()
    {
        if (_navMeshAgent.enabled)
        {
            _navMeshAgent.SetDestination(PlayerMovement.Instance.transform.position);
            if (InAttackRange)
                Attack();
        }
    }

    bool InAttackRange => Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= _attackRange;

    void Attack()
    {
        _navMeshAgent.enabled = false;
        _animator.SetTrigger("Attack");
    }

    // Animation Callback
    void AttackHit()
    {
        if (InAttackRange)
            SceneManager.LoadScene(0);
    }
    void AttackComplete() => _navMeshAgent.enabled = true;

    void OnEnable() => _health = _startingHealth;

    void OnCollisionEnter(Collision collision)
    {
        if (Dead)
            return;

        var blasterShot = collision.collider.GetComponent<BlasterShot>();
        if (blasterShot)
        {
            if (_hitRoutine != null)
                StopCoroutine(_hitRoutine);

            _hitRoutine = StartCoroutine(TakeHit());
        }
    }

    private IEnumerator TakeHit()
    {
        _navMeshAgent.enabled = false;

        _health--;
        if (Dead)
        {
            GetComponent<Collider>().enabled = false;
            _animator.SetTrigger("Died");
            Died?.Invoke();
            yield return null;
            Destroy(gameObject, 5f);
        }
        else
        {
            _animator.SetTrigger("Hit");
            yield return new WaitForSeconds(1f);
            _navMeshAgent.enabled = true;
        }
    }
}