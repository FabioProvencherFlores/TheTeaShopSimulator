using UnityEngine;

public class StoreVolumeUpdater : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        NPCController npc = other.GetComponent<NPCController>();
        if (npc != null)
        {
            npc.SetIsInsideStore(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NPCController npc = other.GetComponent<NPCController>();
        if (npc != null)
        {
            npc.SetIsInsideStore(false);
        }
    }
}
