using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIG_PayForItems", menuName = "AI Data/Goal/Pay For Items")]
public class AIG_PayForItems : AIG_BaseCell
{
    private NPCCashRegisterHelper cashRegisteryHelper;

    public override CustomerGoal GetCustomerGoal() { return CustomerGoal.GoToCashRegister; }
    public override bool IsGoalCompleted(CustomerState aState, NPCManager aNPCManager)
    {
        return aState.IsTransactionPayed;
    }

    public override bool IsStartingStateValid(CustomerState aState, NPCManager aNPCManager)
    {
        return !aState.IsTransactionPayed && aState.HasFoundDesiredItems;
    }

    public override void PostGoalCompletion(NPCController aController)
    {
        cashRegisteryHelper.GetPayingStation().WaitingNPC = null;
        aController.DestinationReached.RemoveListener(OnDestinationReached);
    }

    public override void ProcessNPCGoal(NPCController aController, NPCManager aNPCManager)
    {
        cashRegisteryHelper = aNPCManager.GetCashRegisteryHelper();
        aController.SetNewTargetPosition(cashRegisteryHelper.transform.position);
        aController.DestinationReached.AddListener(OnDestinationReached);
    }

    private void OnDestinationReached(NPCController aController)
    {
        cashRegisteryHelper.GetPayingStation().WaitingNPC = aController;
    }
}
