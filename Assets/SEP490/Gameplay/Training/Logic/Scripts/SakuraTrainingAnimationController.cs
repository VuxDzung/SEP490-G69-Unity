using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using SEP490G69;
using SEP490G69.Training;
using System;

public class SakuraTrainingAnimationController : MonoBehaviour
{
    [Header("Thành phần của Sakura")]
    [SerializeField] private SpriteRenderer sakuraRenderer;
    [SerializeField] private Sprite sakuraWindUpSprite;
    [SerializeField] private Sprite sakuraHeadbuttSprite;
    [SerializeField] private Sprite sakuraStudySprite;
    [SerializeField] private Sprite sakuraRunningSprite;
    [SerializeField] private Sprite sakuraSwimmingSprite;

    [Header("Sprites - Bài tập EVADE")]
    [SerializeField] private Sprite sakuraSpinningSprite;
    [SerializeField] private Sprite sakuraEvade1Sprite;
    [SerializeField] private Sprite sakuraEvade2Sprite;
    [SerializeField] private Sprite sakuraEvade3Sprite;

    [Header("Đạo cụ - Bài tập EVADE")]
    [SerializeField] private GameObject tennisBallPrefab;
    [SerializeField] private string tennisBallPoolName = "TennisBallPool";
    [SerializeField] private float ballFlySpeed = 0.5f;

    [Header("Cấu hình Bóng bay & Mask")]
    [SerializeField] private Transform sakuraPosBallSpawnLeft;
    [SerializeField] private Transform sakuraPosBallSpawnRight;
    [SerializeField] private Transform sakuraBallContainer;

    [Header("Các Backgrounds (Môi trường)")]
    [SerializeField] private GameObject sakuraBgPowAndEvade;
    [SerializeField] private GameObject sakuraBgStudy;
    [SerializeField] private GameObject sakuraBgRunning;
    [SerializeField] private GameObject sakuraBgSwimming;

    [Header("Môi trường - Tương tác")]
    [SerializeField] private SpriteRenderer sakuraBagRenderer;
    [SerializeField] private Sprite sakuraBagNormalSprite;
    [SerializeField] private Sprite sakuraBagDeformSprite;

    [Header("Môi trường - Cuộn (Parallax cho Running)")]
    [SerializeField] private SpriteRenderer[] sakuraScrollingLayers;
    [SerializeField] private float[] sakuraScrollSpeeds = { 0.05f, 0.2f, 0.5f, 0.8f };

    [Header("Các vị trí - POW & STUDY")]
    [SerializeField] private Transform sakuraPosStart;
    [SerializeField] private Transform sakuraPosWindUp;
    [SerializeField] private Transform sakuraPosImpact;
    [SerializeField] private Transform sakuraPosStudyLeft;
    [SerializeField] private Transform sakuraPosStudyRight;

    [Header("Các vị trí - EVADE, RUN & SWIM")]
    [SerializeField] private Transform sakuraPosEvadeLeft;
    [SerializeField] private Transform sakuraPosEvadeRight;
    [SerializeField] private Transform sakuraPosRunCenter;
    [SerializeField] private Transform sakuraPosSwimming;

    [SerializeField] private float evadeRandomYRange = 0.5f;
    [SerializeField] private float evadeMoveDuration = 0.4f;
    [SerializeField] private float evadePauseDuration = 0.4f;
    [SerializeField] private int evadeSpins = 3;

    private Vector3 sakuraBagOriginalPosition;
    private Vector3 sakuraBagOriginalScale;

    private Sequence sakuraActiveTrainingSeq;
    private int sakuraCurrentEvadeStep = 0;
    private bool sakuraIsScrolling = false;

    private List<Transform> sakuraActiveBalls = new List<Transform>();
    private Tween sakuraTimerTween; // Biến giữ đếm ngược 4-5s

    void Awake()
    {
        sakuraBagOriginalPosition = sakuraBagRenderer.transform.position;
        sakuraBagOriginalScale = sakuraBagRenderer.transform.localScale;
        StopAllSakuraAnimations();
    }

