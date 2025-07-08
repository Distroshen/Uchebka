using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CoinTexts;
    public int Coin;
    private void Start()
    {
        Coin = PlayerPrefs.GetInt("Coins", 0);
    }
    public void AddCoin()
    {
        PlayerPrefs.SetInt("Coins", Coin);
        Coin++;
        PlayerPrefs.Save();
    }
    public void AddCoin5()
    {
        PlayerPrefs.SetInt("Coins", Coin);
        Coin += 5;
        PlayerPrefs.Save();
    }
    void Update()
    {
        CoinTexts.text = Coin.ToString();
    }
}