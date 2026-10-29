using UnityEngine;

[DisallowMultipleComponent]
public class CraneUnit : MonoBehaviour
{
    [Header("Movement Targets")]
    [Tooltip("前後移動させるクレーン本体のTransform")]
    [SerializeField] private Transform mainCrane;

    [Tooltip("左右移動させる台車のTransform")]
    [SerializeField] private Transform mainLifMag;

    [Tooltip("上下移動させるLifMag(Center)のTransform")]
    [SerializeField] private Transform centerLifMag;

    [Tooltip("爪の開閉を制御するCraneClaw")]
    [SerializeField] private CraneClaw claw;

    [Tooltip("景品の保持・解放を制御するPrizeGrabber")]
    [SerializeField] private PrizeGrabber prizeGrabber;

    [Tooltip("上昇後にクレーンが移動する取出口上の目標位置")]
    [SerializeField] private Transform prizeDropPoint;

    [Tooltip("景品取得結果の文字表示を制御する")]
    [SerializeField] private CraneResultDisplay resultDisplay;

    [Tooltip("取得景品を結果画面中央に表示する")]
    [SerializeField] private PrizeResultPresenter prizeResultPresenter;

    [Tooltip("取出口の景品判定Trigger")]
    [SerializeField] private PrizeExitDetector prizeExitDetector;

    [Tooltip("次のゲーム開始時に初期位置へ戻す景品")]
    [SerializeField] private Rigidbody prizeForReset;

    [Header("Speed [m/min]")]
    [SerializeField, Min(0f)] private float forwardBackSpeed = 15f;
    [SerializeField, Min(0f)] private float leftRightSpeed = 15f;
    [SerializeField, Min(0f)] private float upDownSpeed = 15f;

    [Header("Play Settings")]
    [Tooltip("前後左右を操作できる制限時間 [秒]")]
    [SerializeField, Min(0f)] private float playTimeLimit = 15f;

    [Header("Operation Delays")]
    [Tooltip("降下停止から爪を閉じ始めるまでの待ち時間 [秒]")]
    [SerializeField, Min(0f)] private float beforeCloseDelay = 0.7f;

    [Tooltip("爪が閉じてから上昇を始めるまでの待ち時間 [秒]")]
    [SerializeField, Min(0f)] private float afterCloseDelay = 0.7f;

    [Tooltip("上昇完了後、取出口へ移動を始めるまでの待ち時間 [秒]")]
    [SerializeField, Min(0f)] private float beforeMoveToDropDelay = 1f;

    [Tooltip("取出口到着後、爪を開き始めるまでの待ち時間 [秒]")]
    [SerializeField, Min(0f)] private float beforeReleaseDelay = 1f;

    [Tooltip("PRIZE GET!表示後、景品を中央表示するまでの待ち時間 [秒]")]
    [SerializeField, Min(0f)] private float beforePrizeDisplayDelay = 1.5f;

    [Tooltip("景品を離してから失敗と判定するまでの待ち時間 [秒]")]
    [SerializeField, Min(0f)] private float prizeDetectionTimeout = 5f;

    [Header("Movement Ranges (local position)")]
    [SerializeField] private float minZ = -20f;
    [SerializeField] private float maxZ = 25f;
    [SerializeField] private float minX = -0.368f;
    [SerializeField] private float maxX = 0.368f;
    [Tooltip("LifMag(Center)のローカルY座標の下限")]
    [SerializeField] private float minY = -4.5546412f;
    [Tooltip("LifMag(Center)のローカルY座標の上限")]
    [SerializeField] private float maxY = -0.3034226f;


    // クレーンの動作状態を管理する。
    // Idle: 通常操作可能
    // Descending: 自動降下中
    // WaitingBeforeClose: 降下停止後の待機
    // Closing: 爪を閉じている途中
    // WaitingBeforeAscending: 爪が閉じた後の待機
    // Ascending: 自動上昇中
    // MovingToDropPoint: 上昇後、取出口の上へ自動移動中
    // WaitingBeforeMoveToDropPoint: 上昇完了後、取出口へ移動する前の待機
    // WaitingBeforeRelease: 取出口到着後、爪を開くまで待機中
    // OpeningAtDropPoint: 取出口で爪を開いている途中
    private enum CraneState
    {
        Idle,
        Descending,
        WaitingBeforeClose,
        Closing,
        WaitingBeforeAscending,
        Ascending,
        WaitingBeforeMoveToDropPoint,
        MovingToDropPoint,
        WaitingBeforeRelease,
        OpeningAtDropPoint,
        WaitingForPrizeDetection,
        WaitingBeforePrizeDisplay,
        ReturningHome,
        ResultComplete
    }

