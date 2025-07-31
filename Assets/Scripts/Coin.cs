using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioSource coinFX;

    [SerializeField] private float rotationSpeed = 180f; // �������� �������� � ��������/���

    [Header("��������� ������")]
    [SerializeField] private float jumpHeight = 0.5f;    // ������ ������
    [SerializeField] private float jumpSpeed = 3f;       // �������� ������

    private Vector3 startPosition; // ��������� ������� �������
    private float timer;           // ������ ��� ��������

    void Start()
    {
        jumpHeight = Random.Range(0.3f, 0.7f);
        // ��������� ��������� �������
        startPosition = transform.position;
    }

    void Update()
    {
        // �������� ������ ��� Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // �������� ������ (�������� �����-����)
        timer += Time.deltaTime * jumpSpeed;
        float yOffset = Mathf.Pow(Mathf.Sin(timer), 2) * jumpHeight;

        // ��������� ����� �������
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        coinFX.Play();
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.AddCoin(); // ��������� 1 ������
        this.gameObject.SetActive(false);
    }
}
