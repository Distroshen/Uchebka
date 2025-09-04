using System.Collections.Generic;
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

    [System.Serializable]
    private class HighScoreWrapper
    {
        public List<HighScoreEntry> highScores;

        public HighScoreWrapper(List<HighScoreEntry> highScores)
        {
            this.highScores = highScores;
        }
    }

    public List<HighScoreEntry> highScores = new List<HighScoreEntry>();
    private const int maxEntries = 10;
    private const string playerPrefsKey = "HighScores";

    // ������������ �������� ��� �����������
    private HighScoreEntry _lastEntry;
    private bool _isInitialized = false;

    void Start()
    {
        if (!PlayerPrefs.HasKey(playerPrefsKey))
        {
            InitializeDefaultScores();
        }
        else
        {
            LoadHighScores();
        }

        _isInitialized = true;

        // �������� ��������� ������ ��� �������� �������
        UpdateCachedLastEntry();
    }

    private void InitializeDefaultScores()
    {
        // ���������� �������������� ��������� ������ ��� ���������� ��������
        HighScoreEntry[] defaultScores = {
            new HighScoreEntry("Dev1", 50),
            new HighScoreEntry("Dev2", 45),
            new HighScoreEntry("Dev3", 40),
            new HighScoreEntry("Dev4", 35),
            new HighScoreEntry("Dev5", 30),
            new HighScoreEntry("Dev6", 25),
            new HighScoreEntry("Dev7", 20),
            new HighScoreEntry("Dev8", 15),
            new HighScoreEntry("Dev9", 11),
            new HighScoreEntry("Dev10", 1)
        };

        highScores.AddRange(defaultScores);
        SaveHighScores();
    }

    public void AddNewScore(string name, int score)
    {
        // ������� ����� ������
        var newEntry = new HighScoreEntry(name, score);

        // ������� ������� ��� �������
        int insertIndex = 0;
        for (; insertIndex < highScores.Count; insertIndex++)
        {
            if (score > highScores[insertIndex].score)
            {
                break;
            }
        }

        // ��������� �� ���������� �������
        highScores.Insert(insertIndex, newEntry);

        // ������� ������ ������ ���� �������� �����
        if (highScores.Count > maxEntries)
        {
            highScores.RemoveAt(highScores.Count - 1);
        }

        // ��������� ������������ ��������� ������
        UpdateCachedLastEntry();

        SaveHighScores();
    }

    public bool IsNewHighScore(int score)
    {
        // ���������� ������������ ��������� ������ ��� ������� ��������
        return highScores.Count < maxEntries || score > _lastEntry.score;
    }

    private void SaveHighScores()
    {
        if (!_isInitialized) return;

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

    private void UpdateCachedLastEntry()
    {
        if (highScores.Count > 0)
        {
            _lastEntry = highScores[highScores.Count - 1];
        }
        else
        {
            _lastEntry = null;
        }
    }

    // ���������������� ����� ��� ����������� �������
    public void DisplayHighScores()
    {
        if (!Application.isEditor) return;

        Debug.Log("=== HIGH SCORES ===");
        for (int i = 0; i < highScores.Count; i++)
        {
            Debug.Log($"{i + 1}. {highScores[i].playerName}: {highScores[i].score}");
        }
    }

    // ������� �������� (��� �������)
    public void ClearHighScores()
    {
        highScores.Clear();
        PlayerPrefs.DeleteKey(playerPrefsKey);
        UpdateCachedLastEntry();
    }
}