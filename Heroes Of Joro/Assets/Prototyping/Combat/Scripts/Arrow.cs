using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Rigidbody _rigidBody;

    [SerializeField] private float _speed;
    #endregion

    private float _damage;

    public void Fire(float damage)
    {
        _damage = damage;

        _rigidBody.velocity = transform.forward * _speed;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
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
}
