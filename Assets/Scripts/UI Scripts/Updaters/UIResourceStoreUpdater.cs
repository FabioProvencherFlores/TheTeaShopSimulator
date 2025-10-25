using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResourceStoreUpdater : MonoBehaviour
{
	UIComputerController myController;

    [Header("Data")]
    [SerializeField] GameObject itemPanelObj;
    [SerializeField] GameObject shopItemPrefab;
    List<UIStoreItemUpdater> myLoadedUIItems = new List<UIStoreItemUpdater>();

    public void Init(UIComputerController aController, ResourceManager aResourceManager)
	{
		myController = aController;
        myLoadedUIItems.Clear();

        List<OnlineShopItem> storeItems;
        aResourceManager.GetStorePurchasableItems(out storeItems);
        int idx = 0;
        foreach (OnlineShopItem item in storeItems)
        {
            GameObject newItem = GameObject.Instantiate(shopItemPrefab);
            newItem.transform.SetParent(itemPanelObj.transform, false);
            UIStoreItemUpdater uiItem = newItem.GetComponent<UIStoreItemUpdater>();
            uiItem.SetItemData(item);
            uiItem.MyShopIntex = idx;
            uiItem.OnWorldMovementLockedChange.AddListener(OnUIItemClicked);
            myLoadedUIItems.Add(uiItem);

            ++idx;
        }
    }

    private void OnUIItemClicked(UIStoreItemUpdater.ButtonType aButtonType, int anIdx)
    {
        if (anIdx >= myLoadedUIItems.Count) return;
        UIStoreItemUpdater clickedItem = myLoadedUIItems[anIdx];
        myController.RequestResourceStoreAction(aButtonType, clickedItem.GetItemSubtype());
    }

    public void OnBackButtonClicked()
	{
		myController.RequestBackToHomeScreen();
	}
}
