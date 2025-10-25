using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Supplier Data", menuName = "Supplier/Supplier Data")]
public class SupplierData : ScriptableObject
{
    public SupplierAvailability availabilityAtStart;
    public ResourceBundleUnlock[] resourceBundles;
}
