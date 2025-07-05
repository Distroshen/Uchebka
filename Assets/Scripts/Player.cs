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

    public bool Life = true;

    void Start()
    {
        Life = true;
        // ���������� ���������
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
        // ������ ��������� ������
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
            // ������� �������� � ������� �������
            float newX = Mathf.Lerp(transform.position.x, targetXPosition, Time.deltaTime * Speedz);
            newX = Mathf.Clamp(newX, LeftLimit, RightLimit);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }

    void Update()
    {
        if (!isMobilePlatform) return;

        // ��������� ������� ��� ��������� ���������
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // ����������� ������� ������� � ������� ����������
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
}
