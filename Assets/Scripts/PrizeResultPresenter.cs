using UnityEngine;

[DisallowMultipleComponent]
public class PrizeResultPresenter : MonoBehaviour
{
    [Header("Display Settings")]
    [Tooltip("結果表示時の景品の大きさ")]
    [SerializeField, Min(0.01f)] private float displayScale = 1f;

    [Tooltip("結果表示時の景品の向き")]
    [SerializeField] private Vector3 displayRotation = Vector3.zero;

    [Header("Result Camera")]
    [Tooltip("結果表示専用の景品Layer")]
    [SerializeField] private string resultLayerName = "ResultPrize";

    // 結果表示前のLayerを保存する。
    private Transform[] displayedPrizeTransforms;
    private int[] originalLayers;

    // 取得した景品を、このオブジェクトの位置に固定して表示する。
    public void ShowPrize(Rigidbody prize)
    {
        if (prize == null) return;

        Transform prizeTransform = prize.transform;

        int resultLayer = LayerMask.NameToLayer(resultLayerName);

        if (resultLayer == -1)
        {
            Debug.LogError("ResultPrize Layerが見つかりません。");
            return;
        }

        // 景品本体とすべての子の元Layerを保存する。
        displayedPrizeTransforms =
            prizeTransform.GetComponentsInChildren<Transform>(true);

        originalLayers =
            new int[displayedPrizeTransforms.Length];

        for (int i = 0; i < displayedPrizeTransforms.Length; i++)
        {
            originalLayers[i] =
                displayedPrizeTransforms[i].gameObject.layer;

            displayedPrizeTransforms[i].gameObject.layer =
                resultLayer;
        }

        // 現在の見た目の大きさを保存する。
        Vector3 originalWorldScale = prizeTransform.lossyScale;

        // 結果表示中は物理演算を止める。
        prize.velocity = Vector3.zero;
        prize.angularVelocity = Vector3.zero;
        prize.useGravity = false;
        prize.isKinematic = true;
        prize.detectCollisions = false;

        // 結果表示用の位置へ移動する。
        prizeTransform.SetParent(transform, false);
        prizeTransform.localPosition = Vector3.zero;
        prizeTransform.localRotation =
            Quaternion.Euler(displayRotation);

        prizeTransform.localScale =
            originalWorldScale * displayScale;
    }

    // 次ゲーム開始時にLayerを元へ戻す。
    public void RestorePrizeLayer()
    {
        if (displayedPrizeTransforms == null ||
            originalLayers == null)
        {
            return;
        }

        int count = Mathf.Min(
            displayedPrizeTransforms.Length,
            originalLayers.Length
        );

        for (int i = 0; i < count; i++)
        {
            if (displayedPrizeTransforms[i] != null)
            {
                displayedPrizeTransforms[i].gameObject.layer =
                    originalLayers[i];
            }
        }

        displayedPrizeTransforms = null;
        originalLayers = null;
    }
}