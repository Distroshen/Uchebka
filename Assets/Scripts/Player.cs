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

    void Start()
    {
        Life = true;
        // Определяем платформу
        isMobilePlatform = Application.isMobilePlatform;

        if (isMobilePlatform)
        {
            targetXPosition = transform.position.x;
        }

        isImmortal = false;
        if (Life == true)
        {
            StartCoroutine(SS());
        }
    }

    void FixedUpdate()
    {
        // Всегда двигаемся вперед
        transform.Translate(Vector3.forward * Time.deltaTime * Speed, Space.World);

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
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (transform.position.x > LeftLimit)
            {
                transform.Translate(Vector3.left * Time.deltaTime * Speedz, Space.World);
            }
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (transform.position.x < RightLimit)
            {
                transform.Translate(Vector3.right * Time.deltaTime * Speedz, Space.World);
            }
        }
    }

    void MobileMovement()
    {
        if (isTouching)
        {
            // Плавное движение к целевой позиции
            float newX = Mathf.Lerp(transform.position.x, targetXPosition, Time.deltaTime * Speedz);
            newX = Mathf.Clamp(newX, LeftLimit, RightLimit);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }

    void Update()
    {
        if (!isMobilePlatform) return;

        // Обработка касаний для мобильных устройств
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // Преобразуем позицию касания в мировые координаты
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
                targetXPosition = touchPosition.x;
                isTouching = true;
            }
        }
        else
        {
            isTouching = false;
        }
    }
    IEnumerator SS()
    {
        yield return new WaitForSeconds(7);
        if (Speed < MaxSpeed)
        {
            Speed += 1;
            StartCoroutine(SS());
        }
    }
    public void BustU()
    {
        Speed = Speed + 5;
        Speedz = Speedz + 5;
    }
    public void BustU2()
    {
        Speed = Speed - 5;
        Speedz = Speedz - 5;
    }
    public void Dead1()
    {
        FindAnyObjectByType<Scor>().OnPlayerDeath();
        Life = false;
        Speed = 0;
        MaxSpeed = 0;
        Speedz = 0;
        Player1.GetComponent<Animator>().Play("Death1");
        mainCam.GetComponent<Animator>().Play("AC");
        Fadeout.SetActive(true);
        Invoke("LP", 3f);
    }
    void LP()
    {
        LosePanel.SetActive(true);
    }
}
