using UnityEngine;

public class WatermelonRoll : MonoBehaviour
{
    [Header("Настройки движения")]
    public float rotationSpeed = 180f; // Скорость вращения (градусов в секунду)
    private void Update()
    {
        // Вращение вокруг оси X (для реалистичного качения)
        float rotation = rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation, 0f, 0f, Space.Self);
    }
}