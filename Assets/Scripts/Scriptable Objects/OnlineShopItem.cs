using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Online Shop Item", menuName = "Items/Online shop")]
public class OnlineShopItem : ScriptableObject
{
    public string myItemName;
    public ItemTypeUID myItemTypeUID;
    public ItemSubtypesUID mySubtypesUID;
    public float myShopPrice;
    public string myDescription;
    public GameObject myDeliverablePrefab;
}
