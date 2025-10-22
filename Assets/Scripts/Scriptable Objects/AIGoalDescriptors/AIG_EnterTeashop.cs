using UnityEngine;

[CreateAssetMenu(fileName = "AIGoalEnterTeashopDescriptor", menuName = "AI Data/Goal/EnterTeashopDescriptor")]
public class AIG_EnterTeashop : AIG_BaseCell
{
    public override CustomerGoal GetCustomerGoal() { return CustomerGoal.EnterTeashop; }

    public override bool IsGoalCompleted(CustomerState aState, NPCManager aNPCManager)
    {
        return aState.IsInsideStore && !aState.IsWalking;
    }

    public override bool IsStartingStateValid(CustomerState aState, NPCManager aNPCManager)
    {
        return !aState.IsInsideStore && !aState.HasFoundDesiredItems;
    }

    public override void ProcessNPCGoal(NPCController aController, NPCManager aNPCManager)
    {
        aController.SetNewTargetPosition(aNPCManager.GetArrivalPosition());
    }
}
