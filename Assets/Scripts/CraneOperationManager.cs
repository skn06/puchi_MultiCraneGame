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

    [Header("Keyboard Movement")]
    [Tooltip("前後方向の+Zへ移動するキー")]
    [SerializeField] private KeyCode forwardKey = KeyCode.E;
    [Tooltip("前後方向の-Zへ移動するキー")]
    [SerializeField] private KeyCode backwardKey = KeyCode.D;
    [Tooltip("左右方向の+Xへ移動するキー")]
    [SerializeField] private KeyCode positiveXKey = KeyCode.S;
    [Tooltip("左右方向の-Xへ移動するキー")]
    [SerializeField] private KeyCode negativeXKey = KeyCode.F;

    [Header("Joystick Axes")]
    [Tooltip("左右移動に使うInput Managerの軸名")]
    [SerializeField] private string joyStickHorizontal = "JoyStick2Horizontal";
    [Tooltip("左右の入力方向を反転する")]
    [SerializeField] private bool invertHorizontal;

    [Tooltip("前後移動に使うInput Managerの軸名")]
    [SerializeField] private string joyStickVertical = "JoyStick2Vertical";
    [Tooltip("前後の入力方向を反転する")]
    [SerializeField] private bool invertVertical;

    [Header("Lift Toggle")]
    [Tooltip("Keyboardモードで降下開始／降下中の停止を行うキー")]
    [SerializeField] private KeyCode liftToggleKey = KeyCode.T;
    [Tooltip("Joystickモードで降下開始／降下中の停止を行うボタン")]
    [SerializeField] private KeyCode joystickLiftToggleButton = KeyCode.Joystick1Button0;

    [Header("Claw Toggle")]
    [Tooltip("Keyboardモードで爪を開閉するキー")]
    [SerializeField] private KeyCode clawToggleKey = KeyCode.G;
    [Tooltip("Joystickモードで爪を開閉するボタン。通常操作中のみ有効")]
    [SerializeField] private KeyCode joystickClawToggleButton = KeyCode.Joystick1Button1;

    [Header("Next Game")]
    [Tooltip("Keyboardモードで結果画面から次のゲームを開始するキー")]
    [SerializeField] private KeyCode nextGameKey = KeyCode.Space;
    [Tooltip("Joystickモードで結果画面から次のゲームを開始するボタン")]
    [SerializeField] private KeyCode joystickNextGameButton = KeyCode.Joystick1Button2;

    private void Update()
    {
        if (crane == null || !crane.isActiveAndEnabled) return;

        crane.NextGameKey = inputMode == InputMode.Keyboard
            ? nextGameKey : joystickNextGameButton;

        // ボタンの押下はUpdateで読み、選択した入力モードだけを受け付ける。
        if (inputMode == InputMode.Keyboard)
        {
            HandleButtons(liftToggleKey, clawToggleKey, nextGameKey);
            return;
        }

        HandleButtons(
            joystickLiftToggleButton,
            joystickClawToggleButton,
            joystickNextGameButton
        );
    }

    private void HandleButtons(KeyCode liftButton, KeyCode clawButton, KeyCode nextButton)
    {
        if (Input.GetKeyDown(liftButton))
        {
            crane.ToggleLift();
        }

        if (Input.GetKeyDown(clawButton))
        {
            crane.ToggleClaw();
        }

        if (Input.GetKeyDown(nextButton))
        {
            crane.TryStartNextGame();
        }
    }

    private void FixedUpdate()
    {
        if (crane == null || !crane.isActiveAndEnabled) return;

        if (inputMode == InputMode.Keyboard)
        {
            crane.MoveForwardBack(GetKeyboardAxis(forwardKey, backwardKey));
            crane.MoveLeftRight(GetKeyboardAxis(positiveXKey, negativeXKey));
            return;
        }

        crane.MoveForwardBack(ReadJoystickAxis(joyStickVertical, invertVertical));
        crane.MoveLeftRight(ReadJoystickAxis(joyStickHorizontal, invertHorizontal));
    }

    private float ReadJoystickAxis(string axisName, bool invert)
    {
        // 軸名を空にすると、その方向のジョイスティック入力を無効にできる。
        if (string.IsNullOrWhiteSpace(axisName)) return 0f;

        float value = Input.GetAxis(axisName);
        if (Mathf.Abs(value) < deadZone) return 0f;

        return invert ? -value : value;
    }

    private static float GetKeyboardAxis(KeyCode positive, KeyCode negative)
    {
        return (Input.GetKey(positive) ? 1f : 0f)
             - (Input.GetKey(negative) ? 1f : 0f);
    }
}
