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

    [Header("Speed [m/min]")]
    [SerializeField, Min(0f)] private float forwardBackSpeed = 15f;
    [SerializeField, Min(0f)] private float leftRightSpeed = 15f;
    [SerializeField, Min(0f)] private float upDownSpeed = 15f;

    [Header("Movement Ranges (local position)")]
    [SerializeField] private float minZ = -20f;
    [SerializeField] private float maxZ = 25f;
    [SerializeField] private float minX = -0.368f;
    [SerializeField] private float maxX = 0.368f;

    [Header("Lift Limits (local Y)")]
    [Tooltip("ON: 再生開始時のLifMag(Center)の位置を上限にする。OFF: Max Yを上限にする")]
    [SerializeField] private bool useInitialPositionAsUpperLimit = true;

    [Tooltip("LifMag(Center)のローカルY座標の下限。上限より高い値の場合は上限位置までに制限する")]
    [SerializeField] private float minY = -4.5546412f;
    [Tooltip("Use Initial Position As Upper LimitがOFFの場合に使うローカルY座標の上限")]
    [SerializeField] private float maxY = -0.3034226f;

    private float initialLiftY;
    private bool liftMoving;
    private bool movingToBottom;

    private void Awake()
    {
        if (centerLifMag != null) initialLiftY = centerLifMag.localPosition.y;
    }

    // 最初の押下で下降、次の押下で上昇。移動中の押下では反転する。
    public void ToggleLift()
    {
        if (centerLifMag == null) return;
        movingToBottom = !movingToBottom;
        liftMoving = true;
    }

    private void FixedUpdate()
    {
        if (!liftMoving || centerLifMag == null) return;

        float upperY = useInitialPositionAsUpperLimit ? initialLiftY : maxY;
        float lowerY = Mathf.Min(minY, upperY);
        float targetY = movingToBottom ? lowerY : upperY;
        Vector3 position = centerLifMag.localPosition;
        float step = GetLocalStep(centerLifMag, Vector3.up, upDownSpeed);
        position.y = Mathf.MoveTowards(position.y, targetY, step);
        centerLifMag.localPosition = position;

        // MoveTowardsで端点に正確に止める。
        if (position.y == targetY) liftMoving = false;
    }

    private void OnDisable()
    {
        liftMoving = false;
    }

    public void MoveForwardBack(float input)
    {
        Move(mainCrane, Vector3.forward, input, forwardBackSpeed, minZ, maxZ, 2);
    }

    public void MoveLeftRight(float input)
    {
        Move(mainLifMag, Vector3.right, input, leftRightSpeed, minX, maxX, 0);
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
        if (!useInitialPositionAsUpperLimit && minY > maxY) minY = maxY;

        forwardBackSpeed = Mathf.Max(0f, forwardBackSpeed);
        leftRightSpeed = Mathf.Max(0f, leftRightSpeed);
        upDownSpeed = Mathf.Max(0f, upDownSpeed);
    }
}
