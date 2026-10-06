using UnityEngine;

[DisallowMultipleComponent]
public class CraneOperationManager : MonoBehaviour
{
    public enum InputMode
    {
        Keyboard,
        Joystick
    }

    [Header("Cranes")]
    [SerializeField] private CraneUnit[] cranes = new CraneUnit[0];
    [SerializeField] private int currentCraneIndex;

    [Header("Input")]
    [SerializeField] private InputMode inputMode = InputMode.Keyboard;
    [SerializeField] private float deadZone = 0.1f;

    [Header("Joystick Axes")]
    [SerializeField] private string joyStickHorizontal = "JoyStick2Horizontal";
    [SerializeField] private string joyStickVertical = "JoyStick2Vertical";
    [SerializeField] private string joyStickLift = "JoyStick3Vertical";

    public CraneUnit CurrentCrane
    {
        get
        {
            if (cranes == null || currentCraneIndex < 0 || currentCraneIndex >= cranes.Length)
                return null;
            return cranes[currentCraneIndex];
        }
    }

    public int CurrentCraneIndex => currentCraneIndex;
    public int ActiveCraneCount => cranes == null ? 0 : cranes.Length;

    private void Update()
    {
        SelectCraneWithNumberKeys();
        HandleSpeedKeys();
    }

    private void FixedUpdate()
    {
        CraneUnit crane = CurrentCrane;
        if (crane == null) return;

        if (inputMode == InputMode.Keyboard)
        {
            crane.MoveMainCraneZ(GetKeyboardAxis(KeyCode.W, KeyCode.S));
            crane.MoveMainLifMagX(GetKeyboardAxis(KeyCode.D, KeyCode.A));
            crane.MoveMainLifMagY(GetKeyboardAxis(KeyCode.R, KeyCode.F));

            // J/L: 左端と右端、I/K: 内側のリフマグを動かします。
            float outer = GetKeyboardAxis(KeyCode.L, KeyCode.J);
            float inner = GetKeyboardAxis(KeyCode.I, KeyCode.K);
            crane.MoveLifMagX(0, outer);
            crane.MoveLifMagX(4, outer);
            crane.MoveLifMagX(1, inner);
            crane.MoveLifMagX(3, inner);
            return;
        }

        crane.MoveMainCraneZ(ApplyDeadZone(Input.GetAxis(joyStickVertical)));
        crane.MoveMainLifMagX(ApplyDeadZone(Input.GetAxis(joyStickHorizontal)));
        crane.MoveMainLifMagY(ApplyDeadZone(Input.GetAxis(joyStickLift)));
    }

    public void HandleCraneSelection(int craneIndex)
    {
        if (cranes == null || craneIndex < 0 || craneIndex >= cranes.Length)
        {
            Debug.LogWarning($"クレーン番号が範囲外です: {craneIndex}");
            return;
        }

        currentCraneIndex = craneIndex;
        CurrentCrane.ResetSpeedLevel();
    }

    public void SelectNextCrane()
    {
        if (ActiveCraneCount == 0) return;
        HandleCraneSelection((currentCraneIndex + 1) % ActiveCraneCount);
    }

    public void IncreaseZSpeed() => CurrentCrane?.IncreaseZSpeed();
    public void DecreaseZSpeed() => CurrentCrane?.DecreaseZSpeed();
    public void IncreaseMainLifMagXSpeed() => CurrentCrane?.IncreaseMainLifMagXSpeed();
    public void DecreaseMainLifMagXSpeed() => CurrentCrane?.DecreaseMainLifMagXSpeed();
    public void IncreaseMainLifMagYSpeed() => CurrentCrane?.IncreaseMainLifMagYSpeed();
    public void DecreaseMainLifMagYSpeed() => CurrentCrane?.DecreaseMainLifMagYSpeed();

    private void SelectCraneWithNumberKeys()
    {
        if (cranes == null) return;

        for (int i = 0; i < Mathf.Min(cranes.Length, 9); i++)
        {
            KeyCode key = (KeyCode)((int)KeyCode.Alpha1 + i);
            if (Input.GetKeyDown(key)) HandleCraneSelection(i);
        }
    }

    private void HandleSpeedKeys()
    {
        if (Input.GetKeyDown(KeyCode.Z)) IncreaseZSpeed();
        if (Input.GetKeyDown(KeyCode.X)) DecreaseZSpeed();
        if (Input.GetKeyDown(KeyCode.C)) IncreaseMainLifMagXSpeed();
        if (Input.GetKeyDown(KeyCode.V)) DecreaseMainLifMagXSpeed();
        if (Input.GetKeyDown(KeyCode.B)) IncreaseMainLifMagYSpeed();
        if (Input.GetKeyDown(KeyCode.N)) DecreaseMainLifMagYSpeed();
    }

    private static float GetKeyboardAxis(KeyCode positive, KeyCode negative)
    {
        return (Input.GetKey(positive) ? 1f : 0f) - (Input.GetKey(negative) ? 1f : 0f);
    }

    private float ApplyDeadZone(float value)
    {
        return Mathf.Abs(value) < deadZone ? 0f : value;
    }

    private void OnValidate()
    {
        deadZone = Mathf.Clamp01(deadZone);
        if (ActiveCraneCount == 0) currentCraneIndex = 0;
        else currentCraneIndex = Mathf.Clamp(currentCraneIndex, 0, ActiveCraneCount - 1);
    }
}
