using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Variable de character controller
    private CharacterController _controller;

    //Variables de InputAction
    private InputAction _moveAction;
    private InputAction _jumpAction;

    //Variable de la dirección
    private Vector2 _moveInputValue;

    //Variables de velocidad y fuerza de salto
    private float _movementSpeed = 10;
    private float _jumpForce = 5;

    //Variable de Grund Sensor
    [SerializeField]private Transform _groundPosition;
    [SerializeField]private float _sensorRadius;
    [SerializeField]private LayerMask _groundLayer;

    //Variable de gravedad
    [SerializeField]private float _gravity = -9.81f;
    private Vector3 _playerGravity;

    //Variable para la rotación
    private float _turnSmoothVelocity;
    private float _smoothTime = 0.5f;

    //Variable de cámara
    public Transform _cameraTransform;

    //Variable de animación
    private Animator _animator;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];

        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        _moveInputValue = _moveAction.ReadValue<Vector2>();

        Movement();

        Gravity();

        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        _animator.SetBool("IsGrounded", IsGrounded());
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_moveInputValue.x, 0, _moveInputValue.y);

        float TargetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
        float SmoothDampAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _cameraTransform.eulerAngles.y, ref _turnSmoothVelocity, _smoothTime);

        transform.rotation = Quaternion.Euler(0, SmoothDampAngle, 0);

        if(direction != Vector3.zero)
        {
            Vector3 _moveDirection = Quaternion.Euler(0, TargetAngle, 0) * Vector3.forward;
            _controller.Move(_moveDirection * _movementSpeed * Time.deltaTime);
        }
        
        _animator.SetFloat("Horizontal", _moveInputValue.x, 0.2f, Time.deltaTime);
        _animator.SetFloat("Vertical", _moveInputValue.y, 0.2f, Time.deltaTime);
    }

    void Gravity()
    {
        if(!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }

        if(!IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = _gravity;
        }

        _controller.Move(_playerGravity * Time.deltaTime);
    }

    void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_jumpForce * -2 * _gravity);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_groundPosition.position, _sensorRadius, _groundLayer);
    }
}
