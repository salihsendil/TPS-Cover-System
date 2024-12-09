using UnityEngine;

public class CoverCrouchingState : IAnimState
{
    public void EnterState(PlayerAnimController player)
    {
        player.IsCrouchCovering = true;
        player.Animator.SetBool(player.IsCrouchCoveringHash, true);
    }

    public void ExitState(PlayerAnimController player)
    {
        player.IsCrouchCovering = false;
        player.Animator.SetBool(player.IsCrouchCoveringHash, false);
    }

    public void UpdateState(PlayerAnimController player)
    {
        
    }
}
