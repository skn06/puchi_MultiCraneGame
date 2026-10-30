using UnityEngine;

[DisallowMultipleComponent]
public class ClawDescentContactManager : MonoBehaviour
{
    [Header("Stop Targets")]
    [Tooltip("床・台として降下停止判定するLayer")]
    [SerializeField] private LayerMask stopSurfaceLayers;

    [Header("Debug")]
    [SerializeField] private bool showDebugLog = true;

    private readonly int[] contactCounts = new int[3];

    // 爪上部の連結部が景品・台に接触している数。
    private int bodyContactCount = 0;

    // 上部連結部が何かに接触しているか。
    public bool BodyTouching => bodyContactCount > 0;

    // 3本の爪または上部連結部のどこかが停止対象に接触しているか。
    public bool AnyTouching =>
        BodyTouching || TouchingFingerCount > 0;

    public bool Finger1Touching => contactCounts[0] > 0;
    public bool Finger2Touching => contactCounts[1] > 0;
    public bool Finger3Touching => contactCounts[2] > 0;

    public int TouchingFingerCount
    {
        get
        {
            int count = 0;

            if (Finger1Touching) count++;
            if (Finger2Touching) count++;
            if (Finger3Touching) count++;

            return count;
        }
    }

    public bool IsFingerTouching(int fingerNumber)
    {
        int index = fingerNumber - 1;

        if (index < 0 || index >= contactCounts.Length)
            return false;

        return contactCounts[index] > 0;
    }

    // Prizeタグの景品、または指定Layerの床・台だけを停止対象にする。
    public bool IsStopTarget(Collider other)
    {
        if (other == null) return false;

        Rigidbody rigidbody = other.attachedRigidbody;

        if (rigidbody != null && rigidbody.CompareTag("Prize"))
        {
            return true;
        }

        int layerBit = 1 << other.gameObject.layer;

        return (stopSurfaceLayers.value & layerBit) != 0;
    }

    public void RegisterContact(int fingerNumber, Collider other)
    {
        int index = fingerNumber - 1;

        if (index < 0 || index >= contactCounts.Length)
            return;

        contactCounts[index]++;

        if (showDebugLog)
        {
            Debug.Log(
                "Descent contact ON : Claw "
                + fingerNumber
                + " / "
                + other.name
            );
        }
    }

    public void UnregisterContact(int fingerNumber, Collider other)
    {
        int index = fingerNumber - 1;

        if (index < 0 || index >= contactCounts.Length)
            return;

        contactCounts[index] =
            Mathf.Max(0, contactCounts[index] - 1);

        if (showDebugLog)
        {
            Debug.Log(
                "Descent contact OFF : Claw "
                + fingerNumber
                + " / "
                + other.name
            );
        }
    }

    // 爪上部の連結部が停止対象に接触したときに呼ぶ。
    public void RegisterBodyContact(Collider other)
    {
        bodyContactCount++;

        if (showDebugLog)
        {
            Debug.Log(
                "Descent BODY contact ON : "
                + other.name
            );
        }
    }

    // 爪上部の連結部が停止対象から離れたときに呼ぶ。
    public void UnregisterBodyContact(Collider other)
    {
        bodyContactCount =
            Mathf.Max(0, bodyContactCount - 1);

        if (showDebugLog)
        {
            Debug.Log(
                "Descent BODY contact OFF : "
                + other.name
            );
        }
    }

    public void ResetContacts()
    {
        for (int i = 0; i < contactCounts.Length; i++)
        {
            contactCounts[i] = 0;
        }

        bodyContactCount = 0;
    }
}