using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private GameObject _normalTilePrefab;
    [SerializeField] private GameObject[] _specialTilePrefabs;
    [SerializeField] private int _maxCount = 5;
    [SerializeField] private Transform _tileHolder;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private TileObjectPool _tilePool;

    [Header("Настройки объектов")]
    [SerializeField] private GameObject _coin5;
    [SerializeField] private GameObject _coin;
    [SerializeField] private GameObject _bomb;
    [SerializeField] private GameObject _Ya1;
    [SerializeField] private GameObject _Ya2;
    [SerializeField] private GameObject _bocka1;
    [SerializeField] private GameObject _bocka2;
    [SerializeField] private GameObject _drop;
    [SerializeField] private GameObject _bass;
    [SerializeField] private GameObject _gnome;
    [SerializeField] private float _startSpawnBomb = 3;

    [Header("Вероятности")]
    [Range(0, 100)][SerializeField] private float _specialTileChance = 20f;

    private List<Tile> _tiles = new List<Tile>();
    private float _timer;
    private bool _isActive = true;
    private float _nextSpawnZ = 0f;
    private const float TILE_OFFSET = 6.15f;

    // Кэшированные значения
    private Vector3 _startPosition;
    private bool _hasStartPoint;
    private static readonly int RotationAngle = 180;
    private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

    void Start()
    {
        _hasStartPoint = _startPoint != null;
        _startPosition = _hasStartPoint ? _startPoint.position : Vector3.zero;
        _nextSpawnZ = _startPosition.z;

        CreateFirstTile();
        GenerateInitialTiles();

        StartCoroutine(GenerateTilesCoroutine());
    }

    void FixedUpdate()
    {
        if (!_isActive) return;
        _timer += Time.deltaTime;
    }

    private System.Collections.IEnumerator GenerateTilesCoroutine()
    {
        while (_isActive)
        {
            if (_tiles.Count < _maxCount)
            {
                GenerateTile();
            }
            yield return _waitForFixedUpdate;
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
        GameObject newTileObj = prefab == _normalTilePrefab && _tilePool != null
            ? _tilePool.GetTile(position)
            : Instantiate(prefab, position, Quaternion.identity, _tileHolder);

        if (isRotatable && Random.Range(0, 2) == 1)
        {
            newTileObj.transform.Rotate(0f, RotationAngle, 0f);
        }

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
            return;
        }

        _nextSpawnZ += TILE_OFFSET;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Tile tile) || !_tiles.Contains(tile)) return;

        _tiles.Remove(tile);

        if (tile.gameObject.CompareTag("NormalTile") && _tilePool != null)
        {
            _tilePool.ReturnTile(tile.gameObject);
        }
        else
        {
            Destroy(tile.gameObject);
        }
    }
}