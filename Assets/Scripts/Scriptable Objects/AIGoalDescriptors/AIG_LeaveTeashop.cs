using UnityEngine;

[CreateAssetMenu(fileName = "new AIG_LeaveTeashop", menuName = "AI Data/Goal/Leave Teashop")]
public class AIG_LeaveTeashop : AIG_BaseCell
{
    public override CustomerGoal GetCustomerGoal() { return CustomerGoal.LeaveStore; }

    public override bool IsGoalCompleted(CustomerState aState, NPCManager aNPCManager)
    {
        return !aState.IsInsideStore && !aState.IsWalking;
    }

    public override bool IsStartingStateValid(CustomerState aState, NPCManager aNPCManager)
    {
        return aState.IsInsideStore;
    }

    public override void PostGoalCompletion(NPCController aController)
    {
        aController.myCurrentState.IsReadyToLeave = true;
    }

    public override void ProcessNPCGoal(NPCController aController, NPCManager aNPCManager)
    {
        aController.GracefullyLeaveStore();
    }
}