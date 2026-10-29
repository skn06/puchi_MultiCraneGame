using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class CraneResultDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    [Header("Prize Display Position")]
    [Tooltip("景品を中央表示するときの結果文字の位置")]
    [SerializeField] private Vector2 prizeDisplayTextPosition = new Vector2(0f, 300f);

    private Vector2 initialTextPosition;

    private void Awake()
    {
        if (resultText != null)
        {
            initialTextPosition = resultText.rectTransform.anchoredPosition;
        }
    }

    // 最初は通常位置に取得メッセージを表示する。
    public void ShowSuccess()
    {
        if (resultText == null) return;

        resultText.rectTransform.anchoredPosition = initialTextPosition;
        resultText.text = "PRIZE GET!";
        resultText.gameObject.SetActive(true);
    }

    // 景品を中央表示するとき、文字を上側へ移動する。
    public void MoveForPrizeDisplay()
    {
        if (resultText == null) return;

        resultText.rectTransform.anchoredPosition = prizeDisplayTextPosition;
    }

    public void Hide()
    {
        if (resultText == null) return;

        resultText.gameObject.SetActive(false);
    }
}