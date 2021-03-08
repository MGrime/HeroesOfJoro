using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PickupBase
{
    #region Editor Fields

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _sphereCollider;

    #endregion

    #region Functions
    private void Start()
    {
        // Set a random spin
        Vector3 spin = new Vector3(Random.Range(80.0f, 120.0f), Random.Range(80.0f, 120.0f),
            Random.Range(80.0f, 120.0f));

        _rigidbody.AddTorque(spin);

        // Fire upwards
        _rigidbody.AddForce(new Vector3(0.0f,2.0f),ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Send message
            other.gameObject.SendMessage("PickupCoin");

            Destroy(this.gameObject);
        }
    }
    #endregion
}
