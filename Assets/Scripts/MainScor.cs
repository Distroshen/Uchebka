using TMPro;
using UnityEngine;

public class MainScor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highScoreText;

    private void Start()
    {
        //if (Scor.Instance != null)
        //{
        //_highScoreText.text = $"������: {Scor.Instance.GetCurrentHighScore()}";
        //}
        //else
        ////{
        // �� ������ ���� ���� ��������� ������
        //int savedScore = PlayerPrefs.GetInt("HighScore", 0);
        //_highScoreText.text = $"������: {savedScore}";
        //}
    }
}