using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject fadeout;

    // Кэшированные значения для оптимизации
    private WaitForSeconds waitFor2_8Seconds;
    private WaitForSeconds waitFor2Seconds;
    private WaitForSeconds waitFor3Seconds;

    void Start()
    {
        // Предварительно создаем WaitForSeconds для оптимизации корутин
        waitFor2_8Seconds = new WaitForSeconds(2.8f);
        waitFor2Seconds = new WaitForSeconds(2f);
        waitFor3Seconds = new WaitForSeconds(3f);
    }

    public void StartGame()
    {
        StartCoroutine(LoadSceneWithFade(1, waitFor2_8Seconds));
    }

    public void StartShop()
    {
        StartCoroutine(LoadSceneWithFade(2, waitFor2Seconds));
    }

    public void StartRecord()
    {
        StartCoroutine(LoadSceneWithFade(3, waitFor3Seconds));
    }

    public void StartMusic()
    {
        StartCoroutine(LoadSceneWithFade(3, waitFor3Seconds));
    }

    private IEnumerator LoadSceneWithFade(int sceneIndex, WaitForSeconds waitTime)
    {
        fadeout.SetActive(true);
        yield return waitTime;
        SceneManager.LoadScene(sceneIndex);
    }
}