using UnityEngine;

public class WorkBenchInteraction : PickupableObjectInteraction
{
	[SerializeField] Transform myWorkbenchPosition;
	DropableSubcontainer mySubcontainer;

	[Header("Scene Setup")]
	[SerializeField] PickupableObjectInteraction mySlottedItemAtStart;

	private PickupableObjectInteraction _currentSlotedObject;
	public PickupableObjectInteraction CurrentSlotedObject
	{
		get => _currentSlotedObject;
		set
		{
			if (value != null)
			{
				_currentSlotedObject = value;
                workBenchUtility.StartWorkBench();
			}
			if (value == null)
			{
				workBenchUtility.StopWorkBench();
				_currentSlotedObject = value;
			}
		}
	}
	
	private WorkBenchBase workBenchUtility;

	public override void Awake()
	{
		mySubcontainer = GetComponent<DropableSubcontainer>();
		workBenchUtility = GetComponent<WorkBenchBase>();
		if (workBenchUtility == null)
		{
			Debug.LogError("A Workbench object has not utility. Add it bro to [" + name + "] bro!", this);
		}
		base.Awake();
	}

	private void Start()
	{
		mySubcontainer.MyParentContainer = this;
		if (mySlottedItemAtStart != null) InteractionManager.Instance.DropItemIntoContainer(this, mySlottedItemAtStart);

	}
	public override bool CanInteract()
	{
		if (InteractionManager.Instance.ComplexInteractionMemory != null) return false;

		if (InteractionManager.Instance.IsPlayerCarryingObject())
		{
			if (IsCurrentlySlottedOnContainer() 
				&& InteractionManager.Instance.GetPickupableObject().GetObjectSize() != ItemSizeUID.Large
				&& mySubcontainer.IsEmpty())
			{
				// if I'm slotted AND player has small/medium item AND got nothing on me
				return true;
			}
			if (InteractionManager.Instance.GetPickupableObject().GetInteractableObjectUID() == InteractableObjectUID.WorkbenchObject)
			{
				// cant stack workbenches
				return false;
			}
		}
		// if player has nothing, can always at least pick up, or do the sloted thing
		else
		{
			return true;
		}

		return false; // #todo ce if pourrait etre mis plus clair...
	}

    public override bool Interact(PlayerController player)
    {
        if (IsWorkbenchEmpty()
            && !InteractionManager.Instance.IsPlayerCarryingObject())
        {
			InteractionManager.Instance.RequestPickupItem(this);
            return true;
        }
        if (IsWorkbenchEmpty() && IsCurrentlySlottedOnContainer() && InteractionManager.Instance.IsPlayerCarryingObject())
        {
            // #todo check if held object is not LARGE
            InteractionManager.Instance.RequestDropItemIntoContainer(this);
			return true;
        }

		return false;
    }

	public Transform GetWorkbenchSlotPosition()
	{
		return myWorkbenchPosition;
	}

	public bool IsWorkbenchEmpty()
	{
		return mySubcontainer.IsEmpty();
	}

	public override InteractableObjectUID GetInteractableObjectUID()
	{
		return InteractableObjectUID.WorkbenchObject;
	}

	public DroppedSlotInfo GetDropSlotInfo()
	{
		DroppedSlotInfo newDropInfo = new DroppedSlotInfo();
		newDropInfo.mySlotID = -1;
		newDropInfo.myContainer = null;
		newDropInfo.myIsWorkbenchSlot = true;

		if (mySubcontainer.IsEmpty())
		{
			newDropInfo.myContainer = mySubcontainer;
			newDropInfo.mySlotID = mySubcontainer.GetNextAvailableSlot();
		}

		return newDropInfo;
	}

	public override InteractionUID GetInteractionUID()
	{
		if (IsWorkbenchEmpty()
			&& !InteractionManager.Instance.IsPlayerCarryingObject())
		{
			return InteractionUID.Pickup;
		}
		if (IsWorkbenchEmpty() && IsCurrentlySlottedOnContainer() && InteractionManager.Instance.IsPlayerCarryingObject())
		{
			// #todo check if held object is not LARGE
			return InteractionUID.DropInSlot;
		}

		return InteractionUID.INVALID;
	}

	private protected override Color GetOutlineColor()
	{
		return Color.yellow;
	}
}
