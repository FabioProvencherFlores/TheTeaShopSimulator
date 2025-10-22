using UnityEngine;

[CreateAssetMenu(fileName = "AIGoalList", menuName = "Scriptable Objects/AIGoalList")]
public class NPCArchetype : ScriptableObject
{
    public bool defaultsToLeave = true;
    public float graceWaitPeriodSeconds = 9999f;
    public ItemSubtypesUID[] desiredItemPull = null;

    public AI_Base[] myGoalChain = null;

    public ItemSubtypesUID GetRandomDesiredType () { return desiredItemPull[Random.Range(0, desiredItemPull.Length)]; }
}
