using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioSource coinFX;

    [SerializeField] private float rotationSpeed = 180f; // �������� �������� � ��������/���

    [Header("��������� ������")]
    [SerializeField] private float jumpHeight = 0.4f;    // ������ ������
    [SerializeField] private float jumpSpeed = 1.5f;       // �������� ������

    private Vector3 startPosition; // ��������� ������� �������
    private float timer;           // ������ ��� ��������

    void Start()
    {
        jumpHeight = Random.Range(0.2f, 0.5f);
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
    public void OnTriggerEnter(Collider other)
    {
        //coinFX.Play();
        this.gameObject.SetActive(false);
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.AddCoin(); // ��������� 1 ������

    }
}
