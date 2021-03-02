using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovementScript : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private const float _Speed = 5.0f;
    [SerializeField] private const float _runSpeed = 15.0f;
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private float _turnSmoothTime = 0.1f;
    [SerializeField] private float _gravity = 5.0f;
    [SerializeField] private float _vSpeed = 0.0f;
    [SerializeField] private bool _isTouching = false;

    [SerializeField] private Animator _playerAnimator;
   



    #endregion

    #region Private Data

    private float _turnSmoothVelocity;
    Vector3 moveDirection;

    #endregion

    #region Functions

    private void Start()
    {
              // Start with no control. Dungeon will enable when complete
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        #region Debug keys

        //Debug key to enable movement in character testing scene
        if (Input.GetKey(KeyCode.P) && !enabled) enabled = true;
        else if (Input.GetKey(KeyCode.P) && enabled) enabled = false;

        #endregion
        // Get the input on both axis
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

          // Calculate the movement direction
          Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;
        // If the direction is not null we are moving
        if (direction.magnitude >= 0.1f)
        {
            // Calculate the angle we will finish at
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _playerCamera.eulerAngles.y;

            // Calculate angle from our current angle to the target angle whilst applying smoothing
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);

            // Set the new smoother angle
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            // Move the model in the direction
            moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            if (_characterController.isGrounded)
            {
                _vSpeed = 0;
                _isTouching = true;
            }
            else _isTouching = false;
            _vSpeed -= _gravity * Time.deltaTime;
            moveDirection.y = _vSpeed;

            //Transition to run animation
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _moveSpeed = _runSpeed;
                _playerAnimator.SetBool("isRunning",true);
            }
            else 
            {
                _moveSpeed = _Speed;
                _playerAnimator.SetBool("isRunning", false);

            }


            _characterController.Move(moveDirection.normalized * _moveSpeed * Time.deltaTime);
           


        }
        else
        {
            moveDirection.x = 0.0f;
            moveDirection.z = 0.0f;
        }




        // Animating
        float velocityZ = Vector3.Dot(moveDirection.normalized, transform.forward);
        float velocityX = Vector3.Dot(moveDirection.normalized, transform.right);

        _playerAnimator.SetFloat("VelocityZ", velocityZ, 0.2f, Time.deltaTime);
        _playerAnimator.SetFloat("VelocityX", velocityX, 0.2f, Time.deltaTime);
    }


   
    #endregion
}
