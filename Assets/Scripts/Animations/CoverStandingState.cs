using UnityEngine;

public class CoverStandingState : IAnimState
{
    public void EnterState(PlayerAnimController player)
    {

    }

    public void ExitState(PlayerAnimController player)
    {

    }

    public void UpdateState(PlayerAnimController player)
    {
        if (!player.IsStandCovering)
        {
            player.SwitchState(new IdleState(), player.IsStandCoveringHash, false);
        }
    }
}