    private CraneState state = CraneState.Idle;

    private float stateTimer = 0f;

    private float remainingPlayTime;
    private bool playTimerRunning;

    // ゲーム開始時のクレーン位置。
    private float initialMainCraneZ;
    private float initialMainLifMagX;

    // 取得した景品。
    private Rigidbody detectedPrize;

    // 次のゲーム開始時に景品を元へ戻すための初期状態。
    private Transform prizeInitialParent;
    private Vector3 prizeInitialLocalPosition;
    private Quaternion prizeInitialLocalRotation;
    private Vector3 prizeInitialLocalScale;

    private bool prizeInitialUseGravity;
    private bool prizeInitialIsKinematic;
    private bool prizeInitialDetectCollisions;

    // UI表示用に現在の残り時間を取得できるようにする。
    public float RemainingPlayTime => remainingPlayTime;


    // ゲーム開始時に操作制限時間のカウントを開始する。
    private void Start()
    {
        // 起動時のクレーン位置を初期位置として保存する。
        if (mainCrane != null)
        {
            initialMainCraneZ = mainCrane.localPosition.z;
        }

        if (mainLifMag != null)
        {
            initialMainLifMagX = mainLifMag.localPosition.x;
        }

        // 景品の初期配置と物理状態を保存する。
        if (prizeForReset != null)
        {
            Transform prizeTransform = prizeForReset.transform;

            prizeInitialParent = prizeTransform.parent;
            prizeInitialLocalPosition = prizeTransform.localPosition;
            prizeInitialLocalRotation = prizeTransform.localRotation;
            prizeInitialLocalScale = prizeTransform.localScale;

            prizeInitialUseGravity = prizeForReset.useGravity;
            prizeInitialIsKinematic = prizeForReset.isKinematic;
            prizeInitialDetectCollisions = prizeForReset.detectCollisions;
        }

        if (resultDisplay != null)
        {
            resultDisplay.HideAll();
        }

        ResetPlayTimer();
    }

    // 降下 → 待機 → 爪を閉じる → 待機 → 上昇 → 取出口へ移動、
    // の自動動作を状態に応じて実行する。
    private void Update()
    {
        // 結果画面でSpaceを押したら次のゲームへ進む。
        if (state == CraneState.ResultComplete
            && Input.GetKeyDown(KeyCode.Space))
        {
            StartNextGame();
            return;
        }

        if (!playTimerRunning) return;
        if (state != CraneState.Idle) return;

        remainingPlayTime -= Time.deltaTime;

        if (remainingPlayTime <= 0f)
        {
            remainingPlayTime = 0f;
            BeginDescending();
        }
    }

    // 降下・待機・爪の開閉・上昇など、自動動作中は前後左右の移動を禁止する。
    private bool IsOperationLocked
    {
        get
        {
            return state != CraneState.Idle
                || (claw != null && claw.IsMoving);
        }
    }

    // Tで自動降下する。降下中に再度Tを押すとその場で停止し、
    // 一定時間待機した後に爪を閉じて自動上昇する。
    public void ToggleLift()
    {
        if (centerLifMag == null) return;

        // 爪が開閉途中ならTは受け付けない。
        if (claw != null && claw.IsMoving) return;

        // 待機中にTを押したら、制限時間を終了して降下開始。
        if (state == CraneState.Idle)
        {
            BeginDescending();
            return;
        }

        // 降下中にTを押したら、その場で停止して爪を閉じる。
        if (state == CraneState.Descending)
        {
            BeginWaitingBeforeClose();
        }
    }

    // Gキーによる爪の手動開閉。
    public void ToggleClaw()
    {
        if (claw == null) return;

        // 自動操作中は手動で爪を操作できない。
        if (state != CraneState.Idle) return;

        // 開閉途中で再度押して反転するのを防ぐ。
        if (claw.IsMoving) return;

        claw.Toggle();
    }

