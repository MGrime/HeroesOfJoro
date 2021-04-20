using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : PickupBase
{
    #region Editor Fields

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _sphereCollider;

    #endregion

    #region Functions
    private void Start()
    {
        // Set a spin force
        _rigidbody.AddTorque(new Vector3(0.0f, 100.0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerMage")
        {
            // Send message
            other.gameObject.SendMessage("PickupMana");

            Destroy(this.gameObject);
        }
    }

    #endregion
}
