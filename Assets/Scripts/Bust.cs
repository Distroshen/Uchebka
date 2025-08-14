using System.Collections;
using UnityEngine;

public class Bust : MonoBehaviour
{
    [SerializeField] AudioSource BustFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BustFX.Play();
            StartCoroutine(Bust1());
        }

    }

    IEnumerator Bust1()
    {
        FindAnyObjectByType<Player>().BustU();
        yield return new WaitForSeconds(7);
        FindAnyObjectByType<Player>().BustU2();
    }

    void Update()
    {
        transform.Rotate(0, 40 * Time.deltaTime, 0);
    }
}
