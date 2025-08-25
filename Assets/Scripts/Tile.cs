using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Transform> _points = new List<Transform>(); // Точки спавна объектов на тайле

    // Префабы объектов
    private GameObject _coin;     // Обычная монета
    private GameObject _bomb;     // Бомба-препятствие
    private GameObject _coin5;    // Монета x5
    private GameObject _Ya1;      // Препятствие "Ящик 1"
    private GameObject _Ya2;      // Препятствие "Ящик 2"
    private GameObject _bocka1;   // Препятствие "Бочка 1"
    private GameObject _bocka2;   // Препятствие "Бочка 2"
    private GameObject _drop;     // Усилитель скорости (капля)
    private GameObject _bass;     // Усилитель брони (бас-гитара)
    private GameObject _gnome;    // Сложное препятствие (гном)

    // Настройки генерации
    private float _startSpawnBomb; // Время начала спавна бомб
    private float _timer;          // Текущее игровое время

    // Вероятности появления препятствий (%)
    private const float BombChance = 30f;  // Шанс бомбы
    private const float YaChance = 15f;    // Шанс ящика
    private const float BockaChance = 20f; // Шанс бочки
    private const float GnomeChance = 1f;  // Шанс гнома (самый низкий)
    private const int MaxObstaclesPerTile = 2; // Макс. препятствий на тайл

    // Система шансов для усилителей
    private static float _boostersChance = 10f;       // Текущий шанс усилителя
    private const float BoosterDecayFactor = 0.9f;    // Множитель уменьшения шанса
    private const float MinBoosterChance = 0.5f;      // Минимальный шанс

    // Кэш вертикальных смещений для префабов
    private Dictionary<GameObject, float> _prefabBottomOffsets = new Dictionary<GameObject, float>();

    //[SerializeField] private AudioSource BombFX; // Звуковой эффект бомбы
    public bool Life; // Флаг активности тайла

    // Инициализация тайла
    public void Initialize(
        GameObject coin5, GameObject coin, GameObject bomb,
        GameObject Ya1, GameObject Ya2, GameObject bocka1, GameObject bocka2,
        float startSpawnBomb, float timer,
        GameObject drop, GameObject bass, GameObject gnome)
    {
        // Присвоение префабов
        _coin5 = coin5;
        _coin = coin;
        _bomb = bomb;
        _Ya1 = Ya1;
        _Ya2 = Ya2;
        _bocka1 = bocka1;
        _bocka2 = bocka2;
        _drop = drop;
        _bass = bass;
        _gnome = gnome;

        // Настройка параметров
        _startSpawnBomb = startSpawnBomb;
        _timer = timer;

        // Генерация объектов на тайле
        List<Transform> availablePoints = new List<Transform>(_points);
        GenerateObjects(ref availablePoints);
    }

    // Основной метод генерации объектов
    private void GenerateObjects(ref List<Transform> points)
    {
        // Режим спавна при малом количестве точек
        if (points.Count <= 2)
        {
            SpawnOneBombsMode();
        }
        else // Стандартный режим
        {
            StandardSpawning();
        }
    }

    // Режим спавна с одной бомбой
    private void SpawnOneBombsMode()
    {
        List<Transform> availablePoints = new List<Transform>(_points);
        SpawnObjectAtRandomPoint(ref availablePoints, _bomb); // Спавн бомбы
        FillRemainingPoints(availablePoints); // Заполнение оставшихся точек
    }

    // Стандартный режим генерации
    private void StandardSpawning()
    {
        List<Transform> availablePoints = new List<Transform>(_points);
        SpawnObstacles(ref availablePoints); // Спавн препятствий
        FillRemainingPoints(availablePoints); // Заполнение оставшихся точек
    }

    // Генерация препятствий
    private void SpawnObstacles(ref List<Transform> points)
    {
        if (points.Count == 0) return;

        // Расчет шанса появления препятствия
        float obstacleChance = _timer < _startSpawnBomb ? 0 :
            Mathf.Clamp(20f + (_timer / 2f), 0f, 50f); // Зависит от времени

        int obstaclesToSpawn = Mathf.Min(MaxObstaclesPerTile, points.Count);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            if (Random.Range(0f, 100f) < obstacleChance)
            {
                GameObject obstaclePrefab = GetRandomObstacle();
                SpawnObjectAtRandomPoint(ref points, obstaclePrefab);
            }
            obstacleChance *= 0.5f; // Уменьшение шанса для след. препятствия
        }
    }

    // Выбор случайного препятствия
    private GameObject GetRandomObstacle()
    {
        float randomValue = Random.Range(0f, 100f);
        float currentChance = BombChance;

        // Весовая система выбора
        if (randomValue < BombChance) return _bomb;
        currentChance += YaChance;
        if (randomValue < currentChance) return _Ya1;
        currentChance += YaChance;
        if (randomValue < currentChance) return _Ya2;
        currentChance += BockaChance;
        if (randomValue < currentChance) return _bocka1;
        currentChance += GnomeChance;
        if (randomValue < currentChance) return _gnome;

        return _bocka2;
    }

    // Заполнение оставшихся точек
    private void FillRemainingPoints(List<Transform> points)
    {
        if (points.Count == 0) return;

        // Группировка точек по Z-координате (ряды)
        var rows = points
            .GroupBy(p => Mathf.Round(p.position.z * 100f) / 100f)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var row in rows)
        {
            var randomPoint = row.OrderBy(x => Random.value).FirstOrDefault();
            if (randomPoint != null)
            {
                // Проверка на спавн усилителя
                if (_timer > 10f && Random.Range(0f, 100f) < _boostersChance)
                {
                    GameObject booster = Random.Range(0, 2) == 0 ? _drop : _bass;
                    SpawnCoin(booster, randomPoint.position);
                    // Уменьшение шанса после спавна
                    _boostersChance = Mathf.Max(MinBoosterChance, _boostersChance * BoosterDecayFactor);
                }
                else // Спавн монет
                {
                    GameObject coinPrefab = ShouldSpawnCoin5() ? _coin5 : _coin;
                    SpawnCoin(coinPrefab, randomPoint.position);
                }
            }
        }
    }

    // Спавн монеты/усилителя
    private void SpawnCoin(GameObject coinPrefab, Vector3 position)
    {
        GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity, transform);
        AdjustObjectPosition(coin, position.y); // Корректировка позиции
    }

    // Спавн объекта в случайной точке
    private void SpawnObjectAtRandomPoint(ref List<Transform> points, GameObject prefab)
    {
        if (points.Count == 0) return;

        Transform point = null;

        // Особый режим для гнома (требует 3 точки)
        if (prefab == _gnome && points.Count >= 3)
        {
            // Группировка точек по Z-координате
            var groups = points
                .GroupBy(p => Mathf.Round(p.position.z * 100f) / 100f)
                .Where(g => g.Count() >= 3)
                .ToList();

            if (groups.Count > 0)
            {
                // Выбор случайной группы
                var group = groups[Random.Range(0, groups.Count)];
                var sortedPoints = group.OrderBy(p => p.position.x).ToList();

                // Поиск трех последовательных точек
                for (int i = 0; i < sortedPoints.Count - 2; i++)
                {
                    float diff1 = sortedPoints[i + 1].position.x - sortedPoints[i].position.x;
                    float diff2 = sortedPoints[i + 2].position.x - sortedPoints[i + 1].position.x;

                    // Проверка равномерного распределения
                    if (Mathf.Abs(diff1 - diff2) < 0.1f && diff1 > 0.1f)
                    {
                        // Нашли подходящие точки
                        Transform leftPoint = sortedPoints[i];
                        point = sortedPoints[i + 1]; // Центральная точка для гнома
                        Transform rightPoint = sortedPoints[i + 2];

                        // Удаление использованных точек
                        points.Remove(leftPoint);
                        points.Remove(point);
                        break;
                    }
                }
            }
        }

        // Стандартный выбор точки
        if (point == null)
        {
            if (prefab == _gnome) // Замена гнома на бомбу
            {
                prefab = _bomb;
            }

            int randomIndex = Random.Range(0, points.Count);
            point = points[randomIndex];
            points.RemoveAt(randomIndex);
        }

        // Создание объекта
        GameObject spawnedObject = Instantiate(prefab, point.position, Quaternion.identity, transform);
        ApplyRandomRotation(spawnedObject); // Случайный поворот
        AdjustObjectPosition(spawnedObject, point.position.y); // Корректировка позиции
    }

    // Случайный поворот объектов с тегом "Поворот"
    private void ApplyRandomRotation(GameObject spawnedObject)
    {
        if (spawnedObject.CompareTag("Поворот"))
        {
            bool shouldRotate = Random.Range(0, 2) == 1;
            if (shouldRotate)
            {
                spawnedObject.transform.Rotate(0f, 90f, 0f, Space.Self);
            }
        }
    }

    // Корректировка вертикальной позиции объекта
    private void AdjustObjectPosition(GameObject obj, float groundY)
    {
        float bottomOffset = GetBottomOffsetForPrefab(obj);
        Vector3 newPosition = obj.transform.position;

        // Коэффициенты коррекции для разных типов объектов
        float reductionFactor = 0.2f; // По умолчанию

        if (obj.CompareTag("Поворот")) reductionFactor = 2f;
        else if (obj == _bomb) reductionFactor = 0.3f;
        else if (obj == _drop || obj == _bass) reductionFactor = 0.4f;
        else if (obj == _gnome) reductionFactor = 0.35f;

        // Установка новой позиции
        newPosition.y = groundY + bottomOffset * reductionFactor;
        obj.transform.position = newPosition;
    }

    // Получение вертикального смещения для префаба
    private float GetBottomOffsetForPrefab(GameObject prefab)
    {
        // Использование кэшированного значения
        if (_prefabBottomOffsets.ContainsKey(prefab))
        {
            return _prefabBottomOffsets[prefab];
        }

        // Вычисление смещения
        GameObject temp = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        temp.SetActive(false);
        float bottomOffset = CalculateBottomOffset(temp);
        Destroy(temp);

        // Кэширование результата
        _prefabBottomOffsets[prefab] = bottomOffset;
        return bottomOffset;
    }

    // Расчет вертикального смещения
    private float CalculateBottomOffset(GameObject obj)
    {
        // Через коллайдер
        Collider col = obj.GetComponentInChildren<Collider>();
        if (col != null)
        {
            return col.bounds.size.y / 2f;
        }

        // Через рендерер
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.y / 2f;
        }

        Debug.LogWarning($"No Collider or Renderer found on {obj.name}");
        return 0f;
    }

    // Проверка на спавн монеты x5
    private bool ShouldSpawnCoin5()
    {
        return _timer > 60f; // После 60 секунд игры
    }

    // Получение конечной позиции тайла
    public float GetEndPosition()
    {
        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
        {
            return col.bounds.max.z; // По коллайдеру
        }
        return transform.position.z + transform.localScale.z / 2f; // По трансформу
    }

    // Обработка столкновения с игроком
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerTrigger();
        }
    }

    // Обработка триггера игрока
    private void HandlePlayerTrigger()
    {
        //BombFX.Play(); // Звук взрыва
        this.gameObject.SetActive(false); // Деактивация тайла
        Life = false; // Обновление статуса
        FindAnyObjectByType<BustB>().B1(); // Вызов метода разрушения
    }

    // Сброс шанса усилителей
    public static void ResetBoostersChance()
    {
        _boostersChance = 10f;
    }

    private void Start()
    {
        SpawnTile();
    }
    public void SpawnTile()
    {
    }
}