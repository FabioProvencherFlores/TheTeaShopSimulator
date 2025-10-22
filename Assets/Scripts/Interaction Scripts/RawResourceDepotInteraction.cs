using TMPro;
using UnityEngine;

public class RawResourceDepotInteraction : ObjectInteractionBase
{
    [SerializeField] ResourceItem itemDescriptor;
    [SerializeField] TMP_Text visualText;
    [SerializeField] float lootQuantity = 500f;
    public override bool CanInteract()
    {
        if (InteractionManager.Instance.IsPlayerCarryingObject()) return false;
        if (InteractionManager.Instance.ComplexInteractionMemory != null) return false;

        return true;
    }

    private protected override Color GetOutlineColor()
    {
        return Color.yellow;
    }

    public override InteractionUID GetInteractionUID()
    {
        return InteractionUID.PickupResourceDepot;
    }

    public override bool Interact(PlayerController player)
    {
        if (ResourceManager.Instance.RequestRawResourcePickup(this))
        {
            return true;
        }

        return false;
    }

    private void Start()
    {
        visualText.text = itemDescriptor.myItemName;
    }

    public ItemSubtypesUID GetResourceDepotItemType()
    {
        return itemDescriptor.mySubtypesUID;
    }

    public float GetResourceQuantityToLoot()
    {
        return lootQuantity;
    }
}
