using UnityEngine;

public interface IAnimState
{
    void EnterState(PlayerAnimController player);
    void UpdateState(PlayerAnimController player);
    void ExitState(PlayerAnimController player);

}
