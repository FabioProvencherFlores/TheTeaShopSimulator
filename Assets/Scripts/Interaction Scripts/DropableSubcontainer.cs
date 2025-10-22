using UnityEngine;

public class DropableSubcontainer : MonoBehaviour
{
    [SerializeField] Transform[] dropableSlots;
	[SerializeField] GameObject inEditorVisuals;
	private PickupableObjectInteraction[] mySlottedItems;
    int _maxSlots;
    int _nbAvailableSlots;

	public ObjectInteractionBase MyParentContainer { get; set; }

	public WorkBenchInteraction CurrentWorkBenchObject {  get; set; }

	private void Awake()
    {
        _maxSlots = dropableSlots.Length;
		_nbAvailableSlots = _maxSlots;
		mySlottedItems = new PickupableObjectInteraction[_maxSlots];
        for (int i = 0; i < dropableSlots.Length; i++)
        {
            mySlottedItems[i] = null;
        }

		if (inEditorVisuals != null) inEditorVisuals.SetActive(false);

	}

    public bool HasSlotsAvailable() { return _nbAvailableSlots > 0; }
    public bool IsEmpty() { return _nbAvailableSlots == _maxSlots && CurrentWorkBenchObject == null; }

	public bool HasResourceTypeSlotted(ItemSubtypesUID aResourceType)
	{
		foreach (PickupableObjectInteraction slottedItem in mySlottedItems)
		{
			if (slottedItem != null)
			{
				if (slottedItem.ItemHasResourceTag(aResourceType))
				{
					return true;
				}
			}
		}
		return false;
	}

    public PickupableObjectInteraction GetPickableItemContainingType(ItemSubtypesUID aResourceType)
    {
        foreach (PickupableObjectInteraction slottedItem in mySlottedItems)
        {
            if (slottedItem != null)
            {
                if (slottedItem.ItemHasResourceTag(aResourceType))
                {
                    return slottedItem;
                }
            }
        }
        return null;
    }

    public int GetNextAvailableSlot()
    {
		if (CurrentWorkBenchObject != null) return -1;
		for (int i = 0; i < dropableSlots.Length; i++)
		{
			if (mySlottedItems[i] == null)
            {
                return i;
            }
		}

		return -1;
    }

	public void ReserveSlotAtIDx(int anIdx, PickupableObjectInteraction aDroppedItem)
	{
		if (anIdx >= _maxSlots) return;
		if (anIdx < 0)
		{
			_nbAvailableSlots = 0;
		}
		else
		{
			_nbAvailableSlots--;
			mySlottedItems[anIdx] = aDroppedItem;
		}
	}

	public void FreeupSlotAtIdx(int anIdx)
	{
		if (anIdx >= _maxSlots) return;

		if (anIdx < 0)
		{
			_nbAvailableSlots = _maxSlots;
		}
		else
		{
			_nbAvailableSlots++;
			mySlottedItems[anIdx] = null;
		}

		CurrentWorkBenchObject = null;
	}

	public Transform GetMediumSizeSlotTransform()
	{
		return ((WorkBenchInteraction)MyParentContainer).GetWorkbenchSlotPosition();
    }

	public Transform GetSlotTransformAtIdx(int anIdx)
    {
        if (anIdx >= _maxSlots) return null;
		if (anIdx < 0) return transform;
		return dropableSlots[anIdx];
    }

}
