using System.Collections.Generic;
using UnityEngine;
using static UIItem_ResourceUpdater;

public class UIComputerController : MonoBehaviour
{

    [Header("Computer Window Updaters")]
    [SerializeField] UIHomePageUpdater homePageUpdater;
    [SerializeField] UIResourceStoreUpdater resourceStoreUpdater;
    [SerializeField] UISupplyStoreUpdater supplyStoreUpdater;

    [Header("Data")]
    [SerializeField] GameObject itemPannel;
    [SerializeField] GameObject gridObj;

    List<UIItem_ResourceUpdater> myLoadedUIItems = new List<UIItem_ResourceUpdater>();

    private ResourceManager _resourceManagerInstance;
    private SupplierManager _supplierManagerInstance;
    bool _isInit = false;
    
    void Start()
    {
        if (_resourceManagerInstance == null) _resourceManagerInstance = ResourceManager.Instance;
        if (_supplierManagerInstance == null) _supplierManagerInstance = SupplierManager.Instance;

        if (_isInit) return;
        else Init();
    }

    void Init()
    {
        GoToHomeScreen();
    }

    public void RequestResourceStoreAction(UIItem_ResourceUpdater.ButtonType anAction, ItemSubtypesUID aResourceSubtype)
    {
        if (anAction == UIItem_ResourceUpdater.ButtonType.Buy)
        {
            _resourceManagerInstance.RequestPurchaseFromOnlineStore(aResourceSubtype);
        }
    }

    private void GoToHomeScreen()
    {
        homePageUpdater.gameObject.SetActive(true);
        resourceStoreUpdater.gameObject.SetActive(false);
        supplyStoreUpdater.gameObject.SetActive(false);

        homePageUpdater.Init(this);
    }

    private void GoToResourceStore()
    {
        homePageUpdater.gameObject.SetActive(false);
        resourceStoreUpdater.gameObject.SetActive(true);
        supplyStoreUpdater.gameObject.SetActive(false);

        resourceStoreUpdater.Init(this, _resourceManagerInstance);
    }

    private void GoToSupplyStore()
    {
        homePageUpdater.gameObject.SetActive(false);
        resourceStoreUpdater.gameObject.SetActive(false);
        supplyStoreUpdater.gameObject.SetActive(true);

        supplyStoreUpdater.Init(this, _supplierManagerInstance);
    }

    public void RequestUnlockBundle(int anID)
    {
        _supplierManagerInstance.UnlockBundleFromID(anID);
    }


    public void OnExitButtonClicked()
    {
        RequestCloseComputer();
	}

    public void RequestCloseComputer()
    {
		// cleanup if needed
		myLoadedUIItems.Clear();
		_isInit = false;

		GameManager.Instance.GoToRegularGameplay();
	}

    public void RequestGoToResourceStore()
    {
        GoToResourceStore();
    }

    public void RequestBackToHomeScreen()
    {
        GoToHomeScreen();
    }

    public void RequestGoToSupplyStore()
    {
        GoToSupplyStore();
    }
}
