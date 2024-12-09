using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("References")]
    private PlayerInput _playerInput;

    [Header("Player Movement Variables")]
    private Vector2 _playerMovementInput;
    private Vector3 _playerMovementVector;

    [Header("Getters and Setters")]
    public Vector3 PlayerMovementVector { get => _playerMovementVector; }

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

        _playerInput = new PlayerInput();
        OnInputEnable();
    }

    void Start()
    {
        _playerInput.Movement.Move.started += GetMovementInputVector;
        _playerInput.Movement.Move.performed += GetMovementInputVector;
        _playerInput.Movement.Move.canceled += GetMovementInputVector;
        _playerInput.Interactions.Cover.performed += GetCoverKeyPress;
    }


    void Update()
    {

    }

    void GetMovementInputVector(InputAction.CallbackContext callback)
    {
        _playerMovementInput = callback.ReadValue<Vector2>();
        _playerMovementVector = new Vector3(_playerMovementInput.x, 0f, _playerMovementInput.y);
    }

    void GetCoverKeyPress(InputAction.CallbackContext callback)
    {
        PlayerCharacterController.Instance.HandleCrouchingCoverRay();
        if (!PlayerCharacterController.Instance.CanTakeCrouchCover)
        {
            PlayerCharacterController.Instance.HandleStandingCoverRay();
        }
    }

    void OnInputEnable()
    {
        _playerInput.Enable();
    }

    void OnInputDisable()
    {
        _playerInput.Disable();
        _playerInput.Movement.Move.started -= GetMovementInputVector;
        _playerInput.Movement.Move.performed -= GetMovementInputVector;
        _playerInput.Movement.Move.canceled -= GetMovementInputVector;
        _playerInput.Interactions.Cover.performed -= GetCoverKeyPress;
    }

}