    void Update()
    {
        if (sakuraIsScrolling && sakuraScrollingLayers != null)
        {
            for (int i = 0; i < sakuraScrollingLayers.Length; i++)
            {
                if (sakuraScrollingLayers[i] != null && sakuraScrollingLayers[i].material != null)
                {
                    Vector2 offset = sakuraScrollingLayers[i].material.mainTextureOffset;
                    offset.x += sakuraScrollSpeeds[i] * Time.deltaTime;
                    sakuraScrollingLayers[i].material.mainTextureOffset = offset;
                }
            }
        }
    }

    private void StopAllSakuraAnimations()
    {
        if (sakuraActiveTrainingSeq != null) sakuraActiveTrainingSeq.Kill();
        if (sakuraTimerTween != null) sakuraTimerTween.Kill();

        DOTween.Kill(sakuraRenderer.transform);
        DOTween.Kill(sakuraBagRenderer.transform);

        // THAY ĐỔI QUAN TRỌNG: Cứu các quả bóng ra khỏi Container trước khi vứt vào Pool
        foreach (var ball in sakuraActiveBalls)
        {
            if (ball != null)
            {
                DOTween.Kill(ball);
                ball.SetParent(null); // <- Cứu bóng khỏi bị Destroy theo Prefab

                if (PoolManager.Pools.ContainsKey(tennisBallPoolName))
                {
                    PoolManager.Pools[tennisBallPoolName].DespawnObject(ball);
                }
            }
        }
        sakuraActiveBalls.Clear();

        sakuraIsScrolling = false;
        sakuraRenderer.transform.position = sakuraPosStart.position;
        sakuraRenderer.transform.rotation = Quaternion.identity;
        sakuraRenderer.flipX = false;
        sakuraRenderer.gameObject.SetActive(false); // Ẩn nhân vật khi không tập

        sakuraBagRenderer.transform.position = sakuraBagOriginalPosition;
        sakuraBagRenderer.transform.localScale = sakuraBagOriginalScale;
        sakuraBagRenderer.sprite = sakuraBagNormalSprite;
        sakuraBagRenderer.enabled = false;

        SetSakuraActiveBackground(null);
    }

    private void SetSakuraActiveBackground(GameObject activeBg)
    {
        if (sakuraBgPowAndEvade) sakuraBgPowAndEvade.SetActive(false);
        if (sakuraBgStudy) sakuraBgStudy.SetActive(false);
        if (sakuraBgRunning) sakuraBgRunning.SetActive(false);
        if (sakuraBgSwimming) sakuraBgSwimming.SetActive(false);

        if (activeBg) activeBg.SetActive(true);
    }

    // ================== HÀM GỌI TỪ BÊN NGOÀI (GAME CONTROLLER) ==================
    public void PlayTrainingAnim(ETrainingType trainingType, Action onComplete)
    {
        StopAllSakuraAnimations();
        sakuraRenderer.gameObject.SetActive(true); // Bật model Sakura lên

        // Chọn bài tập dựa vào enum
        switch (trainingType)
        {
            case ETrainingType.Boxing: PlaySakuraBoxingTraining(); break;
            case ETrainingType.Study: PlaySakuraStudyTraining(); break;
            case ETrainingType.Run: PlaySakuraRunTraining(); break;
            case ETrainingType.Swim: PlaySakuraSwimTraining(); break;
            case ETrainingType.Dodge: PlaySakuraDodgeTraining(); break;
            default: PlaySakuraStudyTraining(); break;
        }

        // Đếm ngược 3 giây, sau đó dừng toàn bộ và gọi hàm callback trả kết quả
        sakuraTimerTween = DOVirtual.DelayedCall(3f, () =>
        {
            StopAllSakuraAnimations();
            onComplete?.Invoke();
        });
    }

