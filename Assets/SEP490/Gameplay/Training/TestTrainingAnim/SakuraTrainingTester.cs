using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SakuraTrainingTester : MonoBehaviour
{
    [Header("Giao diện nút bấm (Chỉ để Test)")]
    public Button buttonTrainPower;
    public Button buttonTrainIntelligence;
    public Button buttonTrainEvade;
    public Button buttonTrainRun;
    public Button buttonTrainSwim; // Nút test bơi

    [Header("Thành phần của Sakura")]
    public SpriteRenderer sakuraRenderer;
    public Sprite spriteSakuraWindUp;
    public Sprite spriteSakuraHeadbutt;
    public Sprite spriteSakuraStudy;
    public Sprite spriteSakuraRunning;
    public Sprite spriteSakuraSwimming; // Sprite cho bơi

    [Header("Sprites - Bài tập EVADE")]
    public Sprite spriteSakuraSpinning;
    public Sprite spriteSakuraEvade1;
    public Sprite spriteSakuraEvade2;
    public Sprite spriteSakuraEvade3;

    [Header("Các Backgrounds (Môi trường)")]
    public GameObject bgPowAndEvade;
    public GameObject bgStudy;
    public GameObject bgRunning;
    public GameObject bgSwimming;

    [Header("Môi trường - Tương tác")]
    public SpriteRenderer bagRenderer;
    public Sprite spriteBagNormal;
    public Sprite spriteBagDeform;

    [Header("Môi trường - Cuộn (Parallax cho Running)")]
    public SpriteRenderer[] scrollingLayers; // Kéo các lớp nền của Running vào đây
    public float[] scrollSpeeds = { 0.05f, 0.2f, 0.5f, 0.8f };

    [Header("Các vị trí - POW & STUDY")]
    public Transform positionStart;
    public Transform positionWindUp;
    public Transform positionImpact;
    public Transform posStudyLeft;
    public Transform posStudyRight;

    [Header("Các vị trí - EVADE, RUN & SWIM")]
    public Transform posEvadeLeft;
    public Transform posEvadeRight;
    public Transform posRunCenter;
    public Transform posSwimming; // Điểm trung tâm khi bơi

    public float evadeRandomYRange = 0.5f;
    [SerializeField] private float evadeMoveDuration = 0.25f;
    [SerializeField] private float evadePauseDuration = 0.3f;
    [SerializeField] private int evadeSpins = 3;

    private Vector3 bagOriginalPosition;
    private Vector3 bagOriginalScale;

    private Sequence activeTrainingSequence;
    private int currentEvadeStep = 0;

    // Cờ kiểm soát trạng thái
    private bool isScrolling = false;

    void Start()
    {
        bagOriginalPosition = bagRenderer.transform.position;
        bagOriginalScale = bagRenderer.transform.localScale;

        // Đăng ký sự kiện nút bấm
        if (buttonTrainPower) buttonTrainPower.onClick.AddListener(PlayPowerTraining);
        if (buttonTrainIntelligence) buttonTrainIntelligence.onClick.AddListener(PlayIntelligenceTraining);
        if (buttonTrainEvade) buttonTrainEvade.onClick.AddListener(PlayEvadeTraining);
        if (buttonTrainRun) buttonTrainRun.onClick.AddListener(PlayRunTraining);
        if (buttonTrainSwim) buttonTrainSwim.onClick.AddListener(PlaySwimmingTraining);

        StopAllAnimations();
    }

    void Update()
    {
        // Xử lý cuộn nền liên tục (chỉ chạy khi isScrolling = true)
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
        DOTween.Kill(sakuraRenderer.transform); // Dừng tất cả tween đang chạy trên Sakura
        DOTween.Kill(bagRenderer.transform);

        isScrolling = false;

        sakuraRenderer.transform.position = positionStart.position;
        sakuraRenderer.transform.rotation = Quaternion.identity;

        bagRenderer.transform.position = bagOriginalPosition;
        bagRenderer.transform.localScale = bagOriginalScale;
        bagRenderer.sprite = spriteBagNormal;
        bagRenderer.enabled = false;
    }

    // Hàm mới: Quản lý bật/tắt các Background
    private void SetActiveBackground(GameObject activeBg)
    {
        if (bgPowAndEvade) bgPowAndEvade.SetActive(false);
        if (bgStudy) bgStudy.SetActive(false);
        if (bgRunning) bgRunning.SetActive(false);
        if (bgSwimming) bgSwimming.SetActive(false);

        if (activeBg) activeBg.SetActive(true);
    }

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

        // Float logic
        sakuraRenderer.transform.DOMoveY(posStudyLeft.position.y + 0.5f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        sakuraRenderer.transform.DORotate(new Vector3(0, 0, 360f), 6f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

        activeTrainingSequence.SetLoops(-1, LoopType.Restart);
    }

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
        Transform startPos = (currentEvadeStep % 2 == 0) ? posEvadeLeft : posEvadeRight;
        Transform endPos = (currentEvadeStep % 2 == 0) ? posEvadeRight : posEvadeLeft;
        Sprite[] evadeSprites = { spriteSakuraEvade1, spriteSakuraEvade2, spriteSakuraEvade3 };
        Sprite targetSprite = evadeSprites[currentEvadeStep % 3];

        float randomYOffset = Random.Range(-evadeRandomYRange, evadeRandomYRange);
        Vector3 targetPosition = new Vector3(endPos.position.x, startPos.position.y + randomYOffset, startPos.position.z);

        activeTrainingSequence.AppendCallback(() => {
            sakuraRenderer.sprite = spriteSakuraSpinning;
            sakuraRenderer.transform.position = startPos.position;
            sakuraRenderer.transform.rotation = Quaternion.identity;
        });

        activeTrainingSequence.Append(sakuraRenderer.transform.DOMove(targetPosition, evadeMoveDuration).SetEase(Ease.Linear));
        activeTrainingSequence.Insert(0, sakuraRenderer.transform.DORotate(new Vector3(0, 360f * evadeSpins, 0), evadeMoveDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        activeTrainingSequence.AppendCallback(() => {
            sakuraRenderer.transform.rotation = (endPos == posEvadeLeft) ? Quaternion.Euler(0, 180f, 0) : Quaternion.identity;
            sakuraRenderer.sprite = targetSprite;
        });

        activeTrainingSequence.AppendInterval(evadePauseDuration);
        activeTrainingSequence.OnComplete(() => {
            currentEvadeStep++;
            RunNextEvadeStep();
        });
    }

    private void PlayRunTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgRunning);
        isScrolling = true; // Chỉ bật cuộn nền cho Run

        sakuraRenderer.sprite = spriteSakuraRunning;
        sakuraRenderer.transform.position = posRunCenter.position;
        sakuraRenderer.transform.rotation = Quaternion.identity;

        activeTrainingSequence = DOTween.Sequence();
        float startY = posRunCenter.position.y;
        float floatHeight = 0.3f;

        activeTrainingSequence.Append(sakuraRenderer.transform.DOMoveY(startY + floatHeight, 0.6f).SetEase(Ease.InOutSine));
        activeTrainingSequence.SetLoops(-1, LoopType.Yoyo);
    }

    // BÀI TẬP MỚI: SWIMMING
    private void PlaySwimmingTraining()
    {
        StopAllAnimations();
        SetActiveBackground(bgSwimming);
        // isScrolling = false; (Đã được set ở StopAllAnimations)

        sakuraRenderer.sprite = spriteSakuraSwimming;
        sakuraRenderer.transform.position = posSwimming.position;
        sakuraRenderer.transform.rotation = Quaternion.identity;

        // Thay vì dùng Sequence nối tiếp, ta chạy nhiều Tween đồng thời với chu kỳ (thời gian) khác nhau 
        // để tạo cảm giác bơi lội ngẫu nhiên và mềm mại hơn.

        float moveXAmount = 1.2f; // Biên độ sang trái/phải
        float moveYAmount = 0.4f; // Biên độ lên/xuống
        float rotateAngle = 10f;  // Biên độ xoay Z

        // 1. Lên xuống (nhịp nhanh hơn một chút)
        sakuraRenderer.transform.DOMoveY(posSwimming.position.y + moveYAmount, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // 2. Sang trái phải (nhịp chậm hơn)
        sakuraRenderer.transform.DOMoveX(posSwimming.position.x + moveXAmount, 1.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // 3. Xoay nhẹ ở trục Z
        Transform t = sakuraRenderer.transform;

        t.localRotation = Quaternion.Euler(0, 0, -rotateAngle);

        t.DOLocalRotate(new Vector3(0, 0, rotateAngle), 1f)
         .SetEase(Ease.InOutSine)
         .SetLoops(-1, LoopType.Yoyo);
    }
}