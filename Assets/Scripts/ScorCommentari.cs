using TMPro;
using UnityEngine;

public class ScoreTitleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private string _childTitle = "Детский";
    [SerializeField] private string _manTitle = "Мужской";

    private int _lastProcessedScore = 0;
    private int score;

    void Start()
    {
        score = PlayerPrefs.GetInt("HighScore", 0);
    }
    private void Update()
    {
        CheckScore();
    }

    public void CheckScore()
    {
        // Если уже показывали для этого уровня счета - пропускаем
        if ((score >= 100 && _lastProcessedScore < 100) ||
            (score >= 200 && _lastProcessedScore < 200))
        {
            ShowTitle();
            _lastProcessedScore = score;
        }
    }

    private void ShowTitle()
    {
        if (score >= 200)
        {
            _titleText.text = _manTitle;
        }
        else if (score >= 100)
        {
            _titleText.text = _childTitle;
        }

        _titleText.gameObject.SetActive(true);
    }
}