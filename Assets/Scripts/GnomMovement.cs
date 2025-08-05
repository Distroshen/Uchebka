using UnityEngine;

public class GnomeMovement : MonoBehaviour
{
    [Header("Настройки прыжка")]
    [SerializeField] private float jumpHeight = 0.3f;        // Высота прыжка
    [SerializeField] private float sideStep = 0.2f;          // Ширина шага в сторону
    [SerializeField] private float jumpDuration = 0.5f;      // Продолжительность одного прыжка
    [SerializeField] private float bodyTilt = 15f;           // Наклон тела при прыжке

    private float _jumpTimer;
    private bool _isJumpingLeft;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    void Start()
    {
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;
        _jumpTimer = Random.Range(0f, jumpDuration); // Случайная начальная фаза
    }

    void Update()
    {
        // Обновляем таймер прыжка
        _jumpTimer += Time.deltaTime;

        // Сбрасываем таймер при завершении цикла
        if (_jumpTimer > jumpDuration)
        {
            _jumpTimer = 0f;
            _isJumpingLeft = !_isJumpingLeft; // Меняем направление
        }

        // Вычисляем прогресс прыжка (0-1)
        float progress = _jumpTimer / jumpDuration;

        // Рассчитываем позицию и вращение
        Vector3 newPosition = CalculateJumpPosition(progress);
        Quaternion newRotation = CalculateBodyTilt(progress);

        // Применяем трансформации
        transform.localPosition = newPosition;
        transform.localRotation = newRotation;
    }

    private Vector3 CalculateJumpPosition(float progress)
    {
        // Параболическая траектория прыжка (y = 4 * h * x * (1 - x))
        float height = 4f * jumpHeight * progress * (1f - progress);

        // Боковое смещение (с плавным началом и концом)
        float sideMovement = Mathf.SmoothStep(0f, 1f, progress);
        if (!_isJumpingLeft) sideMovement = -sideMovement;
        float xPos = _startPosition.x + sideMovement * sideStep;

        return new Vector3(
            xPos,
            _startPosition.y + height,
            _startPosition.z
        );
    }

    private Quaternion CalculateBodyTilt(float progress)
    {
        // Максимальный наклон в середине прыжка
        float tiltAmount = Mathf.Sin(progress * Mathf.PI) * bodyTilt;

        // Направление наклона зависит от направления прыжка
        float tiltDirection = _isJumpingLeft ? 1f : -1f;

        return Quaternion.Euler(
            _startRotation.eulerAngles.x,
            _startRotation.eulerAngles.y,
            _startRotation.eulerAngles.z + tiltAmount * tiltDirection
        );
    }

    // Для визуализации траектории в редакторе
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Vector3 startPos = transform.position;

        // Рисуем возможные траектории прыжков
        for (int i = 0; i < 20; i++)
        {
            float progress = i / 20f;
            float height = 4f * jumpHeight * progress * (1f - progress);

            // Влево
            Vector3 leftPos = new Vector3(
                startPos.x - progress * sideStep,
                startPos.y + height,
                startPos.z
            );

            // Вправо
            Vector3 rightPos = new Vector3(
                startPos.x + progress * sideStep,
                startPos.y + height,
                startPos.z
            );

            if (i > 0)
            {
                Gizmos.DrawLine(leftPos, new Vector3(
                    startPos.x - (i - 1) / 20f * sideStep,
                    startPos.y + 4f * jumpHeight * (i - 1) / 20f * (1f - (i - 1) / 20f),
                    startPos.z
                ));

                Gizmos.DrawLine(rightPos, new Vector3(
                    startPos.x + (i - 1) / 20f * sideStep,
                    startPos.y + 4f * jumpHeight * (i - 1) / 20f * (1f - (i - 1) / 20f),
                    startPos.z
                ));
            }
        }
    }
}