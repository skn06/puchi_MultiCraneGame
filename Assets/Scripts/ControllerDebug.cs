using UnityEngine;

[DisallowMultipleComponent]
public class ControllerDebug : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool enableLogging = false;

    [Header("Axes")]
    [SerializeField] private string axis1 = "JoyStick2Horizontal";
    [SerializeField] private string axis2 = "JoyStick2Vertical";
    [SerializeField] private string axis3 = "JoyStick3Horizontal";
    [SerializeField] private string axis4 = "JoyStick3Vertical";
    [SerializeField] private string axis5 = "JoyStick2Slider";
    [SerializeField] private float logThreshold = 0.01f;

    private void Update()
    {
        // Input Managerに軸が未登録の環境でエラーを出さないよう、
        // 明示的に有効にした時だけジョイスティックを読み取ります。
        if (!enableLogging) return;

        float v1 = Input.GetAxis(axis1);
        float v2 = Input.GetAxis(axis2);
        float v3 = Input.GetAxis(axis3);
        float v4 = Input.GetAxis(axis4);
        float v5 = Input.GetAxis(axis5);

        if (Mathf.Abs(v1) > logThreshold ||
            Mathf.Abs(v2) > logThreshold ||
            Mathf.Abs(v3) > logThreshold ||
            Mathf.Abs(v4) > logThreshold ||
            Mathf.Abs(v5) > logThreshold)
        {
            Debug.Log($"{axis1}: {v1:F3}, {axis2}: {v2:F3}, " +
                      $"{axis3}: {v3:F3}, {axis4}: {v4:F3}, " +
                      $"{axis5}: {v5:F3}");
        }
    }
}
