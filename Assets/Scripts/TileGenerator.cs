using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private GameObject _normalTilePrefab; // Префаб обычного тайла
    [SerializeField] private GameObject[] _specialTilePrefabs; // Префабы специальных тайлов
    [SerializeField] private int _maxCount = 5; // Максимальное количество активных тайлов
    [SerializeField] private Transform _tileHolder; // Контейнер для тайлов
    [SerializeField] private Transform _startPoint; // Стартовая позиция

    [Header("Настройки объектов")]
    // Префабы объектов для генерации на тайлах:
    [SerializeField] private GameObject _coin5; // Монета x5
    [SerializeField] private GameObject _coin; // Обычная монета
    [SerializeField] private GameObject _bomb; // Бомба
    [SerializeField] private GameObject _Ya1; // Ящик 1
    [SerializeField] private GameObject _Ya2; // Ящик 2
    [SerializeField] private GameObject _bocka1; // Бочка 1
    [SerializeField] private GameObject _bocka2; // Бочка 2
    [SerializeField] private GameObject _drop; // Усилитель скорости
    [SerializeField] private GameObject _bass; // Усилитель брони
    [SerializeField] private GameObject _gnome; // Препятствие "гном"
    [SerializeField] private float _startSpawnBomb = 3; // Время до появления бомб

    [Header("Вероятности")]
    [Range(0, 100)][SerializeField] private float _specialTileChance = 20f; // Шанс спец.тайла

    private List<Tile> _tiles = new List<Tile>(); // Список активных тайлов
    private float _timer; // Игровой таймер
    private bool _isActive = true; // Флаг активности генератора
    private Dictionary<GameObject, float> _tileLengthCache = new Dictionary<GameObject, float>(); // Кэш длин тайлов

    void Start()
    {
        CachePrefabLengths(); // Кэширование длин префабов
        CreateFirstTile(); // Создание первого тайла
        GenerateInitialTiles(); // Генерация начальных тайлов
    }

    void Update()
    {
        if (!_isActive) return; // Проверка активности

        _timer += Time.deltaTime; // Обновление игрового времени

        // Генерация новых тайлов при необходимости
        if (_tiles.Count < _maxCount)
        {
            GenerateTile();
        }
    }

    // Кэширование длин префабов тайлов
    private void CachePrefabLengths()
    {
        CacheTileLength(_normalTilePrefab);
        foreach (var prefab in _specialTilePrefabs)
        {
            CacheTileLength(prefab);
        }
    }

    // Расчет и кэширование длины конкретного префаба
    private void CacheTileLength(GameObject tilePrefab)
    {
        if (_tileLengthCache.ContainsKey(tilePrefab)) return; // Пропуск если уже в кэше

        // Определение длины через коллайдер или трансформ
        if (tilePrefab.TryGetComponent(out Collider collider))
        {
            _tileLengthCache[tilePrefab] = collider.bounds.size.z;
        }
        else
        {
            _tileLengthCache[tilePrefab] = tilePrefab.transform.localScale.z;
        }
    }

    // Создание стартового тайла
    private void CreateFirstTile()
    {
        Vector3 startPos = _startPoint != null ? _startPoint.position : Vector3.zero;
        CreateTile(_normalTilePrefab, startPos, false);
    }

    // Генерация начального набора тайлов
    private void GenerateInitialTiles()
    {
        int tilesToCreate = Mathf.Max(0, _maxCount - _tiles.Count);
        for (int i = 0; i < tilesToCreate; i++)
        {
            GenerateTile();
        }
    }

    // Основная логика генерации нового тайла
    private void GenerateTile()
    {
        if (_tiles.Count == 0)
        {
            CreateFirstTile();
            return;
        }

        Tile lastTile = GetLastValidTile(); // Получение последнего валидного тайла
        if (lastTile == null) return;

        // Выбор префаба для нового тайла
        GameObject prefabToUse = GetRandomTilePrefab(out bool isRotatable);
        // Расчет позиции спавна
        Vector3 spawnPosition = CalculateSpawnPosition(lastTile, prefabToUse);

        CreateTile(prefabToUse, spawnPosition, isRotatable);
    }

    // Случайный выбор типа тайла
    private GameObject GetRandomTilePrefab(out bool isRotatable)
    {
        isRotatable = false;
        bool isSpecial = Random.Range(0f, 100f) <= _specialTileChance;

        // Выбор спец.тайла
        if (isSpecial && _specialTilePrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, _specialTilePrefabs.Length);
            GameObject specialPrefab = _specialTilePrefabs[randomIndex];
            // Проверка на поворотный тайл
            isRotatable = specialPrefab.CompareTag("Поворот");
            return specialPrefab;
        }

        return _normalTilePrefab; // Обычный тайл по умолчанию
    }

    // Расчет позиции для нового тайла
    private Vector3 CalculateSpawnPosition(Tile lastTile, GameObject nextPrefab)
    {
        float nextTileLength = _tileLengthCache[nextPrefab]; // Длина нового тайла
        float lastTileEndZ = GetTileEndPosition(lastTile); // Конечная позиция последнего тайла

        // Позиция рассчитывается как конец предыдущего + половина длины нового
        return new Vector3(
            _startPoint.position.x,
            _startPoint.position.y,
            lastTileEndZ + nextTileLength
        );
    }

    // Получение конечной Z-позиции тайла
    private float GetTileEndPosition(Tile tile)
    {
        // Через коллайдер
        if (tile.TryGetComponent(out Collider collider))
        {
            return collider.bounds.max.z;
        }
        // Через трансформ
        return tile.transform.position.z + tile.transform.localScale.z / 2f;
    }

    // Создание экземпляра тайла
    private void CreateTile(GameObject prefab, Vector3 position, bool isRotatable)
    {
        GameObject newTileObj = Instantiate(prefab, position, Quaternion.identity, _tileHolder);
        // Фиксация позиции по X и Y
        newTileObj.transform.position = new Vector3(
            _startPoint.position.x,
            _startPoint.position.y,
            position.z += 10f
        );

        // Случайный поворот для поворотных тайлов
        if (isRotatable && Random.Range(0, 2) == 1)
        {
            newTileObj.transform.Rotate(0f, 180f, 0f);
        }

        // Инициализация компонента Tile
        if (newTileObj.TryGetComponent(out Tile newTile))
        {
            newTile.Initialize(
                _coin5, _coin, _bomb,
                _Ya1, _Ya2, _bocka1, _bocka2,
                _startSpawnBomb, _timer,
                _drop, _bass, _gnome
            );
            _tiles.Add(newTile);
        }
        else
        {
            Debug.LogError("Отсутствует компонент Tile!", newTileObj);
            Destroy(newTileObj);
        }
    }

    // Поиск последнего валидного тайла в списке
    private Tile GetLastValidTile()
    {
        if (_tiles.Count == 0) return null;

        // Поиск с конца списка
        for (int i = _tiles.Count - 1; i >= 0; i--)
        {
            if (_tiles[i] != null) return _tiles[i];
            _tiles.RemoveAt(i); // Удаление невалидных ссылок
        }

        return null;
    }

    // Обработчик столкновений для удаления пройденных тайлов
    private void OnTriggerEnter(Collider other)
    {
        // Проверка на компонент Tile
        if (other.TryGetComponent(out Tile tile) && _tiles.Contains(tile))
        {
            _tiles.Remove(tile);
            Destroy(tile.gameObject);
        }
    }
}