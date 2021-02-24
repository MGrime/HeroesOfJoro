using UnityEngine;

public class BlasterShot : MonoBehaviour
{
    [SerializeField] float _velocity = 15f;

    public void Launch(Vector3 direction)
    {
        direction.Normalize();
        transform.forward = direction;
        GetComponent<Rigidbody>().velocity = direction * _velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}