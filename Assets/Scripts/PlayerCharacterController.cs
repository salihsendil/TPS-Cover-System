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
    PlayerAnimController _playerAnimController;

    [Header("Movement Variables")]
    private Vector3 _movementVector;
    [SerializeField] private float _speed = 1.8f;

    [Header("Cover System Variables")]
    private float _rayLength = 2.5f;

    private float _verticalStandingCoverOffsetY = 1.5f;
    private Vector3 _verticalStandingCoverOffsetVector;

    private float _verticalCrouchingCoverOffsetY = 1f;
    private Vector3 _verticalCrouchingCoverOffsetVector;

    private bool _canTakeCrouchCover;
    private bool _canTakeStandingCover;

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
        _playerAnimController = GetComponent<PlayerAnimController>();

        _verticalStandingCoverOffsetVector = new Vector3(0f, _verticalStandingCoverOffsetY, 0f);
        _verticalCrouchingCoverOffsetVector = new Vector3(0f, _verticalCrouchingCoverOffsetY, 0f);
    }

    void Update()
    {
        if (_playerAnimController.IsCrouchCovering)
        {
            ////Vector3 _surfaceSlope = Quaternion.Euler(hitCrouch.normal).eulerAngles;

            //Vector3 surfaceNormal = hitCrouch.normal;

            //// Karakterin s�rt�n� normale hizalamak i�in hedef d�n��� hesapla
            //Quaternion targetRotation = Quaternion.LookRotation(-surfaceNormal);

            //// Y ekseninde 180 derece ekle
            //targetRotation *= Quaternion.Euler(0, 180, 0);

            //// Karakteri yava��a hedef d�n��e �evir
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
            ////transform.position = Vector3.Slerp(transform.position, hitCrouch.point, Time.deltaTime);
            //Quaternion.LookRotation(hitCrouch.point);

            //transform.rotation = Quaternion.Slerp(transform.rotation, _surfaceSlope, Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        //HandleGroundCheck();
    }

    private void HandleMovement()
    {
        _movementVector = InputManager.Instance.PlayerMovementVector * _speed;
        _rb.linearVelocity = new Vector3(_movementVector.x, _rb.linearVelocity.y, _movementVector.z);

        _playerAnimController.IsWalking = _rb.linearVelocity != Vector3.zero ? true : false;
    }

    public void HandleCrouchingCoverRay()
    {
        Ray ray = new Ray(transform.position + _verticalCrouchingCoverOffsetVector, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction, Color.blue);
        if (Physics.Raycast(ray, out hitCrouch, _rayLength))
        {
            if (hitCrouch.collider != null)
            {
                Vector3 surfaceNormal = hitCrouch.normal;

                // Normali bir Quaternion d�n���ne �evir
                Quaternion normalRotation = Quaternion.LookRotation(surfaceNormal);

                // Euler a��lar�n� al
                Vector3 eulerAngles = normalRotation.eulerAngles;

                Debug.Log($"Normale g�re Euler a��lar�: {eulerAngles}");

                float _characterAnimTurn = 145f;

                if (eulerAngles.y > 180)
                {
                    float a = 360 - eulerAngles.y;
                    float b = 180 - a;
                    float c = b - _characterAnimTurn;
                    _playerAnimController.Animator.SetBool(_playerAnimController.IsMirroredHash, true);
                    Debug.Log("mirror var, if condition, d�nmesi gereken: " + c);
                }
                else
                {
                    float a = 180 - eulerAngles.y;
                    float b = _characterAnimTurn - a;
                    _playerAnimController.Animator.SetBool(_playerAnimController.IsMirroredHash, false);
                    Debug.Log("mirror yok, else condition, d�nmesi gereken: " + b);
                }

                _canTakeCrouchCover = true;
                _playerAnimController.IsCrouchCovering = true;
            }
        }
    }

    public void HandleStandingCoverRay()
    {
        Ray ray = new Ray(transform.position + _verticalStandingCoverOffsetVector, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (Physics.Raycast(ray, out hitStand, _rayLength))
        {
            if (hitStand.collider != null)
            {
                Debug.Log("Standing Cover Object Found: " + hitStand.collider.name);
                _canTakeStandingCover = true;
                _playerAnimController.IsStandCovering = true;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward + _verticalStandingCoverOffsetVector, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _verticalStandingCoverOffsetVector, 0.1f);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitStand.point, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + _verticalCrouchingCoverOffsetVector, 0.1f);
        Gizmos.DrawWireSphere(transform.position + transform.forward + _verticalCrouchingCoverOffsetVector, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(hitCrouch.point, 0.1f);

    }

    //private void HandleGroundCheck()
    //{
    //    Ray ray = new Ray(transform.position, Vector3.down);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, 5f))
    //    {
    //        //Debug.Log("hit: " + hit.collider.name);
    //    }
    //    Debug.DrawRay(ray.origin, ray.direction, Color.red);
    //}

}
