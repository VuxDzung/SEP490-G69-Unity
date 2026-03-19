using UnityEngine;

public class SpinEffect : MonoBehaviour
{
    [Header("Cài đặt vòng quay ngang")]
    public float speed = 3f;
    public float width = 1.5f;
    public float height = 0.3f;

    [Header("Cài đặt phập phồng (Ảo giác 3D)")]
    public float baseScale = 1f;
    public float scaleAmount = 0.4f;

    private Transform[] stars;

    void Start()
    {
        int childCount = transform.childCount;
        stars = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            stars[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        // Cho từng ngôi sao chạy theo hình Oval
        for (int i = 0; i < stars.Length; i++)
        {
            // Tính toán góc để các ngôi sao luôn cách đều nhau (dù bạn có 2, 3 hay 5 ngôi sao)
            float angle = (Time.time * speed) + (i * Mathf.PI * 2f / stars.Length);

            // Công thức hình Elip tạo ảo giác 3D nằm ngang
            float x = Mathf.Cos(angle) * width;
            float y = Mathf.Sin(angle) * height;

            // Áp dụng vị trí mới cho ngôi sao
            stars[i].localPosition = new Vector3(x, y, 0);

            // 3. HIỆU ỨNG TO NHỎ (Scale)
            // Hàm Sin(angle) tạo ra một làn sóng lượn từ -1 đến 1.
            // Chúng ta trừ đi làn sóng này để: Khi y nằm dưới (trước mặt), sao to lên. Khi y nằm trên (sau đầu), sao nhỏ đi.
            float currentScale = baseScale - (Mathf.Sin(angle) * scaleAmount);

            // Áp dụng kích thước mới cho ngôi sao
            stars[i].localScale = new Vector3(currentScale, currentScale, 1f);
        }
    }
}

