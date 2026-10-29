using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class CraneResultDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text nextGameText;

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

    // 景品取得成功を表示する。
    public void ShowSuccess()
    {
        if (resultText == null) return;

        resultText.rectTransform.anchoredPosition = initialTextPosition;
        resultText.text = "PRIZE GET!";
        resultText.gameObject.SetActive(true);

        if (nextGameText != null)
        {
            nextGameText.gameObject.SetActive(false);
        }
    }

    // 景品を獲得できなかった場合の表示。
    public void ShowFailure()
    {
        if (resultText == null) return;

        resultText.rectTransform.anchoredPosition = initialTextPosition;
        resultText.text = "TRY AGAIN!";
        resultText.gameObject.SetActive(true);

        if (nextGameText != null)
        {
            nextGameText.gameObject.SetActive(false);
        }
    }

    // 景品を中央表示するとき、結果文字を上へ移動する。
    public void MoveForPrizeDisplay()
    {
        if (resultText == null) return;

        resultText.rectTransform.anchoredPosition = prizeDisplayTextPosition;
    }

    // 次のゲームへの案内を表示する。
    public void ShowNextGamePrompt()
    {
        if (nextGameText == null) return;

        nextGameText.text = "PRESS SPACE TO PLAY AGAIN";
        nextGameText.gameObject.SetActive(true);
    }

    // 結果画面をすべて消す。
    public void HideAll()
    {
        if (resultText != null)
        {
            resultText.gameObject.SetActive(false);
        }

        if (nextGameText != null)
        {
            nextGameText.gameObject.SetActive(false);
        }
    }

    // 以前のCraneUnit.csとの互換用。
    public void Hide()
    {
        HideAll();
    }
}