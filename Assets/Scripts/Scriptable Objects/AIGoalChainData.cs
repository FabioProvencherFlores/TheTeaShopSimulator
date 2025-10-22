using System;
using UnityEngine;
using UnityEngine.Events;

public class AI_Base : ScriptableObject
{
    public virtual CustomerGoal GetCustomerGoal() 
    {
        Debug.LogError("AI Goal does not has valid Goal Type", this);
        return CustomerGoal.TakeDecision;
    }
}

public class AIG_BaseCell : AI_Base
{
    [HideInInspector] public UnityEvent GoalCompleted = new UnityEvent();
    public virtual void OnUpdate() { }
    public virtual bool IsStartingStateValid(CustomerState aState, NPCManager aNPCManager) {  return false; }
    public virtual bool IsGoalCompleted(CustomerState aState, NPCManager aNPCManager) { return false; }
    public virtual void ProcessNPCGoal(NPCController aController, NPCManager aNPCManager) { }
    public virtual void PostGoalCompletion(NPCController aController) { }
}

public class AIF_BaseFork : AI_Base
{
    public override CustomerGoal GetCustomerGoal() { return CustomerGoal.TakeDecision; }
    public virtual bool ResolveCondition(CustomerState aState) { return false; }
    public AI_Base[] falseChain;
}

