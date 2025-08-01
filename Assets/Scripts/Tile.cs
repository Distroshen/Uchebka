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
    private const int MaxObstaclesPerTile = 2;

    // Кэш для смещений объектов
    private Dictionary<GameObject, float> _prefabBottomOffsets = new Dictionary<GameObject, float>();

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
        List<Transform> availablePoints = new List<Transform>(_points);
        SpawnObjectAtRandomPoint(ref availablePoints, _bomb);
        FillRemainingPoints(availablePoints);
    }

    private void StandardSpawning()
    {
        List<Transform> availablePoints = new List<Transform>(_points);
        SpawnObstacles(ref availablePoints);
        FillRemainingPoints(availablePoints);
    }

    private void SpawnObstacles(ref List<Transform> points)
    {
        if (points.Count == 0) return;

        float obstacleChance = _timer < _startSpawnBomb ? 0 :
            Mathf.Clamp(20f + (_timer / 2f), 0f, 50f);

        int obstaclesToSpawn = Mathf.Min(MaxObstaclesPerTile, points.Count);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            if (Random.Range(0f, 100f) < obstacleChance)
            {
                GameObject obstaclePrefab = GetRandomObstacle();
                SpawnObjectAtRandomPoint(ref points, obstaclePrefab);
            }
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

        var rows = points
            .GroupBy(p => Mathf.Round(p.position.z * 100f) / 100f)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var row in rows)
        {
            var randomPoint = row.OrderBy(x => Random.value).FirstOrDefault();
            if (randomPoint != null)
            {
                GameObject coinPrefab = ShouldSpawnCoin5() ? _coin5 : _coin;
                SpawnCoin(coinPrefab, randomPoint.position);
            }
        }
    }

    private void SpawnCoin(GameObject coinPrefab, Vector3 position)
    {
        GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity, transform);
        AdjustObjectPosition(coin, position.y);
    }

    private void SpawnObjectAtRandomPoint(ref List<Transform> points, GameObject prefab)
    {
        if (points.Count == 0) return;

        int randomIndex = Random.Range(0, points.Count);
        Transform point = points[randomIndex];
        points.RemoveAt(randomIndex);

        GameObject spawnedObject = Instantiate(prefab, point.position, Quaternion.identity, transform);
        ApplyRandomRotation(spawnedObject);
        AdjustObjectPosition(spawnedObject, point.position.y);
    }

    private void ApplyRandomRotation(GameObject spawnedObject)
    {
        if (spawnedObject.CompareTag("Поворот"))
        {
            bool shouldRotate = Random.Range(0, 2) == 1;
            if (shouldRotate)
            {
                spawnedObject.transform.Rotate(0f, 90f, 0f, Space.Self);
            }
        }//
    }

    private void AdjustObjectPosition(GameObject obj, float groundY)
    {
        float bottomOffset = GetBottomOffsetForPrefab(obj);
        Vector3 newPosition = obj.transform.position;

        // Разные коэффициенты для разных объектов
        float reductionFactor = 0.2f; // По умолчанию

        if (obj.CompareTag("Поворот"))
        {
            // Для повернутых объектов - сильнее "утопить"
            reductionFactor = 2;//
        }
        else if (obj == _bomb)
        {
            // Для бомб - меньше утапливать
            reductionFactor = 0.3f;
        }

        newPosition.y = groundY + bottomOffset * reductionFactor;
        obj.transform.position = newPosition;
    }

    private float GetBottomOffsetForPrefab(GameObject prefab)
    {
        // Если смещение уже в кэше - возвращаем его
        if (_prefabBottomOffsets.ContainsKey(prefab))
        {
            return _prefabBottomOffsets[prefab];
        }

        // Создаем временный экземпляр для вычисления
        GameObject temp = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        temp.SetActive(false);

        float bottomOffset = CalculateBottomOffset(temp);

        Destroy(temp);

        // Кэшируем результат
        _prefabBottomOffsets[prefab] = bottomOffset;
        return bottomOffset;
    }

    private float CalculateBottomOffset(GameObject obj)
    {
        // Пробуем получить коллайдер
        Collider col = obj.GetComponentInChildren<Collider>();
        if (col != null)
        {
            Bounds bounds = col.bounds;
            return bounds.size.y / 2f;
        }

        // Если коллайдера нет, пробуем получить рендерер
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            return bounds.size.y / 2f;
        }

        // Если нет ни коллайдера, ни рендерера
        Debug.LogWarning($"No Collider or Renderer found on {obj.name}");
        return 0f;
    }

    private bool ShouldSpawnCoin5()
    {
        return _timer > 60f;
    }

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