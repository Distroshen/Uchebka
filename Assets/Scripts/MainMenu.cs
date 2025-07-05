using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject fadeout;
    void Start()
    {
    }

    void Update()
    {

    }

    public void StartGame()
    {
        StartCoroutine(START());
    }

    IEnumerator START()
    {
        fadeout.SetActive(true);
        yield return new WaitForSeconds(2.8f);
        SceneManager.LoadScene(1);
    }

    public void StartShop()
    {
        StartCoroutine(START1());
    }
    IEnumerator START1()
    {
        fadeout.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(2);
    }


    public void StartRecord()
    {
        StartCoroutine(START2());
    }
    IEnumerator START2()
    {
        fadeout.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(3);
    }

    public void StartMusic()
    {
        StartCoroutine(START3());
    }
    IEnumerator START3()
    {
        fadeout.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(4);
    }
}
