using UnityEngine;

[DisallowMultipleComponent]
public class PrizeResultPresenter : MonoBehaviour
{
    [Header("Display Settings")]
    [Tooltip("結果表示時の景品の大きさ")]
    [SerializeField, Min(0.01f)] private float displayScale = 1f;

    [Tooltip("結果表示時の景品の向き")]
    [SerializeField] private Vector3 displayRotation = Vector3.zero;

    // 取得した景品を、このオブジェクトの位置に固定して表示する。
    public void ShowPrize(Rigidbody prize)
    {
        if (prize == null) return;

        Transform prizeTransform = prize.transform;

        // 現在の見た目の大きさを保存する。
        Vector3 originalWorldScale = prizeTransform.lossyScale;

        // 結果表示中は物理演算を止める。
        prize.velocity = Vector3.zero;
        prize.angularVelocity = Vector3.zero;
        prize.useGravity = false;
        prize.isKinematic = true;
        prize.detectCollisions = false;

        // カメラ中央の表示位置へ移動する。
        prizeTransform.SetParent(transform, false);
        prizeTransform.localPosition = Vector3.zero;
        prizeTransform.localRotation = Quaternion.Euler(displayRotation);
        prizeTransform.localScale = originalWorldScale * displayScale;
    }
}