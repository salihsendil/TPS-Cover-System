using UnityEngine;

public class CoverCrouchingState : IAnimState
{
    public void EnterState(PlayerAnimController player)
    {
        player.GetComponent<CapsuleCollider>().center = new Vector3(0f, 0.7f, 0f);
        player.GetComponent<CapsuleCollider>().height = 1.4f;
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
