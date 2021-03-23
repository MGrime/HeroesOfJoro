using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private Animator _enemyAnimator;

    //public bool _lookAround = true;
    // Start is called before the first frame update
    void Start()
    {
        if (_enemyAnimator != null)//Default starting state
        {

        }
    }

    //public void LookAround()
    //{
    //    //_enemyAnimator.SetBool("isWalking", !_lookAround);
    //}
}
