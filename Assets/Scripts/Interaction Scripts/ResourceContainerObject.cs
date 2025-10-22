using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ResourceContainerObject : MonoBehaviour
{
    [SerializeField] float amountAtTheStart = 0f;
    [SerializeField] ItemSubtypesUID resourceAtTheSTart = ItemSubtypesUID.INVALID;
    [SerializeField] TMP_Text  myShownText;
    [SerializeField] float myMaxContainerCapacity;

    private ResourceWallet myResourceWallet;
    private ItemSizeUID myItemSize = ItemSizeUID.Small;
    bool myShouldShowResource = false;
    bool myShouldShowQuantity = false;

    private void Awake()
    {
        myResourceWallet = ScriptableObject.CreateInstance<ResourceWallet>();
        myResourceWallet.SetMaxObjectCapacity(myMaxContainerCapacity);
        PickupableObjectInteraction pickupable = GetComponent<PickupableObjectInteraction>();
        if (pickupable !=null) myItemSize = pickupable.GetObjectSize();
    }

    private void Start()
	{
        if (myShownText == null)
        {
            Debug.LogError("Missing Text linked in prefab", this);
        }

        HideText();
        myResourceWallet.OnQuantityUpdated.AddListener(UpdateText);

        if (!myResourceWallet.HasResource()) 
            myResourceWallet.SetQuantityAndResource(amountAtTheStart, resourceAtTheSTart);
	}

    public void HideText()
    {
        myShouldShowResource = false;
        myShouldShowQuantity = false;
        myShownText.text = "";
    }

    public void ShowResource() { myShouldShowResource = true; UpdateText(myResourceWallet.CurrentQuantity); }
    public void ShowQuantity() { myShouldShowQuantity= true; UpdateText(myResourceWallet.CurrentQuantity); }

    public ItemSizeUID GetScoopSize() { return myItemSize; }

    public bool ChangeResourceType(ItemSubtypesUID aNewResourceType)
    {
        if (myResourceWallet.CurrentQuantity > 0f) return false;

        myResourceWallet.SetQuantityAndResource(0f, aNewResourceType);
        return true;
    }

    private void UpdateText(float newQuantity)
    {
        string toShow = "";
        if (myShouldShowResource) toShow += (newQuantity > 0f ? myResourceWallet.GetResourceSubtype().ToString() : "Empty") + " ";
        if (myShouldShowQuantity) toShow += newQuantity.ToString();
        myShownText.text = toShow;
    }

    public float GetCurrentQuantity() { return myResourceWallet.CurrentQuantity;  }
    public ItemSubtypesUID GetCurrentResource() { return myResourceWallet.GetResourceSubtype();  }
    public void AddQuantity(float anAddedAmount)
    {
        myResourceWallet.AddQuantity(anAddedAmount);
	}

    public void RegisterToQuantityChange(UnityAction<float> aCallback)
    {
        if (myResourceWallet != null) myResourceWallet.OnQuantityUpdated.AddListener(aCallback);
    }

    public void DeregisterFromQuantityChange(UnityAction<float> aCallback)
    {
        if (myResourceWallet != null) myResourceWallet.OnQuantityUpdated.RemoveListener(aCallback);
    }
}
