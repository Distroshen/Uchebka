using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    [System.Serializable]
    public class HighScoreEntry
    {
        public string playerName;
        public int score;

        public HighScoreEntry(string name, int score)
        {
            this.playerName = name;
            this.score = score;
        }
    }

    public List<HighScoreEntry> highScores = new List<HighScoreEntry>();
    private const int maxEntries = 10;
    private const string playerPrefsKey = "HighScores";

    void Start()
    {
        // Инициализируем дефолтные значения (разработчики)
        if (!PlayerPrefs.HasKey(playerPrefsKey))
        {
            InitializeDefaultScores();
        }

        LoadHighScores();
    }

    private void InitializeDefaultScores()
    {
        highScores.Add(new HighScoreEntry("Dev1", 50));
        highScores.Add(new HighScoreEntry("Dev2", 45));
        highScores.Add(new HighScoreEntry("Dev3", 40));
        highScores.Add(new HighScoreEntry("Dev4", 35));
        highScores.Add(new HighScoreEntry("Dev5", 30));
        highScores.Add(new HighScoreEntry("Dev6", 25));
        highScores.Add(new HighScoreEntry("Dev7", 20));
        highScores.Add(new HighScoreEntry("Dev8", 15));
        highScores.Add(new HighScoreEntry("Dev9", 11));
        highScores.Add(new HighScoreEntry("Dev10", 1));

        SaveHighScores();
    }

    public void AddNewScore(string name, int score)
    {
        highScores.Add(new HighScoreEntry(name, score));

        // Сортируем по убыванию и оставляем только топ-10
        highScores = highScores
            .OrderByDescending(x => x.score)
            .Take(maxEntries)
            .ToList();

        SaveHighScores();
    }

    public bool IsNewHighScore(int score)
    {
        // Если таблица не заполнена или счет больше последнего в топ-10
        if (highScores.Count < maxEntries || score > highScores.Last().score)
        {
            return true;
        }
        return false;
    }

    private void SaveHighScores()
    {
        string json = JsonUtility.ToJson(new HighScoreWrapper(highScores));
        PlayerPrefs.SetString(playerPrefsKey, json);
        PlayerPrefs.Save();
    }

    private void LoadHighScores()
    {
        string json = PlayerPrefs.GetString(playerPrefsKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            HighScoreWrapper wrapper = JsonUtility.FromJson<HighScoreWrapper>(json);
            highScores = wrapper.highScores;
        }
    }

    // Вспомогательный класс для сериализации списка
    [System.Serializable]
    private class HighScoreWrapper
    {
        public List<HighScoreEntry> highScores;

        public HighScoreWrapper(List<HighScoreEntry> highScores)
        {
            this.highScores = highScores;
        }
    }

    // Для отображения таблицы (вызовите этот метод в UI)
    public void DisplayHighScores()
    {
        Debug.Log("=== HIGH SCORES ===");
        for (int i = 0; i < highScores.Count; i++)
        {
            Debug.Log($"{i + 1}. {highScores[i].playerName}: {highScores[i].score}");
        }
    }
}