using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource Bundle Unlock", menuName = "Supplier/Resource Bundle Unlock")]
public class ResourceBundleUnlock : ScriptableObject
{
    public string shopEntryName;
    public int priceToUnlock;
    public ItemSubtypesUID[] itemsSubtypesInBundle;
    [HideInInspector] public int InternalID;
}
