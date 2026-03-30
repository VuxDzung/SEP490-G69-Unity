using UnityEngine;
using DG.Tweening;

public class FloatingAnimation : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatHeight = 0.2f; // Biên độ trôi nổi (lên/xuống)
    [SerializeField] private float duration = 1.5f;    // Thời gian cho 1 nhịp lơ lửng (nên để dài > 1.2s)

    [Header("Tilt Settings (Optional)")]
    [SerializeField] private float tiltAngle = 3f;     // Góc nghiêng nhẹ qua lại (độ)
    [SerializeField] private float tiltDuration = 2f;  // Thời gian nghiêng (nên để lệch nhịp với duration ở trên để nhìn tự nhiên hơn)

    private Vector3 _startLocalPos;

    private void Awake()
    {
        // Lưu lại vị trí gốc ban đầu ngay khi vừa sinh ra
        _startLocalPos = transform.localPosition;
    }

    private void OnEnable()
    {
        // Reset lại vị trí và góc (Rất quan trọng khi dùng Object Pooling)
        transform.localPosition = _startLocalPos;
        transform.localRotation = Quaternion.identity;

        // 1. Hiệu ứng lơ lửng (lên xuống)
        transform.DOLocalMoveY(_startLocalPos.y + floatHeight, duration)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);

        // 2. Hiệu ứng chòng chành (nghiêng qua nghiêng lại)
        // Nếu bạn không thích nghiêng thì có thể set tiltAngle = 0 ở Inspector
        transform.DOLocalRotate(new Vector3(0, 0, tiltAngle), tiltDuration)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        // Hủy Tween khi object bị ẩn (cất vào Pool)
        DOTween.Kill(transform);
    }

    private void OnDestroy()
    {
        // Đề phòng trường hợp object bị Destroy hẳn
        DOTween.Kill(transform);
    }
}