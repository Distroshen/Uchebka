using UnityEngine;

public class BustB : MonoBehaviour
{
    [SerializeField] AudioSource bustBFX;
    [SerializeField] private AudioSource bombFX;
    [SerializeField] GameObject bMesh;

    private bool bron;
    private Player player;
    private WaitForSeconds waitForSevenSeconds;

    private void Start()
    {
        bron = false;

        // Кэшируем игрока один раз
        player = Player.Instance != null ? Player.Instance :
                FindAnyObjectByType<Player>();

        // Предварительно создаем WaitForSeconds для оптимизации
        waitForSevenSeconds = new WaitForSeconds(7f);
    }

    // Метод для обработки столкновения
    private void OnTriggerEnter(Collider other)
    {
        if (bron || !other.CompareTag("Player")) return;

        if (bustBFX != null)
        {
            bustBFX.Play();
        }

        StartCoroutine(Bronya());
    }

    private System.Collections.IEnumerator Bronya()
    {
        bron = true;

        // Визуально показываем броню (если есть меш)
        if (bMesh != null)
        {
            bMesh.SetActive(true);
        }

        yield return waitForSevenSeconds;

        bron = false;

        // Скрываем броню
        if (bMesh != null)
        {
            bMesh.SetActive(false);
        }
    }

    public void B1()
    {
        if (bron)
        {
            Debug.Log("Броня активна, урон блокирован");
            return;
        }

        // Воспроизводим звук взрыва
        if (bombFX != null)
        {
            bombFX.Play();
        }

        // Вызываем смерть игрока
        if (player != null)
        {
            player.Dead1();
        }
    }
}