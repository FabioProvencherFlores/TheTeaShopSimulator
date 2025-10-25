using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResourceStoreUpdater : MonoBehaviour
{
	UIComputerController myController;

    [Header("Data")]
    [SerializeField] GameObject itemPanelObj;
    [SerializeField] GameObject shopItemPrefab;
    List<UIItem_ResourceUpdater> myLoadedUIItems = new List<UIItem_ResourceUpdater>();

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
            UIItem_ResourceUpdater uiItem = newItem.GetComponent<UIItem_ResourceUpdater>();
            uiItem.SetItemData(item);
            uiItem.MyShopIntex = idx;
            uiItem.OnWorldMovementLockedChange.AddListener(OnUIItemClicked);
            myLoadedUIItems.Add(uiItem);

            ++idx;
        }
    }

    private void OnUIItemClicked(UIItem_ResourceUpdater.ButtonType aButtonType, int anIdx)
    {
        if (anIdx >= myLoadedUIItems.Count) return;
        UIItem_ResourceUpdater clickedItem = myLoadedUIItems[anIdx];
        myController.RequestResourceStoreAction(aButtonType, clickedItem.GetItemSubtype());
    }

    public void OnBackButtonClicked()
	{
        for (int i = 0; i < itemPanelObj.transform.childCount; i++)
        {
            Transform child = itemPanelObj.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        myController.RequestBackToHomeScreen();
	}
}
