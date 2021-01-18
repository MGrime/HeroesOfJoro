using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBase : MonoBehaviour
{
    #region Editor Fields
    // Want this accessible by warrior
    public bool _swinging;

    [SerializeField] private float _attackDamage;
    [SerializeField] private float _swingSpeed; // Degrees per frame

    #endregion

    #region Private Data

    // So the way unity stores the data
    // It goes from 45 -> 0 jumps to 360 -> 315
    // Other way goes 315 -> 360 jumps to 0 -> 45
    private bool _directionNegative;
    private float _previousAngle = 0.0f;    // Store previous angle to find flip
    private bool _flipped;

    #endregion

    #region Functions

    void Start()
    {
        _swinging = false;
        _directionNegative = true;
        _flipped = false;
        _previousAngle = transform.parent.localRotation.eulerAngles.y;
    }    
    // Update is called once per frame
    void Update()
    {
        if (_swinging)
        {
            float rotateAmount = _swingSpeed * Time.deltaTime;
            if (_directionNegative) // 45 - > -45
            {
                rotateAmount *= -1.0f;  // Invert
            }

            _previousAngle = transform.parent.localRotation.eulerAngles.y;
            // Rotate
            transform.parent.Rotate(0.0f, rotateAmount, 0.0f);

            // This is a cheap bodge catch all for when a big change happens
            if (Mathf.Abs(transform.parent.localRotation.eulerAngles.y - _previousAngle) > 180.0f)
            {
                _flipped = true;
            }
            
            // Check if we hit swing time
            // The numbers look funny because unity's interal holding of this number is scuffed
            bool finished = false;
            if (_directionNegative)
            {
                if (transform.parent.localRotation.eulerAngles.y <= 315.0f && _flipped)
                {
                    finished = true;
                }
            }
            else
            {
                if (transform.parent.localRotation.eulerAngles.y >= 45.0f && _flipped)
                {
                    finished = true;
                }
            }

            if (finished)
            {
                _swinging = false;
                _directionNegative = !_directionNegative;
                _flipped = false;
            }
        }
    }

    // Message functions
    public void OnTriggerEnter(Collider other)
    {
        if (_swinging)
        {
            if (other.tag != "Player" && other.tag != "Spell")
            {
                if (other.name == "EnemyControls")
                {
                    other.gameObject.SendMessage("SetDamage", _attackDamage);
                    other.gameObject.SendMessage("ReduceHealth");
                }
                if (other.name == "Face(Clone)")
                {
                    other.gameObject.SendMessage("ReduceProjectileHealth", _attackDamage);
                }
            }
        }
        
    }

    #endregion

}
