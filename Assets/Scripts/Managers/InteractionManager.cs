using UnityEngine;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    private static InteractionManager _instance;
    public static InteractionManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("No Interaction Manager is found");

            return _instance;
        }
    }

    private Transform _targetHoldPosition;
    private PickupableObjectInteraction _playerHeldObject;
    public ComplexInteraction ComplexInteractionMemory {  get; set ; }
    [HideInInspector] public PlayerController PlayerReference { get; set ; }

    void Awake()
    {
        _instance = this;
    }

    void LateUpdate()
    {
        if (_playerHeldObject != null && _targetHoldPosition != null)
        {
            _playerHeldObject.transform.position = _targetHoldPosition.position;
            _playerHeldObject.transform.rotation = _targetHoldPosition.rotation;
        }
    }

    public bool CanMatchComplexInteraction(ComplexInteraction anotherComplexInteraction)
    {
        if (ComplexInteractionMemory == null) return false;

        if (ComplexInteractionMemory.gameObject == anotherComplexInteraction.gameObject) return false;

        InteractionUID matchingInteract = anotherComplexInteraction.GetMatchingComplexInteraction(ComplexInteractionMemory.GetInteractionTypes());
        if (matchingInteract == InteractionUID.TransferResource)
        {
            ResourceContainerObject currentLookingAtContainer = anotherComplexInteraction.GetComponent<ResourceContainerObject>();
            ResourceContainerObject rememberedContainer = ComplexInteractionMemory.GetComponent<ResourceContainerObject>();

            if (currentLookingAtContainer.GetCurrentQuantity() == 0f) return true; // if empty, can put anything
            return currentLookingAtContainer.GetCurrentResource() == rememberedContainer.GetCurrentResource(); // otherwise must match
        }

        return false;
    }

    public bool IsPlayerCarryingObject()
    {
        return _playerHeldObject != null;
    }

    public PickupableObjectInteraction GetPickupableObject()
    {
        return _playerHeldObject;
    }

    public void RequestResourceTransferInteraction(ResourceContainerObject aReceiver)
    {
        if (ComplexInteractionMemory == null) return;

        ResourceContainerObject giver = ComplexInteractionMemory.gameObject.GetComponent<ResourceContainerObject>();
        if (giver == null) return;

        if (!ResourceManager.Instance.RequestResourceTransfer(giver, aReceiver))
        {
            Debug.LogWarning("Resource transfer Interaction could not be completed successfully from [" 
                + giver.name + "] to [" + aReceiver.name + "]", this);
            return;
        }


        ComplexInteractionMemory = null;
	}

    public void RequestNPCPickupItem(PickupableObjectInteraction aPickupableItem)
    {
        aPickupableItem.SetPickedUpState();
        GameObject.Destroy(aPickupableItem.gameObject);
    }

	public void RequestPickupItem(PickupableObjectInteraction aPickupableItem)
    {

        _targetHoldPosition = PlayerReference.GetSmallHoldPosition();
        _playerHeldObject = aPickupableItem;
        aPickupableItem.SetPickedUpState();
    }
    public bool RequestCashRegisteryInteraction(PayingStationInteraction aPayingStation)
    {
        if (aPayingStation.CanInteract())
        {
            aPayingStation.WaitingNPC.PayForTransaction();
            ResourceManager.Instance.RequestAddPlayerMoneyForTransaction(20f);
            return true;
        }
        return false;
    }

    public void DropItemIntoContainer(WorkBenchInteraction aDropableContainerObject, PickupableObjectInteraction aObjectToDrop)
    {
        ItemSizeUID heldSize = aObjectToDrop.GetObjectSize();
        DroppedSlotInfo droppedPositionInfo = aDropableContainerObject.GetDropSlotInfo();
        aDropableContainerObject.CurrentSlotedObject = aObjectToDrop;

        DropIntoContainer(droppedPositionInfo, heldSize, aObjectToDrop);
    }

    public void RequestDropItemIntoContainer(WorkBenchInteraction aDropableContainerObject)
    {
		if (_playerHeldObject == null)
		{
			Debug.LogError("Trying to drop interaction without currently held item");
			return;
		}

        DropItemIntoContainer(aDropableContainerObject, _playerHeldObject);
	}

	public void DropItemIntoContainer(DropableContainerObject aDropableContainerObject, Vector3 aDropLocation, PickupableObjectInteraction aObjectToDrop)
    {
        ItemSizeUID heldSize = aObjectToDrop.GetObjectSize();
        DroppedSlotInfo droppedPositionInfo = aDropableContainerObject.GetNextSlotPlacement(heldSize, aDropLocation);

        DropIntoContainer(droppedPositionInfo, heldSize, aObjectToDrop);
    }

	public void RequestDropItemIntoContainer(DropableContainerObject aDropableContainerObject, Vector3 aDropLocation)
    {
        if (_playerHeldObject == null)
        {
            Debug.LogError("Trying to drop interaction without currently held item");
            return;
        }

        DropItemIntoContainer(aDropableContainerObject, aDropLocation, _playerHeldObject);
    }

    private void DropIntoContainer(DroppedSlotInfo aDropInfo, ItemSizeUID aSize, PickupableObjectInteraction aObjectToDrop)
    {
		if (aDropInfo.myContainer != null)
		{
			Transform droppedPosition = aDropInfo.myContainer.transform;
			if (aSize == ItemSizeUID.Small)
			{
				droppedPosition = aDropInfo.GetSlotPosition();
			}
            else if (aSize == ItemSizeUID.Medium && aDropInfo.myIsWorkbenchSlot)
            {
                droppedPosition = aDropInfo.myContainer.GetMediumSizeSlotTransform();
            }

			if (aObjectToDrop.GetInteractableObjectUID() == InteractableObjectUID.WorkbenchObject)
			{
                aDropInfo.myContainer.CurrentWorkBenchObject = (WorkBenchInteraction)aObjectToDrop;
			}

            aObjectToDrop.transform.position = droppedPosition.position;
			aObjectToDrop.transform.rotation = droppedPosition.rotation;
			aObjectToDrop.SetDroppedState(aDropInfo);
			_playerHeldObject = null;
			_targetHoldPosition = null;
		}
	}
    public void RequestDropItemOnGround(Vector3 aFloorPosition, Quaternion aFloorRotation)
    {
        if (_playerHeldObject == null) return;

		_playerHeldObject.transform.position = aFloorPosition;
		_playerHeldObject.transform.rotation = aFloorRotation;
		_playerHeldObject.SetDroppedState();
		_playerHeldObject = null;
		_targetHoldPosition = null;
	}
}
