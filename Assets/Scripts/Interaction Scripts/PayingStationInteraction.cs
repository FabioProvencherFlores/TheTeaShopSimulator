using System;
using Unity.VisualScripting;
using UnityEngine;

public class PayingStationInteraction : ObjectInteractionBase
{
    [DoNotSerialize] public NPCController WaitingNPC { get; set; }

    public override InteractionUID GetInteractionUID()
    {
        return InteractionUID.CashRegistery;
    }

    public override bool CanInteract()
    {
        return WaitingNPC != null;
    }

    public override bool Interact(PlayerController player)
    {

        if (InteractionManager.Instance.RequestCashRegisteryInteraction(this))
        {
            return true;
        }
        
        return false;
    }

    private protected override Color GetOutlineColor()
    {
        return Color.yellow;
    }
}
