using UnityEngine;

public enum InteractionUID
{
    INVALID,
    Pickup,
    DropInSlot,
    DropOnWorkbench,
    TransferResource,
    OpenComputer,
    PickupResourceDepot,
    CashRegistery
}

public enum InteractableObjectUID
{
    INVALID,
    GenericObject,
    WorkbenchObject
}

public class DroppedSlotInfo
{
    public Transform GetSlotPosition()
    {
        //if (myIsWorkbenchSlot) return myContainer.CurrentWorkBenchObject.GetWorkbenchSlotPosition();
        return myContainer.GetSlotTransformAtIdx(mySlotID);
    }

    public void FreeUpContainerSlot()
    {
        if (myContainer == null) return;
        
        myContainer.FreeupSlotAtIdx(mySlotID);
        
        if (myIsWorkbenchSlot)
        {
            ((WorkBenchInteraction)myContainer.MyParentContainer).CurrentSlotedObject = null;
        }
    }

	public void ReserveContainerSlot(PickupableObjectInteraction aDroppedObject)
	{
		myContainer.ReserveSlotAtIDx(mySlotID, aDroppedObject);
	}

	public DropableSubcontainer myContainer;
    public int mySlotID;
    public bool myIsWorkbenchSlot = false;
}
