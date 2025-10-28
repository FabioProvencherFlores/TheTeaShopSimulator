using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class UISupplyStoreUpdater : MonoBehaviour
{
    UIComputerController myController;

    [Header("Data")]
    [SerializeField] GameObject itemPanelObj;
    [SerializeField] GameObject supplyItemPrefab;
    List<UIItem_SupplyBundleUpdater> myLoadedUIItems = new List<UIItem_SupplyBundleUpdater>();

    public void Init(UIComputerController aController, SupplierManager aSupplierManager)
    {
        myController = aController;
        myLoadedUIItems.Clear();


        List<ResourceBundleUnlock> availableBundles;
        aSupplierManager.GetBundlesAvailableForUnlock(out availableBundles);
        int idx = 0;
        foreach (ResourceBundleUnlock item in availableBundles)
        {
            GameObject newItem = GameObject.Instantiate(supplyItemPrefab);
            newItem.transform.SetParent(itemPanelObj.transform, false);
            UIItem_SupplyBundleUpdater uiItem = newItem.GetComponent<UIItem_SupplyBundleUpdater>();
            uiItem.SetItemData(item);
            uiItem.MyShopIntex = idx;
            uiItem.OnSupplyDropUnlocked.AddListener(OnUIBundleUnlocked);
            myLoadedUIItems.Add(uiItem);

            ++idx;
        }

    }

    private void OnUIBundleUnlocked(int anID)
    {
        myController.RequestUnlockBundle(anID);
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

    public void OnCloseButtonClicked()
    {
		for (int i = 0; i < itemPanelObj.transform.childCount; i++)
		{
			Transform child = itemPanelObj.transform.GetChild(i);
			DestroyImmediate(child.gameObject);
		}

		myController.RequestCloseComputer();
	}
}