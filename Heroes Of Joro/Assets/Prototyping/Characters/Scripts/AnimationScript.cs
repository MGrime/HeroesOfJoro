using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private bool _toWalk=true;
    // Start is called before the first frame update
    void Start()
    {
        if (_playerAnimator != null)//Default starting state
        {
            _playerAnimator.SetBool("isWalking", false);
            _playerAnimator.SetBool("isRunning", false);
            _playerAnimator.SetBool("isIdle", true);
            _toWalk = true;
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
            }
            if (Input.GetKey(KeyCode.LeftShift))//Transition to Run
            {
                _playerAnimator.SetBool("isWalking", false);
                _playerAnimator.SetBool("isIdle", false);
                _playerAnimator.SetBool("isRunning", true);
                _toWalk = false;
            }
            else if (!_toWalk) _toWalk = true;
        }
        //Transition to idle
        else 
        {
            _playerAnimator.SetBool("isWalking", false);
            _playerAnimator.SetBool("isRunning", false);
            _playerAnimator.SetBool("isIdle", true);
        }
    }
}
