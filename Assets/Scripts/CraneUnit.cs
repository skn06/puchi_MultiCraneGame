using UnityEngine;

[DisallowMultipleComponent]
public class CraneUnit : MonoBehaviour
{
    [Header("Movement Targets")]
    [Tooltip("前後移動させるクレーン本体のTransform")]
    [SerializeField] private Transform mainCrane;

    [Tooltip("左右移動させる台車のTransform")]
    [SerializeField] private Transform mainLifMag;

    [Tooltip("上下移動させるLifMag(Center)のTransform")]
    [SerializeField] private Transform centerLifMag;

    [Tooltip("爪の開閉を制御するCraneClaw")]
    [SerializeField] private CraneClaw claw;

    [Header("Speed [m/min]")]
    [SerializeField, Min(0f)] private float forwardBackSpeed = 15f;
    [SerializeField, Min(0f)] private float leftRightSpeed = 15f;
    [SerializeField, Min(0f)] private float upDownSpeed = 15f;

    [Header("Operation Delays")]
    [Tooltip("降下停止から爪を閉じ始めるまでの待ち時間 [秒]")]
    [SerializeField, Min(0f)] private float beforeCloseDelay = 0.7f;

    [Tooltip("爪が閉じてから上昇を始めるまでの待ち時間 [秒]")]
    [SerializeField, Min(0f)] private float afterCloseDelay = 0.7f;

    [Header("Movement Ranges (local position)")]
    [SerializeField] private float minZ = -20f;
    [SerializeField] private float maxZ = 25f;
    [SerializeField] private float minX = -0.368f;
    [SerializeField] private float maxX = 0.368f;
    [Tooltip("LifMag(Center)のローカルY座標の下限")]
    [SerializeField] private float minY = -4.5546412f;
    [Tooltip("LifMag(Center)のローカルY座標の上限")]
    [SerializeField] private float maxY = -0.3034226f;


    // クレーンの動作状態を管理する。
    // Idle: 通常操作可能
    // Descending: 自動降下中
    // WaitingBeforeClose: 降下停止後の待機
    // Closing: 爪を閉じている途中
    // WaitingBeforeAscending: 爪が閉じた後の待機
    // Ascending: 自動上昇中
    private enum CraneState
    {
        Idle,
        Descending,
        WaitingBeforeClose,
        Closing,
        WaitingBeforeAscending,
        Ascending
    }

    private CraneState state = CraneState.Idle;

    private float stateTimer = 0f;

    // 降下・待機・爪の開閉・上昇など、自動動作中は前後左右の移動を禁止する。
    private bool IsOperationLocked
    {
        get
        {
            return state != CraneState.Idle
                || (claw != null && claw.IsMoving);
        }
    }

    // Tで自動降下する。降下中に再度Tを押すとその場で停止し、
    // 一定時間待機した後に爪を閉じて自動上昇する。
    public void ToggleLift()
    {
        if (centerLifMag == null) return;

        // 爪が開閉途中ならTは受け付けない。
        if (claw != null && claw.IsMoving) return;

        // 待機中にTを押したら降下開始。
        if (state == CraneState.Idle)
        {
            state = CraneState.Descending;
            return;
        }

        // 降下中にTを押したら、その場で停止して爪を閉じる。
        if (state == CraneState.Descending)
        {
            BeginWaitingBeforeClose();
        }
    }

    // Gキーによる爪の手動開閉。
    public void ToggleClaw()
    {
        if (claw == null) return;

        // 自動操作中は手動で爪を操作できない。
        if (state != CraneState.Idle) return;

        // 開閉途中で再度押して反転するのを防ぐ。
        if (claw.IsMoving) return;

        claw.Toggle();
    }

