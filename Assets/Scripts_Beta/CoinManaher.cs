using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int CurrentCoins { get; private set; } = 10; // ��������� ����������

    public void SpendCoins(int amount)
    {
        CurrentCoins = Mathf.Max(0, CurrentCoins - amount);
        // ����� ����� �������� ������� ������� ��� ���������� UI
    }

    public void AddCoins(int amount)
    {
        CurrentCoins += amount;
    }
}