using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCharacterController : MonoBehaviour
{
    public static PlayerCharacterController Instance { get; private set; }

    [Header("Test Variables")]
    RaycastHit hitStand;
    RaycastHit hitCrouch;

    [Header("References")]
    Rigidbody _rb;
    CapsuleCollider _collider;
    PlayerAnimController _playerAnimController;

    [Header("Movement Variables")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _acceleration = 3.4f;
    private Vector3 _movementVector;

    [Header("Rotation Variables")]
    private Vector3 _cameraDirection;
    [SerializeField] private float _rotatingSpeed = 5f;

    [Header("Cover System Variables")]
    private float _rayLength = 1.5f;
    private float _coverRotationTolerance = 7.5f;

    private Vector3 _coverRotationEulerAngles;
    private Vector3 _coverTargetPosition;


    private float _verticalStandingCoverOffsetY = 1.5f;
    private Vector3 _verticalStandingCoverOffsetVector;

    private float _verticalCrouchingCoverOffsetY = 1f;
    private Vector3 _verticalCrouchingCoverOffsetVector;

    private bool _canTakeCrouchCover;
    private bool _canTakeStandingCover;
    private bool _isInCover;
    private bool _canGetOutFromCover;

    [Header("Getters and Setters")]
    public bool CanTakeCrouchCover { get => _canTakeCrouchCover; set => _canTakeCrouchCover = value; }
    public bool CanTakeStandingCover { get => _canTakeStandingCover; set => _canTakeStandingCover = value; }

    private void Awake()
    {
        #region SingletonPattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _playerAnimController = GetComponent<PlayerAnimController>();

        _verticalStandingCoverOffsetVector = new Vector3(0f, _verticalStandingCoverOffsetY, 0f);
        _verticalCrouchingCoverOffsetVector = new Vector3(0f, _verticalCrouchingCoverOffsetY, 0f);
    }

    private void FixedUpdate()
    {
        if (InputManager.Instance.PlayerMovementVector != Vector3.zero)
        {
            HandleRotation();
        }
        HandleMovement();


        if (_canTakeStandingCover)
        {
            TakePlayerToCoverPos(hitStand);
            _playerAnimController.IsStandCovering = true;
        }

        else if (_canTakeCrouchCover)
        {
            TakePlayerToCoverPos(hitCrouch);
            _playerAnimController.IsCrouchCovering = true;
        }

        if (!_isInCover && _canGetOutFromCover)
        {
            //GettingOutFromCover();
        }

    }



    private void HandleMovement()
    {
        Vector3 _mainCamFwd = Camera.main.transform.forward;
        Vector3 _mainCamRight = Camera.main.transform.right;

        _mainCamFwd.y = _mainCamRight.y = 0f;

        _mainCamFwd.Normalize();
        _mainCamRight.Normalize();

        _movementVector = InputManager.Instance.PlayerMovementVector.x * _mainCamRight + InputManager.Instance.PlayerMovementVector.z * _mainCamFwd;

        Debug.Log($"Movement Vector before multiple: {_movementVector}");
        Debug.Log($"ad pressed: {InputManager.Instance.PlayerMovementVector.x}");

        _movementVector *= _speed;

        if (_rb.linearVelocity.magnitude < _movementVector.magnitude)
        {
            _rb.linearVelocity += _movementVector * _acceleration * Time.deltaTime;
        }

        Debug.Log($"Velocity: {_rb.linearVelocity}");

        _playerAnimController.IsWalking = _rb.linearVelocity != Vector3.zero ? true : false;
    }

    private void HandleRotation()
    {
        _cameraDirection = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
        _cameraDirection.Normalize();

        Quaternion _targetRot = Quaternion.LookRotation(_cameraDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRot, 1f);
    }



    public void CheckPlayerCoverSituation()
    {
        HandleStandingCoverRay();

        if (!_canTakeStandingCover)
        {
            HandleCrouchingCoverRay();
        }

        if (_isInCover)
        {
            _canGetOutFromCover = true;
        }
    }

    public void HandleStandingCoverRay()
    {
        Ray ray = new Ray(transform.position + _verticalStandingCoverOffsetVector, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 5f);
        if (Physics.Raycast(ray, out hitStand, _rayLength))
        {
            if (hitStand.collider == null) { return; }
            _canTakeStandingCover = true;
        }
    }

    public void HandleCrouchingCoverRay()
    {
        Ray ray = new Ray(transform.position + _verticalCrouchingCoverOffsetVector, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction, Color.blue, 5f);
        if (Physics.Raycast(ray, out hitCrouch, _rayLength))
        {
            if (hitCrouch.collider == null) { return; }
            _canTakeCrouchCover = true;
        }
    }

    private void TakePlayerToCoverPos(RaycastHit hit)
    {
        _coverTargetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z - _collider.radius / 2 - 0.2f);
        ApplyRotationToCoverState(CalculateRotationForCovering(hit.normal));
        transform.position = Vector3.Slerp(transform.position, _coverTargetPosition, Time.deltaTime * 3f);

        _isInCover = true;
    }

    private Quaternion CalculateRotationForCovering(Vector3 normal)
    {
        return Quaternion.LookRotation(normal);
    }

    private void ApplyRotationToCoverState(Quaternion targetRotation)
    {
        _coverRotationEulerAngles = targetRotation.eulerAngles;
        _coverRotationEulerAngles.y -= _coverRotationTolerance;
        _playerAnimController.UpdateAnimationMirrorState(_coverRotationEulerAngles.y);
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              Quaternion.Euler(_coverRotationEulerAngles),
                                              Time.deltaTime * 3f);
    }

    void GettingOutFromCover()
    {
        //this gonna fix

        //Debug.Log(transform.rotation.eulerAngles);


        Vector3 test = transform.rotation.eulerAngles + new Vector3(0, 180, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(test), Time.deltaTime * 3f);

        transform.position = Vector3.Slerp(transform.position, transform.position + Vector3.back * 2, Time.deltaTime * 3f);

        _isInCover = false;
        _canTakeStandingCover = _canTakeCrouchCover = false;
        _playerAnimController.IsStandCovering = _playerAnimController.IsCrouchCovering = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;//Stand Cover Pos Ray Start Point
        Gizmos.DrawWireSphere(transform.position + _verticalStandingCoverOffsetVector, 0.15f);

        Gizmos.color = Color.blue;//Crouch Cover Pos Ray Start Point
        Gizmos.DrawWireSphere(transform.position + _verticalCrouchingCoverOffsetVector, 0.15f);

        Gizmos.color = Color.red;//Stand Cover Ray Hit Point
        Gizmos.DrawWireCube(hitStand.point, Vector3.one * 0.1f);

        Gizmos.color = Color.blue;//Stand Cover Ray Hit Point
        Gizmos.DrawWireCube(hitCrouch.point, Vector3.one * 0.1f);

    }

}
