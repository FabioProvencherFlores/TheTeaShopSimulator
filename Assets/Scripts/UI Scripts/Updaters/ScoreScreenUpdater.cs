using TMPro;
using UnityEngine;

public class ScoreScreenUpdater : MonoBehaviour
{
    [SerializeField] TMP_Text textPannel;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        EndDayScore score = GameManager.Instance.myDayScore;
        textPannel.text = "Satisfied Customers: " + score.mySatisfiedCustomers.ToString()
            + "\nAngry Customers: " + score.myDisatisfiedCustomers.ToString()
            + "\nTotal Profits: " + score.myTotalProfits.ToString();
    }

    public void OnNextDayClicked()
    {
        GameManager.Instance.RequestGoToNextDay();
    }
}
