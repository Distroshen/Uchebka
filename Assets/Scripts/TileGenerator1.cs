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

    [Header("Вероятности")]
    [Range(0, 100)][SerializeField] private float _specialTileChance = 20f;

    private List<Tile> _tiles = new List<Tile>();
    private float _timer;
    private bool _isActive = true;
    private float _nextSpawnZ = 0f; // Текущая позиция для спавна
    private const float TILE_OFFSET = 10f; // Фиксированное расстояние между тайлами

    void Start()
    {
        _nextSpawnZ = _startPoint != null ? _startPoint.position.z : 0f;
        CreateFirstTile();
        GenerateInitialTiles();
    }

    void Update()
    {
        if (!_isActive) return;

        _timer += Time.deltaTime;

        if (_tiles.Count < _maxCount)
        {
            GenerateTile();
        }
    }

    private void CreateFirstTile()
    {
        Vector3 startPos = _startPoint != null ? _startPoint.position : Vector3.zero;
        CreateTile(_normalTilePrefab, startPos, false);
        _nextSpawnZ += TILE_OFFSET; // Увеличиваем позицию для следующего тайла
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
        if (_tiles.Count == 0)
        {
            CreateFirstTile();
            return;
        }

        GameObject prefabToUse = GetRandomTilePrefab(out bool isRotatable);
        Vector3 spawnPosition = CalculateSpawnPosition();

        CreateTile(prefabToUse, spawnPosition, isRotatable);
    }

    private GameObject GetRandomTilePrefab(out bool isRotatable)
    {
        isRotatable = false;
        bool isSpecial = Random.Range(0f, 100f) <= _specialTileChance;

        if (isSpecial && _specialTilePrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, _specialTilePrefabs.Length);
            GameObject specialPrefab = _specialTilePrefabs[randomIndex];
            isRotatable = specialPrefab.CompareTag("Поворот");
            return specialPrefab;
        }

        return _normalTilePrefab;
    }

    private Vector3 CalculateSpawnPosition()
    {
        return new Vector3(
            _startPoint.position.x,
            _startPoint.position.y,
            _nextSpawnZ
        );
    }

    private void CreateTile(GameObject prefab, Vector3 position, bool isRotatable)
    {
        GameObject newTileObj = Instantiate(prefab, position, Quaternion.identity, _tileHolder);

        if (isRotatable && Random.Range(0, 2) == 1)
        {
            newTileObj.transform.Rotate(0f, 180f, 0f);
        }

        if (newTileObj.TryGetComponent(out Tile newTile))
        {
            _tiles.Add(newTile);
        }
        else
        {
            Debug.LogError("Отсутствует компонент Tile!", newTileObj);
            Destroy(newTileObj);
        }

        _nextSpawnZ += TILE_OFFSET; // Увеличиваем позицию для следующего тайла
    }

    private Tile GetLastValidTile()
    {
        if (_tiles.Count == 0) return null;

        // Ищем последнюю валидную плитку с конца
        for (int i = _tiles.Count - 1; i >= 0; i--)
        {
            if (_tiles[i] != null) return _tiles[i];
            _tiles.RemoveAt(i);
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Tile tile) && _tiles.Contains(tile))
        {
            _tiles.Remove(tile);
            Destroy(tile.gameObject);
        }
    }
}