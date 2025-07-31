using System.Collections.Generic;
using System.Linq;
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

        List<Transform> availablePoints = new List<Transform>(_points);
        GenerateObjects(ref availablePoints);
    }

    private void GenerateObjects(ref List<Transform> points)
    {
        // Проверка на специальное событие
        if (points.Count <= 2)
        {
            SpawnOneBombsMode();
        }
        else
        {
            StandardSpawning();
        }
    }

    private void SpawnOneBombsMode()
    {
        // Создаем копию списка точек для безопасного удаления
        List<Transform> availablePoints = new List<Transform>(_points);

        // Спавним 1 бомбы на случайных позициях
        SpawnObjectAtRandomPoint(ref availablePoints, _bomb);

        // Заполняем остальные точки монетами
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
        if (points.Count == 0) return;

        // Группируем точки по рядам (по координате Z)
        var rows = points
            .GroupBy(p => Mathf.Round(p.position.z * 100f) / 100f) // Группировка с округлением
            .OrderBy(g => g.Key) // Сортируем по Z
            .ToList();

        foreach (var row in rows)
        {
            // Выбираем случайную точку в ряду
            var randomPoint = row.OrderBy(x => Random.value).FirstOrDefault();
            if (randomPoint != null)
            {
                GameObject coinPrefab = ShouldSpawnCoin5() ? _coin5 : _coin;
                Instantiate(coinPrefab, randomPoint.position, Quaternion.identity, transform);
            }
        }
    }

    private void SpawnObjectAtRandomPoint(ref List<Transform> points, GameObject prefab)
    {
        if (points.Count == 0) return;

        int randomIndex = Random.Range(0, points.Count);
        Transform point = points[randomIndex];
        points.RemoveAt(randomIndex);

        // Создаем объект
        GameObject spawnedObject = Instantiate(
            prefab,
            point.position,
            Quaternion.identity,
            transform
        );

        // Применяем случайный поворот для объектов с тегом "Поворот"
        ApplyRandomRotation(spawnedObject);
    }

    private void ApplyRandomRotation(GameObject spawnedObject)
    {
        if (spawnedObject.CompareTag("Поворот"))
        {
            // Случайно решаем, нужно ли применять поворот (50% шанс)
            bool shouldRotate = Random.Range(0, 2) == 1;

            if (shouldRotate)
            {
                /// Применяем поворот на 90 градусов по оси X
                spawnedObject.transform.Rotate(0f, 90f, 0f, Space.Self);
            }
        }
    }

    private bool ShouldSpawnCoin5()
    {
        return _timer > 60f;
    }

    // Добавляем метод для получения конечной позиции тайла
    public float GetEndPosition()
    {
        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
        {
            return col.bounds.max.z;
        }
        return transform.position.z + transform.localScale.z / 2f;
    }
}