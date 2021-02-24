using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [SerializeField] float _speed = 5f;
    [SerializeField] LayerMask _aimLayerMask;
    [SerializeField] private Animator _animator;
    void Start()
    {
        //_animator = GetComponentInChildren<Animator>();

    }
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        AimTowardMouse();
        // Reading the Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        // Moving
        if (movement.magnitude > 0)
        {
            movement.Normalize();
            movement *= _speed * Time.deltaTime;
            transform.Translate(movement, Space.World);
        }

      
    }

    void AimTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _aimLayerMask))
        {
            var _direction = hitInfo.point - transform.position;
            _direction.y = 0.0f;
            _direction.Normalize();
            transform.forward = _direction;
        }
    }
}