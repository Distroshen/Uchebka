using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioSource coinFX;

    [SerializeField] private float rotationSpeed = 180f; // Скорость вращения в градусах/сек

    [Header("Настройки прыжка")]
    [SerializeField] private float jumpHeight = 0.5f;    // Высота прыжка
    [SerializeField] private float jumpSpeed = 3f;       // Скорость прыжка

    private Vector3 startPosition; // Начальная позиция монетки
    private float timer;           // Таймер для анимации

    void Start()
    {
        jumpHeight = Random.Range(0.3f, 0.7f);
        // Сохраняем начальную позицию
        startPosition = transform.position;
    }

    void Update()
    {
        // Вращение вокруг оси Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Анимация прыжка (движение вверх-вниз)
        timer += Time.deltaTime * jumpSpeed;
        float yOffset = Mathf.Pow(Mathf.Sin(timer), 2) * jumpHeight;

        // Применяем новую позицию
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        coinFX.Play();
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.AddCoin(); // Добавляем 1 монету
        this.gameObject.SetActive(false);
    }
}
