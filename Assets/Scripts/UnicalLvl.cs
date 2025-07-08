using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PremiumButton : MonoBehaviour
{
    [SerializeField] private Button premiumButton;
    [SerializeField] private int UnlockScore = 3000;
    [SerializeField] private GameObject lockedIcon;
    [SerializeField] private GameObject unlockedIcon;

    private int _score;

    void Start()
    {
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        _score = PlayerPrefs.GetInt("HighScore", 0);
        bool isUnlocked = _score >= UnlockScore;

        premiumButton.interactable = isUnlocked;
        lockedIcon.SetActive(!isUnlocked);
        unlockedIcon.SetActive(isUnlocked);
    }

    public void OnPremiumButtonClick()
    {
        SceneManager.LoadScene(5);
    }
}