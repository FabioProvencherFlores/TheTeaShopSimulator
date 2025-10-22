using System.Collections.Generic;
using UnityEngine;

public class UIComputerController : MonoBehaviour
{

    [Header("Computer Screen Controllers")]
    [SerializeField] UISingleWindowController myResourceStoreController;

    [SerializeField] GameObject itemPannel;
    [SerializeField] GameObject gridObj;

    List<UIStoreItemUpdater> myLoadedUIItems = new List<UIStoreItemUpdater>();

    private ResourceManager _resourceManagerInstance;
    bool _isInit = false;
    
    void Start()
    {
        if (_resourceManagerInstance == null) _resourceManagerInstance = ResourceManager.Instance;

        if (_isInit) return;
        else Init();
    }

    void Init()
    {
        List<OnlineShopItem> storeItems;
        _resourceManagerInstance.GetStorePurchasableItems(out storeItems);
        int idx = 0;
        foreach (OnlineShopItem item in storeItems)
        {
            GameObject newItem = GameObject.Instantiate(itemPannel);
            newItem.transform.SetParent(gridObj.transform, false);
            UIStoreItemUpdater uiItem = newItem.GetComponent<UIStoreItemUpdater>();
            uiItem.SetItemData(item);
            uiItem.MyShopIntex = idx;
            uiItem.OnWorldMovementLockedChange.AddListener(OnUIItemClicked);
            myLoadedUIItems.Add(uiItem);

            ++idx;
        }

        _isInit = true;
    }

    private void OnUIItemClicked(UIStoreItemUpdater.ButtonType aButtonType, int anIdx)
    {
        if (anIdx >= myLoadedUIItems.Count) return;

        UIStoreItemUpdater clickedItem = myLoadedUIItems[anIdx];
        if (aButtonType == UIStoreItemUpdater.ButtonType.Buy)
        {
            _resourceManagerInstance.RequestPurchaseFromOnlineStore(clickedItem.GetItemSubtype());
        }
    }

    public void OnExitButtonClicked()
    {
        // cleanup if needed
        GameManager.Instance.GoToRegularGameplay();
    }

    public void OnBackButtonClicked()
    {
        // when we have multiple windows in the computer
    }
}
