using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIItem_SupplyBundleUpdater : MonoBehaviour
{
    public enum ButtonType
    {
        Buy,
        AddQte,
        SubstractQte
    }

    [SerializeField] TMP_Text bundleName;
    [SerializeField] TMP_Text price;

    public UnityEvent<int> OnSupplyDropUnlocked = new UnityEvent<int>();

    public int MyShopIntex { get; set; }

    public void SetItemData(ResourceBundleUnlock bundleInfo)
    {
        bundleName.text = bundleInfo.shopEntryName;
        price.text = bundleInfo.priceToUnlock.ToString();
    }

    public void OnClickedBuyButton()
    {
        OnSupplyDropUnlocked.Invoke(MyShopIntex);
    }
}
