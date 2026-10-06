using UnityEngine;

[DisallowMultipleComponent]
public class CraneOperationManager : MonoBehaviour
{
    public enum InputMode
    {
        Keyboard,
        Joystick
    }

    [Header("Controlled Crane")]
    [SerializeField] private CraneUnit crane;

    [Header("Input")]
    [SerializeField] private InputMode inputMode = InputMode.Keyboard;
    [SerializeField, Range(0f, 1f)] private float deadZone = 0.1f;

    [Header("Joystick Axes")]
    [Tooltip("左右移動に使うInput Managerの軸名")]
    [SerializeField] private string joyStickHorizontal = "JoyStick2Horizontal";

    [Tooltip("前後移動に使うInput Managerの軸名")]
    [SerializeField] private string joyStickVertical = "JoyStick2Vertical";

    [Tooltip("上下移動に使うInput Managerの軸名")]
    [SerializeField] private string joyStickLift = "JoyStick3Vertical";

    private void FixedUpdate()
    {
        if (crane == null) return;

        if (inputMode == InputMode.Keyboard)
        {
            // W/S: 前後、A/D: 左右、R/F: 上下
            crane.MoveForwardBack(GetKeyboardAxis(KeyCode.W, KeyCode.S));
            crane.MoveLeftRight(GetKeyboardAxis(KeyCode.D, KeyCode.A));
            crane.MoveUpDown(GetKeyboardAxis(KeyCode.R, KeyCode.F));
            return;
        }

        crane.MoveForwardBack(ReadJoystickAxis(joyStickVertical));
        crane.MoveLeftRight(ReadJoystickAxis(joyStickHorizontal));
        crane.MoveUpDown(ReadJoystickAxis(joyStickLift));
    }

    private float ReadJoystickAxis(string axisName)
    {
        float value = Input.GetAxis(axisName);
        return Mathf.Abs(value) < deadZone ? 0f : value;
    }

    private static float GetKeyboardAxis(KeyCode positive, KeyCode negative)
    {
        return (Input.GetKey(positive) ? 1f : 0f)
             - (Input.GetKey(negative) ? 1f : 0f);
    }
}
