using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using SEP490G69;
using SEP490G69.Training;
using System;

public class SeriasTrainingAnimationController : BaseTrainingAnimationController
{
    [Header("Thành phần của Serias")]
    [SerializeField] private SpriteRenderer seriasRenderer; // Cho POW, DODGE, STUDY, SWIM
    [SerializeField] private Animator seriasAnimator;       // Cho RUN
    [SerializeField] private Sprite spriteSeriasWindUp;

    [Header("Sprites - POW")]
    [SerializeField] private Sprite spriteSeriasHurricaneKick;

    [Header("Sprites - DODGE")]
    [SerializeField] private Sprite spriteSeriasLanding;
    [SerializeField] private Sprite spriteSeriasSpinning;

    [Header("Sprites - STUDY & SWIM")]
    [SerializeField] private Sprite spriteSeriasStudy;
    [SerializeField] private Sprite spriteSeriasSwimming;

    [Header("Môi trường (Backgrounds)")]
    [SerializeField] private GameObject bgPowerAndDodge;
    [SerializeField] private GameObject bgRunning;
    [SerializeField] private GameObject bgStudy;
    [SerializeField] private GameObject bgSwimming;

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

    [Header("Các vị trí - POW & STUDY & SWIM")]
    [SerializeField] private Transform positionStart;
    [SerializeField] private Transform positionBackWindUp;
    [SerializeField] private Transform positionImpact;
    [SerializeField] private Transform positionLanding;
    [SerializeField] private Transform posStudy;
    [SerializeField] private Transform posSwimming;

    [Header("Các vị trí - DODGE & RUN")]
    [SerializeField] private Transform posEvadeLeft;
    [SerializeField] private Transform posEvadeRight;
    [SerializeField] private Transform posRunCenter;

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
    private Tween seriasTimerTween; // Đếm ngược 3s cho training thật
    private int currentEvadeStep = 0;
    private List<Transform> activeBalls = new List<Transform>();
    private bool isScrolling = false;

    void Awake()
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

