using UnityEngine;

[DisallowMultipleComponent]
public class CraneUnit : MonoBehaviour
{
    [System.Serializable]
    public class LifMagSetting
    {
        public Transform target;
        public float minX;
        public float maxX;
        public bool movable = true;
    }

    [Header("References")]
    [SerializeField] private Transform mainCrane;
    [SerializeField] private Transform mainLifMag;

    [Header("LifMag Settings (left to right)")]
    [SerializeField] private LifMagSetting[] lifMags = new LifMagSetting[5];

    [Header("Z Speed (MainCrane) [m/min]")]
    [SerializeField] private float[] zSpeeds = { 7.5f, 20f, 37.5f, 70f };
    [SerializeField] private int zSpeedIndex;

    [Header("MainLifMag X Speed [m/min]")]
    [SerializeField] private float[] mainLifMagXSpeeds = { 4.2f, 10.5f, 21f, 42f };
    [SerializeField] private int mainLifMagXSpeedIndex;

    [Header("MainLifMag Y Speed [m/min]")]
    [SerializeField] private float[] mainLifMagYSpeeds = { 1.2f, 3f, 6f, 12f };
    [SerializeField] private int mainLifMagYSpeedIndex;

    [Header("LifMag Speed [m/min]")]
    [SerializeField] private float lifOuterSpeed = 3.54f;
    [SerializeField] private float lifInnerSpeed = 1.785f;

    [Header("Movement Ranges")]
    [SerializeField] private float minZ = -20f;
    [SerializeField] private float maxZ = 25f;
    [SerializeField] private float minMainX = -0.368f;
    [SerializeField] private float maxMainX = 0.368f;
    [SerializeField] private float minMainY = -5.31f;
    [SerializeField] private float maxMainY = -0.156f;

    public string ZSpeedDisplayText { get; private set; }
    public string MainLifMagXSpeedDisplayText { get; private set; }
    public string MainLifMagYSpeedDisplayText { get; private set; }

    public float CurrentMainCraneZSpeed => GetSpeed(zSpeeds, zSpeedIndex);
    public float CurrentMainLifMagXSpeed => GetSpeed(mainLifMagXSpeeds, mainLifMagXSpeedIndex);
    public float CurrentMainLifMagYSpeed => GetSpeed(mainLifMagYSpeeds, mainLifMagYSpeedIndex);

    private void Awake()
    {
        UpdateSpeedTexts();
    }

    public void MoveMainCraneZ(float input)
    {
        if (mainCrane == null) return;
        MoveLocal(mainCrane, Vector3.forward, input, CurrentMainCraneZSpeed, minZ, maxZ, 2);
    }

    public void MoveMainLifMagX(float input)
    {
        if (mainLifMag == null) return;
        MoveLocal(mainLifMag, Vector3.right, input, CurrentMainLifMagXSpeed, minMainX, maxMainX, 0);
    }

    public void MoveMainLifMagY(float input)
    {
        if (mainLifMag == null) return;
        MoveLocal(mainLifMag, Vector3.up, input, CurrentMainLifMagYSpeed, minMainY, maxMainY, 1);
    }

    public void MoveLifMagX(int index, float input)
    {
        if (lifMags == null || index < 0 || index >= lifMags.Length) return;

        LifMagSetting lifMag = lifMags[index];
        if (lifMag == null || !lifMag.movable || lifMag.target == null) return;

        float speed = (index == 0 || index == lifMags.Length - 1)
            ? lifOuterSpeed
            : lifInnerSpeed;
        MoveLocal(lifMag.target, Vector3.right, input, speed, lifMag.minX, lifMag.maxX, 0);
    }

    public void ChangeZSpeed() => SetZSpeedIndex(zSpeedIndex + 1);
    public void ChangeMainLifMagXSpeed() => SetMainLifMagXSpeedIndex(mainLifMagXSpeedIndex + 1);
    public void ChangeMainLifMagYSpeed() => SetMainLifMagYSpeedIndex(mainLifMagYSpeedIndex + 1);
    public void IncreaseZSpeed() => SetZSpeedIndex(zSpeedIndex + 1);
    public void DecreaseZSpeed() => SetZSpeedIndex(zSpeedIndex - 1);
    public void IncreaseMainLifMagXSpeed() => SetMainLifMagXSpeedIndex(mainLifMagXSpeedIndex + 1);
    public void DecreaseMainLifMagXSpeed() => SetMainLifMagXSpeedIndex(mainLifMagXSpeedIndex - 1);
    public void IncreaseMainLifMagYSpeed() => SetMainLifMagYSpeedIndex(mainLifMagYSpeedIndex + 1);
    public void DecreaseMainLifMagYSpeed() => SetMainLifMagYSpeedIndex(mainLifMagYSpeedIndex - 1);

    public void ResetSpeedLevel()
    {
        zSpeedIndex = 0;
        mainLifMagXSpeedIndex = 0;
        mainLifMagYSpeedIndex = 0;
        UpdateSpeedTexts();
    }

    public void UpdateSpeedTexts()
    {
        ZSpeedDisplayText = FormatSpeed(zSpeeds, zSpeedIndex);
        MainLifMagXSpeedDisplayText = FormatSpeed(mainLifMagXSpeeds, mainLifMagXSpeedIndex);
        MainLifMagYSpeedDisplayText = FormatSpeed(mainLifMagYSpeeds, mainLifMagYSpeedIndex);
    }

    private void SetZSpeedIndex(int index)
    {
        zSpeedIndex = ClampIndex(index, zSpeeds);
        UpdateSpeedTexts();
    }

    private void SetMainLifMagXSpeedIndex(int index)
    {
        mainLifMagXSpeedIndex = ClampIndex(index, mainLifMagXSpeeds);
        UpdateSpeedTexts();
    }

    private void SetMainLifMagYSpeedIndex(int index)
    {
        mainLifMagYSpeedIndex = ClampIndex(index, mainLifMagYSpeeds);
        UpdateSpeedTexts();
    }

    private static void MoveLocal(Transform target, Vector3 direction, float input,
        float speedPerMinute, float min, float max, int axis)
    {
        Vector3 position = target.localPosition;
        position += direction * input * (speedPerMinute / 60f) * Time.fixedDeltaTime;
        position[axis] = Mathf.Clamp(position[axis], min, max);
        target.localPosition = position;
    }

    private static float GetSpeed(float[] speeds, int index)
    {
        return speeds == null || speeds.Length == 0 ? 0f : speeds[ClampIndex(index, speeds)];
    }

    private static int ClampIndex(int index, float[] values)
    {
        return values == null || values.Length == 0 ? 0 : Mathf.Clamp(index, 0, values.Length - 1);
    }

    private static string FormatSpeed(float[] speeds, int index)
    {
        if (speeds == null || speeds.Length == 0) return "0.0 (0/0)";
        index = ClampIndex(index, speeds);
        return $"{speeds[index]:0.###} ({index + 1}/{speeds.Length})";
    }

    private void OnValidate()
    {
        minZ = Mathf.Min(minZ, maxZ);
        minMainX = Mathf.Min(minMainX, maxMainX);
        minMainY = Mathf.Min(minMainY, maxMainY);
        UpdateSpeedTexts();
    }
}
