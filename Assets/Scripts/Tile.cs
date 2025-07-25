using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Transform> _points = new List<Transform>();

    private GameObject _coin;
    private GameObject _bomb;
    private GameObject _coin5;
    private float _startSpawnBomb;
    private float _timer;

    void Start()
    {
        {
            // �������� �� ����������� ������� ����� 60 ������
            if (_timer > 5f && Random.Range(0f, 100f) < 10f)
            {
                // ����� "��� �����"
                SpawnTwoBombsMode();
            }
            else
            {
                // ����������� ���������
                StandardSpawning();
            }
        }
    }

    private void SpawnTwoBombsMode()
    {
        // ���������, ��� ����� ���������� (������� 2)
        if (_points.Count <= 2)
        {
            StandardSpawning();
            return;
        }

        // ������� ������ ��������� ��������
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < _points.Count; i++)
        {
            availableIndices.Add(i);
        }

        // �������� ������ �����
        int firstBombIndex = Random.Range(0, availableIndices.Count);
        SafeCreateObject(availableIndices[firstBombIndex], _bomb);
        availableIndices.RemoveAt(firstBombIndex);

        // �������� ������ ����� (������ availableIndices.Count ���������� �� 1)
        int secondBombIndex = Random.Range(0, availableIndices.Count);
        SafeCreateObject(availableIndices[secondBombIndex], _bomb);
        availableIndices.RemoveAt(secondBombIndex);

        // ��������� ����� ��������� �������� (����� 60 ������ - coin5)
        foreach (int remainingIndex in availableIndices)
        {
            SafeCreateObject(remainingIndex, ShouldSpawnCoin5() ? _coin : _coin5);
        }
    }

    private void StandardSpawning()
    {
        int randomPointIndex = Random.Range(0, _points.Count);

        if (_timer < _startSpawnBomb)
        {
            SafeCreateObject(randomPointIndex, ShouldSpawnCoin5() ? _coin : _coin5 );
        }
        else
        {
            float chanceSpawnBomb = 20 + (_timer / 2);
            chanceSpawnBomb = Mathf.Clamp(chanceSpawnBomb, 0, 50);

            if (Random.Range(0, 100) < chanceSpawnBomb)
            {
                SafeCreateObject(randomPointIndex, _bomb);
            }
            else
            {
                SafeCreateObject(randomPointIndex, ShouldSpawnCoin5() ? _coin : _coin5);
            }
        }
    }

    // ���������, ����� �� �������� coin5 (����� 60 ������)
    private bool ShouldSpawnCoin5()
    {
        return _timer > 60f;
    }

    private void SafeCreateObject(int pointIndex, GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab, _points[pointIndex].position, Quaternion.identity);
        newObj.transform.SetParent(transform);
    }

    public void Initialize(GameObject coin, GameObject coin5, GameObject bomb, float startSpawnBomb, float timer)
    {
        _coin5 = coin5;
        _coin = coin;
        _bomb = bomb;
        _timer = timer;
        _startSpawnBomb = startSpawnBomb;
    }
}