    // 降下 → 待機 → 爪を閉じる → 待機 → 上昇 → 取出口へ移動
    // → 待機 → 爪を開いて景品を解放、の自動動作を状態に応じて実行する。
    private void FixedUpdate()
    {
        if (centerLifMag == null) return;

        float step = GetLocalStep(
            centerLifMag,
            Vector3.up,
            upDownSpeed
        );

        Vector3 position = centerLifMag.localPosition;

        switch (state)
        {
            case CraneState.Descending:

                position.y = Mathf.MoveTowards(
                    position.y,
                    minY,
                    step
                );

                centerLifMag.localPosition = position;

                // 最下部まで来たら、少し待ってから爪を閉じる。
                if (Mathf.Approximately(position.y, minY))
                {
                    BeginWaitingBeforeClose();
                }

                break;

            case CraneState.WaitingBeforeClose:

                stateTimer -= Time.fixedDeltaTime;

                if (stateTimer <= 0f)
                {
                    BeginClosing();
                }

                break;

            case CraneState.Closing:

                // 爪が閉じ終わった時点で、2本以上の爪が
                // 同じ景品に触れていればその景品を保持する。
                if (claw == null || !claw.IsMoving)
                {
                    if (prizeGrabber != null)
                    {
                        prizeGrabber.TryGrab();
                    }

                    state = CraneState.WaitingBeforeAscending;
                    stateTimer = afterCloseDelay;
                }

                break;

            case CraneState.WaitingBeforeAscending:

                stateTimer -= Time.fixedDeltaTime;

                if (stateTimer <= 0f)
                {
                    state = CraneState.Ascending;
                }

                break;

            case CraneState.Ascending:

                position.y = Mathf.MoveTowards(
                    position.y,
                    maxY,
                    step
                );

                centerLifMag.localPosition = position;

                // 最上部まで戻ったら、少し待ってから取出口へ移動する。
                if (Mathf.Approximately(position.y, maxY))
                {
                    state = CraneState.WaitingBeforeMoveToDropPoint;
                    stateTimer = beforeMoveToDropDelay;
                }
            
                break;

            case CraneState.WaitingBeforeMoveToDropPoint:

                stateTimer -= Time.fixedDeltaTime;

                if (stateTimer <= 0f)
                {
                    state = CraneState.MovingToDropPoint;
                }

                break;

            case CraneState.MovingToDropPoint:

                if (prizeDropPoint == null
                    || mainCrane == null
                    || mainLifMag == null)
                {
                    state = CraneState.Idle;
                    break;
                }

                // PrizeDropPointのワールド座標を、
                // 各移動対象が使用しているローカル座標へ変換する。
                Vector3 mainCraneTargetLocal =
                    mainCrane.parent != null
                        ? mainCrane.parent.InverseTransformPoint(prizeDropPoint.position)
                        : prizeDropPoint.position;

                Vector3 mainLifMagTargetLocal =
                    mainLifMag.parent != null
                        ? mainLifMag.parent.InverseTransformPoint(prizeDropPoint.position)
                        : prizeDropPoint.position;

                // 通常の移動範囲を超えないように目標位置を制限する。
                float targetZ = Mathf.Clamp(
                    mainCraneTargetLocal.z,
                    minZ,
                    maxZ
                );

                float targetX = Mathf.Clamp(
                    mainLifMagTargetLocal.x,
                    minX,
                    maxX
                );

                Vector3 mainCranePosition = mainCrane.localPosition;
                Vector3 mainLifMagPosition = mainLifMag.localPosition;

                float forwardBackStep =
                    GetLocalStep(mainCrane, Vector3.forward, forwardBackSpeed);

                float leftRightStep =
                    GetLocalStep(mainLifMag, Vector3.right, leftRightSpeed);

                // 前後方向を取出口へ近づける。
                mainCranePosition.z = Mathf.MoveTowards(
                    mainCranePosition.z,
                    targetZ,
                    forwardBackStep
                );

                // 左右方向を取出口へ近づける。
                mainLifMagPosition.x = Mathf.MoveTowards(
                    mainLifMagPosition.x,
                    targetX,
                    leftRightStep
                );

                mainCrane.localPosition = mainCranePosition;
                mainLifMag.localPosition = mainLifMagPosition;

                // 前後・左右とも取出口に到着したら、
                // 少し待ってから爪を開く。
                if (Mathf.Approximately(mainCranePosition.z, targetZ)
                    && Mathf.Approximately(mainLifMagPosition.x, targetX))
                {
                    state = CraneState.WaitingBeforeRelease;
                    stateTimer = beforeReleaseDelay;
                }

                break;

            case CraneState.WaitingBeforeRelease:

                stateTimer -= Time.fixedDeltaTime;

                if (stateTimer <= 0f)
                {
                    // 取出口で爪を開き始める。
                    if (claw != null)
                    {
                        claw.Open();
                    }

                    // 保持している景品を解放し、重力で取出口へ落下させる。
                    if (prizeGrabber != null)
                    {
                        prizeGrabber.Release();
                    }

                    state = CraneState.OpeningAtDropPoint;
                }

                break;

            case CraneState.OpeningAtDropPoint:

                // 爪が完全に開いたら、景品が取出口へ入るのを待つ。
                if (claw == null || !claw.IsMoving)
                {
                    state = CraneState.WaitingForPrizeDetection;
                    stateTimer = prizeDetectionTimeout;
                }

                break;

            case CraneState.WaitingForPrizeDetection:

                stateTimer -= Time.fixedDeltaTime;

                // 制限時間内に景品が検出されなければ失敗とする。
                if (stateTimer <= 0f)
                {
                    if (resultDisplay != null)
                    {
                        resultDisplay.ShowFailure();
                    }

                    state = CraneState.ReturningHome;
                }

                break;

            case CraneState.WaitingBeforePrizeDisplay:

                stateTimer -= Time.fixedDeltaTime;

                if (stateTimer <= 0f)
                {
                    // 景品を画面中央へ表示する。
                    if (prizeResultPresenter != null && detectedPrize != null)
                    {
                        prizeResultPresenter.ShowPrize(detectedPrize);
                    }

                    // 同時にPRIZE GET!を景品と重ならない位置へ移動する。
                    if (resultDisplay != null)
                    {
                        resultDisplay.MoveForPrizeDisplay();
                    }

                    // 同時にクレーンの初期位置への復帰を開始する。
                    state = CraneState.ReturningHome;
                }

                break;

            case CraneState.ReturningHome:

                Vector3 returnCranePosition = mainCrane.localPosition;
                Vector3 returnLifMagPosition = mainLifMag.localPosition;

                float returnForwardBackStep =
                    GetLocalStep(mainCrane, Vector3.forward, forwardBackSpeed);

                float returnLeftRightStep =
                    GetLocalStep(mainLifMag, Vector3.right, leftRightSpeed);

                returnCranePosition.z = Mathf.MoveTowards(
                    returnCranePosition.z,
                    initialMainCraneZ,
                    returnForwardBackStep
                );

                returnLifMagPosition.x = Mathf.MoveTowards(
                    returnLifMagPosition.x,
                    initialMainLifMagX,
                    returnLeftRightStep
                );

                mainCrane.localPosition = returnCranePosition;
                mainLifMag.localPosition = returnLifMagPosition;

                // 前後・左右ともゲーム開始時の位置へ戻ったら停止する。
                if (Mathf.Approximately(returnCranePosition.z, initialMainCraneZ)
                    && Mathf.Approximately(returnLifMagPosition.x, initialMainLifMagX))
                {
                    state = CraneState.ResultComplete;

                    // クレーンが初期位置へ戻ったら次ゲームへの案内を表示する。
                    if (resultDisplay != null)
                    {
                        resultDisplay.ShowNextGamePrompt();
                    }
                }

                break;

            case CraneState.ResultComplete:

                // 結果画面を表示したまま待機する。
                break;               
        }
    }
    
