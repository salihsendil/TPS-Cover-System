using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    private IAnimState _currentState;
    private Animator _animator;

    private int _isWalkingHash;
    private int _isStandCoveringHash;
    private int _isCrouchCoveringHash;

    //Getters - Setters
    public int IsWalkingHash { get => _isWalkingHash; set => _isWalkingHash = value; }
    public int IsStandCoveringHash { get => _isStandCoveringHash; set => _isStandCoveringHash = value; }
    public int IsCrouchCoveringHash { get => _isCrouchCoveringHash; set => _isCrouchCoveringHash = value; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isStandCoveringHash = Animator.StringToHash("isStandCovering");
        _isCrouchCoveringHash = Animator.StringToHash("isCrouchCovering");

    }
    void Start()
    {
        _currentState = new IdleState();
    }

    void Update()
    {
        
    }

    public void ChangeAnimState(IAnimState state)
    {
        _currentState.ExitState(this);
        _currentState = state;
        _currentState.EnterState(this);
    }

}
