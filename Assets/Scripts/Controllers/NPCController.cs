using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum CustomerGoal
{
    TakeDecision,
    Idle,
    EnterTeashop,
    LookForDesiredItems,
    GoToHelpDesk,
    GoToCashRegister,
    LeaveStore,
    WalkDistance
}

public struct CustomerState
{
    // AgentState
    public bool IsInsideStore { get; set; }
    public bool IsWalking { get; set; }
    public bool HasFoundDesiredItems { get; set; }
    public bool IsNotAbleToFindItems { get; set; }
    public bool IsReadyToLeave { get; set; }
    public bool IsTransactionPayed { get; set; }

    public bool IsTransitioningGoal { get; set; }

    public void ResetState()
    {
        IsWalking = false;
        IsInsideStore = false;
        HasFoundDesiredItems = false;
        IsNotAbleToFindItems = false;
        IsTransactionPayed = false;
        IsReadyToLeave = false;
        IsTransitioningGoal = false;
    }

    public override string ToString()
    {
        return "{STATE {IsWalking:" + IsWalking.ToString()
            + "} {IsInsideStore:" + IsInsideStore.ToString()
            + "} {HasFoundDesiredItems:" + HasFoundDesiredItems.ToString()
            + "} {IsNotAbleToFindItems:" + IsNotAbleToFindItems.ToString()
            + "} {HasPaidForItems:" + IsReadyToLeave.ToString()
            + "} {IsTransactionPayed:" + IsTransactionPayed.ToString()
            + "} {IsTransitioningGoal:" + IsTransitioningGoal.ToString()
            + "} }";
    }
}

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    private NavMeshAgent myNavmeshAgent;

    private NPCManager myNPCManagerInstance;

    ResourceWallet myNPCWallet;

    // AgentState
    public CustomerState myCurrentState = new CustomerState();
    Vector3 debugTargetPos = Vector3.zero;
    Vector3 myStartingPosition;

    private Queue<AI_Base> myGoals = new Queue<AI_Base>();

    // anims
    private Animator myAnimator;

    public AIG_BaseCell CurrentGoal { get; private set; }

    public UnityEvent<NPCController> GoalUpdate = new UnityEvent<NPCController>();
    public UnityEvent<NPCController> DestinationReached = new UnityEvent<NPCController>();

    private void Awake()
    {
        myNavmeshAgent = GetComponent<NavMeshAgent>();
        myCurrentState.ResetState();
        myNPCWallet = ScriptableObject.CreateInstance<ResourceWallet>();
        myNPCWallet.SetQuantityAndResource(0f, ItemSubtypesUID.Green);
        Vector3 myStartingPosition = transform.position;
        myAnimator = GetComponent<Animator>();
    }

    public ItemSubtypesUID GetDesiredItem()
    {
        return myNPCWallet.GetResourceSubtype();
    }

    public void PayForTransaction()
    {
        SetIsTransactionPayed(true);
    }

    private void SetNewGoalChain(AI_Base[] aChain)
    {
        myGoals.Clear();
        foreach (AI_Base goal in aChain)
        {
            myGoals.Enqueue(Instantiate(goal));
        }
    }

    public void Init(NPCManager aManager, NPCArchetype anArchetype)
    {
        myNPCManagerInstance = aManager;
        SetNewGoalChain(anArchetype.myGoalChain);
        ItemSubtypesUID desiredType = anArchetype.GetRandomDesiredType(); // #todo this should be pulled from SupplierManager, depending on what is bought in computer
        myNPCWallet.SetQuantityAndResource(0f, desiredType);

        print("Going off to the shop cuz I need some " + desiredType.ToString());
    }

    private void Update()
    {
        if (myCurrentState.IsWalking)
        {
            if (NavmeshAgentReachedTarget())
            {
                DestinationReached.Invoke(this);
                GoalUpdate.Invoke(this);
            }
        }
    }

    private IEnumerator NotifyGoalCompleted()
    {
        if (myCurrentState.IsTransitioningGoal) yield break;

        myCurrentState.IsTransitioningGoal = true;
        yield return new WaitForSeconds(1f);
        myNPCManagerInstance.NotifyGoalReached(this);
    }


    public bool GoToNextGoal()
    {
        if (CurrentGoal != null)
        {
            #region debug print
            if (Selection.Contains(gameObject))
            {
                StringBuilder printState = new StringBuilder();
                printState.Append("State Before Changing goal of [");
                printState.Append(CurrentGoal.GetCustomerGoal().ToString());
                printState.Append("]  ");
                printState.Append(myCurrentState.ToString());
            }
            #endregion

            CurrentGoal.PostGoalCompletion(this);
            CurrentGoal.GoalCompleted.RemoveListener(OnGoalCompleted);
            Destroy(CurrentGoal);
            CurrentGoal = null;
        }

        if (myCurrentState.IsReadyToLeave) myNPCManagerInstance.NotifyShoppingComplete(this);

        if (myGoals.Count == 0) return false;

        while(myGoals.Peek().GetCustomerGoal() == CustomerGoal.TakeDecision)
        {
            AIF_BaseFork fork = (AIF_BaseFork)myGoals.Dequeue();
            if (!fork.ResolveCondition(myCurrentState))
            {
                SetNewGoalChain(fork.falseChain);
            }
        }

        CurrentGoal = (AIG_BaseCell)myGoals.Dequeue();

        if (!CurrentGoal.IsStartingStateValid(myCurrentState, myNPCManagerInstance))
        {
            if (!CurrentGoal.IsGoalCompleted(myCurrentState, myNPCManagerInstance))
            {
                StringBuilder error = new StringBuilder();
                error.Append("Can't start current goal of [");
                error.Append(CurrentGoal.GetCustomerGoal().ToString());
                error.Append("] with current state --- ");
                error.Append(myCurrentState.ToString());
                Debug.LogWarning(error.ToString(), this);
                return false;
            }
        }
        CurrentGoal.GoalCompleted.AddListener(OnGoalCompleted);
        CurrentGoal.ProcessNPCGoal(this, myNPCManagerInstance);

        #region debug print
        if (Selection.Contains(gameObject))
        {
            StringBuilder printState = new StringBuilder();
            printState.Append("State After Changing goal to [");
            printState.Append(CurrentGoal.GetCustomerGoal().ToString());
            printState.Append("]  ");
            Debug.Log(printState.ToString(), this);
        }
        #endregion

        myCurrentState.IsTransitioningGoal = false;
        return true;
    }

    private void OnGoalCompleted()
    {
        StartCoroutine(NotifyGoalCompleted());
    }

    public void GracefullyLeaveStore()
    {
        SetNewTargetPosition(myStartingPosition);
        myCurrentState.IsReadyToLeave = true;
    }

    private void OnDrawGizmos()
    {
        if (debugTargetPos != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(debugTargetPos, 0.5f);
        }
    }

    #region Helpers Functions
    public void CheckTabletThenMoveOn(NPCTabletteLookerHelper aHelper)
    {
        StartCoroutine(StartLookingAround(aHelper));
    }

    private IEnumerator StartLookingAround(NPCTabletteLookerHelper aHelper)
    {
        if (aHelper == null) yield return null;

        PickupableObjectInteraction pickupableObject = aHelper.GetInteractableOfType(myNPCWallet.GetResourceSubtype());
        if (pickupableObject != null)
        {
            // yaye found the thing
            ResourceContainerObject container = pickupableObject.GetComponent<ResourceContainerObject>();
            if (ResourceManager.Instance.RequestResourcePickupFromNPC(container, myNPCWallet))
            {
                InteractionManager.Instance.RequestNPCPickupItem(pickupableObject);
                SetFoundDesiredItems(true);
            }
        }

        yield return new WaitForSeconds(2f);

        if (!myCurrentState.HasFoundDesiredItems) GoalUpdate.Invoke(this);
    }

    bool NavmeshAgentReachedTarget()
    {
        if (!myNavmeshAgent.pathPending)
        {
            if (myNavmeshAgent.remainingDistance <= myNavmeshAgent.stoppingDistance)
            {
                if (!myNavmeshAgent.hasPath || myNavmeshAgent.velocity.sqrMagnitude == 0f)
                {
                    SetIsWalking(false);
                    return true;
                }
            }
        }

        return false;
    }

    public void SetNewTargetPosition(Vector3 aNewPos)
    {
        myNavmeshAgent.SetDestination(aNewPos);
        debugTargetPos = aNewPos;
        SetIsWalking(true);
    }

    #endregion
    #region State Updates
    private void OnStateUpdated()
    {
        if (CurrentGoal.IsGoalCompleted(myCurrentState, myNPCManagerInstance)) StartCoroutine(NotifyGoalCompleted());
    }

    public void SetIsTransactionPayed(bool aNewValue)
    {
        if (aNewValue != myCurrentState.IsTransactionPayed)
        {
            myCurrentState.IsTransactionPayed = aNewValue;
            OnStateUpdated();
        }
    }

    public void SetFoundDesiredItems(bool aNewValue)
    {
        if (aNewValue != myCurrentState.HasFoundDesiredItems)
        {
            myCurrentState.HasFoundDesiredItems = aNewValue;
            OnStateUpdated();
        }
    }

    public void SetIsInsideStore(bool aNewValue)
    {
        if (aNewValue != myCurrentState.IsInsideStore)
        {
            myCurrentState.IsInsideStore = aNewValue;
            OnStateUpdated();
        }
    }

    private void SetIsWalking(bool aNewValue)
    {
        if (aNewValue != myCurrentState.IsWalking)
        {
            myCurrentState.IsWalking = aNewValue;
            myAnimator.SetBool("IsWalking", aNewValue);
            OnStateUpdated();
        }
    }
    #endregion
}
