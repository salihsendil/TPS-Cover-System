using UnityEngine;

public class IdleState : IAnimState
{
    public void EnterState(PlayerAnimController player)
    {

    }

    public void ExitState(PlayerAnimController player)
    {

    }

    public void UpdateState(PlayerAnimController player)
    {
        if (player.IsWalking)
        {
            player.SwitchState(new WalkingState(), player.IsWalkingHash, true);
        }
        else if (player.IsCrouchCovering)
        {
            player.SwitchState(new CoverCrouchingState(), player.IsCrouchCoveringHash, true);
        }

    }
}
