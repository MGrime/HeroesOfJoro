using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovementScript : MonoBehaviour
{
    #region EDITOR FIELDS

    // Constants to control movement
    [SerializeField] private const float Speed = 5.0f;
    [SerializeField] private const float RunSpeed = 12.0f;

    // References to important parts of the character object to track
    [SerializeField] private CharacterController _characterController = null;
    [SerializeField] private Transform _playerCamera = null;

    // Variables to control other aspects of the characters movement
    [SerializeField] private float _turnSmoothTime = 0.1f;
    [SerializeField] private float _gravity = 5.0f;
    [SerializeField] private float _vSpeed;

    // Reference to the sound object to play when moving
    [SerializeField] private AudioSource _footstepSound;
    public AudioSource FootstepSound
    {
        get => _footstepSound;
        set => _footstepSound = value;
    }

    // Reference to the animator to control animation state
    [SerializeField] private Animator _playerAnimator = null;

    #endregion

    #region PUBLIC ACCESSORS

    // Track how many clicks for chained animations
    public static int NumOfClicks;

    #endregion

    #region PRIVATE DATA

    // Time the button was last clicked
    private float _lastClick;
    private float _lastAttack;

    // Delay between each click
    private float combatDelay = 1.0f;

    // Variables to store how our current movement state
    private float _moveSpeed = 10.0f;
    private float _turnSmoothVelocity;
    private Vector3 _moveDirection;

    #endregion

    #region FUNCTION
    private void Awake()
    {
        // Start with no control. Dungeon will enable when complete
        enabled = false;
    }

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
            // Ensure correct animation state is set
            _playerAnimator.SetBool("isIdle", false);

            // Play the sound if it is not already playing
            if (!_footstepSound.isPlaying)
            {
                _footstepSound.Play();
            }

            // Calculate the angle we will finish at
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _playerCamera.eulerAngles.y;

            // Calculate angle from our current angle to the target angle whilst applying smoothing
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);

            // Set the new smoother angle
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            // Move the model in the direction
            _moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            if (_characterController.isGrounded)
            {
                _vSpeed = 0;
            }
            // Reduce vertical speed by gravity 
            _vSpeed -= _gravity * Time.deltaTime;

            // Set our current direction tracker y to vertical speed
            _moveDirection.y = _vSpeed;

            // Transition to run animation
            // Requires key to be held down and the archer cannot be drawing the bow
            if (Input.GetKey(KeyCode.LeftShift) && !_playerAnimator.GetBool("isDrawn"))
            {
                // If the move speed has not yet reached 
                if (_moveSpeed < RunSpeed)
                { 
                    // Gradually increase the move speed. This will be a mostly imperceptible transition but it helps smoothing 
                    _moveSpeed += 10.0f * Time.deltaTime;
                }
                
                // Set correct animator state
                _playerAnimator.SetBool("isRunning",true);
                _playerAnimator.SetBool("isRunningArcher", true);

            }
            else 
            {
                // When we aren't running ensure speed locks to standard move speed
                _moveSpeed = Speed;

                // Set correct animator state
                _playerAnimator.SetBool("isRunning", false);
                _playerAnimator.SetBool("isRunningArcher", false);


            }

            // Use tracked variables to move the character
            _characterController.Move(_moveDirection.normalized * _moveSpeed * Time.deltaTime);
        }
        else
        {
            // Remove any remaining movement direction
            _moveDirection.x = 0.0f;
            _moveDirection.z = 0.0f;

            // Ensure correct idle animation plays
            _playerAnimator.SetBool("isIdle", true);

            // If the sound is still playing stop it
            if (_footstepSound.isPlaying)
            {
                _footstepSound.Stop();
            }
        }


        // Calculate Z and X velocity for animation blending
        var velocityZ = Vector3.Dot(_moveDirection.normalized, transform.forward);
        var velocityX = Vector3.Dot(_moveDirection.normalized, transform.right);

        // Set animation blending variables 
        _playerAnimator.SetFloat("VelocityZ", velocityZ, 0.2f, Time.deltaTime);
        _playerAnimator.SetFloat("VelocityX", velocityX, 0.2f, Time.deltaTime);

        // Clicking is tracked backwards to ensure nice chaining of animations based on how long it takes you to click
        _lastClick -= Time.deltaTime;

        // Tracks how long since our last attack was started
        _lastAttack += Time.deltaTime;

        // Update number of clicks in animator
        if (NumOfClicks < 0)
        {
            _playerAnimator.SetInteger("numberOfClicks", NumOfClicks);
        }

        // If the mouse was just clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Set that we are attacking
            _playerAnimator.SetBool("isAttacking", true);

            // Call onclick logic
            OnClick();

            // Stop archer from running if he is as cannot run while attacking
            _playerAnimator.SetBool("isRunningArcher", false);


        }
        // Mouse was just released
        else if (Input.GetMouseButtonUp(0))
        {
            // Set attacking is done
            _playerAnimator.SetBool("isAttacking", false);

            // Reset time since last attack
            _lastAttack = 0.0f;
        }
        // If last click goes under zero its been long enough since last click to reset clicks to 0
        else if (_lastClick <= 0.0f)
        {
            ResetNumOfClicks();
        }
        // If our last attack was long enough ago
        else if (_lastAttack > 0.2f)
        {
            // Reset all attack chaining animations
            _playerAnimator.SetBool("Attack1", false);
            _playerAnimator.SetBool("Attack2", false);
            _playerAnimator.SetBool("Attack3", false);

            // Reset clicks to 0
            ResetNumOfClicks();
            _playerAnimator.SetInteger("numberOfClicks", NumOfClicks);


        }

        // If we have right clicked and are idling
        if (Input.GetMouseButtonDown(1) && _playerAnimator.GetBool("isIdle"))
        {
            // Set special attack animation
            _playerAnimator.SetBool("isSpecialAttack", true);
        }

        // If we have right clicked are not running (but are moving)
        if (Input.GetMouseButtonDown(1) && !_playerAnimator.GetBool("isRunning"))
        {
            // Set blocking animation
            _playerAnimator.SetBool("isBlocking", true);

        }
        // Mouse has been released
        else if (Input.GetMouseButtonUp(1))
        {
            // Reset both
            _playerAnimator.SetBool("isSpecialAttack", false);
            _playerAnimator.SetBool("isBlocking", false);

        }
        // If we start running regardless of click state
        if (_playerAnimator.GetBool("isRunning"))
        {
            // Reset both
            _playerAnimator.SetBool("isSpecialAttack", false);
            _playerAnimator.SetBool("isBlocking", false);
        }
    }

    // Left click logic
    void OnClick()
    {
        // No longer blocking
        _playerAnimator.SetBool("isBlocking", false);

        //Record time of last button click
        _lastClick = combatDelay;

        // Increase click count and update
        NumOfClicks++;
        _playerAnimator.SetInteger("numberOfClicks", NumOfClicks);

        // If we are more than one click or equal
        if (NumOfClicks >= 1)
        {
            // First attack anim
            _playerAnimator.SetBool("AttackOne", true);
        }
        // Else this is anything less than 2
        else if (NumOfClicks < 2)
        {
            // Second attack anim
            _playerAnimator.SetBool("AttackTwo", false);
        }
        //Limit the number of combos by clamping
        NumOfClicks = Mathf.Clamp(NumOfClicks, 0, 3);
       
    }
    public static void ResetNumOfClicks() 
    { 
        NumOfClicks = 0;
    }
    #endregion
}
