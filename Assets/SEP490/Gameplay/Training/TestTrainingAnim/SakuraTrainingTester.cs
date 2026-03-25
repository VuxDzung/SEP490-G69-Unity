using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using SEP490G69;

public class SakuraTrainingTester : MonoBehaviour
{
    [Header("Giao diện nút bấm (Chỉ để Test)")]
    [SerializeField] private Button buttonTrainPower;
    [SerializeField] private Button buttonTrainIntelligence;
    [SerializeField] private Button buttonTrainEvade;
    [SerializeField] private Button buttonTrainRun;
    [SerializeField] private Button buttonTrainSwim;

    [Header("Thành phần của Sakura")]
    [SerializeField] private SpriteRenderer sakuraRenderer;
    [SerializeField] private Sprite spriteSakuraWindUp;
    [SerializeField] private Sprite spriteSakuraHeadbutt;
    [SerializeField] private Sprite spriteSakuraStudy;
    [SerializeField] private Sprite spriteSakuraRunning;
    [SerializeField] private Sprite spriteSakuraSwimming;

    [Header("Sprites - Bài tập EVADE")]
    [SerializeField] private Sprite spriteSakuraSpinning;
    [SerializeField] private Sprite spriteSakuraEvade1;
    [SerializeField] private Sprite spriteSakuraEvade2;
    [SerializeField] private Sprite spriteSakuraEvade3;

    [Header("Đạo cụ - Bài tập EVADE (Sử dụng PoolManager)")]
    [SerializeField] private GameObject tennisBallPrefab;
    [SerializeField] private string tennisBallPoolName = "TennisBallPool";
    [SerializeField] private float ballFlySpeed = 0.5f;

    [Header("Cấu hình Bóng bay & Mask")]
    [SerializeField] private Transform posBallSpawnLeft;   // Điểm bắn bóng bên trái
    [SerializeField] private Transform posBallSpawnRight;  // Điểm bắn bóng bên phải
    [SerializeField] private Transform ballContainer;      // Nơi chứa bóng để gắn Sprite Mask

    [Header("Các Backgrounds (Môi trường)")]
    [SerializeField] private GameObject bgPowAndEvade;
    [SerializeField] private GameObject bgStudy;
    [SerializeField] private GameObject bgRunning;
    [SerializeField] private GameObject bgSwimming;

    [Header("Môi trường - Tương tác")]
    [SerializeField] private SpriteRenderer bagRenderer;
    [SerializeField] private Sprite spriteBagNormal;
    [SerializeField] private Sprite spriteBagDeform;

    [Header("Môi trường - Cuộn (Parallax cho Running)")]
    [SerializeField] private SpriteRenderer[] scrollingLayers;
    [SerializeField] private float[] scrollSpeeds = { 0.05f, 0.2f, 0.5f, 0.8f };

    [Header("Các vị trí - POW & STUDY")]
    [SerializeField] private Transform positionStart;
    [SerializeField] private Transform positionWindUp;
    [SerializeField] private Transform positionImpact;
    [SerializeField] private Transform posStudyLeft;
    [SerializeField] private Transform posStudyRight;

    [Header("Các vị trí - EVADE, RUN & SWIM")]
    [SerializeField] private Transform posEvadeLeft;
    [SerializeField] private Transform posEvadeRight;
    [SerializeField] private Transform posRunCenter;
    [SerializeField] private Transform posSwimming;

    [SerializeField] private float evadeRandomYRange = 0.5f;
    [SerializeField] private float evadeMoveDuration = 0.4f; // Tăng nhẹ thời gian di chuyển để nhìn rõ mưa bóng
    [SerializeField] private float evadePauseDuration = 0.4f;
    [SerializeField] private int evadeSpins = 3;

    private Vector3 bagOriginalPosition;
    private Vector3 bagOriginalScale;

    private Sequence activeTrainingSequence;
    private int currentEvadeStep = 0;
    private bool isScrolling = false;

    private List<Transform> activeBalls = new List<Transform>();

    void Start()
    {
        bagOriginalPosition = bagRenderer.transform.position;
        bagOriginalScale = bagRenderer.transform.localScale;

        if (buttonTrainPower) buttonTrainPower.onClick.AddListener(PlayPowerTraining);
        if (buttonTrainIntelligence) buttonTrainIntelligence.onClick.AddListener(PlayIntelligenceTraining);
        if (buttonTrainEvade) buttonTrainEvade.onClick.AddListener(PlayEvadeTraining);
        if (buttonTrainRun) buttonTrainRun.onClick.AddListener(PlayRunTraining);
        if (buttonTrainSwim) buttonTrainSwim.onClick.AddListener(PlaySwimmingTraining);

        StopAllAnimations();
    }

