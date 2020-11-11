﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private float _lookRadius = 10.0f;
    [SerializeField] private float _smoothRotation = 5.0f;
    [SerializeField] private GameObject _patrolPoint;
    #endregion
    #region Private Data
    private Transform _target;
    private NavMeshAgent _agent;
    private Vector3 _startingPosition;
    private bool _switchPoints;
    #endregion
    #region Functions
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _startingPosition = transform.position;
        _patrolPoint.transform.position = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 5.0f);
        _switchPoints = true;
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_target)
        {
            _target = PlayerManager._instance.PlayerTracker.transform;
        }

        float distanceToPlayer = Vector3.Distance(_target.position, transform.position);
        float distanceToStart = Vector3.Distance(_startingPosition, transform.position);
        float distanceToPoint = Vector3.Distance(_patrolPoint.transform.position, transform.position);

        if (distanceToPlayer <= _lookRadius)
        {
            //Start following the player
            _agent.SetDestination(_target.position);
            _switchPoints = true;
            //
            if (distanceToPlayer <= _agent.stoppingDistance)
            {
                //TODO:Attack player (*to be implemented)

                //Face the target
                FaceTarget();
            }
        }
        else //Patrol
        {
            //Return to starting position
            if ( _switchPoints) _agent.SetDestination(_startingPosition);
            if (distanceToStart <= _agent.stoppingDistance)//Go to patrol point
            {
                _agent.SetDestination(_patrolPoint.transform.position);
                _switchPoints = false;
            }
            if (distanceToPoint <= _agent.stoppingDistance)//Go to start point
            {
                _switchPoints = true;
            }
            
        }
    }
    #endregion 
    #region Enemy Behaviour Functions
   
    void FaceTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime*_smoothRotation);
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