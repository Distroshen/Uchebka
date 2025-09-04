using UnityEngine;

public class Bust : MonoBehaviour
{
    [SerializeField] AudioSource bustFX;
    [SerializeField] private float rotationSpeed = 40f;

    private Transform bustTransform;
    private Player player;
    private bool isUsed = false;
    private WaitForSeconds waitForSevenSeconds;

    void Start()
    {
        bustTransform = transform;

        // �������� ������ ���� ���
        player = Player.Instance != null ? Player.Instance :
                FindAnyObjectByType<Player>();

        // �������������� ������� WaitForSeconds ��� �����������
        waitForSevenSeconds = new WaitForSeconds(7f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed || !other.CompareTag("Player")) return;

        isUsed = true;

        // ������������� ����
        if (bustFX != null)
        {
            bustFX.Play();
        }

        StartCoroutine(BustEffect());
    }

    private System.Collections.IEnumerator BustEffect()
    {
        if (player != null)
        {
            player.BustU();
            yield return waitForSevenSeconds;
            player.BustU2();
        }

        // ������������ ������ ����� �������������
        gameObject.SetActive(false);
    }

    void Update()
    {
        bustTransform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    // ����� ��� ������ ��������� ��� ��������� ������������� �������
    public void ResetBust()
    {
        isUsed = false;
        gameObject.SetActive(true);
    }
}