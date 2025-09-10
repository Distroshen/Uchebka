using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public float Speed = 3;
    [SerializeField] private float MaxSpeed = 14;
    [SerializeField] private float Speedz = 6;
    [SerializeField] private float RightLimit = 1.75f;
    [SerializeField] private float LeftLimit = -1.75f;

    public bool isImmortal;
    private bool isMobilePlatform;
    private bool isTouching = false;
    private float targetXPosition;
    
    [SerializeField] GameObject mainCam;
    [SerializeField] GameObject Fadeout;
    [SerializeField] GameObject Player1;
    [SerializeField] GameObject LosePanel;
    public bool Life = true;

    // Кэшированные компоненты
    private Transform playerTransform;
    private Animator playerAnimator;
    private Animator cameraAnimator;
    private Scor scoreManager;
    private Camera mainCamera;
    private WaitForSeconds waitForSevenSeconds;

    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Кэшируем компоненты
        playerTransform = transform;
        playerAnimator = Player1.GetComponent<Animator>();
        cameraAnimator = mainCam.GetComponent<Animator>();
        scoreManager = FindFirstObjectByType<Scor>();
        mainCamera = Camera.main;

        // Предварительно создаем WaitForSeconds
        waitForSevenSeconds = new WaitForSeconds(7f);
    }

    void Start()
    {
        Life = true;
        // Определяем платформу
        isMobilePlatform = Application.isMobilePlatform;

        if (isMobilePlatform)
        {
            targetXPosition = playerTransform.position.x;
        }

        isImmortal = false;
        if (Life)
        {
            StartCoroutine(SpeedIncreaseRoutine());
        }
    }

    void FixedUpdate()
    {
        if (!Life) return;

        // Всегда двигаемся вперед
        playerTransform.Translate(Vector3.forward * Time.deltaTime * Speed, Space.World);

        if (isMobilePlatform)
        {
            MobileMovement();
        }
        else
        {
            PCMovement();
        }
    }

    void PCMovement()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }

        if (horizontalInput != 0f)
        {
            float newX = playerTransform.position.x + horizontalInput * Time.deltaTime * Speedz;
            newX = Mathf.Clamp(newX, LeftLimit, RightLimit);
            playerTransform.position = new Vector3(newX, playerTransform.position.y, playerTransform.position.z);
        }
    }

    void MobileMovement()
    {
        if (isTouching)
        {
            // Плавное движение к целевой позиции
            float newX = Mathf.Lerp(playerTransform.position.x, targetXPosition, Time.deltaTime * Speedz);
            newX = Mathf.Clamp(newX, LeftLimit, RightLimit);
            playerTransform.position = new Vector3(newX, playerTransform.position.y, playerTransform.position.z);
        }
    }

    void Update()
    {
        if (!isMobilePlatform || !Life) return;

        // Обработка касаний для мобильных устройств
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // Преобразуем позицию касания в мировые координаты
                Vector3 touchPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
                targetXPosition = touchPosition.x;
                isTouching = true;
            }
        }
        else
        {
            isTouching = false;
        }
    }

    IEnumerator SpeedIncreaseRoutine()
    {
        while (Speed <= MaxSpeed && Life)
        {
            yield return waitForSevenSeconds;

            if (!Life) yield break;

            Speed += 1;

            if (Speed >= Speedz + 6)
            {
                Speedz += 1;
            }
        }
    }

    public void BustU()
    {
        Speed += 5;
        Speedz += 5;
    }

    public void BustU2()
    {
        Speed -= 5;
        Speedz -= 5;
    }

    public void Dead1()
    {
        if (!Life) return;

        Life = false;
        scoreManager.OnPlayerDeath();
        Speed = 0;
        MaxSpeed = 0;
        Speedz = 0;

        //playerAnimator.Play("Death1");
        cameraAnimator.Play("AC");

        Fadeout.SetActive(true);
        Invoke("ShowLosePanel", 3f);
    }

    void ShowLosePanel()
    {
        LosePanel.SetActive(true);
    }
}