    // ================= CÁC BÀI TẬP =================
    private void PlaySakuraBoxingTraining()
    {
        SetSakuraActiveBackground(sakuraBgPowAndEvade);
        sakuraBagRenderer.enabled = true;
        sakuraRenderer.sprite = sakuraWindUpSprite;

        sakuraActiveTrainingSeq = DOTween.Sequence();
        sakuraActiveTrainingSeq.Append(sakuraRenderer.transform.DOMove(sakuraPosWindUp.position, 0.4f).SetEase(Ease.OutQuad));
        sakuraActiveTrainingSeq.Append(sakuraRenderer.transform.DOMove(sakuraPosImpact.position, 0.1f).SetEase(Ease.InExpo));
        sakuraActiveTrainingSeq.AppendCallback(() => {
            sakuraRenderer.sprite = sakuraHeadbuttSprite;
            sakuraBagRenderer.sprite = sakuraBagDeformSprite;
            sakuraBagRenderer.transform.DOShakePosition(0.15f, strength: new Vector3(0.3f, 0, 0), vibrato: 20);
            sakuraBagRenderer.transform.DOShakeScale(0.15f, strength: 0.1f, vibrato: 10);
        });
        sakuraActiveTrainingSeq.AppendInterval(0.2f);
        sakuraActiveTrainingSeq.AppendCallback(() => {
            sakuraRenderer.sprite = sakuraWindUpSprite;
            sakuraBagRenderer.sprite = sakuraBagNormalSprite;
            sakuraBagRenderer.transform.position = sakuraBagOriginalPosition;
            sakuraBagRenderer.transform.localScale = sakuraBagOriginalScale;
        });
        sakuraActiveTrainingSeq.Append(sakuraRenderer.transform.DOMove(sakuraPosStart.position, 0.3f).SetEase(Ease.OutBack));
        sakuraActiveTrainingSeq.SetLoops(-1, LoopType.Restart);
    }