    public override void StopAllAnimations()
    {
        if (activeTrainingSequence != null) activeTrainingSequence.Kill();
        if (seriasTimerTween != null) seriasTimerTween.Kill();

        if (seriasRenderer != null)
        {
            DOTween.Kill(seriasRenderer.transform);
            seriasRenderer.transform.localScale = seriasOriginalScale;
        }

        if (seriasAnimator != null) DOTween.Kill(seriasAnimator.transform);
        if (bagRenderer != null) DOTween.Kill(bagRenderer.transform);

        // THAY ĐỔI QUAN TRỌNG: Cứu các quả bóng ra khỏi Container trước khi vứt vào Pool
        foreach (var ball in activeBalls)
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
        activeBalls.Clear();

        isScrolling = false;

        if (seriasRenderer != null)
        {
            seriasRenderer.transform.position = positionStart != null ? positionStart.position : Vector3.zero;
            seriasRenderer.transform.rotation = Quaternion.identity;
            seriasRenderer.flipX = false;
            seriasRenderer.gameObject.SetActive(false); // Ẩn khi không tập
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

        SetActiveBackground(null);
    }

    private void SetActiveBackground(GameObject activeBg)
    {
        if (bgPowerAndDodge) bgPowerAndDodge.SetActive(false);
        if (bgRunning) bgRunning.SetActive(false);
        if (bgStudy) bgStudy.SetActive(false);
        if (bgSwimming) bgSwimming.SetActive(false);

        if (activeBg) activeBg.SetActive(true);
    }

    // ================== HÀM GỌI TỪ BÊN NGOÀI (GAME CONTROLLER) ==================
    public override void PlayTrainingAnim(ETrainingType trainingType, Action onComplete)
    {
        StopAllAnimations();

        // Mặc định bật Sprite Renderer (ngoại trừ bài RUN sẽ tự chuyển sang Animator)
        if (seriasRenderer != null) seriasRenderer.gameObject.SetActive(true);

        switch (trainingType)
        {
            case ETrainingType.Boxing: PlayHurricaneKickTraining(); break;
            case ETrainingType.Study: PlayStudyTraining(); break;
            case ETrainingType.Run: PlayRunTraining(); break;
            case ETrainingType.Swim: PlaySwimmingTraining(); break;
            case ETrainingType.Dodge: PlayDodgeTraining(); break;
            default: PlayStudyTraining(); break;
        }

        // Đếm ngược 3 giây, sau đó dừng toàn bộ và gọi hàm callback trả kết quả
        seriasTimerTween = DOVirtual.DelayedCall(3f, () =>
        {
            StopAllAnimations();
            onComplete?.Invoke();
        });
    }

    // ================= BÀI TẬP POW (HURRICANE KICK) =================
    private void PlayHurricaneKickTraining()
    {
        SetActiveBackground(bgPowerAndDodge);
        if (bagRenderer != null) bagRenderer.enabled = true;

        activeTrainingSequence = DOTween.Sequence();

        Vector3 windUpPos = positionBackWindUp != null ? positionBackWindUp.position : (positionStart != null ? positionStart.position + new Vector3(-0.5f, 0, 0) : Vector3.zero);
        Vector3 impactPos = positionImpact != null ? positionImpact.position : Vector3.zero;
        Vector3 landingPos = positionLanding != null ? positionLanding.position : (positionStart != null ? positionStart.position + new Vector3(0.5f, 0, 0) : Vector3.zero);

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

        activeTrainingSequence.Insert(tStartAttack, seriasRenderer.transform.DOMove(impactPos, flyDuration).SetEase(Ease.OutSine));
        activeTrainingSequence.Insert(tStartAttack, seriasRenderer.transform.DORotate(new Vector3(0, 360f * totalSpins, 0), totalSpinDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        float tImpact = tStartAttack + flyDuration;
        activeTrainingSequence.InsertCallback(tImpact, () => {
            if (bagRenderer != null)
            {
                bagRenderer.sprite = spriteBagDeform;
                int impactVibrato = totalSpins * 6;
                bagRenderer.transform.DOShakePosition(hoverDuration, strength: new Vector3(0.35f, 0, 0), vibrato: impactVibrato);
                bagRenderer.transform.DOShakeScale(hoverDuration, strength: 0.15f, vibrato: impactVibrato);
            }
        });

        float tDrop = tImpact + hoverDuration;
        activeTrainingSequence.InsertCallback(tDrop, () => {
            seriasRenderer.transform.rotation = Quaternion.identity;
            seriasRenderer.sprite = spriteSeriasWindUp;
        });

        activeTrainingSequence.Insert(tDrop, seriasRenderer.transform.DOMove(landingPos, dropDuration).SetEase(Ease.InQuad));

        float tResetAndSlide = tDrop + dropDuration;
        activeTrainingSequence.InsertCallback(tResetAndSlide, () => {
            if (bagRenderer != null)
            {
                bagRenderer.sprite = spriteBagNormal;
                bagRenderer.transform.position = bagOriginalPosition;
                bagRenderer.transform.localScale = bagOriginalScale;
            }
        });

        activeTrainingSequence.Insert(tResetAndSlide, seriasRenderer.transform.DOMove(windUpPos, slideBackDuration).SetEase(Ease.OutExpo));

        float tEnd = tResetAndSlide + slideBackDuration + 0.1f;
        activeTrainingSequence.InsertCallback(tEnd, () => { });
        activeTrainingSequence.SetLoops(-1, LoopType.Restart);
    }

    // ================= BÀI TẬP STUDY (HỌC TẬP) =================
    private void PlayStudyTraining()
    {
        SetActiveBackground(bgStudy);

        seriasRenderer.sprite = spriteSeriasStudy;
        seriasRenderer.transform.position = posStudy != null ? posStudy.position : (positionStart != null ? positionStart.position : Vector3.zero);
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
        SetActiveBackground(bgRunning);
        isScrolling = true;

        if (seriasRenderer != null) seriasRenderer.gameObject.SetActive(false);

        if (seriasAnimator != null)
        {
            seriasAnimator.gameObject.SetActive(true);
            seriasAnimator.transform.position = posRunCenter != null ? posRunCenter.position : Vector3.zero;
            seriasAnimator.transform.rotation = Quaternion.identity;
            seriasAnimator.Play("SeriasRunning");

            activeTrainingSequence = DOTween.Sequence();
            activeTrainingSequence.Append(seriasAnimator.transform.DOMoveY(seriasAnimator.transform.position.y + 0.1f, 0.3f).SetEase(Ease.InOutSine));
            activeTrainingSequence.SetLoops(-1, LoopType.Yoyo);
        }
    }

    // ================= BÀI TẬP SWIM (BƠI) =================
    private void PlaySwimmingTraining()
    {
        SetActiveBackground(bgSwimming);

        seriasRenderer.sprite = spriteSeriasSwimming;
        seriasRenderer.transform.position = posSwimming != null ? posSwimming.position : (positionStart != null ? positionStart.position : Vector3.zero);
        seriasRenderer.flipX = false;

        activeTrainingSequence = DOTween.Sequence();
        Transform t = seriasRenderer.transform;

        activeTrainingSequence.Join(t.DOMoveY(t.position.y + 0.4f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
        activeTrainingSequence.Join(t.DOMoveX(t.position.x + 1.2f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));

        t.localRotation = Quaternion.Euler(0, 0, -10f);
        activeTrainingSequence.Join(t.DOLocalRotate(new Vector3(0, 0, 10f), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
    }

    // ================= BÀI TẬP DODGE (NÉ TRÁNH) =================
    private void PlayDodgeTraining()
    {
        SetActiveBackground(bgPowerAndDodge);

        seriasRenderer.transform.position = posEvadeLeft != null ? posEvadeLeft.position : Vector3.zero;
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

        if (startPos == null || endPos == null) return;

        float spinAngleZ = isMovingRight ? -360f * evadeSpins : 360f * evadeSpins;

        activeTrainingSequence.AppendCallback(() => {
            seriasRenderer.sprite = spriteSeriasSpinning;
            seriasRenderer.flipX = !isMovingRight;
            seriasRenderer.transform.position = startPos.position;
            seriasRenderer.transform.rotation = Quaternion.identity;
        });

        activeTrainingSequence.Append(seriasRenderer.transform.DOJump(endPos.position, evadeJumpPower, 1, evadeMoveDuration).SetEase(Ease.Linear));
        activeTrainingSequence.Insert(0, seriasRenderer.transform.DORotate(new Vector3(0, 0, spinAngleZ), evadeMoveDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        int totalBalls = UnityEngine.Random.Range(3, 6);
        float timeStep = evadeMoveDuration / totalBalls;
        bool isLeftTurn = UnityEngine.Random.value > 0.5f;

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
        if (!PoolManager.Pools.ContainsKey(tennisBallPoolName)) return;

        float randomY = UnityEngine.Random.Range(-evadeRandomYRange, evadeRandomYRange);
        Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + randomY, spawnPoint.position.z);
        Vector3 endPos = new Vector3(destPoint.position.x, destPoint.position.y + randomY, destPoint.position.z);

        Transform ball = PoolManager.Pools[tennisBallPoolName].Spawn(tennisBallPrefab.transform, spawnPos, Quaternion.identity, ballContainer);

        if (ball != null)
        {
            activeBalls.Add(ball);
            ball.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            ball.DOMove(endPos, ballFlySpeed).SetEase(Ease.Linear).OnComplete(() => {
                DOTween.Kill(ball);
                activeBalls.Remove(ball);

                // Cứu bóng ra trước khi cất
                ball.SetParent(null);

                if (PoolManager.Pools.ContainsKey(tennisBallPoolName))
                {
                    PoolManager.Pools[tennisBallPoolName].DespawnObject(ball);
                }
            });
        }
    }
}