    // 操作時間を終了し、現在位置から自動降下を開始する。
    private void BeginDescending()
    {
        if (state != CraneState.Idle) return;

        playTimerRunning = false;

        // 降下開始時は爪を開いた状態にする。
        if (claw != null)
        {
            claw.Open();
        }

        state = CraneState.Descending;
    }

    // 制限時間を初期値に戻し、操作時間のカウントを開始する。
    private void ResetPlayTimer()
    {
        remainingPlayTime = playTimeLimit;
        playTimerRunning = true;
    }

    // 降下を停止し、爪を閉じるまでの待機を開始する。
    private void BeginWaitingBeforeClose()
    {
        state = CraneState.WaitingBeforeClose;
        stateTimer = beforeCloseDelay;
    }

    // 待機終了後、現在位置で爪を閉じる処理を開始する。
    private void BeginClosing()
    {
        state = CraneState.Closing;

        if (claw != null)
        {
            claw.Close();
        }
    }

    // 景品が取出口へ入ったら取得成功を表示する。
    public void NotifyPrizeDetected(Rigidbody prize)
    {
        if (prize == null) return;

        // すでに結果処理へ進んでいる場合は無視する。
        if (detectedPrize != null) return;

        // 景品を落としている最中以外の誤判定を防ぐ。
        if (state != CraneState.OpeningAtDropPoint
            && state != CraneState.WaitingForPrizeDetection)
        {
            return;
        }

        detectedPrize = prize;

        if (resultDisplay != null)
        {
            resultDisplay.ShowSuccess();
        }

        state = CraneState.WaitingBeforePrizeDisplay;
        stateTimer = beforePrizeDisplayDelay;
    }

