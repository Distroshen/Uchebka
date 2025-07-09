using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int CurrentCoins { get; private set; } = 10; // —тартовое количество

    public void SpendCoins(int amount)
    {
        CurrentCoins = Mathf.Max(0, CurrentCoins - amount);
        // «десь можно добавить систему событий дл€ обновлени€ UI
    }

    public void AddCoins(int amount)
    {
        CurrentCoins += amount;
    }
}