    // 降下 → 待機 → 爪を閉じる → 待機 → 上昇、の自動動作を状態に応じて実行する。
    private void FixedUpdate()
    {
        if (centerLifMag == null) return;

        float step = GetLocalStep(
            centerLifMag,
            Vector3.up,
            upDownSpeed
        );

        Vector3 position = centerLifMag.localPosition;

        switch (state)
        {
            case CraneState.Descending:

                position.y = Mathf.MoveTowards(
                    position.y,
                    minY,
                    step
                );

                centerLifMag.localPosition = position;

                // 最下部まで来たら、少し待ってから爪を閉じる。
                if (Mathf.Approximately(position.y, minY))
                {
                    BeginWaitingBeforeClose();
                }

                break;

            case CraneState.WaitingBeforeClose:

                stateTimer -= Time.fixedDeltaTime;

                if (stateTimer <= 0f)
                {
                    BeginClosing();
                }

                break;

            case CraneState.Closing:

                // 爪が完全に閉じ終わったら、少し待ってから上昇する。
                if (claw == null || !claw.IsMoving)
                {
                    state = CraneState.WaitingBeforeAscending;
                    stateTimer = afterCloseDelay;
                }

                break;

            case CraneState.WaitingBeforeAscending:

                stateTimer -= Time.fixedDeltaTime;

                if (stateTimer <= 0f)
                {
                    state = CraneState.Ascending;
                }

                break;

            case CraneState.Ascending:

                position.y = Mathf.MoveTowards(
                    position.y,
                    maxY,
                    step
                );

                centerLifMag.localPosition = position;

                // 最上部まで戻ったら通常操作に戻る。
                if (Mathf.Approximately(position.y, maxY))
                {
                    state = CraneState.Idle;
                }
            
                break;
        }
    }

    // 降下を停止し、爪を閉じるまでの待機を開始する。
    private void BeginWaitingBeforeClose()
    {
        state = CraneState.WaitingBeforeClose;
        stateTimer = beforeCloseDelay;
    }

    // 待機終了後、現在位置で爪を閉じる処理を開始する。
    private void BeginClosing()
    {
        state = CraneState.Closing;

        if (claw != null)
        {
            claw.Close();
        }
    }

    private void OnDisable()
    {
        state = CraneState.Idle;
        stateTimer = 0f;
    }

    // 通常状態のときだけ前後移動を許可する。
    public void MoveForwardBack(float input)
    {
        if (IsOperationLocked) return;

        Move(
            mainCrane,
            Vector3.forward,
            input,
            forwardBackSpeed,
            minZ,
            maxZ,
            2
        );
    }

    // 通常状態のときだけ左右移動を許可する。
    public void MoveLeftRight(float input)
    {
        if (IsOperationLocked) return;

        Move(
            mainLifMag,
            Vector3.right,
            input,
            leftRightSpeed,
            minX,
            maxX,
            0
        );
    }

    private static void Move(Transform target, Vector3 direction, float input,
        float speedPerMinute, float min, float max, int axis)
    {
        if (target == null || Mathf.Approximately(input, 0f)) return;

        Vector3 position = target.localPosition;
        position += direction * Mathf.Clamp(input, -1f, 1f)
            * GetLocalStep(target, direction, speedPerMinute);
        position[axis] = Mathf.Clamp(position[axis], min, max);
        target.localPosition = position;
    }

    private static float GetLocalStep(Transform target, Vector3 direction, float speedPerMinute)
    {
        // 親の拡大率を補正し、1 Unity unit = 1 mとして実際の速度を指定する。
        float worldUnitsPerLocalUnit = target.parent == null
            ? 1f : target.parent.TransformVector(direction).magnitude;
        if (worldUnitsPerLocalUnit <= Mathf.Epsilon) return 0f;

        return Mathf.Max(0f, speedPerMinute) / 60f
            * Time.fixedDeltaTime / worldUnitsPerLocalUnit;
    }

    private void OnValidate()
    {
        if (minZ > maxZ) minZ = maxZ;
        if (minX > maxX) minX = maxX;
        if (minY > maxY) minY = maxY;

        forwardBackSpeed = Mathf.Max(0f, forwardBackSpeed);
        leftRightSpeed = Mathf.Max(0f, leftRightSpeed);
        upDownSpeed = Mathf.Max(0f, upDownSpeed);
    }
}
