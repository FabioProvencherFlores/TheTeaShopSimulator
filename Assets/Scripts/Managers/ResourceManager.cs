using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

public class ResourceManager : MonoBehaviour
{
	#region Instance
	private static ResourceManager _instance;
	public static ResourceManager Instance
	{
		get
		{
			if (_instance is null)
				Debug.LogError("No Game Manager is found");

			return _instance;
		}
	}
	#endregion

	Dictionary<ItemSubtypesUID, ResourceItem> myResourcesDescriptors = new Dictionary<ItemSubtypesUID, ResourceItem>();
	Dictionary<ItemSubtypesUID, OnlineShopItem> myShopItemsDescriptors = new Dictionary<ItemSubtypesUID, OnlineShopItem>();
	List<ItemSubtypesUID> mySellableResources = new List<ItemSubtypesUID>();


	[Header("Player Wallets")]
	[SerializeField] ItemSubtypesUID[] myPlayerWalletsTypes = { };
	private Dictionary<ItemSubtypesUID, ResourceWallet> myplayerWallets = new Dictionary<ItemSubtypesUID, ResourceWallet>();

	[Header("Resource Spawn Settings")]
	[SerializeField] ItemDeliverySpotController myDeliverySpotController;

	[HideInInspector] public UnityEvent<OnlineShopItem> OnStoreItemPurchased = new UnityEvent<OnlineShopItem>();


    void Awake()
	{
		_instance = this;

		if (myDeliverySpotController == null) { Debug.LogError("No delivery Spot was setup in this scene, or it wasnt linked", this); }

		// load resources items descriptors
		ResourceItem[] allResourceItems = Resources.LoadAll<ResourceItem>("Items/Resource Items/");
		foreach (ResourceItem item in allResourceItems)
		{
			myResourcesDescriptors.Add(item.mySubtypesUID, item);
		}

		// load player wallets
        foreach (ItemSubtypesUID resourceType in myPlayerWalletsTypes)
        {
            ResourceWallet newWallet = ScriptableObject.CreateInstance<ResourceWallet>();
			newWallet.SetQuantityAndResource(0f, resourceType);
			myplayerWallets.Add(resourceType, newWallet);
        }

        // load shop item descriptors
        OnlineShopItem[] allShopItems = Resources.LoadAll<OnlineShopItem>("Items/Shop Items/");
        foreach (OnlineShopItem item in allShopItems)
        {
            myShopItemsDescriptors.Add(item.mySubtypesUID, item);
        }

		OnStoreItemPurchased.AddListener(OnStoreItemPostPurchased);
    }

	private ResourceWallet GetPlayerWalletOfType(ItemSubtypesUID aResourceType)
	{
        ResourceWallet wallet;
        myplayerWallets.TryGetValue(aResourceType, out wallet);
		return wallet;
    }

	public void RegisterToPlayerMoneyChange(UnityAction<float> aCallback)
	{
        ResourceWallet moneyWallet = GetPlayerWalletOfType(ItemSubtypesUID.Money);
        if (moneyWallet != null)
        {
			moneyWallet.OnQuantityUpdated.AddListener(aCallback);
        }
    }

    public void DeregisterFromPlayerMoneyChange(UnityAction<float> aCallback)
    {
		ResourceWallet moneyWallet = GetPlayerWalletOfType(ItemSubtypesUID.Money);
        if (moneyWallet != null)
        {
            moneyWallet.OnQuantityUpdated.RemoveListener(aCallback);
        }
    }

    public ResourceItem GetResourceItem(ItemSubtypesUID anItemSubtype)
	{
		ResourceItem item = null;
		if (myResourcesDescriptors.TryGetValue(anItemSubtype, out item)) return item;
		return null;
	}

	public bool RequestResourceTransfer(ResourceContainerObject aGiver, ResourceContainerObject aReceiver)
	{
		if (aGiver.GetCurrentResource() != aReceiver.GetCurrentResource())
		{
			if (!aReceiver.ChangeResourceType(aGiver.GetCurrentResource()))
			{
				Debug.LogWarning("Could not change resource during transfer from [" + aGiver.name + "] to [" + aReceiver.name + "]", this);
				return false;
			}
		}

        float movedAmount = 1f; // #todo this should be a minigame
		switch(aGiver.GetScoopSize())
		{
			case ItemSizeUID.Small:
				movedAmount = 10f;
				break;
			case ItemSizeUID.Medium:
				movedAmount = 50f;
				break;
			case ItemSizeUID.Large:
				movedAmount = 1000f;
				break;
		}

		if (aGiver.GetCurrentQuantity() < movedAmount) return false;

		aGiver.AddQuantity(-movedAmount);
		aReceiver.AddQuantity(movedAmount);
		return true;
	}

