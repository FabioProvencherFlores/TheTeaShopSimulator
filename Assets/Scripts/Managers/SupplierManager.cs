using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum SupplierAvailability
{
    INVALID,
    AVAILABLE,
    PENDING_CONDITION
}

public class SupplierManager : MonoBehaviour
{
    #region Instance
    private static SupplierManager _instance;
    public static SupplierManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("No Supplier Manager is found");

            return _instance;
        }
    }
    #endregion

    SupplierData[] allSuppliersData;
    Dictionary<int, ResourceBundleUnlock> myBundleDescriptors = new Dictionary<int, ResourceBundleUnlock>();

    [HideInInspector] public UnityEvent<ResourceBundleUnlock> OnBundleUnlock = new UnityEvent<ResourceBundleUnlock>();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        allSuppliersData = Resources.LoadAll<SupplierData>("Progression/Suppliers/");
        int currentId = 0;
        foreach (SupplierData supplierData in allSuppliersData)
        {
            if (supplierData.availabilityAtStart == SupplierAvailability.AVAILABLE)
            {
                foreach (ResourceBundleUnlock bundle in supplierData.resourceBundles)
                {
                    bundle.InternalID = currentId;
                    myBundleDescriptors.Add(currentId, bundle);
                    currentId++;
                }
            }
        }
    }

    public void UnlockBundleFromID(int anID)
    {
        ResourceBundleUnlock bundle = null;
        if (myBundleDescriptors.TryGetValue(anID, out bundle)) OnBundleUnlock.Invoke(bundle);
    }

    public void GetBundlesAvailableForUnlock(out List<ResourceBundleUnlock> aListOfItems)
    {
        aListOfItems = new List<ResourceBundleUnlock>();
        foreach (var item in myBundleDescriptors)
        {
            aListOfItems.Add(item.Value);
        }
    }
}