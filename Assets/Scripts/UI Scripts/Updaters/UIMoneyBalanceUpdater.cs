using TMPro;
using UnityEngine;

public class UIMoneyBalanceUpdater : MonoBehaviour
{
    TMP_Text myUIText;
    private void Awake()
    {
        myUIText = GetComponent<TMP_Text>();
        myUIText.text = "0";
    }

    void Start()
    {
        ResourceManager.Instance.RegisterToPlayerMoneyChange(UpdateBalanceUI);
    }

    private void OnDisable()
    {
        ResourceManager.Instance.DeregisterFromPlayerMoneyChange(UpdateBalanceUI);
    }

    private void UpdateBalanceUI(float newAmount)
    {
        myUIText.text = newAmount.ToString();
    }
}
