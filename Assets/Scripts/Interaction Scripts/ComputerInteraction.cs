using UnityEngine;

public class ComputerInteraction : ObjectInteractionBase
{
    public override bool CanInteract()
    {
        if (InteractionManager.Instance.IsPlayerCarryingObject()) return false;
        if (InteractionManager.Instance.ComplexInteractionMemory != null) return false;

        return true;
    }

    public override bool Interact(PlayerController player)
    {
        GameManager.Instance.GoToComputerInteractionGameplay();
        return true;
    }


    public override InteractionUID GetInteractionUID()
    {
        return InteractionUID.OpenComputer;
    }

    private protected override Color GetOutlineColor()
    {
        return Color.black;
    }
}
