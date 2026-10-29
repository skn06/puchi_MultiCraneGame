using UnityEngine;

[DisallowMultipleComponent]
public class PrizeGrabber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform holdPoint;

    [SerializeField] private ClawContactSensor claw1Sensor;
    [SerializeField] private ClawContactSensor claw2Sensor;
    [SerializeField] private ClawContactSensor claw3Sensor;

    private Rigidbody heldPrize;

    private Vector3 heldPositionOffset;
    private Quaternion heldRotationOffset;

    public bool HasPrize => heldPrize != null;

    private void FixedUpdate()
    {
        if (heldPrize == null || holdPoint == null) return;

        // 掴んだときの位置関係を保ったまま、景品を爪に追従させる。
        Vector3 targetPosition =
            holdPoint.position
            + holdPoint.rotation * heldPositionOffset;

        Quaternion targetRotation =
            holdPoint.rotation * heldRotationOffset;

        heldPrize.MovePosition(targetPosition);
        heldPrize.MoveRotation(targetRotation);
    }

    // 2本以上の爪が同じ景品に触れていれば、その景品を保持する。
    public void TryGrab()
    {
        if (heldPrize != null || holdPoint == null) return;

        Rigidbody prize = FindGrabbablePrize();

        if (prize == null) return;

        heldPrize = prize;

        heldPositionOffset =
            Quaternion.Inverse(holdPoint.rotation)
            * (heldPrize.position - holdPoint.position);

        heldRotationOffset =
            Quaternion.Inverse(holdPoint.rotation)
            * heldPrize.rotation;

        heldPrize.velocity = Vector3.zero;
        heldPrize.angularVelocity = Vector3.zero;

        heldPrize.useGravity = false;
        heldPrize.isKinematic = true;
    }

    // 同じ景品に2本以上の爪が接触しているか確認する。
    private Rigidbody FindGrabbablePrize()
    {
        Rigidbody prize = claw1Sensor != null
            ? claw1Sensor.AnyTouchingPrize
            : null;

        if (prize != null)
        {
            if ((claw2Sensor != null && claw2Sensor.IsTouching(prize))
                || (claw3Sensor != null && claw3Sensor.IsTouching(prize)))
            {
                return prize;
            }
        }

        prize = claw2Sensor != null
            ? claw2Sensor.AnyTouchingPrize
            : null;

        if (prize != null
            && claw3Sensor != null
            && claw3Sensor.IsTouching(prize))
        {
            return prize;
        }

        return null;
    }

    // 取出口で景品を離すための処理。
    public void Release()
    {
        if (heldPrize == null) return;

        heldPrize.isKinematic = false;
        heldPrize.useGravity = true;

        heldPrize.velocity = Vector3.zero;
        heldPrize.angularVelocity = Vector3.zero;

        heldPrize = null;
    }
}