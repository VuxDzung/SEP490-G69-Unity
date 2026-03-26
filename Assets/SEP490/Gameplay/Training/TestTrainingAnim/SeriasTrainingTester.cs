using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class SeriasTrainingTester : MonoBehaviour
{
    [Header("Giao diện nút bấm")]
    [SerializeField] private Button buttonTrainPower;
    [SerializeField] private Button buttonTrainDodge;
    [SerializeField] private Button buttonTrainRun;
    [SerializeField] private Button buttonTrainIntelligence;
    [SerializeField] private Button buttonTrainSwim; // THÊM MỚI: Nút bơi

    [Header("Thành phần của Serias")]
    [SerializeField] private SpriteRenderer seriasRenderer;
    [SerializeField] private Animator seriasAnimator;
    [SerializeField] private Sprite spriteSeriasWindUp;

    [Header("Sprites - POW")]
    [SerializeField] private Sprite spriteSeriasHurricaneKick;

    [Header("Sprites - DODGE")]
    [SerializeField] private Sprite spriteSeriasLanding;
    [SerializeField] private Sprite spriteSeriasSpinning;

    [Header("Sprites - STUDY")]
    [SerializeField] private Sprite spriteSeriasStudy;

    [Header("Sprites - SWIM")]
    [SerializeField] private Sprite spriteSeriasSwimming; // THÊM MỚI: Hình bơi

    [Header("Môi trường")]
    [SerializeField] private GameObject bgPowerAndDodge;
    [SerializeField] private GameObject bgRunning;
    [SerializeField] private GameObject bgStudy;
    [SerializeField] private GameObject bgSwimming; // THÊM MỚI: Background bơi

    [Header("Môi trường - Cuộn (Parallax cho Running)")]
    [SerializeField] private SpriteRenderer[] scrollingLayers;
    [SerializeField] private float[] scrollSpeeds = { 0.05f, 0.2f, 0.5f, 0.8f };

    [Header("Tương tác - Bao cát (POW)")]
    [SerializeField] private SpriteRenderer bagRenderer;
    [SerializeField] private Sprite spriteBagNormal;
    [SerializeField] private Sprite spriteBagDeform;

    [Header("Đạo cụ - Bóng bay (DODGE)")]
    [SerializeField] private GameObject tennisBallPrefab;
    [SerializeField] private string tennisBallPoolName = "TennisBallPool";
    [SerializeField] private float ballFlySpeed = 0.5f;
    [SerializeField] private Transform posBallSpawnLeft;
    [SerializeField] private Transform posBallSpawnRight;
    [SerializeField] private Transform ballContainer;

    [Header("Các vị trí - POW")]
    [SerializeField] private Transform positionStart;
    [SerializeField] private Transform positionBackWindUp;
    [SerializeField] private Transform positionImpact;
    [SerializeField] private Transform positionLanding;

    [Header("Các vị trí - DODGE")]
    [SerializeField] private Transform posEvadeLeft;
    [SerializeField] private Transform posEvadeRight;

    [Header("Các vị trí - RUN")]
    [SerializeField] private Transform posRunCenter;

    [Header("Các vị trí - STUDY")]
    [SerializeField] private Transform posStudy;

    [Header("Các vị trí - SWIM")]
    [SerializeField] private Transform posSwimming; // THÊM MỚI: Vị trí bơi

    [Header("Cấu hình - DODGE")]
    [SerializeField] private float evadeJumpPower = 2.5f;
    [SerializeField] private float evadeMoveDuration = 0.5f;
    [SerializeField] private float evadePauseDuration = 0.4f;
    [SerializeField] private int evadeSpins = 3;
    [SerializeField] private float evadeRandomYRange = 0.5f;

    private Vector3 bagOriginalPosition;
    private Vector3 bagOriginalScale;

    private Vector3 seriasOriginalScale;

    private Sequence activeTrainingSequence;
    private int currentEvadeStep = 0;
    private List<Transform> activeBalls = new List<Transform>();

    private bool isScrolling = false;

    void Start()
    {
        if (bagRenderer != null)
        {
            bagOriginalPosition = bagRenderer.transform.position;
            bagOriginalScale = bagRenderer.transform.localScale;
        }

        if (seriasRenderer != null)
        {
            seriasOriginalScale = seriasRenderer.transform.localScale;
        }

        if (buttonTrainPower) buttonTrainPower.onClick.AddListener(PlayHurricaneKickTraining);
        if (buttonTrainDodge) buttonTrainDodge.onClick.AddListener(PlayDodgeTraining);
        if (buttonTrainRun) buttonTrainRun.onClick.AddListener(PlayRunTraining);
        if (buttonTrainIntelligence) buttonTrainIntelligence.onClick.AddListener(PlayStudyTraining);
        if (buttonTrainSwim) buttonTrainSwim.onClick.AddListener(PlaySwimmingTraining); // Bắt sự kiện

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

        if (seriasRenderer != null)
        {
            DOTween.Kill(seriasRenderer.transform);
            seriasRenderer.transform.localScale = seriasOriginalScale;
        }

        if (seriasAnimator != null) DOTween.Kill(seriasAnimator.transform);
        if (bagRenderer != null) DOTween.Kill(bagRenderer.transform);

        foreach (var ball in activeBalls)
        {
            if (ball != null) DOTween.Kill(ball);
        }
        activeBalls.Clear();

        // Fix tạm thời: Thay đổi tùy thuộc vào thư viện Pool Manager bạn đang dùng
        // Dòng này đang dùng class giả định SEP490G69, vui lòng đảm bảo đúng namespace của bạn.
        /* if (SEP490G69.PoolManager.Pools.ContainsKey(tennisBallPoolName))
        {
            SEP490G69.PoolManager.Pools[tennisBallPoolName].DespawnAll();
        }
        */

        isScrolling = false;

        if (seriasRenderer != null)
        {
            seriasRenderer.gameObject.SetActive(true);
            seriasRenderer.transform.position = positionStart.position;
            seriasRenderer.transform.rotation = Quaternion.identity;
            seriasRenderer.flipX = false;
        }

        if (seriasAnimator != null)
        {
            seriasAnimator.gameObject.SetActive(false);
        }

        if (bagRenderer != null)
        {
            bagRenderer.transform.position = bagOriginalPosition;
            bagRenderer.transform.localScale = bagOriginalScale;
            bagRenderer.sprite = spriteBagNormal;
            bagRenderer.enabled = false;
        }
    }

    private void SetActiveBackground(GameObject activeBg)
    {
        if (bgPowerAndDodge) bgPowerAndDodge.SetActive(false);
        if (bgRunning) bgRunning.SetActive(false);
        if (bgStudy) bgStudy.SetActive(false);
        if (bgSwimming) bgSwimming.SetActive(false); // Xử lý tắt nền bơi

        if (activeBg) activeBg.SetActive(true);
    }

    // ================= BÀI TẬP SWIM (BƠI) =================
    private void PlaySwimmingTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgSwimming);

        seriasRenderer.sprite = spriteSeriasSwimming;
        seriasRenderer.transform.position = posSwimming != null ? posSwimming.position : positionStart.position;
        seriasRenderer.flipX = false;

        // Sequence ảo để gom chung quản lý (thực tế các tween này chạy song song, set yoyo vĩnh viễn)
        activeTrainingSequence = DOTween.Sequence();

        Transform t = seriasRenderer.transform;

        // 1. Nhấp nhô lên xuống nhẹ
        activeTrainingSequence.Join(t.DOMoveY(t.position.y + 0.4f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));

        // 2. Trôi về phía trước rồi lùi lại
        activeTrainingSequence.Join(t.DOMoveX(t.position.x + 1.2f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));

        // 3. Lắc lư cơ thể (Góc -10 đến 10 độ)
        t.localRotation = Quaternion.Euler(0, 0, -10f);
        activeTrainingSequence.Join(t.DOLocalRotate(new Vector3(0, 0, 10f), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
    }

    // ================= BÀI TẬP STUDY (HỌC TẬP) =================
    private void PlayStudyTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgStudy);

        seriasRenderer.sprite = spriteSeriasStudy;
        seriasRenderer.transform.position = posStudy != null ? posStudy.position : positionStart.position;
        seriasRenderer.flipX = false;

        activeTrainingSequence = DOTween.Sequence();

        Vector3 targetScale = new Vector3(
            seriasOriginalScale.x * 1.07f,
            seriasOriginalScale.y * 0.93f,
            seriasOriginalScale.z
        );

        activeTrainingSequence.Append(seriasRenderer.transform.DOScale(targetScale, 0.35f).SetEase(Ease.InOutCubic));

        activeTrainingSequence.SetLoops(-1, LoopType.Yoyo);
    }

    // ================= BÀI TẬP RUN (CHẠY BỘ) =================
    private void PlayRunTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgRunning);
        isScrolling = true;

        if (seriasRenderer != null) seriasRenderer.gameObject.SetActive(false);

        if (seriasAnimator != null)
        {
            seriasAnimator.gameObject.SetActive(true);
            seriasAnimator.transform.position = posRunCenter.position;
            seriasAnimator.transform.rotation = Quaternion.identity;
            seriasAnimator.Play("SeriasRunning");

            activeTrainingSequence = DOTween.Sequence();
            activeTrainingSequence.Append(seriasAnimator.transform.DOMoveY(posRunCenter.position.y + 0.1f, 0.3f).SetEase(Ease.InOutSine));
            activeTrainingSequence.SetLoops(-1, LoopType.Yoyo);
        }
    }

    // ================= BÀI TẬP POW (HURRICANE KICK) =================
    private void PlayHurricaneKickTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgPowerAndDodge);
        bagRenderer.enabled = true;

        activeTrainingSequence = DOTween.Sequence();

        Vector3 windUpPos = positionBackWindUp != null ? positionBackWindUp.position : positionStart.position + new Vector3(-0.5f, 0, 0);
        Vector3 landingPos = positionLanding != null ? positionLanding.position : positionStart.position + new Vector3(0.5f, 0, 0);

        float tSetup = 0f;
        float tStartAttack = 0.2f;
        float flyDuration = 0.3f;
        float hoverDuration = 0.6f;
        float dropDuration = 0.15f;
        float slideBackDuration = 0.3f;

        int totalSpins = 6;
        float totalSpinDuration = flyDuration + hoverDuration;

        activeTrainingSequence.InsertCallback(tSetup, () => {
            seriasRenderer.transform.position = windUpPos;
            seriasRenderer.sprite = spriteSeriasWindUp;
            seriasRenderer.transform.rotation = Quaternion.identity;
        });

        activeTrainingSequence.InsertCallback(tStartAttack, () => {
            if (spriteSeriasHurricaneKick != null) seriasRenderer.sprite = spriteSeriasHurricaneKick;
        });

        activeTrainingSequence.Insert(tStartAttack, seriasRenderer.transform.DOMove(positionImpact.position, flyDuration).SetEase(Ease.OutSine));
        activeTrainingSequence.Insert(tStartAttack, seriasRenderer.transform.DORotate(new Vector3(0, 360f * totalSpins, 0), totalSpinDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        float tImpact = tStartAttack + flyDuration;
        activeTrainingSequence.InsertCallback(tImpact, () => {
            bagRenderer.sprite = spriteBagDeform;
            int impactVibrato = totalSpins * 6;
            bagRenderer.transform.DOShakePosition(hoverDuration, strength: new Vector3(0.35f, 0, 0), vibrato: impactVibrato);
            bagRenderer.transform.DOShakeScale(hoverDuration, strength: 0.15f, vibrato: impactVibrato);
        });

        float tDrop = tImpact + hoverDuration;
        activeTrainingSequence.InsertCallback(tDrop, () => {
            seriasRenderer.transform.rotation = Quaternion.identity;
            seriasRenderer.sprite = spriteSeriasWindUp;
        });

        activeTrainingSequence.Insert(tDrop, seriasRenderer.transform.DOMove(landingPos, dropDuration).SetEase(Ease.InQuad));

        float tResetAndSlide = tDrop + dropDuration;
        activeTrainingSequence.InsertCallback(tResetAndSlide, () => {
            bagRenderer.sprite = spriteBagNormal;
            bagRenderer.transform.position = bagOriginalPosition;
            bagRenderer.transform.localScale = bagOriginalScale;
        });

        activeTrainingSequence.Insert(tResetAndSlide, seriasRenderer.transform.DOMove(windUpPos, slideBackDuration).SetEase(Ease.OutExpo));

        float tEnd = tResetAndSlide + slideBackDuration + 0.1f;
        activeTrainingSequence.InsertCallback(tEnd, () => { });
        activeTrainingSequence.SetLoops(-1, LoopType.Restart);
    }

    // ================= BÀI TẬP DODGE (NÉ TRÁNH) =================
    private void PlayDodgeTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgPowerAndDodge);

        seriasRenderer.transform.position = posEvadeLeft.position;
        seriasRenderer.sprite = spriteSeriasLanding;
        seriasRenderer.flipX = false;

        currentEvadeStep = 0;
        RunNextDodgeStep();
    }

    private void RunNextDodgeStep()
    {
        if (this == null || !gameObject.activeInHierarchy) return;

        activeTrainingSequence = DOTween.Sequence();

        bool isMovingRight = (currentEvadeStep % 2 == 0);
        Transform startPos = isMovingRight ? posEvadeLeft : posEvadeRight;
        Transform endPos = isMovingRight ? posEvadeRight : posEvadeLeft;

        float spinAngleZ = isMovingRight ? -360f * evadeSpins : 360f * evadeSpins;

        activeTrainingSequence.AppendCallback(() => {
            seriasRenderer.sprite = spriteSeriasSpinning;
            seriasRenderer.flipX = !isMovingRight;
            seriasRenderer.transform.position = startPos.position;
            seriasRenderer.transform.rotation = Quaternion.identity;
        });

        activeTrainingSequence.Append(seriasRenderer.transform.DOJump(endPos.position, evadeJumpPower, 1, evadeMoveDuration).SetEase(Ease.Linear));
        activeTrainingSequence.Insert(0, seriasRenderer.transform.DORotate(new Vector3(0, 0, spinAngleZ), evadeMoveDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        int totalBalls = Random.Range(3, 6);
        float timeStep = evadeMoveDuration / totalBalls;
        bool isLeftTurn = Random.value > 0.5f;

        for (int i = 0; i < totalBalls; i++)
        {
            float spawnDelay = timeStep * i;
            Transform spawnPoint = isLeftTurn ? posBallSpawnLeft : posBallSpawnRight;
            Transform destPoint = isLeftTurn ? posBallSpawnRight : posBallSpawnLeft;

            activeTrainingSequence.InsertCallback(spawnDelay, () => ShootTennisBall(spawnPoint, destPoint));
            isLeftTurn = !isLeftTurn;
        }

        activeTrainingSequence.AppendCallback(() => {
            seriasRenderer.transform.rotation = Quaternion.identity;
            seriasRenderer.sprite = spriteSeriasLanding;
            seriasRenderer.flipX = !isMovingRight;
        });

        activeTrainingSequence.Append(seriasRenderer.transform.DOPunchScale(new Vector3(0.02f, -0.05f, 0), 0.15f, 2, 0.5f));
        activeTrainingSequence.Join(seriasRenderer.transform.DOShakePosition(0.1f, new Vector3(0f, 0.05f, 0f), 10));

        activeTrainingSequence.AppendInterval(evadePauseDuration);
        activeTrainingSequence.OnComplete(() => {
            currentEvadeStep++;
            RunNextDodgeStep();
        });
    }

    private void ShootTennisBall(Transform spawnPoint, Transform destPoint)
    {
        if (tennisBallPrefab == null || spawnPoint == null || destPoint == null) return;

        if (!SEP490G69.PoolManager.Pools.ContainsKey(tennisBallPoolName)) return;

        float randomY = Random.Range(-evadeRandomYRange, evadeRandomYRange);
        Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + randomY, spawnPoint.position.z);
        Vector3 endPos = new Vector3(destPoint.position.x, destPoint.position.y + randomY, destPoint.position.z);

        Transform ball = SEP490G69.PoolManager.Pools[tennisBallPoolName].Spawn(tennisBallPrefab.transform, spawnPos, Quaternion.identity, ballContainer);

        if (ball != null)
        {
            activeBalls.Add(ball);
            ball.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            ball.DOMove(endPos, ballFlySpeed).SetEase(Ease.Linear).OnComplete(() => {
                DOTween.Kill(ball);
                activeBalls.Remove(ball);
                if (SEP490G69.PoolManager.Pools.ContainsKey(tennisBallPoolName))
                {
                    SEP490G69.PoolManager.Pools[tennisBallPoolName].DespawnObject(ball);
                }
            });
        }
    }
}