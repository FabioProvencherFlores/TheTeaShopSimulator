using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class ResourceWallet : ScriptableObject
{
    private float myCurrentQuantity;
    private float myOverflowedQuantity;
    private ResourceItem myCurrentResource;
    private float myObjectCapacity = float.MaxValue;
    private float myMaxQuantity;

    public UnityEvent<float> OnQuantityUpdated = new UnityEvent<float>();

    public bool HasResource() {  return myCurrentResource != null; }
    public float CurrentQuantity
    {
        get => myCurrentQuantity;
        private set
        {
            if (myCurrentResource == null) return;
            myCurrentQuantity = Mathf.Clamp(value, 0f, myMaxQuantity);
            if (!myCurrentResource.myAllowPartialAmount) myCurrentQuantity = Mathf.Round(myCurrentQuantity);
        }
    }
    public ItemSubtypesUID GetResourceSubtype() { return myCurrentResource.mySubtypesUID; }
    public void SetMaxObjectCapacity(float aMaxCapacity) { myObjectCapacity = aMaxCapacity; }
    public void AddQuantity(float anAddedAmount)
    {
        float newTotal = myCurrentQuantity + anAddedAmount;
        UpdateQuantity(newTotal);
    }

    public void SetQuantityAndResource(float aNewAmount, ItemSubtypesUID aNewResourceType)
    {
        UpdateResource(aNewResourceType);
        UpdateQuantity(aNewAmount);
    }

    public float ConsumeOverflowedQuantity()
    {
        float overflow = myOverflowedQuantity;
        myOverflowedQuantity = 0f;
        return overflow;
    }

    private void UpdateQuantity(float aNewAmount)
    {
        float previousAmount = myCurrentQuantity;
        CurrentQuantity = aNewAmount;
        myOverflowedQuantity += math.max(0f, aNewAmount - myCurrentQuantity);
        if (previousAmount != myCurrentQuantity) OnQuantityUpdated.Invoke(myCurrentQuantity);
    }

    private void UpdateResource(ItemSubtypesUID aNewResourceType)
    {
        myOverflowedQuantity = 0f;
        myCurrentResource = ResourceManager.Instance.GetResourceItem(aNewResourceType);
        if (myCurrentResource == null) Debug.LogError("Wallet contains a resourcetype unknown to the Resource Manager. Did you forget to add: " + aNewResourceType.ToString(), this);

        myMaxQuantity = math.min(myObjectCapacity, myCurrentResource.myMaxValue);
    }
}
