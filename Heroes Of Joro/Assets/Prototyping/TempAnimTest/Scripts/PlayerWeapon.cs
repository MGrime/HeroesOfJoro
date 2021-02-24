using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] BlasterShot _shot;
    [SerializeField] Transform _firePoint;
    [SerializeField] float _delay = 1f;
    [SerializeField] LayerMask _aimLayerMask;

    float _nextFireTime;
    private Transform _sphere;

    void Update()
    {
        AimTowardMouse();

        if (ReadyToFire())
            Fire();
    }

    bool ReadyToFire() => Time.time >= _nextFireTime;

    void Fire()
    {
        _nextFireTime = Time.time + _delay;
        var shot = Instantiate(_shot, _firePoint.position, Quaternion.Euler(transform.forward));
        shot.Launch(transform.forward);
    }

    private void Awake()
    {
        _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        _sphere.GetComponent<Collider>().enabled = false;
        _sphere.gameObject.SetActive(false);
    }

    void AimTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _aimLayerMask))
        {
            var destination = hitInfo.point;
            destination.y = transform.position.y;

            var _direction = destination - transform.position;
            _direction.y = 0f;
            _direction.Normalize();
            Debug.DrawRay(transform.position, _direction, Color.green);
            transform.rotation = Quaternion.LookRotation(_direction, transform.up);

            _sphere.position = destination;
        }
    }
}