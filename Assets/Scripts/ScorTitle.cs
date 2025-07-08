using TMPro;
using UnityEngine;

public class ScorTitle : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _MainScoreText;
    private int _score;
    void Start()
    {
        _score = PlayerPrefs.GetInt("HighScore", 0);
    }
    void Update()
    {
        _MainScoreText.text = _score.ToString();
    }
}
