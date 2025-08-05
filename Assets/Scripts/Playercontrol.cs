using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player speed; // Класс с параметром Speed

    [Header("Колебания скорости")]
    [SerializeField] private float _variation = 2f;    // Разброс скорости
    [SerializeField] private float _frequency = 0.5f;  // Частота изменений

    private float _speedTimer;
    private float _currentSpeed;
    private bool _isFinish; // Флаг для проверки тега

    void Start()
    {
        _currentSpeed = speed.Speed;
        _speedTimer = Random.Range(0f, 10f);
        _isFinish = gameObject.CompareTag("Finish"); // Проверяем тег при старте
    }

    void FixedUpdate()
    {
        if (_isFinish)
        {
            // Для объектов с тегом "Finish" - скорость с колебаниями
            _speedTimer += Time.fixedDeltaTime * _frequency;
            _currentSpeed = speed.Speed + Mathf.Sin(_speedTimer) * _variation;
        }
        else
        {
            // Для остальных объектов - постоянная скорость
            _currentSpeed = speed.Speed;
        }

        // Применяем движение
        transform.Translate(Vector3.forward * Time.fixedDeltaTime * _currentSpeed, Space.World);
    }
}