using UnityEngine;

public class ComplexInteraction : MonoBehaviour
{
	[SerializeField] InteractionUID[] myInteractionUIDs = { InteractionUID.INVALID };

	public InteractionUID[] GetInteractionTypes() { return myInteractionUIDs; }
	public InteractionUID GetMatchingComplexInteraction(InteractionUID[] anotherComplexInteractions)
	{
		foreach (InteractionUID otherInteraction in anotherComplexInteractions)
		{
			foreach (InteractionUID interaction in myInteractionUIDs)
			{
				if (interaction == otherInteraction) return interaction;
			}
		}

		return InteractionUID.INVALID;
	}

	public virtual bool CanInteract()
	{
        foreach (InteractionUID interaction in myInteractionUIDs)
        {
            if (interaction == InteractionUID.TransferResource)
			{
				ResourceContainerObject container = GetComponent<ResourceContainerObject>();
				if (container != null)
				{
					// this is only allowed if the complex (the giver) container HAS any resource in it
					return container.GetCurrentQuantity() > 0f;
				}
			}
        }

        return false;
	}
}
