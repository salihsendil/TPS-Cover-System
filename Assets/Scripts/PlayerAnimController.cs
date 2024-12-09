using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    [Header("References")]
    private IAnimState _currentState;
    private Animator _animator;

    [Header("Animator Hash Variables")]
    private int _isWalkingHash;
    private int _isStandCoveringHash;
    private int _isCrouchCoveringHash;
    private int _isMirroredHash;

    [Header("Player Current State Variables")]
    private bool _isWalking;
    private bool _isStandCovering;
    private bool _isCrouchCovering;
    private bool _isMirrored;


    [Header("Getter and Setters")]
    //Getters - Setters
    public Animator Animator { get => _animator; set => _animator = value; }

    public int IsWalkingHash { get => _isWalkingHash; set => _isWalkingHash = value; }
    public int IsStandCoveringHash { get => _isStandCoveringHash; set => _isStandCoveringHash = value; }
    public int IsCrouchCoveringHash { get => _isCrouchCoveringHash; set => _isCrouchCoveringHash = value; }
    public int IsMirroredHash { get => _isMirroredHash; set => _isMirroredHash = value; }

    public bool IsWalking { get => _isWalking; set => _isWalking = value; }
    public bool IsStandCovering { get => _isStandCovering; set => _isStandCovering = value; }
    public bool IsCrouchCovering { get => _isCrouchCovering; set => _isCrouchCovering = value; }
    public bool IsMirrored { get => _isMirrored; set => _isMirrored = value; }

    public IAnimState CurrentState { get => _currentState; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isStandCoveringHash = Animator.StringToHash("isStandCovering");
        _isCrouchCoveringHash = Animator.StringToHash("isCrouchCovering");
        _isMirroredHash = Animator.StringToHash("isMirrored");
    }
    void Start()
    {
        _currentState = new IdleState();
        _currentState.EnterState(this);
    }

    void Update()
    {
        _currentState.UpdateState(this);
    }

    public void SwitchState(IAnimState state, int stateHash, bool stateSituation)
    {
        _currentState.ExitState(this);
        _currentState = state;
        _animator.SetBool(stateHash, stateSituation);
        _currentState.EnterState(this);
    }

    public void UpdateAnimationMirrorState(float angle)
    {
        _isMirrored = angle > 180;
        _animator.SetBool(IsMirroredHash, IsMirrored);
    }

}