	public bool RequestResourcePickupFromNPC(ResourceContainerObject aContainer, ResourceWallet aNPCWallet)
	{
		if (aContainer.GetCurrentResource() == aNPCWallet.GetResourceSubtype())
		{
			aNPCWallet.AddQuantity(aContainer.GetCurrentQuantity());
			return true;
		}
		return false;
	}

	public bool RequestRawResourcePickup(RawResourceDepotInteraction aResourceDepot)
	{
		if (aResourceDepot.GetResourceDepotItemType() == ItemSubtypesUID.Money)
		{
            ResourceWallet wallet;
            myplayerWallets.TryGetValue(aResourceDepot.GetResourceDepotItemType(), out wallet);
			if (wallet != null)
			{
				wallet.AddQuantity(aResourceDepot.GetResourceQuantityToLoot());
				aResourceDepot.OnHoveredChange(false);
                GameObject.Destroy(aResourceDepot.gameObject);
                return true;
			}
		}
		return false;
	}

	public void RequestAddPlayerMoneyForTransaction(float anAmount)
	{
        ResourceWallet moneyWallet = GetPlayerWalletOfType(ItemSubtypesUID.Money);
        if (moneyWallet != null)
        {
			GameManager.Instance.myDayScore.myTotalProfits += anAmount; // #todo use amount change listener instead
            moneyWallet.AddQuantity(anAmount);
        }
    }

	private void OnStoreItemPostPurchased(OnlineShopItem anItemPurchased)
	{
		if (anItemPurchased.myItemTypeUID == ItemTypeUID.Tea)
		{
			// if this tea was not available, make it available for customers now!
			if (!mySellableResources.Contains(anItemPurchased.mySubtypesUID)) mySellableResources.Add(anItemPurchased.mySubtypesUID);
		}
    }

	public ItemSubtypesUID[] GetRandomDesiredCustomerItems(int anAmount)
	{
		List<ItemSubtypesUID> potentialItems = mySellableResources;

		while (potentialItems.Count > anAmount)
		{
			if (potentialItems.Count == 0) break;

			int randomIdx = Random.Range(0, potentialItems.Count);
			potentialItems.RemoveAt(randomIdx);
        }

		return potentialItems.ToArray();
	}

	public bool RequestPurchaseFromOnlineStore(ItemSubtypesUID anItemSubtypeToDeliver)
    {
		OnlineShopItem storeItem;
        if (myShopItemsDescriptors.TryGetValue(anItemSubtypeToDeliver, out storeItem))
		{
			//not enough money
			ResourceWallet moneyWallet = GetPlayerWalletOfType(ItemSubtypesUID.Money);
			if (moneyWallet.CurrentQuantity < storeItem.myShopPrice) return false;

			moneyWallet.AddQuantity(-storeItem.myShopPrice);
            PickupableObjectInteraction deliveredContainer = myDeliverySpotController.DelivertInNextAvailableSlot(storeItem.myDeliverablePrefab);

            if (storeItem.myItemTypeUID == ItemTypeUID.Tea)
			{
				ResourceContainerObject containerObject = deliveredContainer.GetComponent<ResourceContainerObject>();
				if (containerObject == null) Debug.LogError("This resource container has no container component", this);
                containerObject.ChangeResourceType(anItemSubtypeToDeliver);
				containerObject.AddQuantity(16000f);
			}

			OnStoreItemPurchased.Invoke(storeItem);
			return true;
        }

        return false;
    }

	public void GetStorePurchasableItems(out List<OnlineShopItem> aListOfItems)
	{
		aListOfItems = new List<OnlineShopItem>();
        foreach (var item in myShopItemsDescriptors)
        {
			// #todo ajouter logique de "purchasable", lorsqu on va avoir des trucs a debloquer
			aListOfItems.Add(item.Value);
		}
    }

}
