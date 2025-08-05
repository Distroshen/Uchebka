using TMPro;
using UnityEngine;
using System.Collections;

public class ScoreTitleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;

    // Массив званий и соответствующих порогов
    [SerializeField]
    private TitleLevel[] titleLevels = {
        new TitleLevel("Ребёнок", 0),
        new TitleLevel("Амеба", 200),
        new TitleLevel("Пацан", 400),
        new TitleLevel("Мужицкий", 800),
        new TitleLevel("Легенда", 1500)
    };

    private int _lastProcessedLevel = -1;
    private int score;

    [System.Serializable]
    public class TitleLevel
    {
        public string title;
        public int requiredScore;

        public TitleLevel(string title, int requiredScore)
        {
            this.title = title;
            this.requiredScore = requiredScore;
        }
    }

    void Start()
    {
        score = PlayerPrefs.GetInt("HighScore", 0);
        CheckInitialTitle();
    }

    private void CheckInitialTitle()
    {
        // Показываем текущее звание при старте
        int currentLevel = GetCurrentLevel();
        if (currentLevel >= 0)
        {
            _titleText.text = titleLevels[currentLevel].title;
            _titleText.gameObject.SetActive(true);
            _lastProcessedLevel = currentLevel;
        }
    }

    private void Update()
    {
        CheckScore();
    }

    public void CheckScore()
    {
        int currentLevel = GetCurrentLevel();

        // Если уровень изменился
        if (currentLevel > _lastProcessedLevel)
        {
            ShowTitle(currentLevel);
            _lastProcessedLevel = currentLevel;
        }
    }

    private int GetCurrentLevel()
    {
        // Определяем текущий уровень звания
        for (int i = titleLevels.Length - 1; i >= 0; i--)
        {
            if (score >= titleLevels[i].requiredScore)
            {
                return i;
            }
        }
        return -1;
    }

    private void ShowTitle(int level)
    {
        if (level >= 0 && level < titleLevels.Length)
        {
            _titleText.text = titleLevels[level].title;
            _titleText.gameObject.SetActive(true);

            // Здесь можно добавить анимацию
            StartCoroutine(AnimateTitle());
        }
    }

    private IEnumerator AnimateTitle()
    {
        // Пример простой анимации без DOTween
        float duration = 2f;
        float elapsed = 0f;
        Vector3 originalScale = _titleText.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Случайный масштаб
            float randomScale = Random.Range(0.8f, 1.2f);
            _titleText.transform.localScale = originalScale * randomScale;

            // Случайный наклон
            Vector3 randomRotation = new Vector3(
                Random.Range(-15f, 15f),
                Random.Range(-15f, 15f),
                Random.Range(-15f, 15f)
            );
            _titleText.transform.localRotation = Quaternion.Euler(randomRotation);

            // Дрожание
            Vector3 shakeOffset = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                0
            );
            _titleText.transform.localPosition += shakeOffset * Time.deltaTime;

            yield return null;
        }

        // Возврат к исходному состоянию
        _titleText.transform.localScale = originalScale;
        _titleText.transform.localRotation = Quaternion.identity;
    }
}