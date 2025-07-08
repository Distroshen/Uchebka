using TMPro;
using UnityEngine;

public class MainScor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highScoreText;

    private void Start()
    {
        //if (Scor.Instance != null)
        //{
        //_highScoreText.text = $"Рекорд: {Scor.Instance.GetCurrentHighScore()}";
        //}
        //else
        ////{
        // На случай если меню загружено первым
        //int savedScore = PlayerPrefs.GetInt("HighScore", 0);
        //_highScoreText.text = $"Рекорд: {savedScore}";
        //}
    }
}