    private void PlaySakuraStudyTraining()
    {
        SetSakuraActiveBackground(sakuraBgStudy);
        sakuraRenderer.sprite = sakuraStudySprite;
        sakuraRenderer.transform.position = sakuraPosStudyLeft.position;

        sakuraActiveTrainingSeq = DOTween.Sequence();
        sakuraActiveTrainingSeq.Append(sakuraRenderer.transform.DOMoveX(sakuraPosStudyRight.position.x, 6f).SetEase(Ease.InOutQuad));
        sakuraActiveTrainingSeq.Append(sakuraRenderer.transform.DOMoveX(sakuraPosStudyLeft.position.x, 6f).SetEase(Ease.InOutQuad));

        sakuraRenderer.transform.DOMoveY(sakuraPosStudyLeft.position.y + 0.5f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        sakuraRenderer.transform.DORotate(new Vector3(0, 0, 360f), 6f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        sakuraActiveTrainingSeq.SetLoops(-1, LoopType.Restart);
    }

    private void PlaySakuraRunTraining()
    {
        SetSakuraActiveBackground(sakuraBgRunning);
        sakuraIsScrolling = true;
        sakuraRenderer.sprite = sakuraRunningSprite;
        sakuraRenderer.transform.position = sakuraPosRunCenter.position;

        sakuraActiveTrainingSeq = DOTween.Sequence();
        sakuraActiveTrainingSeq.Append(sakuraRenderer.transform.DOMoveY(sakuraPosRunCenter.position.y + 0.3f, 0.6f).SetEase(Ease.InOutSine));
        sakuraActiveTrainingSeq.SetLoops(-1, LoopType.Yoyo);
    }

    private void PlaySakuraSwimTraining()
    {
        SetSakuraActiveBackground(sakuraBgSwimming);
        sakuraRenderer.sprite = sakuraSwimmingSprite;
        sakuraRenderer.transform.position = sakuraPosSwimming.position;

        sakuraRenderer.transform.DOMoveY(sakuraPosSwimming.position.y + 0.4f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        sakuraRenderer.transform.DOMoveX(sakuraPosSwimming.position.x + 1.2f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        Transform t = sakuraRenderer.transform;
        t.localRotation = Quaternion.Euler(0, 0, -10f);
        t.DOLocalRotate(new Vector3(0, 0, 10f), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void PlaySakuraDodgeTraining()
    {
        SetSakuraActiveBackground(sakuraBgPowAndEvade);
        sakuraRenderer.transform.position = sakuraPosEvadeLeft.position;
        sakuraCurrentEvadeStep = 0;
        RunSakuraNextEvadeStep();
    }

    private void RunSakuraNextEvadeStep()
    {
        if (this == null || !gameObject.activeInHierarchy) return;

        sakuraActiveTrainingSeq = DOTween.Sequence();

        bool isMovingRight = (sakuraCurrentEvadeStep % 2 == 0);
        Transform startPos = isMovingRight ? sakuraPosEvadeLeft : sakuraPosEvadeRight;
        Transform endPos = isMovingRight ? sakuraPosEvadeRight : sakuraPosEvadeLeft;

        Sprite[] evadeSprites = { sakuraEvade1Sprite, sakuraEvade2Sprite, sakuraEvade3Sprite };
        Sprite targetSprite = evadeSprites[sakuraCurrentEvadeStep % 3];

        float randomYOffset = UnityEngine.Random.Range(-evadeRandomYRange, evadeRandomYRange);
        Vector3 targetPosition = new Vector3(endPos.position.x, startPos.position.y + randomYOffset, startPos.position.z);

        sakuraActiveTrainingSeq.AppendCallback(() => {
            sakuraRenderer.sprite = sakuraSpinningSprite;
            sakuraRenderer.flipX = !isMovingRight;
            sakuraRenderer.transform.position = startPos.position;
            sakuraRenderer.transform.rotation = Quaternion.identity;
        });

        sakuraActiveTrainingSeq.Append(sakuraRenderer.transform.DOMove(targetPosition, evadeMoveDuration).SetEase(Ease.OutSine));
        sakuraActiveTrainingSeq.Insert(0, sakuraRenderer.transform.DORotate(new Vector3(0, 360f * evadeSpins, 0), evadeMoveDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        int totalBalls = UnityEngine.Random.Range(3, 6);
        float timeStep = evadeMoveDuration / totalBalls;
        bool isLeftTurn = UnityEngine.Random.value > 0.5f;

        for (int i = 0; i < totalBalls; i++)
        {
            float spawnDelay = timeStep * i;
            Transform spawnPoint = isLeftTurn ? sakuraPosBallSpawnLeft : sakuraPosBallSpawnRight;
            Transform destPoint = isLeftTurn ? sakuraPosBallSpawnRight : sakuraPosBallSpawnLeft;

            sakuraActiveTrainingSeq.InsertCallback(spawnDelay, () => ShootSakuraTennisBall(spawnPoint, destPoint));
            isLeftTurn = !isLeftTurn;
        }

        sakuraActiveTrainingSeq.AppendCallback(() => {
            sakuraRenderer.transform.rotation = Quaternion.identity;
            sakuraRenderer.sprite = targetSprite;
            sakuraRenderer.flipX = isMovingRight;
        });

        sakuraActiveTrainingSeq.AppendInterval(evadePauseDuration);
        sakuraActiveTrainingSeq.OnComplete(() => {
            sakuraCurrentEvadeStep++;
            RunSakuraNextEvadeStep();
        });
    }

    private void ShootSakuraTennisBall(Transform spawnPoint, Transform destPoint)
    {
        if (tennisBallPrefab == null || spawnPoint == null || destPoint == null) return;
        if (!PoolManager.Pools.ContainsKey(tennisBallPoolName)) return;

        float randomY = UnityEngine.Random.Range(-evadeRandomYRange, evadeRandomYRange);
        Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + randomY, spawnPoint.position.z);
        Vector3 endPos = new Vector3(destPoint.position.x, destPoint.position.y + randomY, destPoint.position.z);

        Transform ball = PoolManager.Pools[tennisBallPoolName].Spawn(tennisBallPrefab.transform, spawnPos, Quaternion.identity, sakuraBallContainer);

        if (ball != null)
        {
            sakuraActiveBalls.Add(ball);
            ball.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

            ball.DOMove(endPos, ballFlySpeed).SetEase(Ease.Linear).OnComplete(() => {
                DOTween.Kill(ball);
                sakuraActiveBalls.Remove(ball);

                // THAY ĐỔI QUAN TRỌNG: Cứu bóng ra trước khi cất
                ball.SetParent(null);

                if (PoolManager.Pools.ContainsKey(tennisBallPoolName))
                {
                    PoolManager.Pools[tennisBallPoolName].DespawnObject(ball);
                }
            });
        }
    }
}