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

        // �������� ������ ���� ���
        player = Player.Instance != null ? Player.Instance :
                FindAnyObjectByType<Player>();

        // �������������� ������� WaitForSeconds ��� �����������
        waitForSevenSeconds = new WaitForSeconds(7f);
    }

    // ����� ��� ��������� ������������
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

        // ��������� ���������� ����� (���� ���� ���)
        if (bMesh != null)
        {
            bMesh.SetActive(true);
        }

        yield return waitForSevenSeconds;

        bron = false;

        // �������� �����
        if (bMesh != null)
        {
            bMesh.SetActive(false);
        }
    }

    public void B1()
    {
        if (bron)
        {
            Debug.Log("����� �������, ���� ����������");
            return;
        }

        // ������������� ���� ������
        if (bombFX != null)
        {
            bombFX.Play();
        }

        // �������� ������ ������
        if (player != null)
        {
            player.Dead1();
        }
    }
}