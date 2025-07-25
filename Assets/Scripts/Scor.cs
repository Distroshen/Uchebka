using TMPro;
using UnityEngine;

public class Scor : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float _scoreMultiplier = 2f;

    [SerializeField] public TextMeshProUGUI _currentScoreText;
    [SerializeField] public TextMeshProUGUI _highScoreText;
    [SerializeField] public Transform _player;
    private int _currentScore;
    private int _highScore;
    public Player player;

    private const string HighScoreKey = "HighScore";



    private void Start()
    {
        LoadHighScore();
        UpdateHighScoreDisplay();
    }

    private void Update()
    {
        if (player.Life == false || _player == null)
        {
            return;
        }
        else
        {
            UpdateCurrentScore();
            CheckHighScore();
        }
    }

    private void UpdateCurrentScore()
    {
        _currentScore = Mathf.FloorToInt(   _player.position.z / _scoreMultiplier);
        _currentScoreText.text = _currentScore.ToString();
    }

    private void CheckHighScore()
    {
        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            UpdateHighScoreDisplay();
        }
    }

    public void OnPlayerDeath()
    {
            SaveHighScore();
    }

    private void SaveHighScore()
    {
        if (_currentScore > PlayerPrefs.GetInt(HighScoreKey, 0))
        {
            PlayerPrefs.SetInt(HighScoreKey, _currentScore);
            PlayerPrefs.Save();
        }
    }

    private void LoadHighScore()
    {
        _highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public void UpdateHighScoreDisplay()
    {
        if (_highScoreText != null)
        {
            _highScoreText.text = $"Рекорд: {_highScore}";
        }
    }
}