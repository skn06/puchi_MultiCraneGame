using UnityEngine;

[DisallowMultipleComponent]
public class CraneUnit : MonoBehaviour
{
    [Header("Movement Targets")]
    [Tooltip("前後移動させるクレーン本体のTransform")]
    [SerializeField] private Transform mainCrane;

    [Tooltip("左右・上下移動させるリフマグ全体のTransform")]
    [SerializeField] private Transform mainLifMag;

    [Header("Speed [m/min]")]
    [SerializeField] private float forwardBackSpeed = 20f;
    [SerializeField] private float leftRightSpeed = 10.5f;
    [SerializeField] private float upDownSpeed = 3f;

    [Header("Movement Ranges (local position)")]
    [SerializeField] private float minZ = -20f;
    [SerializeField] private float maxZ = 25f;
    [SerializeField] private float minX = -0.368f;
    [SerializeField] private float maxX = 0.368f;
    [SerializeField] private float minY = -5.31f;
    [SerializeField] private float maxY = -0.156f;

    public void MoveForwardBack(float input)
    {
        Move(mainCrane, Vector3.forward, input, forwardBackSpeed, minZ, maxZ, 2);
    }

    public void MoveLeftRight(float input)
    {
        Move(mainLifMag, Vector3.right, input, leftRightSpeed, minX, maxX, 0);
    }

    public void MoveUpDown(float input)
    {
        Move(mainLifMag, Vector3.up, input, upDownSpeed, minY, maxY, 1);
    }

    private static void Move(Transform target, Vector3 direction, float input,
        float speedPerMinute, float min, float max, int axis)
    {
        if (target == null || Mathf.Approximately(input, 0f)) return;

        Vector3 position = target.localPosition;
        position += direction * input * (speedPerMinute / 60f) * Time.fixedDeltaTime;
        position[axis] = Mathf.Clamp(position[axis], min, max);
        target.localPosition = position;
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
