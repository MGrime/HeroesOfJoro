using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAttack : MonoBehaviour
{
    //Projectile object
    [SerializeField] private GameObject _enemyProjectile = null;


    //Shoot a projectile towards the player
    void CreateProjectile()
    {
        Instantiate(_enemyProjectile, transform.position + new Vector3(0, 2.0f, 0.0f), Quaternion.identity);
    }
}
