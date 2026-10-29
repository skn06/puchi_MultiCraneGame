using UnityEngine;

[DisallowMultipleComponent]
public class CraneClaw : MonoBehaviour
{
    [Header("Claw Rotates")]
    [SerializeField] private Transform claw1Rotate;
    [SerializeField] private Transform claw2Rotate;
    [SerializeField] private Transform claw3Rotate;

    [Header("Prize Contact Sensors")]
    [SerializeField] private ClawContactSensor claw1Sensor;
    [SerializeField] private ClawContactSensor claw2Sensor;
    [SerializeField] private ClawContactSensor claw3Sensor;

    [Header("Open Angles")]
    [SerializeField] private float claw1OpenY = 0f;
    [SerializeField] private float claw2OpenY = 0f;
    [SerializeField] private float claw3OpenY = 0f;

    [Header("Closed Angles")]
    [SerializeField] private float claw1ClosedY = 25f;
    [SerializeField] private float claw2ClosedY = 25f;
    [SerializeField] private float claw3ClosedY = 25f;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 90f;

    
    private bool isClosed = false;

    // 景品に接触した爪は、そのプレイ中はそれ以上閉じない。
    private bool claw1StoppedByPrize = false;
    private bool claw2StoppedByPrize = false;
    private bool claw3StoppedByPrize = false;

    private void Update()
    {
        float target1 = isClosed ? claw1ClosedY : claw1OpenY;
        float target2 = isClosed ? claw2ClosedY : claw2OpenY;
        float target3 = isClosed ? claw3ClosedY : claw3OpenY;

        // 閉じている途中で景品に触れた爪は、その位置で停止させる。
        if (isClosed)
        {
            if (!claw1StoppedByPrize &&
                claw1Sensor != null &&
                claw1Sensor.IsTouchingPrize)
            {
                claw1StoppedByPrize = true;
            }
        

            if (!claw2StoppedByPrize &&
                claw2Sensor != null &&
                claw2Sensor.IsTouchingPrize)
            {
                claw2StoppedByPrize = true;
            }

            if (!claw3StoppedByPrize &&
                claw3Sensor != null &&
                claw3Sensor.IsTouchingPrize)
            {
                claw3StoppedByPrize = true;
            }
        }

        // 景品に当たった爪以外は通常どおり目標角度まで動かす。
        if (!claw1StoppedByPrize)
        {
            RotateTo(claw1Rotate, target1);
        }

        if (!claw2StoppedByPrize)
        {
            RotateTo(claw2Rotate, target2);
        }

        if (!claw3StoppedByPrize)
        {
            RotateTo(claw3Rotate, target3);
        }
    }

    private void RotateTo(Transform target, float targetY)
    {
        if (target == null) return;

        Vector3 angles = target.localEulerAngles;

        float newY = Mathf.MoveTowardsAngle(
            angles.y,
            targetY,
            rotationSpeed * Time.deltaTime
        );

        target.localRotation = Quaternion.Euler(0f, newY, 0f);
    }

    public bool IsMoving
    
    {
        get
        {
            float target1 = isClosed ? claw1ClosedY : claw1OpenY;
            float target2 = isClosed ? claw2ClosedY : claw2OpenY;
            float target3 = isClosed ? claw3ClosedY : claw3OpenY;

            bool claw1Moving =
                !claw1StoppedByPrize && !IsAtTarget(claw1Rotate, target1);

            bool claw2Moving =
                !claw2StoppedByPrize && !IsAtTarget(claw2Rotate, target2);

            bool claw3Moving =
                !claw3StoppedByPrize && !IsAtTarget(claw3Rotate, target3);

            return claw1Moving || claw2Moving || claw3Moving;
        }
    }

    private bool IsAtTarget(Transform target, float targetY)
    {
        if (target == null) return true;

        return Mathf.Abs(
            Mathf.DeltaAngle(target.localEulerAngles.y, targetY)
        ) < 0.1f;
    }

    public void Toggle()
    {
        if (isClosed)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    public void Open()
    {
        isClosed = false;

        // 次に閉じるときのため、景品による停止状態を解除する。
        ResetPrizeStops();
    }

    public void Close()
    {
        // 新しい閉じ動作を始めるため、前回の接触状態をリセットする。
        ResetPrizeStops();

        isClosed = true;
    }

    private void ResetPrizeStops()
    {
        claw1StoppedByPrize = false;
        claw2StoppedByPrize = false;
        claw3StoppedByPrize = false;
    }
}