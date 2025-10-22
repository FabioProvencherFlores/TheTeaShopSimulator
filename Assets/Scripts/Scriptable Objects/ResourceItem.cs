using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Item", menuName = "Items/Resource")]
public class ResourceItem : ScriptableObject
{
	public string myItemName;
	public ItemSubtypesUID mySubtypesUID;
	public float myMaxValue = float.MaxValue;
	public bool myAllowPartialAmount;
}