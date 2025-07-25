using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BustB : MonoBehaviour
{
    [SerializeField] AudioSource BustBFX;
    [SerializeField] private AudioSource BombFX;
    [SerializeField] GameObject BMesh;
    private bool Bron;

    private void Start()
    {
        Bron = false;
    }

    // Метод для обработки столкновения
    private void OnTriggerEnter(Collider other)
    {
        BustBFX.Play();
        StartCoroutine(Bronya());
    }
    IEnumerator Bronya()
    {
        Bron = true;
        yield return new WaitForSeconds(7);
        Bron = false;
    }

    public void B1()
    {
        if (Bron)
        {
            Debug.Log("1");
        }
        else
        {
            BombFX.Play();
            FindAnyObjectByType<Player>().Dead1();
        }
    }
}
