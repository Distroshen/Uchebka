using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Transform> _points = new List<Transform>();

    // Префабы объектов
    private GameObject _coin;
    private GameObject _bomb;
    private GameObject _coin5;
    private GameObject _Ya1;
    private GameObject _Ya2;
    private GameObject _bocka1;
    private GameObject _bocka2;

    // Настройки
    private float _startSpawnBomb;
    private float _timer;

    // Вероятности
    private const float BombChance = 30f;
    private const float YaChance = 15f;
    private const float BockaChance = 20f;
    private const float TwoBombsChance = 10f;
    private const float TwoBombsTimeThreshold = 5f;
    private const int MaxObstaclesPerTile = 2; // Максимальное количество препятствий на плитке

    public void Initialize(
        GameObject coin5, GameObject coin, GameObject bomb,
        GameObject Ya1, GameObject Ya2, GameObject bocka1, GameObject bocka2,
        float startSpawnBomb, float timer)
    {
        _coin5 = coin5;
        _coin = coin;
        _bomb = bomb;
        _Ya1 = Ya1;
        _Ya2 = Ya2;
        _bocka1 = bocka1;
        _bocka2 = bocka2;
        _startSpawnBomb = startSpawnBomb;
        _timer = timer;

        GenerateObjects();
    }

    private void GenerateObjects()
    {
        // Проверка на специальное событие
        if (_timer > TwoBombsTimeThreshold && Random.Range(0f, 100f) < TwoBombsChance)
        {
            SpawnTwoBombsMode();
        }
        else
        {
            StandardSpawning();
        }
    }

    private void SpawnTwoBombsMode()
    {
        // Создаем копию списка точек для безопасного удаления
        List<Transform> availablePoints = new List<Transform>(_points);

        // Спавним две бомбы на случайных позициях
        SpawnObjectAtRandomPoint(ref availablePoints, _bomb);
        SpawnObjectAtRandomPoint(ref availablePoints, _bomb);

        // Заполняем остальные точки
        FillRemainingPoints(availablePoints);
    }

    private void StandardSpawning()
    {
        // Создаем копию списка точек для безопасного удаления
        List<Transform> availablePoints = new List<Transform>(_points);

        // Спавним основные объекты
        SpawnObstacles(ref availablePoints);

        // Заполняем оставшиеся точки монетами
        FillRemainingPoints(availablePoints);
    }

    private void SpawnObstacles(ref List<Transform> points)
    {
        if (points.Count == 0) return;

        // Определяем общий шанс спавна препятствия
        float obstacleChance = _timer < _startSpawnBomb ? 0 :
            Mathf.Clamp(20f + (_timer / 2f), 0f, 50f);

        // Максимальное количество препятствий
        int obstaclesToSpawn = Mathf.Min(MaxObstaclesPerTile, points.Count);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            // Проверяем шанс для каждого препятствия
            if (Random.Range(0f, 100f) < obstacleChance)
            {
                GameObject obstaclePrefab = GetRandomObstacle();
                SpawnObjectAtRandomPoint(ref points, obstaclePrefab);
            }

            // Уменьшаем шанс для следующего препятствия
            obstacleChance *= 0.5f;
        }
    }

    private GameObject GetRandomObstacle()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < BombChance) return _bomb;
        if (randomValue < BombChance + YaChance) return _Ya1;
        if (randomValue < BombChance + YaChance * 2) return _Ya2;
        if (randomValue < BombChance + YaChance * 2 + BockaChance) return _bocka1;

        return _bocka2;
    }

    private void FillRemainingPoints(List<Transform> points)
    {
        foreach (Transform point in points)
        {
            GameObject coinPrefab = ShouldSpawnCoin5() ? _coin5 : _coin;
            Instantiate(coinPrefab, point.position, Quaternion.identity, transform);
        }
    }

    private void SpawnObjectAtRandomPoint(ref List<Transform> points, GameObject prefab)
    {
        if (points.Count == 0) return;

        int randomIndex = Random.Range(0, points.Count);
        Transform point = points[randomIndex];
        points.RemoveAt(randomIndex);

        Instantiate(prefab, point.position, Quaternion.identity, transform);
    }

    private bool ShouldSpawnCoin5()
    {
        return _timer > 60f;
    }
}