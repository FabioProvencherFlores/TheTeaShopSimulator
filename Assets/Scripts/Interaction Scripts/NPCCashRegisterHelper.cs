using UnityEngine;

public class NPCCashRegisterHelper : MonoBehaviour
{
    [SerializeField] PayingStationInteraction myPayingStation;

    public PayingStationInteraction GetPayingStation() { return myPayingStation; }

    private void Awake()
    {
        if (myPayingStation == null) Debug.Log("Helper was not assigned an object", this);
    }


}
