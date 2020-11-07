using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovementScript : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private float _turnSmoothTime = 0.1f;

    #endregion

    #region Private Data

    private float _turnSmoothVelocity;

    #endregion

    #region Functions

    // Update is called once per frame
    void Update()
    {
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
            Vector3 moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            _characterController.Move(moveDirection.normalized * _moveSpeed * Time.deltaTime);

            // Set the animation to play
            _playerAnimator.SetBool("Moving", true);
        }
        else
        {
            // We aren't moving so stop the animation
            _playerAnimator.SetBool("Moving", false);
        }

    }

    #endregion
}
