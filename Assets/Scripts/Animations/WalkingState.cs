using UnityEngine;

public class WalkingState : IAnimState
{
    public void EnterState(PlayerAnimController player)
    {

    }

    public void ExitState(PlayerAnimController player)
    {
        player.IsWalking = false;
    }

    public void UpdateState(PlayerAnimController player)
    {
        if (!player.IsWalking)
        {
            player.SwitchState(new IdleState(), player.IsWalkingHash, false);
        }
    }
}
