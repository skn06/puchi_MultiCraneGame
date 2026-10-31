using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class CraneResultDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text nextGameText;

    [Header("Text Positions")]

    [Tooltip("PRIZE GET!で景品を中央表示するときの文字位置")]
    [SerializeField] private Vector2 prizeDisplayTextPosition = new Vector2(0f, 300f);

    [Tooltip("PRESS SPACE TO PLAY AGAIN の文字位置")]
    [SerializeField] private Vector2 nextGameTextPosition = new Vector2(0f, -300f);

    private Vector2 initialResultTextPosition;

    private void Awake()
    {
        // TRY AGAIN!や、景品表示前のPRIZE GET!に使う初期位置を保存する。
        if (resultText != null)
        {
            initialResultTextPosition = resultText.rectTransform.anchoredPosition;
        }
    }

    // 景品取得成功を表示する。
    public void ShowSuccess()
    {
        if (resultText == null) return;

        resultText.rectTransform.anchoredPosition = initialResultTextPosition;
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

        resultText.rectTransform.anchoredPosition = initialResultTextPosition;
        resultText.text = "TRY AGAIN!";
        resultText.gameObject.SetActive(true);

        if (nextGameText != null)
        {
            nextGameText.gameObject.SetActive(false);
        }
    }

    // 景品を中央表示するとき、PRIZE GET!を指定位置へ移動する。
    public void MoveForPrizeDisplay()
    {
        if (resultText == null) return;

        resultText.rectTransform.anchoredPosition = prizeDisplayTextPosition;
    }

    // 次のゲームへの案内を指定位置に表示する。
    public void ShowNextGamePrompt(KeyCode nextGameKey = KeyCode.Space)
    {
        if (nextGameText == null) return;

        nextGameText.rectTransform.anchoredPosition = nextGameTextPosition;
        string controlLabel = nextGameKey.ToString()
            .Replace("Joystick", "JOY ")
            .Replace("Button", " BUTTON ")
            .ToUpperInvariant();
        nextGameText.text = $"PRESS {controlLabel} TO PLAY AGAIN";
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