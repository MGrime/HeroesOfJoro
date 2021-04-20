using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovementScript : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private const float _Speed = 5.0f;
    [SerializeField] private const float _runSpeed = 12.0f;
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private float _turnSmoothTime = 0.1f;
    [SerializeField] private float _gravity = 5.0f;
    [SerializeField] private float _vSpeed = 0.0f;
    [SerializeField] private bool _isTouching = false;
    [SerializeField] private AudioSource _footstepSound;

    [SerializeField] private Animator _playerAnimator;
   



    #endregion

    #region Private Data
    public static ThirdPersonMovementScript _instance;
    private float _turnSmoothVelocity;
    Vector3 moveDirection;
    public static int numOfClicks = 0;
    //Time the button was last clicked
    float lastClick = 0.0f;
    float lastAttack = 0.0f;
    //Delay between each click
    float combatDelay = 1.0f;

    #endregion

    #region Functions

    private void Start()
    {
              // Start with no control. Dungeon will enable when complete
        enabled = false;
        _instance = this;
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
            _playerAnimator.SetBool("isIdle", false);
            if (!_footstepSound.isPlaying)
            {
                _footstepSound.Play();
            }

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
            if (Input.GetKey(KeyCode.LeftShift) && !_playerAnimator.GetBool("isDrawn"))
            {
                if (_moveSpeed < _runSpeed)
                { 
                    _moveSpeed +=10.0f*Time.deltaTime;
                }
                _playerAnimator.SetBool("isRunning",true);
                _playerAnimator.SetBool("isRunningArcher", true);

            }
            else 
            {
                _moveSpeed = _Speed;
                _playerAnimator.SetBool("isRunning", false);
                _playerAnimator.SetBool("isRunningArcher", false);


            }


            _characterController.Move(moveDirection.normalized * _moveSpeed * Time.deltaTime);
           


        }
        else
        {
            moveDirection.x = 0.0f;
            moveDirection.z = 0.0f;
            _playerAnimator.SetBool("isIdle", true);

            if (_footstepSound.isPlaying)
                _footstepSound.Stop();
        }




        /** 
         **  Animating
         **/
        float velocityZ = Vector3.Dot(moveDirection.normalized, transform.forward);
        float velocityX = Vector3.Dot(moveDirection.normalized, transform.right);
        //Movement
        _playerAnimator.SetFloat("VelocityZ", velocityZ, 0.2f, Time.deltaTime);
        _playerAnimator.SetFloat("VelocityX", velocityX, 0.2f, Time.deltaTime);
        //Attacking
        lastClick -=Time.deltaTime;
        lastAttack += Time.deltaTime;
        if (numOfClicks < 0) SetNumOfClicks();
        if (Input.GetMouseButtonDown(0))
        {
            _playerAnimator.SetBool("isAttacking", true);
            OnClick();
            _playerAnimator.SetBool("isRunningArcher", false);


        }
        else if (Input.GetMouseButtonUp(0))
        {
            _playerAnimator.SetBool("isAttacking", false);
            // _playerAnimator.SetBool("isRunningArcher", true);
            lastAttack = 0.0f;
        }
        else if (lastClick <= 0.0f) ResetNumOfClicks();
        else if (lastAttack > 0.2f)
        {
            _playerAnimator.SetBool("Attack1", false);
            _playerAnimator.SetBool("Attack2", false);
            _playerAnimator.SetBool("Attack3", false);
            ResetNumOfClicks();
            _playerAnimator.SetInteger("numberOfClicks", numOfClicks);


        }
        /**Special animation for the Mage Sap attack:
         * Conditions to make it work:
         * Get a message to know which attack the mage is using
         * Maybe allow Sap to be used only while the player is not moving 
         * To compensate we could increase Sap's damage
        **/
        if (Input.GetMouseButtonDown(1) && _playerAnimator.GetBool("isIdle"))
        {
            _playerAnimator.SetBool("isSpecialAttack", true);
        }

        if (Input.GetMouseButtonDown(1) && !_playerAnimator.GetBool("isRunning"))
        {
            _playerAnimator.SetBool("isBlocking", true);

        }
        else if (Input.GetMouseButtonUp(1))
        {
            _playerAnimator.SetBool("isSpecialAttack", false);
            _playerAnimator.SetBool("isBlocking", false);

        }
        if (_playerAnimator.GetBool("isRunning"))
        {
            _playerAnimator.SetBool("isSpecialAttack", false);
            _playerAnimator.SetBool("isBlocking", false);
        }
    }
    void OnClick()
    {
        _playerAnimator.SetBool("isBlocking", false);
        //Record time of last button click
        lastClick = combatDelay;
        numOfClicks++;
        _playerAnimator.SetInteger("numberOfClicks", numOfClicks);
        if (numOfClicks >= 1) _playerAnimator.SetBool("AttackOne", true);
        else if (numOfClicks < 2) _playerAnimator.SetBool("AttackTwo", false);
        //Limit the number of combos by clamping
        numOfClicks = Mathf.Clamp(numOfClicks, 0, 3);
       
    }

    public int GetNumOfClicks()
    { return numOfClicks; }
    public void SetNumOfClicks()
    {

        _playerAnimator.SetInteger("numberOfClicks", numOfClicks);
    }
    public static void ResetNumOfClicks() 
    { 
        numOfClicks = 0;
    }
    #endregion
}
