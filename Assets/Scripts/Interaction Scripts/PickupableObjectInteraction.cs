using UnityEngine;

public class PickupableObjectInteraction : ObjectInteractionBase
{
    [Header("Pickup Settings")]
    [SerializeField] public InteractableObjectUID ObjectTypeUID { get; set;  }
    private BoxCollider myBoxCollider;

    public DroppedSlotInfo myDroppedSlotInfo;

    [SerializeField] private ItemSizeUID myObjectSize = ItemSizeUID.Small;

    private ResourceContainerObject container = null;


    public override void Awake()
    {
        base.Awake();
        myBoxCollider = GetComponent<BoxCollider>();
		//myDroppedSlotInfo = new DroppedSlotInfo();
		//myDroppedSlotInfo.myContainer = null;
  //      myDroppedSlotInfo.mySlotID = -1;

        container = GetComponent<ResourceContainerObject>();
    }

    protected override void TurnHoverOn()
    {
        base.TurnHoverOn();
        if (container != null)
        {
            GetComponent<ResourceContainerObject>().ShowResource();
        }
    }

    protected override void TurnHoverOff()
    {
        base.TurnHoverOff();
        if (container != null) GetComponent<ResourceContainerObject>().HideText();
    }

    public override bool CanInteract()
	{
        if (myComplexInteraction != null)
        {
            if (InteractionManager.Instance.CanMatchComplexInteraction(myComplexInteraction)) return true;
        }
            
		return !InteractionManager.Instance.IsPlayerCarryingObject();
	}

    public override bool Interact(PlayerController player)
    {
        if (myComplexInteraction != null && container != null)
        {
            if (InteractionManager.Instance.CanMatchComplexInteraction(myComplexInteraction))
            {
                InteractionManager.Instance.RequestResourceTransferInteraction(container);
                return true;
            }
        }

        InteractionManager.Instance.RequestPickupItem(this);
        return true;
    }

    public ItemSizeUID GetObjectSize() { return myObjectSize; }

    public bool IsCurrentlySlottedOnContainer() { return myDroppedSlotInfo != null; }
	public void SetPickedUpState()
    {
		myBoxCollider.enabled = false;

        if (myDroppedSlotInfo != null)
        {
            myDroppedSlotInfo.FreeUpContainerSlot();
            myDroppedSlotInfo = null; // forget previous container, no need to keep it
        }
    }

    public bool ItemHasResourceTag(ItemSubtypesUID aResourceTag)
    {
        if (container != null)
        {
            ResourceContainerObject container = GetComponent<ResourceContainerObject>();
            if (container != null)
            {
                if (container.GetCurrentResource() == aResourceTag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetDroppedState(DroppedSlotInfo droppedPositionInfo)
    {
        if (myDroppedSlotInfo != null) Debug.LogError("A drop state is set without proper cleanup of previous state", this);
        myDroppedSlotInfo = droppedPositionInfo;
        myDroppedSlotInfo.ReserveContainerSlot(this);

        SetDroppedState();
	}

    // when dropped on floor
	public void SetDroppedState()
	{
		myBoxCollider.enabled = true;
	}

	public override InteractionUID GetInteractionUID()
    {
		ComplexInteraction potentialInteraction = InteractionManager.Instance.ComplexInteractionMemory;
		if (potentialInteraction != null)
		{
            if (myComplexInteraction != null)
            {
				InteractionUID complexInteraction = potentialInteraction.GetMatchingComplexInteraction(myComplexInteraction.GetInteractionTypes());
                if (complexInteraction != InteractionUID.INVALID)
                {
                    return complexInteraction;
                }
            }
		}

		return InteractionUID.Pickup;
    }

    private protected override Color GetOutlineColor()
    {
        return Color.yellow;
    }
}
