using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class PrizeExitDetector : MonoBehaviour
{
    [SerializeField] private CraneUnit crane;

    [Header("Detection Delay")]
    [Tooltip("景品が取出口に入ってから取得表示を出すまでの待ち時間 [秒]")]
    [SerializeField, Min(0f)] private float detectionDelay = 1f;

    private bool prizeDetected = false;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody prize = other.attachedRigidbody;

        // Prizeタグを持つ景品だけを取得対象にする。
        if (prize == null || !prize.CompareTag("Prize")) return;

        // 同じ景品で何度も取得処理が始まるのを防ぐ。
        if (prizeDetected) return;

        prizeDetected = true;

        StartCoroutine(NotifyPrizeAfterDelay(prize));
    }

    // 景品が取出口に入ってから少し待って、取得成功として通知する。
    private IEnumerator NotifyPrizeAfterDelay(Rigidbody prize)
    {
        yield return new WaitForSeconds(detectionDelay);

        if (crane != null && prize != null)
        {
            crane.NotifyPrizeDetected(prize);
        }
    }

    // 次のゲームで再び景品を判定できる状態に戻す。
    public void ResetDetection()
    {
        StopAllCoroutines();
        prizeDetected = false;
    }
}