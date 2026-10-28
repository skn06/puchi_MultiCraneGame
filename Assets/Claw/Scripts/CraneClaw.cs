using UnityEngine;

[DisallowMultipleComponent]
public class CraneClaw : MonoBehaviour
{
    [Header("Claw Rotates")]
    [SerializeField] private Transform claw1Rotate;
    [SerializeField] private Transform claw2Rotate;
    [SerializeField] private Transform claw3Rotate;

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

    private void Update()
    {
        float target1 = isClosed ? claw1ClosedY : claw1OpenY;
        float target2 = isClosed ? claw2ClosedY : claw2OpenY;
        float target3 = isClosed ? claw3ClosedY : claw3OpenY;

        RotateTo(claw1Rotate, target1);
        RotateTo(claw2Rotate, target2);
        RotateTo(claw3Rotate, target3);
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

            return !IsAtTarget(claw1Rotate, target1)
                || !IsAtTarget(claw2Rotate, target2)
                || !IsAtTarget(claw3Rotate, target3);
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
        isClosed = !isClosed;
    }

    public void Open()
    {
        isClosed = false;
    }

    public void Close()
    {
        isClosed = true;
    }
}