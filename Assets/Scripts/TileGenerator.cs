using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private GameObject _normalTilePrefab;
    [SerializeField] private GameObject _specialTilePrefab;
    [SerializeField] private int _maxCount = 5;
    [SerializeField] private Transform _tileHolder;
    [SerializeField] private Transform _startPoint;

    [Header("Настройки объектов")]
    [SerializeField] private GameObject _coin5;
    [SerializeField] private GameObject _coin;
    [SerializeField] private GameObject _bomb;
    [SerializeField] private float _startSpawnBomb = 3;

    [Header("Вероятности")]
    [Range(0, 100)][SerializeField] private float _specialTileChance = 20f;

    private List<Tile> _tiles = new List<Tile>();
    private float _timer;
    private bool _isActive = true;

    void Start()
    {

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
        CreateTile(_normalTilePrefab, startPos); // Первая плитка всегда обычная
    }

    private void GenerateInitialTiles()
    {
        for (int i = _tiles.Count; i < _maxCount; i++)
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

        Vector3 newPos = lastTile.transform.position + Vector3.forward * GetTileLength(lastTile);

        // Случайный выбор типа плитки
        bool shouldCreateSpecial = Random.Range(0f, 100f) <= _specialTileChance;
        GameObject prefabToUse = shouldCreateSpecial ? _specialTilePrefab : _normalTilePrefab;

        CreateTile(prefabToUse, newPos);
    }

    private void CreateTile(GameObject prefab, Vector3 position)
    {
        GameObject newTileObj = Instantiate(prefab, position, Quaternion.identity, _tileHolder);
        Tile newTile = newTileObj.GetComponent<Tile>();

        if (newTile == null)
        {
            Debug.LogError("Отсутствует компонент Tile!", this);
            Destroy(newTileObj);
            return;
        }

        newTile.Initialize(_coin5, _coin, _bomb, _startSpawnBomb, _timer);
        _tiles.Add(newTile);
    }

    private Tile GetLastValidTile()
    {
        if (_tiles.Count == 0) return null;

        Tile lastTile = _tiles[_tiles.Count - 1];
        if (lastTile == null)
        {
            _tiles.RemoveAll(t => t == null);
            return _tiles.Count > 0 ? _tiles[_tiles.Count - 1] : null;
        }
        return lastTile;
    }

    private float GetTileLength(Tile tile)
    {
        // Оптимизация: предполагаем, что все плитки одного размера
        return tile.transform.localScale.z;
        // Альтернатива: можно добавить поле length в Tile
    }

    private void OnTriggerEnter(Collider other)
    {
        Tile tile = other.GetComponent<Tile>();
        if (tile != null && _tiles.Contains(tile))
        {
            _tiles.Remove(tile);
            Destroy(other.gameObject);
        }
    }
}