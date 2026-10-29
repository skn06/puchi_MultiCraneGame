using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class CraneResultDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    // 景品取得成功のメッセージを表示する。
    public void ShowSuccess()
    {
        if (resultText == null) return;

        resultText.text = "PRIZE GET!";
        resultText.gameObject.SetActive(true);
    }

    // 結果表示を非表示にする。
    public void Hide()
    {
        if (resultText == null) return;

        resultText.gameObject.SetActive(false);
    }
}