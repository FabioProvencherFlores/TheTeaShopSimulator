using System.Collections.Generic;
using UnityEngine;
using static UIStoreItemUpdater;

public class UIComputerController : MonoBehaviour
{

    [Header("Computer Window Updaters")]
    [SerializeField] UIHomePageUpdater homePageUpdater;
    [SerializeField] UIResourceStoreUpdater resourceStoreUpdater;

    [Header("Data")]
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
        GoToHomeScreen();
    }

    public void RequestResourceStoreAction(UIStoreItemUpdater.ButtonType anAction, ItemSubtypesUID aResourceSubtype)
    {
        if (anAction == UIStoreItemUpdater.ButtonType.Buy)
        {
            _resourceManagerInstance.RequestPurchaseFromOnlineStore(aResourceSubtype);
        }
    }

    private void GoToHomeScreen()
    {
        homePageUpdater.gameObject.SetActive(true);
        resourceStoreUpdater.gameObject.SetActive(false);

        homePageUpdater.Init(this);
    }

    private void GoToResourceStore()
    {
        homePageUpdater.gameObject.SetActive(false);
        resourceStoreUpdater.gameObject.SetActive(true);

        resourceStoreUpdater.Init(this, _resourceManagerInstance);
    }


    public void RequestGoToResourceStore()
    {
        GoToResourceStore();
    }

    public void OnExitButtonClicked()
    {
        // cleanup if needed
        myLoadedUIItems.Clear();
        _isInit = false;

        GameManager.Instance.GoToRegularGameplay();
    }

    public void RequestBackToHomeScreen()
    {
        GoToHomeScreen();
    }
}
