using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioSource coinFX;
    [SerializeField] private float rotationSpeed = 180f;
    //[SerializeField] private float jumpHeight = 0.4f;
    [SerializeField] private float jumpSpeed = 1.5f;

    private Vector3 startPosition;
    private float timer;
    private Transform coinTransform;
    private GameManager gameManager;
    private bool isCollected = false;

    // Кэшированные значения для оптимизации
    private float cachedJumpHeight;
    private float randomJumpHeight;

    void Start()
    {
        coinTransform = transform;
        startPosition = coinTransform.position;

        // Кэшируем GameManager один раз
        gameManager = GameManager.Instance != null ? GameManager.Instance :
                     FindAnyObjectByType<GameManager>();

        // Случайная высота прыжка
        randomJumpHeight = Random.Range(0.2f, 0.5f);
        cachedJumpHeight = randomJumpHeight;
    }

    void Update()
    {
        // Вращение вокруг оси Y
        coinTransform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Анимация прыжка - используем более эффективный расчет
        timer += Time.deltaTime * jumpSpeed;
        float sinValue = Mathf.Sin(timer);
        float yOffset = sinValue * sinValue * cachedJumpHeight;

        // Применяем новую позицию напрямую к Transform
        Vector3 currentPosition = coinTransform.position;
        coinTransform.position = new Vector3(
            currentPosition.x,
            startPosition.y + yOffset,
            currentPosition.z
        );
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isCollected || !other.CompareTag("Player")) return;

        isCollected = true;

        // Воспроизводим звук
        if (coinFX != null && coinFX.clip != null)
        {
            // Используем PlayClipAtPoint чтобы звук играл даже после деактивации объекта
            AudioSource.PlayClipAtPoint(coinFX.clip, coinTransform.position);
        }

        gameObject.SetActive(false);

        // Добавляем монету
        if (gameManager != null)
        {
            gameManager.AddCoin();
        }
    }

    // Метод для сброса состояния при повторном использовании объекта
    public void ResetCoin()
    {
        isCollected = false;
        timer = 0f;
        startPosition = coinTransform.position;
        randomJumpHeight = Random.Range(0.2f, 0.5f);
        cachedJumpHeight = randomJumpHeight;
        gameObject.SetActive(true);
    }

    // Метод для предварительной настройки монеты при создании через пул
    public void SetupCoin(Vector3 position)
    {
        coinTransform.position = position;
        startPosition = position;
        ResetCoin();
    }
}