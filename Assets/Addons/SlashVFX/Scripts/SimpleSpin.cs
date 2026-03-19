using UnityEngine;

public class SimpleSpin : MonoBehaviour
{
    public float speed = 1000f;
    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
