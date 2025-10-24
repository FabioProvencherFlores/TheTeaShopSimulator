using TMPro;
using UnityEngine;

public class UIHomePageUpdater : MonoBehaviour
{
    [Header("Page Content")]
    [SerializeField] TMP_Text timeText;

    GameManager gameManagerInstance;
    TimeOfDayHoursMinutes currentDisplayedTime;

    UIComputerController myController;

    private void Start()
    {
        gameManagerInstance = GameManager.Instance;
    }

    private void Update()
    {
        currentDisplayedTime.SetTime(gameManagerInstance.TimeOfDayInMinutes);
        timeText.text = currentDisplayedTime.ToString();
    }

    public void Init(UIComputerController aController)
    {
        myController = aController;
    }

    public void OnShopIconClicked()
    {
        Debug.Log("CLICKED");
        if (myController == null) return;

        myController.RequestGoToResourceStore();
    }
}
