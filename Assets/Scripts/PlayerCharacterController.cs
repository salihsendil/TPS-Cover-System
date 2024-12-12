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
    private Vector3 _movementVector;
    [SerializeField] private float _speed = 1.8f;

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

    }

    private void HandleMovement()
    {
        _movementVector = InputManager.Instance.PlayerMovementVector * _speed;
        _rb.linearVelocity = new Vector3(_movementVector.x, _rb.linearVelocity.y, _movementVector.z);

        _playerAnimController.IsWalking = _rb.linearVelocity != Vector3.zero ? true : false;
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
            GettingOutFromCover(); //IT'S GONNA FIX
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
        transform.position = Vector3.Slerp(transform.position, _coverTargetPosition, Time.deltaTime * 3f);
        ApplyRotationToCoverState(CalculateRotationForCovering(hit.normal));

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
        transform.position = Vector3.Slerp(transform.position, transform.position + transform.forward*3f, Time.deltaTime * 3f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * 3f);
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