    // 結果画面でSpaceが押されたら、すべてを初期状態へ戻して次のゲームを開始する。
    private void StartNextGame()
    {
        if (resultDisplay != null)
        {
            resultDisplay.HideAll();
        }

        if (prizeExitDetector != null)
        {
            prizeExitDetector.ResetDetection();
        }

        ResetPrizeToInitialPosition();

        detectedPrize = null;
        stateTimer = 0f;
        state = CraneState.Idle;

        ResetPlayTimer();
    }

    // 景品をゲーム開始時の位置・物理状態へ戻す。
    private void ResetPrizeToInitialPosition()
    {
        if (prizeForReset == null) return;

        Transform prizeTransform = prizeForReset.transform;

        prizeForReset.velocity = Vector3.zero;
        prizeForReset.angularVelocity = Vector3.zero;

        // 移動中に物理演算が干渉しないよう一時的に停止する。
        prizeForReset.isKinematic = true;
        prizeForReset.detectCollisions = false;

        prizeTransform.SetParent(prizeInitialParent, false);
        prizeTransform.localPosition = prizeInitialLocalPosition;
        prizeTransform.localRotation = prizeInitialLocalRotation;
        prizeTransform.localScale = prizeInitialLocalScale;

        Physics.SyncTransforms();

        prizeForReset.detectCollisions = prizeInitialDetectCollisions;
        prizeForReset.useGravity = prizeInitialUseGravity;
        prizeForReset.isKinematic = prizeInitialIsKinematic;
    }

    private void OnDisable()
    {
        state = CraneState.Idle;
        stateTimer = 0f;
        playTimerRunning = false;
    }

    // 通常状態のときだけ前後移動を許可する。
    public void MoveForwardBack(float input)
    {
        if (IsOperationLocked) return;

        Move(
            mainCrane,
            Vector3.forward,
            input,
            forwardBackSpeed,
            minZ,
            maxZ,
            2
        );
    }

    // 通常状態のときだけ左右移動を許可する。
    public void MoveLeftRight(float input)
    {
        if (IsOperationLocked) return;

        Move(
            mainLifMag,
            Vector3.right,
            input,
            leftRightSpeed,
            minX,
            maxX,
            0
        );
    }

    private static void Move(Transform target, Vector3 direction, float input,
        float speedPerMinute, float min, float max, int axis)
    {
        if (target == null || Mathf.Approximately(input, 0f)) return;

        Vector3 position = target.localPosition;
        position += direction * Mathf.Clamp(input, -1f, 1f)
            * GetLocalStep(target, direction, speedPerMinute);
        position[axis] = Mathf.Clamp(position[axis], min, max);
        target.localPosition = position;
    }

    private static float GetLocalStep(Transform target, Vector3 direction, float speedPerMinute)
    {
        // 親の拡大率を補正し、1 Unity unit = 1 mとして実際の速度を指定する。
        float worldUnitsPerLocalUnit = target.parent == null
            ? 1f : target.parent.TransformVector(direction).magnitude;
        if (worldUnitsPerLocalUnit <= Mathf.Epsilon) return 0f;

        return Mathf.Max(0f, speedPerMinute) / 60f
            * Time.fixedDeltaTime / worldUnitsPerLocalUnit;
    }

    private void OnValidate()
    {
        if (minZ > maxZ) minZ = maxZ;
        if (minX > maxX) minX = maxX;
        if (minY > maxY) minY = maxY;

        forwardBackSpeed = Mathf.Max(0f, forwardBackSpeed);
        leftRightSpeed = Mathf.Max(0f, leftRightSpeed);
        upDownSpeed = Mathf.Max(0f, upDownSpeed);
    }
}
