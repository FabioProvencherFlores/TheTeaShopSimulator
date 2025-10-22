using UnityEngine;

public class NPCTabletteLookerHelper : MonoBehaviour
{
    [SerializeField] private DropableSubcontainer mySubcontainerInstance;

    private void Start()
    {
        Debug.DrawLine(transform.position, mySubcontainerInstance.transform.position, Color.red, 60f);
    }

    public PickupableObjectInteraction GetInteractableOfType(ItemSubtypesUID anItemSubtype)
    {
        return mySubcontainerInstance.GetPickableItemContainingType(anItemSubtype);
    }

    private void OnValidate()
    {
        if (mySubcontainerInstance != null) Debug.DrawLine(transform.position, mySubcontainerInstance.transform.position, Color.red, 1f);
    }
}