    void Update()
    {
        if (isScrolling && scrollingLayers != null)
        {
            for (int i = 0; i < scrollingLayers.Length; i++)
            {
                if (scrollingLayers[i] != null && scrollingLayers[i].material != null)
                {
                    Vector2 offset = scrollingLayers[i].material.mainTextureOffset;
                    offset.x += scrollSpeeds[i] * Time.deltaTime;
                    scrollingLayers[i].material.mainTextureOffset = offset;
                }
            }
        }
    }

    private void StopAllAnimations()
    {
        if (activeTrainingSequence != null) activeTrainingSequence.Kill();
        DOTween.Kill(sakuraRenderer.transform);
        DOTween.Kill(bagRenderer.transform);

        foreach (var ball in activeBalls)
        {
            if (ball != null) DOTween.Kill(ball);
        }
        activeBalls.Clear();

        if (PoolManager.Pools.ContainsKey(tennisBallPoolName))
        {
            PoolManager.Pools[tennisBallPoolName].DespawnAll();
        }

        isScrolling = false;

        sakuraRenderer.transform.position = positionStart.position;
        sakuraRenderer.transform.rotation = Quaternion.identity;
        sakuraRenderer.flipX = false;

        bagRenderer.transform.position = bagOriginalPosition;
        bagRenderer.transform.localScale = bagOriginalScale;
        bagRenderer.sprite = spriteBagNormal;
        bagRenderer.enabled = false;
    }

    private void SetActiveBackground(GameObject activeBg)
    {
        if (bgPowAndEvade) bgPowAndEvade.SetActive(false);
        if (bgStudy) bgStudy.SetActive(false);
        if (bgRunning) bgRunning.SetActive(false);
        if (bgSwimming) bgSwimming.SetActive(false);

        if (activeBg) activeBg.SetActive(true);
    }

