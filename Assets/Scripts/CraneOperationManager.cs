using UnityEngine;

[DisallowMultipleComponent]
public class CraneOperationManager : MonoBehaviour
{
    [Header("Controlled Crane")]
    [SerializeField] private CraneUnit crane;

    private void FixedUpdate()
    {
        if (crane == null) return;

        // W/S: 前後、A/D: 左右、R/F: 上下
        crane.MoveForwardBack(GetAxis(KeyCode.W, KeyCode.S));
        crane.MoveLeftRight(GetAxis(KeyCode.D, KeyCode.A));
        crane.MoveUpDown(GetAxis(KeyCode.R, KeyCode.F));
    }

    private static float GetAxis(KeyCode positive, KeyCode negative)
    {
        return (Input.GetKey(positive) ? 1f : 0f)
             - (Input.GetKey(negative) ? 1f : 0f);
    }
}
