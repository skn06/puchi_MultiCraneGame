using UnityEngine;
using TMPro;

public class CraneTimerDisplay : MonoBehaviour
{
    [SerializeField] private CraneUnit crane;
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        if (crane == null || timerText == null) return;

        int seconds = Mathf.CeilToInt(crane.RemainingPlayTime);

        timerText.text = "TIME " + seconds;
    }
}
