using UnityEngine;


[CreateAssetMenu(fileName = "AIGHasFoundItems", menuName = "AI Data/HasFoundItems")]
public class AIF_HasFoundItems : AIF_BaseFork
{
    public  override bool ResolveCondition(CustomerState aState)
    {
        return aState.HasFoundDesiredItems;
    }
}
