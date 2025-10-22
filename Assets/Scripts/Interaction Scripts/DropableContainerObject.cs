using System.Collections.Generic;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DropableContainerObject : ObjectInteractionBase
{

    [Header("Drop Interaction Settings")]
    [SerializeField] 
    DropableSubcontainer[] subcontainers;

    [Header("Scene Setup")]
    [SerializeField] PickupableObjectInteraction[] mySlottedItemsAtStart;

	private void Start()
	{
		foreach (DropableSubcontainer sub in subcontainers)
        {
            sub.MyParentContainer = this;
        }

        StartCoroutine(PutItemsInSlotAtSTart());
	}

    IEnumerator PutItemsInSlotAtSTart()
    {
        yield return new WaitForEndOfFrame();
        foreach (PickupableObjectInteraction item in mySlottedItemsAtStart)
        {
            if (item == null)
            {
                Debug.LogWarning("Null object set to attach at start on " + gameObject.name, this);
                continue;
            }
            if (GameManager.Instance == null) Debug.LogError("Wowowo, you need a Game Manager to use the player, plz", this);
            InteractionManager mi = InteractionManager.Instance;
            mi.DropItemIntoContainer(this, item.transform.position, item);
            if (!item.IsCurrentlySlottedOnContainer()) Debug.LogWarning("No room in container for: " + item.name, this);
        }
    }


    public override bool CanInteract()
	{
        if (!InteractionManager.Instance.IsPlayerCarryingObject()) return false;
        if (InteractionManager.Instance.ComplexInteractionMemory != null) return false;
        
        PickupableObjectInteraction heldObject = InteractionManager.Instance.GetPickupableObject();
        if (heldObject.GetObjectSize() == ItemSizeUID.Small)
        {
		    foreach (DropableSubcontainer subcontainer in subcontainers)
		    {
		    	if (subcontainer.HasSlotsAvailable()) return true;
		    }

        }
        else if (heldObject.GetObjectSize() == ItemSizeUID.Medium)
        {
			foreach (DropableSubcontainer subcontainer in subcontainers)
			{
				if (subcontainer.IsEmpty()) return true;
			}
		}

		return false;
	}

    public override bool Interact(PlayerController player)
    {
        InteractionManager.Instance.RequestDropItemIntoContainer(this, player.GetLastHitLocation());
        return true;
    }


	public override InteractionUID GetInteractionUID()
    {
        return InteractionUID.DropInSlot;
    }

    private protected override Color GetOutlineColor()
    {
        return Color.blue;
    }

	public DroppedSlotInfo GetNextSlotPlacement(ItemSizeUID aSize, Vector3 aSearchPosition)
    {
        DroppedSlotInfo newSlotInfo = new DroppedSlotInfo();
        newSlotInfo.myContainer = null;
        newSlotInfo.mySlotID = -1;

		IEnumerable<DropableSubcontainer> orderedContainers = subcontainers.OrderBy(container => (container.transform.position - aSearchPosition).sqrMagnitude);

		foreach (DropableSubcontainer orderedContainer in orderedContainers)
		{
            if (aSize == ItemSizeUID.Small)
            {
                if (orderedContainer.HasSlotsAvailable())
                {
				    int result = orderedContainer.GetNextAvailableSlot();
                    if (result >= 0)
                    {
                        newSlotInfo.myContainer = orderedContainer;
                        newSlotInfo.mySlotID = result;
                        break;
                    }
                }
                else if (orderedContainer.CurrentWorkBenchObject != null && orderedContainer.CurrentWorkBenchObject.IsWorkbenchEmpty())
                {
                    newSlotInfo.myContainer = orderedContainer;
                    newSlotInfo.myIsWorkbenchSlot = true;

                }

            }
            else if (aSize == ItemSizeUID.Medium)
            {
                if (orderedContainer.IsEmpty())
                {
                    newSlotInfo.myContainer = orderedContainer;
                    newSlotInfo.mySlotID = -1;
                    break;
                }
            }
		}

        return newSlotInfo;
    }
}
