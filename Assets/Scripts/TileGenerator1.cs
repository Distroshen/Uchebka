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
    private Dictionary<GameObject, float> _tileLengthCache = new Dictionary<GameObject, float>();

    void Start()
    {
        CachePrefabLengths();
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

    private void CachePrefabLengths()
    {
        CacheTileLength(_normalTilePrefab);
        foreach (var prefab in _specialTilePrefabs)
        {
            CacheTileLength(prefab);
        }
    }

    private void CacheTileLength(GameObject tilePrefab)
    {
        if (_tileLengthCache.ContainsKey(tilePrefab)) return;

        if (tilePrefab.TryGetComponent(out Collider collider))
        {
            _tileLengthCache[tilePrefab] = collider.bounds.size.z;
        }
        else
        {
            _tileLengthCache[tilePrefab] = tilePrefab.transform.localScale.z;
        }
    }

    private void CreateFirstTile()
    {
        Vector3 startPos = _startPoint != null ? _startPoint.position : Vector3.zero;
        CreateTile(_normalTilePrefab, startPos, false);
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

        Tile lastTile = GetLastValidTile();
        if (lastTile == null) return;

        GameObject prefabToUse = GetRandomTilePrefab(out bool isRotatable);
        Vector3 spawnPosition = CalculateSpawnPosition(lastTile, prefabToUse);

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

    private Vector3 CalculateSpawnPosition(Tile lastTile, GameObject nextPrefab)
    {
        float nextTileLength = _tileLengthCache[nextPrefab];
        float lastTileEndZ = GetTileEndPosition(lastTile);

        return new Vector3(
            _startPoint.position.x,
            _startPoint.position.y,
            lastTileEndZ + nextTileLength / 2f
        );
    }

    private float GetTileEndPosition(Tile tile)
    {
        if (tile.TryGetComponent(out Collider collider))
        {
            return collider.bounds.max.z;
        }
        return tile.transform.position.z + tile.transform.localScale.z / 2f;
    }

    private void CreateTile(GameObject prefab, Vector3 position, bool isRotatable)
    {
        GameObject newTileObj = Instantiate(prefab, position, Quaternion.identity, _tileHolder);
        newTileObj.transform.position = new Vector3(
            _startPoint.position.x,
            _startPoint.position.y,
            position.z
        );

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