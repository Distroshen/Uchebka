using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioSource coinFX;
    private void OnTriggerEnter(Collider other)
    {
        coinFX.Play();
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.AddCoin(); // ��������� 1 ������
        this.gameObject.SetActive(false);
    }
}
