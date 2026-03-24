using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SeriasTrainingTester : MonoBehaviour
{
    [Header("Giao diện nút bấm")]
    [SerializeField] private Button buttonTrainPower; // Nút để test bài tập Pow

    [Header("Thành phần của Serias")]
    [SerializeField] private SpriteRenderer seriasRenderer;
    [SerializeField] private Sprite spriteSeriasWindUp;
    [SerializeField] private Sprite spriteSeriasHurricaneKick;

    [Header("Môi trường")]
    [SerializeField] private GameObject bgPower;

    [Header("Tương tác - Bao cát")]
    [SerializeField] private SpriteRenderer bagRenderer;
    [SerializeField] private Sprite spriteBagNormal;
    [SerializeField] private Sprite spriteBagDeform;

    [Header("Các vị trí - POW")]
    [SerializeField] private Transform positionStart;
    [SerializeField] private Transform positionBackWindUp; // Điểm lùi nhẹ về sau lấy đà
    [SerializeField] private Transform positionImpact;     // Điểm lơ lửng va chạm bao cát
    [SerializeField] private Transform positionLanding;    // THÊM MỚI: Điểm rớt xuống đất sau khi đá xong

    private Vector3 bagOriginalPosition;
    private Vector3 bagOriginalScale;

    private Sequence activeTrainingSequence;

    void Start()
    {
        bagOriginalPosition = bagRenderer.transform.position;
        bagOriginalScale = bagRenderer.transform.localScale;

        if (buttonTrainPower) buttonTrainPower.onClick.AddListener(PlayHurricaneKickTraining);

        StopAllAnimations();
    }

    private void StopAllAnimations()
    {
        if (activeTrainingSequence != null) activeTrainingSequence.Kill();
        DOTween.Kill(seriasRenderer.transform);
        DOTween.Kill(bagRenderer.transform);

        seriasRenderer.transform.position = positionStart.position;
        seriasRenderer.transform.rotation = Quaternion.identity;
        seriasRenderer.flipX = false;

        bagRenderer.transform.position = bagOriginalPosition;
        bagRenderer.transform.localScale = bagOriginalScale;
        bagRenderer.sprite = spriteBagNormal;
        bagRenderer.enabled = false;
    }

    private void SetActiveBackground(GameObject activeBg)
    {
        if (bgPower) bgPower.SetActive(false);
        if (activeBg) activeBg.SetActive(true);
    }

    // ================= BÀI TẬP POW (HURRICANE KICK) =================
    private void PlayHurricaneKickTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgPower);
        bagRenderer.enabled = true;

        activeTrainingSequence = DOTween.Sequence();

        // Lấy các vị trí (nếu chưa kéo thả transform thì dùng fallback tự tính toán)
        Vector3 windUpPos = positionBackWindUp != null ? positionBackWindUp.position : positionStart.position + new Vector3(-0.5f, 0, 0);
        Vector3 landingPos = positionLanding != null ? positionLanding.position : positionStart.position + new Vector3(0.5f, 0, 0);

        // --- CÁC MỐC THỜI GIAN (Điều chỉnh ở đây) ---
        float tSetup = 0f;
        float tStartAttack = 0.2f;               // Nghỉ 0.2s để gom lực
        float flyDuration = 0.3f;                // Thời gian bay từ Windup đến Impact
        float hoverDuration = 0.6f;              // Thời gian lơ lửng xoay ở Impact
        float dropDuration = 0.15f;              // Thời gian rớt từ Impact xuống Landing
        float slideBackDuration = 0.3f;          // Thời gian lướt về lại Windup

        int totalSpins = 6;                      // Tổng số vòng xoay theo trục Y
        float totalSpinDuration = flyDuration + hoverDuration; // Vừa bay vừa lơ lửng đều xoay

        // 1. SETUP: Đứng ở chỗ Windup
        activeTrainingSequence.InsertCallback(tSetup, () => {
            seriasRenderer.transform.position = windUpPos;
            seriasRenderer.sprite = spriteSeriasWindUp;
            seriasRenderer.transform.rotation = Quaternion.identity;
        });

        // 2. BẮT ĐẦU BAY VÀ XOAY
        activeTrainingSequence.InsertCallback(tStartAttack, () => {
            if (spriteSeriasHurricaneKick != null) seriasRenderer.sprite = spriteSeriasHurricaneKick;
        });

        // Di chuyển dần đến Impact
        activeTrainingSequence.Insert(tStartAttack, seriasRenderer.transform.DOMove(positionImpact.position, flyDuration).SetEase(Ease.OutSine));
        // Xoay liên tục theo trục Y (bao phủ cả lúc bay và lúc lơ lửng)
        activeTrainingSequence.Insert(tStartAttack, seriasRenderer.transform.DORotate(new Vector3(0, 360f * totalSpins, 0), totalSpinDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        // 3. IMPACT (Chạm bao cát)
        float tImpact = tStartAttack + flyDuration;
        activeTrainingSequence.InsertCallback(tImpact, () => {
            bagRenderer.sprite = spriteBagDeform;

            // Rung bao cát dựa theo số vòng đã xoay
            int impactVibrato = totalSpins * 6;
            bagRenderer.transform.DOShakePosition(hoverDuration, strength: new Vector3(0.35f, 0, 0), vibrato: impactVibrato);
            bagRenderer.transform.DOShakeScale(hoverDuration, strength: 0.15f, vibrato: impactVibrato);
        });

        // 4. HẠ CÁNH (Rớt xuống Landing Pos)
        float tDrop = tImpact + hoverDuration;
        activeTrainingSequence.Insert(tDrop, seriasRenderer.transform.DOMove(landingPos, dropDuration).SetEase(Ease.InQuad));

        // 5. TRƯỢT VỀ (Reset sprite và trượt về Windup)
        float tResetAndSlide = tDrop + dropDuration;
        activeTrainingSequence.InsertCallback(tResetAndSlide, () => {
            // Dừng xoay, chạm đất
            seriasRenderer.transform.rotation = Quaternion.identity;
            seriasRenderer.sprite = spriteSeriasWindUp;

            // Reset bao cát
            bagRenderer.sprite = spriteBagNormal;
            bagRenderer.transform.position = bagOriginalPosition;
            bagRenderer.transform.localScale = bagOriginalScale;
        });

        // Trượt lùi từ Landing về Windup
        activeTrainingSequence.Insert(tResetAndSlide, seriasRenderer.transform.DOMove(windUpPos, slideBackDuration).SetEase(Ease.OutExpo));

        // 6. KẾT THÚC SEQUENCE & LẶP LẠI
        float tEnd = tResetAndSlide + slideBackDuration + 0.1f; // Nghỉ 0.1s trước vòng mới
        activeTrainingSequence.InsertCallback(tEnd, () => { }); // Trick nhỏ để DOTween biết Sequence dài bao lâu
        activeTrainingSequence.SetLoops(-1, LoopType.Restart);
    }
}