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
}
