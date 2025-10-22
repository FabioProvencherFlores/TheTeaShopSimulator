using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIStoreItemUpdater : MonoBehaviour
{
    public enum ButtonType
    {
        Buy,
        AddQte,
        SubstractQte
    }

    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text price;
    private ItemSubtypesUID myItemSubtype;

    public UnityEvent<ButtonType, int> OnWorldMovementLockedChange = new UnityEvent<ButtonType, int>();

    public int MyShopIntex { get; set; }

    public void SetItemData(OnlineShopItem shopItem)
    {
        itemName.text = shopItem.myItemName;
        price.text = ((int)shopItem.myShopPrice).ToString();
        myItemSubtype = shopItem.mySubtypesUID;
    }
    public ItemSubtypesUID GetItemSubtype() {  return myItemSubtype; }

    public void OnClickedBuyButton()
    {
        OnWorldMovementLockedChange.Invoke(ButtonType.Buy, MyShopIntex);
    }
}
