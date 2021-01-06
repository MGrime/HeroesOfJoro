using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private bool _toWalk=true;
    [SerializeField] private bool _toRun = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_playerAnimator != null)//Default starting state
        {
            _playerAnimator.SetBool("isWalking", false);
            _playerAnimator.SetBool("isRunning", false);
            _playerAnimator.SetBool("isIdle", true);
            _playerAnimator.SetBool("isStopping", false);
            _toWalk = true;
            _toRun = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Default transition to Walk
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (_toWalk)
            {
                _playerAnimator.SetBool("isWalking", true);
                _playerAnimator.SetBool("isIdle", false);
                _playerAnimator.SetBool("isRunning", false);
                _playerAnimator.SetBool("isStopping", false);

            }

            if (Input.GetKey(KeyCode.LeftShift))//Transition to Run
            {
                _playerAnimator.SetBool("isWalking", false);
                _playerAnimator.SetBool("isIdle", false);
                _playerAnimator.SetBool("isRunning", true);
                _toWalk = false;
            }
            if (!_toWalk && !Input.GetKey(KeyCode.LeftShift))
            {
                _playerAnimator.SetBool("isStopping", true);
                _playerAnimator.SetBool("isRunning", false);
                _toWalk = true;
            }
        }
        else if (!_toWalk && !Input.GetKey(KeyCode.LeftShift))
        {
            _playerAnimator.SetBool("isStopping", true);
            _playerAnimator.SetBool("isRunning", false);
            _toWalk = true;
        }
        //Transition to idle
        else
        {
           
            _playerAnimator.SetBool("isStopping", false);
            _playerAnimator.SetBool("isWalking", false);
            _playerAnimator.SetBool("isRunning", false);
            _playerAnimator.SetBool("isIdle", true);
        }
       
        
    }
}
