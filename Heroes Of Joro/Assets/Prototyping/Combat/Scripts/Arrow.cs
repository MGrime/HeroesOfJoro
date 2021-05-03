using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple class to control the arrow as it moves
public class Arrow : MonoBehaviour
{
    #region EDITOR FIELDS

    [SerializeField] private Rigidbody _rigidBody = null;

    [SerializeField] private float _speed = 0.0f;

    #endregion

    #region PRIVATE DATA

    // Damage is set from the firing bow to allow reuse of the basic arrow
    private float _damage;

    #endregion

    #region FUNCTIONS

    // Called from bow function upon creation to initialise damage from the bow and set arrow to fly forward
    public void Fire(float damage)
    {
        _damage = damage;

        _rigidBody.velocity = transform.forward * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" && other.tag != "Projectile")
        {
            if (other.name == "EnemyControls")
            {
                other.gameObject.SendMessage("SetDamage", _damage);
                other.gameObject.SendMessage("ReduceHealth");
            }
            if (other.name == "Face(Clone)")
            {
                other.gameObject.SendMessage("ReduceProjectileHealth", _damage);
            }

            Destroy(this.gameObject);
        }

    }

    #endregion

}
