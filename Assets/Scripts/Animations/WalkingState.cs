using UnityEngine;

public class WalkingState : IAnimState
{
    public void EnterState(PlayerAnimController player)
    {

    }

    public void ExitState(PlayerAnimController player)
    {

    }

    public void UpdateState(PlayerAnimController player)
    {
        if (!player.IsWalking)
        {
            player.SwitchState(new IdleState(), player.IsWalkingHash, false);
        }

        if (player.IsStandCovering)
        {
            player.SwitchState(new CoverStandingState(), player.IsStandCoveringHash, true);
        }

        if (player.IsCrouchCovering)
        {
            player.SwitchState(new CoverCrouchingState(), player.IsCrouchCoveringHash, true);
        }
    }
}
