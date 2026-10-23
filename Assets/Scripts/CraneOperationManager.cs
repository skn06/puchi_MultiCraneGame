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

    [Header("Lift Toggle")]
    [Tooltip("Keyboardモードで下降／上昇を切り替えるキー")]
    [SerializeField] private KeyCode liftToggleKey = KeyCode.T;

    [Tooltip("Joystickモードで下降／上昇を切り替えるボタン。初期値は1台目のButton 0（トリガー）")]
    [SerializeField] private KeyCode joystickLiftToggleButton = KeyCode.Joystick1Button0;

    [Header("Claw Toggle")]

    [Tooltip("Keyboardモードで爪を開閉するキー")]
    [SerializeField] private KeyCode clawToggleKey = KeyCode.G;

    [Tooltip("Joystickモードで爪を開閉するボタン")]
    [SerializeField] private KeyCode joystickClawToggleButton = KeyCode.Joystick1Button1;

    private void Update()
    {
        if (crane == null || !crane.isActiveAndEnabled) return;

        // GetKeyDownはUpdateで読み、短い押下も取りこぼさないようにする。
        // Keyboardモードでは、下降／上昇と爪の開閉をそれぞれ指定キーで切り替える。
        // Joystickモードでは、対応するボタンで下降／上昇と爪の開閉を切り替える。
        if (inputMode == InputMode.Keyboard)
        {
            if (Input.GetKeyDown(liftToggleKey))
            {
                crane.ToggleLift();
            }

            if (Input.GetKeyDown(clawToggleKey))
            {
                crane.ToggleClaw();
            }

            return;
        }

        if (Input.GetKeyDown(joystickLiftToggleButton))
        {
            crane.ToggleLift();
        }

        if (Input.GetKeyDown(joystickClawToggleButton))
        {
            crane.ToggleClaw();
        }
    }

    private void FixedUpdate()
    {
        if (crane == null || !crane.isActiveAndEnabled) return;

        if (inputMode == InputMode.Keyboard)
        {
            // E/D: 前後、S/F: 左右。上下は押し切り替えで自動移動する。
            crane.MoveForwardBack(GetKeyboardAxis(KeyCode.E, KeyCode.D));
            crane.MoveLeftRight(GetKeyboardAxis(KeyCode.S, KeyCode.F));
            return;
        }

        crane.MoveForwardBack(ReadJoystickAxis(joyStickVertical));
        crane.MoveLeftRight(ReadJoystickAxis(joyStickHorizontal));
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
