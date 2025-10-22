using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(fileName = "AIG_LookForItems", menuName = "AI Data/Goal/LookForItems")]
public class AIG_LookForItems : AIG_BaseCell
{
    bool hasGoalStarted = false;
    private List<NPCTabletteLookerHelper> myTabletHelpers = null;
    private NPCTabletteLookerHelper myNextTabletteLooker;

    public override CustomerGoal GetCustomerGoal() { return CustomerGoal.LookForDesiredItems; }
    public override bool IsGoalCompleted(CustomerState aState, NPCManager aNPCManager)
    {
        return aState.HasFoundDesiredItems || aState.IsNotAbleToFindItems;
    }

    public override bool IsStartingStateValid(CustomerState aState, NPCManager aNPCManager)
    {
        return !aState.HasFoundDesiredItems && aState.IsInsideStore;
    }

    public override void PostGoalCompletion(NPCController aController)
    {
        aController.GoalUpdate.AddListener(OnGoalUpdate);
    }

    public override void ProcessNPCGoal(NPCController aController, NPCManager aNPCManager)
    {
        if (!hasGoalStarted)
        {
            aNPCManager.GetTabletLookerHelpers(out myTabletHelpers);
            hasGoalStarted = true;
        }

        if (myTabletHelpers.Count == 0) return;

        aController.GoalUpdate.AddListener(OnGoalUpdate);

        OnGoalUpdate(aController);
    }

    public void OnGoalUpdate(NPCController aController)
    {
        if (myNextTabletteLooker == null)
        {
            if (myTabletHelpers.Count == 0)
            {
                GoalCompleted.Invoke();
                return;
            }

            aController.SetNewTargetPosition(myTabletHelpers[0].transform.position);
            myNextTabletteLooker = myTabletHelpers[0];
            myTabletHelpers.RemoveAt(0);
        }
        else
        {
            aController.CheckTabletThenMoveOn(myNextTabletteLooker);
            myNextTabletteLooker = null;
        }
    }
}
