using UnityEngine;

public class DebugFreeCamera : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float fastMoveMultiplier = 3f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Mouse Pan")]
    [SerializeField] private float panSensitivity = 20f;

    private float rotationX;
    private float rotationY;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;
    }

    private void Update()
    {
        MoveCamera();
        RotateCamera();
    }

    // W/Sで前後移動し、左クリック中のマウス移動で上下左右に移動する。
    private void MoveCamera()
    {
        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= fastMoveMultiplier;
        }

        // W/Sで前後移動する。
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction -= transform.forward;
        }

        transform.position += direction.normalized * speed * Time.deltaTime;

        // 左クリック中は、マウスの移動量に応じて上下左右に移動する。
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 pan =
                -transform.right * mouseX
                -transform.up * mouseY;

            transform.position +=
                pan * panSensitivity * Time.deltaTime;
        }
    }

    // 右クリック中だけマウス操作で視点を回転する。
    private void RotateCamera()
    {
        if (!Input.GetMouseButton(1)) return;

        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX = Mathf.Clamp(rotationX, -89f, 89f);

        transform.rotation = Quaternion.Euler(
            rotationX,
            rotationY,
            0f
        );
    }
}