    // ================= CÁC BÀI TẬP BÌNH THƯỜNG =================
    private void PlayPowerTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgPowAndEvade);
        bagRenderer.enabled = true;
        sakuraRenderer.sprite = spriteSakuraWindUp;

        activeTrainingSequence = DOTween.Sequence();
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMove(positionWindUp.position, 0.4f).SetEase(Ease.OutQuad));
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMove(positionImpact.position, 0.1f).SetEase(Ease.InExpo));
        activeTrainingSequence.AppendCallback(() => {
            sakuraRenderer.sprite = spriteSakuraHeadbutt;
            bagRenderer.sprite = spriteBagDeform;
            bagRenderer.transform.DOShakePosition(0.15f, strength: new Vector3(0.3f, 0, 0), vibrato: 20);
            bagRenderer.transform.DOShakeScale(0.15f, strength: 0.1f, vibrato: 10);
        });
        activeTrainingSequence.AppendInterval(0.2f);
        activeTrainingSequence.AppendCallback(() => {
            sakuraRenderer.sprite = spriteSakuraWindUp;
            bagRenderer.sprite = spriteBagNormal;
            bagRenderer.transform.position = bagOriginalPosition;
            bagRenderer.transform.localScale = bagOriginalScale;
        });
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMove(positionStart.position, 0.3f).SetEase(Ease.OutBack));
        activeTrainingSequence.SetLoops(-1, LoopType.Restart);
    }

    private void PlayIntelligenceTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgStudy);
        sakuraRenderer.sprite = spriteSakuraStudy;
        sakuraRenderer.transform.position = posStudyLeft.position;

        activeTrainingSequence = DOTween.Sequence();
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMoveX(posStudyRight.position.x, 6f).SetEase(Ease.InOutQuad));
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMoveX(posStudyLeft.position.x, 6f).SetEase(Ease.InOutQuad));

        sakuraRenderer.transform.DOMoveY(posStudyLeft.position.y + 0.5f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        sakuraRenderer.transform.DORotate(new Vector3(0, 0, 360f), 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

        activeTrainingSequence.SetLoops(-1, LoopType.Restart);
    }

    private void PlayRunTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgRunning);
        isScrolling = true;

        sakuraRenderer.sprite = spriteSakuraRunning;
        sakuraRenderer.transform.position = posRunCenter.position;

        activeTrainingSequence = DOTween.Sequence();
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMoveY(posRunCenter.position.y + 0.3f, 0.6f).SetEase(Ease.InOutSine));
        activeTrainingSequence.SetLoops(-1, LoopType.Yoyo);
    }

    private void PlaySwimmingTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgSwimming);

        sakuraRenderer.sprite = spriteSakuraSwimming;
        sakuraRenderer.transform.position = posSwimming.position;

        sakuraRenderer.transform.DOMoveY(posSwimming.position.y + 0.4f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        sakuraRenderer.transform.DOMoveX(posSwimming.position.x + 1.2f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        Transform t = sakuraRenderer.transform;
        t.localRotation = Quaternion.Euler(0, 0, -10f);
        t.DOLocalRotate(new Vector3(0, 0, 10f), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    // ================== BÀI TẬP NÉ TRÁNH (EVADE) ==================
    private void PlayEvadeTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgPowAndEvade);

        sakuraRenderer.transform.position = posEvadeLeft.position;
        currentEvadeStep = 0;

        RunNextEvadeStep();
    }

    private void RunNextEvadeStep()
    {
        if (this == null || !gameObject.activeInHierarchy) return;

        activeTrainingSequence = DOTween.Sequence();

        bool isMovingRight = (currentEvadeStep % 2 == 0);
        Transform startPos = isMovingRight ? posEvadeLeft : posEvadeRight;
        Transform endPos = isMovingRight ? posEvadeRight : posEvadeLeft;

        Sprite[] evadeSprites = { spriteSakuraEvade1, spriteSakuraEvade2, spriteSakuraEvade3 };
        Sprite targetSprite = evadeSprites[currentEvadeStep % 3];

        float randomYOffset = Random.Range(-evadeRandomYRange, evadeRandomYRange);
        Vector3 targetPosition = new Vector3(endPos.position.x, startPos.position.y + randomYOffset, startPos.position.z);

        activeTrainingSequence.AppendCallback(() => {
            sakuraRenderer.sprite = spriteSakuraSpinning;
            sakuraRenderer.flipX = !isMovingRight;
            sakuraRenderer.transform.position = startPos.position;
            sakuraRenderer.transform.rotation = Quaternion.identity;
        });

        // Di chuyển và quay
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMove(targetPosition, evadeMoveDuration).SetEase(Ease.OutSine));
        activeTrainingSequence.Insert(0, sakuraRenderer.transform.DORotate(new Vector3(0, 360f * evadeSpins, 0), evadeMoveDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        // ---------------- THAY ĐỔI LOGIC BẮN BÓNG SO LE ----------------
        int totalBalls = Random.Range(3, 6); // Bắn từ 3 đến 5 quả bóng mỗi lượt
        float timeStep = evadeMoveDuration / totalBalls; // Chia đều khoảng thời gian
        bool isLeftTurn = Random.value > 0.5f; // Random xem bên nào (Trái/Phải) được bắn quả đầu tiên

        for (int i = 0; i < totalBalls; i++)
        {
            float spawnDelay = timeStep * i;

            // Luân phiên đảo bên bắn
            Transform spawnPoint = isLeftTurn ? posBallSpawnLeft : posBallSpawnRight;
            Transform destPoint = isLeftTurn ? posBallSpawnRight : posBallSpawnLeft;

            activeTrainingSequence.InsertCallback(spawnDelay, () => ShootTennisBall(spawnPoint, destPoint));

            // Đổi cờ để quả bóng kế tiếp bắn từ hướng ngược lại
            isLeftTurn = !isLeftTurn;
        }
        // -------------------------------------------------------------

        activeTrainingSequence.AppendCallback(() => {
            sakuraRenderer.transform.rotation = Quaternion.identity;
            sakuraRenderer.sprite = targetSprite;
            sakuraRenderer.flipX = isMovingRight;
        });

        activeTrainingSequence.AppendInterval(evadePauseDuration);
        activeTrainingSequence.OnComplete(() => {
            currentEvadeStep++;
            RunNextEvadeStep();
        });
    }

    // ================= LOGIC SINH BÓNG TỪ 2 BÊN =================
    private void ShootTennisBall(Transform spawnPoint, Transform destPoint)
    {
        if (tennisBallPrefab == null || spawnPoint == null || destPoint == null) return;

        if (!PoolManager.Pools.ContainsKey(tennisBallPoolName))
        {
            Debug.LogWarning($"[SpawnPool] Không tìm thấy Pool tên là '{tennisBallPoolName}'.");
            return;
        }

        // Random vị trí Y để bóng bay so le trên/dưới
        float randomY = Random.Range(-evadeRandomYRange, evadeRandomYRange);
        Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + randomY, spawnPoint.position.z);
        Vector3 endPos = new Vector3(destPoint.position.x, destPoint.position.y + randomY, destPoint.position.z);

        Transform ball = PoolManager.Pools[tennisBallPoolName].Spawn(tennisBallPrefab.transform, spawnPos, Quaternion.identity, ballContainer);

        if (ball != null)
        {
            activeBalls.Add(ball);

            ball.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360)
              .SetEase(Ease.Linear)
              .SetLoops(-1, LoopType.Restart);

            ball.DOMove(endPos, ballFlySpeed).SetEase(Ease.Linear).OnComplete(() => {
                DOTween.Kill(ball);
                activeBalls.Remove(ball);

                if (PoolManager.Pools.ContainsKey(tennisBallPoolName))
                {
                    PoolManager.Pools[tennisBallPoolName].DespawnObject(ball);
                }
            });
        }
    }
}

