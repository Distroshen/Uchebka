using System.Collections.Generic;
using UnityEngine;

public class TileGenerator_Back : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private GameObject _normalTilePrefab;
    [SerializeField] private GameObject[] _specialTilePrefabs;
    [SerializeField] private int _maxCount = 5;
    [SerializeField] private Transform _tileHolder;
    [SerializeField] private Transform _startPoint;

    [Header("Настройки пула объектов")]
    [SerializeField] private int _poolSize = 20;
    [SerializeField] private bool _useObjectPool = true;

    [Header("Вероятности")]
    [Range(0, 100)][SerializeField] private float _specialTileChance = 20f;

    private List<Tile> _tiles = new List<Tile>();
    private float _nextSpawnZ = 0f;
    private const float TILE_OFFSET = 10f;

    // Кэшированные значения
    private Vector3 _startPosition;
    private bool _hasStartPoint;
    private static readonly Quaternion ROTATION_180 = Quaternion.Euler(0f, 180f, 0f);

    // Пул объектов
    private Queue<GameObject> _normalTilePool = new Queue<GameObject>();
    private Dictionary<GameObject, bool> _isSpecialTile = new Dictionary<GameObject, bool>();

    void Start()
    {
        _hasStartPoint = _startPoint != null;
        _startPosition = _hasStartPoint ? _startPoint.position : Vector3.zero;
        _nextSpawnZ = _startPosition.z;

        // Инициализируем пул объектов
        if (_useObjectPool)
        {
            InitializeObjectPool();
        }

        CreateFirstTile();
        GenerateInitialTiles();

        // Запускаем корутину для проверки вместо Update
        StartCoroutine(CheckTilesCoroutine());
    }

    private void InitializeObjectPool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject tile = Instantiate(_normalTilePrefab, _tileHolder);
            tile.SetActive(false);
            _normalTilePool.Enqueue(tile);
            _isSpecialTile[tile] = false;
        }
    }

    private System.Collections.IEnumerator CheckTilesCoroutine()
    {
        var waitForSeconds = new WaitForSeconds(0.5f);

        while (true)
        {
            if (_tiles.Count < _maxCount)
            {
                GenerateTile();
            }
            yield return waitForSeconds;
        }
    }

    private void CreateFirstTile()
    {
        CreateTile(_normalTilePrefab, _startPosition, false);
        _nextSpawnZ += TILE_OFFSET;
    }

    private void GenerateInitialTiles()
    {
        int tilesToCreate = Mathf.Max(0, _maxCount - _tiles.Count);
        for (int i = 0; i < tilesToCreate; i++)
        {
            GenerateTile();
        }
    }

    private void GenerateTile()
    {
        GameObject prefabToUse = GetRandomTilePrefab(out bool isRotatable);
        Vector3 spawnPosition = new Vector3(
            _startPosition.x,
            _startPosition.y,
            _nextSpawnZ
        );

        CreateTile(prefabToUse, spawnPosition, isRotatable);
    }

    private GameObject GetRandomTilePrefab(out bool isRotatable)
    {
        isRotatable = false;

        if (_specialTilePrefabs.Length > 0 &&
            Random.Range(0f, 100f) <= _specialTileChance)
        {
            GameObject specialPrefab = _specialTilePrefabs[Random.Range(0, _specialTilePrefabs.Length)];
            isRotatable = specialPrefab.CompareTag("Поворот");
            return specialPrefab;
        }

        return _normalTilePrefab;
    }

    private void CreateTile(GameObject prefab, Vector3 position, bool isRotatable)
    {
        GameObject newTileObj;
        bool isSpecial = prefab != _normalTilePrefab;

        // Используем пул объектов для обычных тайлов
        if (!isSpecial && _useObjectPool && _normalTilePool.Count > 0)
        {
            newTileObj = _normalTilePool.Dequeue();
            newTileObj.transform.position = position;
            newTileObj.transform.rotation = isRotatable && Random.Range(0, 2) == 1 ? ROTATION_180 : Quaternion.identity;
            newTileObj.SetActive(true);
        }
        else
        {
            // Для специальных тайлов или если пул пуст создаем новый объект
            Quaternion rotation = isRotatable && Random.Range(0, 2) == 1 ? ROTATION_180 : Quaternion.identity;
            newTileObj = Instantiate(prefab, position, rotation, _tileHolder);
        }

        _isSpecialTile[newTileObj] = isSpecial;

        if (newTileObj.TryGetComponent(out Tile newTile))
        {
            _tiles.Add(newTile);
        }
        else
        {
            Debug.LogError("Отсутствует компонент Tile!", newTileObj);
            DestroyOrReturnToPool(newTileObj, isSpecial);
            return;
        }

        _nextSpawnZ += TILE_OFFSET;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Tile tile) && _tiles.Contains(tile))
        {
            _tiles.Remove(tile);
            DestroyOrReturnToPool(tile.gameObject, _isSpecialTile.ContainsKey(tile.gameObject) && _isSpecialTile[tile.gameObject]);

            // Немедленно проверяем是否需要 создать новый тайл
            if (_tiles.Count < _maxCount)
            {
                GenerateTile();
            }
        }
    }

    private void DestroyOrReturnToPool(GameObject tileObject, bool isSpecial)
    {
        if (!isSpecial && _useObjectPool)
        {
            // Возвращаем в пул
            tileObject.SetActive(false);
            _normalTilePool.Enqueue(tileObject);
        }
        else
        {
            // Уничтожаем специальные тайлы
            Destroy(tileObject);

            // Удаляем из словаря если это специальный тайл
            if (_isSpecialTile.ContainsKey(tileObject))
            {
                _isSpecialTile.Remove(tileObject);
            }
        }
    }

    // Метод для очистки пула при необходимости
    public void ClearPool()
    {
        while (_normalTilePool.Count > 0)
        {
            GameObject tile = _normalTilePool.Dequeue();
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        _normalTilePool.Clear();
        _isSpecialTile.Clear();
    }

    // Метод для перезаполнения пула
    public void RefillPool()
    {
        ClearPool();
        InitializeObjectPool();
    }
}