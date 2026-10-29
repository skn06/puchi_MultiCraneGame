using UnityEngine;

[DisallowMultipleComponent]
public class PrizeManager : MonoBehaviour
{
    [Header("Prizes")]
    [Tooltip("ゲームで使用するすべての景品")]
    [SerializeField] private Rigidbody[] prizes;

    [Header("Spawn Points")]
    [Tooltip("景品を配置できる候補地点")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Random Settings")]
    [Tooltip("ゲーム開始時にも景品をランダム配置する")]
    [SerializeField] private bool randomizeOnStart = true;

    [Tooltip("配置時に景品のY軸方向の向きもランダムにする")]
    [SerializeField] private bool randomizeYRotation = true;

    private Transform[] originalParents;
    private Vector3[] originalLocalScales;
    private Quaternion[] originalWorldRotations;

    private bool[] originalUseGravity;
    private bool[] originalIsKinematic;
    private bool[] originalDetectCollisions;

    private void Awake()
    {
        SaveInitialPrizeSettings();
    }

    private void Start()
    {
        if (randomizeOnStart)
        {
            ResetAndRandomizePrizes();
        }
    }

    // 景品ごとの元の親・大きさ・物理設定を保存する。
    private void SaveInitialPrizeSettings()
    {
        if (prizes == null) return;

        int count = prizes.Length;

        originalParents = new Transform[count];
        originalLocalScales = new Vector3[count];
        originalWorldRotations = new Quaternion[count];

        originalUseGravity = new bool[count];
        originalIsKinematic = new bool[count];
        originalDetectCollisions = new bool[count];

        for (int i = 0; i < count; i++)
        {
            if (prizes[i] == null) continue;

            Transform prizeTransform = prizes[i].transform;

            originalParents[i] = prizeTransform.parent;
            originalLocalScales[i] = prizeTransform.localScale;
            originalWorldRotations[i] = prizeTransform.rotation;

            originalUseGravity[i] = prizes[i].useGravity;
            originalIsKinematic[i] = prizes[i].isKinematic;
            originalDetectCollisions[i] = prizes[i].detectCollisions;
        }
    }

    // すべての景品を、重複しないランダムなSpawnPointへ再配置する。
    public void ResetAndRandomizePrizes()
    {
        if (prizes == null || prizes.Length == 0) return;

        if (spawnPoints == null || spawnPoints.Length < prizes.Length)
        {
            Debug.LogWarning(
                "SpawnPointの数が景品の数より少ないため、ランダム配置できません。"
            );
            return;
        }

        // SpawnPointの番号を作り、順番をランダムに並べ替える。
        int[] pointOrder = new int[spawnPoints.Length];

        for (int i = 0; i < pointOrder.Length; i++)
        {
            pointOrder[i] = i;
        }

        for (int i = pointOrder.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            int temp = pointOrder[i];
            pointOrder[i] = pointOrder[randomIndex];
            pointOrder[randomIndex] = temp;
        }

        // 各景品を別々のSpawnPointへ配置する。
        for (int i = 0; i < prizes.Length; i++)
        {
            Rigidbody prize = prizes[i];
            if (prize == null) continue;

            Transform spawnPoint = spawnPoints[pointOrder[i]];
            if (spawnPoint == null) continue;

            Transform prizeTransform = prize.transform;

            // 再配置中に物理演算が干渉しないよう一時停止する。
            prize.velocity = Vector3.zero;
            prize.angularVelocity = Vector3.zero;
            prize.isKinematic = true;
            prize.detectCollisions = false;
            prize.useGravity = false;

            // 結果画面に移動していた景品も元の親へ戻す。
            prizeTransform.SetParent(originalParents[i], true);
            prizeTransform.localScale = originalLocalScales[i];

            prizeTransform.position = spawnPoint.position;

            if (randomizeYRotation)
            {
                float randomY = Random.Range(0f, 360f);

                prizeTransform.rotation =
                    Quaternion.AngleAxis(randomY, Vector3.up)
                    * originalWorldRotations[i];
            }
            else
            {
                prizeTransform.rotation = originalWorldRotations[i];
            }
        }

        Physics.SyncTransforms();

        // 景品の物理演算を元の状態へ戻す。
        for (int i = 0; i < prizes.Length; i++)
        {
            Rigidbody prize = prizes[i];
            if (prize == null) continue;

            prize.detectCollisions = originalDetectCollisions[i];
            prize.useGravity = originalUseGravity[i];
            prize.isKinematic = originalIsKinematic[i];
        }
    }
}