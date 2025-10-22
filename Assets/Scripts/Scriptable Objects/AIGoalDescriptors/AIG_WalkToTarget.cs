using UnityEngine;

[CreateAssetMenu(fileName = "new AIG_WalkToTarget", menuName = "AI Data/Goal/Walk To Target")]
public class AIG_WalkToTarget : AIG_BaseCell
{
	[SerializeField] bool isPedestrian = false;

	public override CustomerGoal GetCustomerGoal() { return CustomerGoal.WalkDistance; }

	public override bool IsGoalCompleted(CustomerState aState, NPCManager aNPCManager)
	{
		return !aState.IsWalking;
	}

	public override bool IsStartingStateValid(CustomerState aState, NPCManager aNPCManager)
	{
		return true;
	}

	public override void PostGoalCompletion(NPCController aController)
	{
		if (isPedestrian)
		{
			aController.myCurrentState.IsReadyToLeave = true;
		}
	}

	public override void ProcessNPCGoal(NPCController aController, NPCManager aNPCManager)
	{
		if (isPedestrian)
		{
			aController.SetNewTargetPosition(aNPCManager.GetFurthestSpawnPosition(aController.gameObject.transform.position));
		}
	}
}