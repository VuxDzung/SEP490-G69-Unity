using System.Collections;
using UnityEngine;

public class WindProjectileCardGame : MonoBehaviour
{
    [Header("Cài đặt bay")]
    public float speed = 15f; // Tốc độ bay (Càng to bay càng nhanh)

    [Header("Hiệu ứng Tan biến (Giai đoạn 3)")]
    public GameObject impactVFXPrefab;

    [Header("Thời gian chờ xóa Đuôi gió")]
    [Tooltip("Thời gian chờ để đuôi gió tan hết trước khi xóa hẳn Prefab khỏi Hierarchy")]
    public float delayDestroyTime = 1.0f;

    // Hàm này sẽ được gọi bởi hệ thống Battle của bạn khi một lá bài tung chiêu
    public void FireAtTarget(Vector3 targetPosition)
    {
        // Khởi động "kịch bản" bay
        StartCoroutine(FlyRoutine(targetPosition));
    }

    private IEnumerator FlyRoutine(Vector3 targetPosition)
    {
        // GIAI ĐOẠN 2: BAY ĐẾN MỤC TIÊU
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        // GIAI ĐOẠN 3: CHẠM MỤC TIÊU -> XỬ LÝ TAN BIẾN

        // 1. Sinh ra vụ nổ (Impact)
        if (impactVFXPrefab != null)
        {
            Instantiate(impactVFXPrefab, transform.position, Quaternion.identity);
        }

        // 2. Tắt TẤT CẢ hình ảnh quả cầu (Tìm cả trong vật thể Cha lẫn Con)
        SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in allSprites)
        {
            sr.enabled = false;
        }

        // 3. Tắt tính năng phun hạt của cái đuôi gió (Trail)
        ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in allParticles)
        {
            var emission = ps.emission;
            emission.enabled = false;
        }

        // 4. Đợi cái đuôi bay nốt và tan đi
        yield return new WaitForSeconds(delayDestroyTime);

        // 5. Xóa sổ hoàn toàn khỏi Hierarchy
        Destroy(gameObject);
    }
}
