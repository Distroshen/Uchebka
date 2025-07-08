using UnityEngine;

public class Coin5 : MonoBehaviour
{
    [SerializeField] AudioSource coinFX;
    private void OnTriggerEnter(Collider other)
    {
        coinFX.Play();
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.AddCoin5(); // ��������� 5 �����
        this.gameObject.SetActive(false);
    }
}
