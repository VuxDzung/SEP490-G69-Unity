using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SakuraTrainingTester : MonoBehaviour
{
    [Header("Giao diện nút bấm (Chỉ để Test)")]
    public Button buttonTrainPower;
    public Button buttonTrainIntelligence;
    public Button buttonTrainEvade;
    public Button buttonTrainRun; // Thêm nút test Run

    [Header("Thành phần của Sakura")]
    public SpriteRenderer sakuraRenderer;
    public Sprite spriteSakuraWindUp;
    public Sprite spriteSakuraHeadbutt;
    public Sprite spriteSakuraStudy;
    public Sprite spriteSakuraRunning; // Thêm sprite Run

    [Header("Sprites - Bài tập EVADE")]
    public Sprite spriteSakuraSpinning;
    public Sprite spriteSakuraEvade1;
    public Sprite spriteSakuraEvade2;
    public Sprite spriteSakuraEvade3;

    [Header("Môi trường - Tĩnh")]
    public GameObject staticBackgroundGroup;
    public SpriteRenderer bagRenderer;
    public Sprite spriteBagNormal;
    public Sprite spriteBagDeform;

    [Header("Môi trường - Cuộn (Parallax)")]
    public GameObject scrollingBackgroundGroup;
    public SpriteRenderer[] scrollingLayers; // Kéo 4 lớp: Sky, Levee, Fence, Track vào đây
    public float[] scrollSpeeds = { 0.05f, 0.2f, 0.5f, 0.8f }; // Tốc độ tương ứng cho từng lớp

    [Header("Các vị trí - POW & STUDY")]
    public Transform positionStart;
    public Transform positionWindUp;
    public Transform positionImpact;
    public Transform posStudyLeft;
    public Transform posStudyRight;

    [Header("Các vị trí - EVADE & RUN")]
    public Transform posEvadeLeft;
    public Transform posEvadeRight;
    public Transform posRunCenter; // Điểm đứng khi chạy
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

        buttonTrainPower.onClick.AddListener(PlayPowerTraining);
        buttonTrainIntelligence.onClick.AddListener(PlayIntelligenceTraining);
        buttonTrainEvade.onClick.AddListener(PlayEvadeTraining);
        buttonTrainRun.onClick.AddListener(PlayRunTraining);

        StopAllAnimations();
    }

    void Update()
    {
        // Xử lý cuộn nền liên tục nếu đang ở bài tập Run/Swim
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

        isScrolling = false;

        sakuraRenderer.transform.position = positionStart.position;
        sakuraRenderer.transform.rotation = Quaternion.identity;

        bagRenderer.transform.position = bagOriginalPosition;
        bagRenderer.transform.localScale = bagOriginalScale;
        bagRenderer.sprite = spriteBagNormal;
        bagRenderer.enabled = false;
    }

    // Đổi môi trường hiển thị
    private void SetupEnvironment(bool useScrolling)
    {
        if (staticBackgroundGroup != null) staticBackgroundGroup.SetActive(!useScrolling);
        if (scrollingBackgroundGroup != null) scrollingBackgroundGroup.SetActive(useScrolling);
    }

    private void PlayPowerTraining()
    {
        StopAllAnimations();
        SetupEnvironment(false); // Dùng nền tĩnh
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
        SetupEnvironment(false); // Dùng nền tĩnh
        sakuraRenderer.sprite = spriteSakuraStudy;
        sakuraRenderer.transform.position = posStudyLeft.position;

        activeTrainingSequence = DOTween.Sequence();
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMoveX(posStudyRight.position.x, 4f).SetEase(Ease.InOutQuad));
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMoveX(posStudyLeft.position.x, 4f).SetEase(Ease.InOutQuad));

        // Float logic
        sakuraRenderer.transform.DOMoveY(posStudyLeft.position.y + 0.5f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        sakuraRenderer.transform.DORotate(new Vector3(0, 0, 360f), 6f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

        activeTrainingSequence.SetLoops(-1, LoopType.Restart);
    }

    private void PlayEvadeTraining()
    {
        StopAllAnimations();
        SetupEnvironment(false); // Dùng nền tĩnh
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
        SetupEnvironment(true); // BẬT NỀN CUỘN
        isScrolling = true;

        sakuraRenderer.sprite = spriteSakuraRunning;
        sakuraRenderer.transform.position = posRunCenter.position;
        sakuraRenderer.transform.rotation = Quaternion.identity;

        activeTrainingSequence = DOTween.Sequence();
        float startY = posRunCenter.position.y;
        float floatHeight = 0.3f; // Lơ lửng nhẹ

        // Chỉ cần 1 lệnh đi lên, và dùng LoopType.Yoyo cho toàn bộ Sequence để nó tự dội ngược lại mượt mà
        activeTrainingSequence.Append(sakuraRenderer.transform.DOMoveY(startY + floatHeight, 0.6f).SetEase(Ease.InOutSine));

        activeTrainingSequence.SetLoops(-1, LoopType.Yoyo);
    }
}