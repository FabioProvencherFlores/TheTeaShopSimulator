using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "AIGoalList", menuName = "Scriptable Objects/AIGoalList")]
public class NPCArchetype : ScriptableObject
{
    public bool defaultsToLeave = true;
    public float graceWaitPeriodSeconds = 9999f;
    public int desiredItemAmount = 1;
    public ItemTypeUID[] requiredItemsToSellable;

    public AI_Base[] myGoalChain = null;
    public bool HasRequiredItemsToSpawn(List<ItemTypeUID> availableItemTypes)
    {
        foreach (ItemTypeUID type in requiredItemsToSellable)
        {
            if (!availableItemTypes.Contains(type)) return false;
        }

        return true;
    }
}
