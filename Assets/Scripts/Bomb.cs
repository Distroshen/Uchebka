
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private AudioSource BombFX;
    public bool Life;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BombFX.Play();
            this.gameObject.SetActive(false);
            Life = false;
            FindAnyObjectByType<BustB>().B1();
        }
    }
}
