using TMPro;
using UnityEngine;

public class LoseScreenUpdater : MonoBehaviour
{
    TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = "Reason: " + GameManager.Instance.GetLoseReason() + "\n Score: " + Time.time.ToString();
    }

}
