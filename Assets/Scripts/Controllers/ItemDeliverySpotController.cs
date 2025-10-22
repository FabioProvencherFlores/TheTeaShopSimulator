using UnityEngine;
using UnityEngine.Events;

public class ItemDeliverySpotController : MonoBehaviour
{
    private bool[] myAvailableSpots = new bool[4];


    private void Awake()
    {
        myAvailableSpots = new bool[8] { true, true, true, true, true, true, true, true };
    }


    public PickupableObjectInteraction DelivertInNextAvailableSlot(GameObject anObjectTopDeliver)
    {
        for (int i = 0; i < myAvailableSpots.Length; ++i)
        {
            if (myAvailableSpots[i])
            {
                GameObject newDelivery = Instantiate(anObjectTopDeliver);
                newDelivery.transform.position = transform.position + transform.forward * i;
                PickupableObjectInteraction pickupableObj = newDelivery.GetComponent<PickupableObjectInteraction>();
                if (pickupableObj != null)
                {
                    pickupableObj.OnInteracted.AddListener(OnObjectWasPickupedUp);
                    pickupableObj.myDeliverySpotIdx = i;
                }
                myAvailableSpots[i] = false;
                return pickupableObj; 
            }
        }
        return null;
    }

    private void OnObjectWasPickupedUp(ObjectInteractionBase anInteractable)
    {
        if (anInteractable.myDeliverySpotIdx < 0 || anInteractable.myDeliverySpotIdx > 3) return;
        myAvailableSpots[anInteractable.myDeliverySpotIdx] = true;
        anInteractable.OnInteracted.RemoveListener(OnObjectWasPickupedUp);
    }
}
