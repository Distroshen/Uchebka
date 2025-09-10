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

    // ������������ �������� ��� �����������
    private float cachedJumpHeight;
    private float randomJumpHeight;

    void Start()
    {
        coinTransform = transform;
        startPosition = coinTransform.position;

        // �������� GameManager ���� ���
        gameManager = GameManager.Instance != null ? GameManager.Instance :
                     FindAnyObjectByType<GameManager>();

        // ��������� ������ ������
        randomJumpHeight = Random.Range(0.2f, 0.5f);
        cachedJumpHeight = randomJumpHeight;
    }

    void Update()
    {
        // �������� ������ ��� Y
        coinTransform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // �������� ������ - ���������� ����� ����������� ������
        timer += Time.deltaTime * jumpSpeed;
        float sinValue = Mathf.Sin(timer);
        float yOffset = sinValue * sinValue * cachedJumpHeight;

        // ��������� ����� ������� �������� � Transform
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

        // ������������� ����
        if (coinFX != null && coinFX.clip != null)
        {
            // ���������� PlayClipAtPoint ����� ���� ����� ���� ����� ����������� �������
            AudioSource.PlayClipAtPoint(coinFX.clip, coinTransform.position);
        }

        gameObject.SetActive(false);

        // ��������� ������
        if (gameManager != null)
        {
            gameManager.AddCoin();
        }
    }

    // ����� ��� ������ ��������� ��� ��������� ������������� �������
    public void ResetCoin()
    {
        isCollected = false;
        timer = 0f;
        startPosition = coinTransform.position;
        randomJumpHeight = Random.Range(0.2f, 0.5f);
        cachedJumpHeight = randomJumpHeight;
        gameObject.SetActive(true);
    }

    // ����� ��� ��������������� ��������� ������ ��� �������� ����� ���
    public void SetupCoin(Vector3 position)
    {
        coinTransform.position = position;
        startPosition = position;
        ResetCoin();
    }
}