using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private Transform parentContainer;
    [SerializeField] private float tileLength = 10f;
    [SerializeField] private Vector3 startPosition = Vector3.zero;

    private Queue<GameObject> tilePool = new Queue<GameObject>();
    private Queue<GameObject> coinPool = new Queue<GameObject>();
    private Queue<GameObject> bombPool = new Queue<GameObject>();
    private List<GameObject> activeTiles = new List<GameObject>();
    private Vector3 nextTilePosition;

    private void Awake()
    {
        Instance = this;
        nextTilePosition = startPosition;
        InitializePools();
    }

    private void Start()
    {
        for (int i = 0; i < 13; i++)
        {
            SpawnTile();
        }
    }

    private void InitializePools()
    {
        // Initialize Tile pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(tilePrefab, parentContainer);
            obj.SetActive(false);
            tilePool.Enqueue(obj);
        }

        // Initialize Coin pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(coinPrefab, parentContainer);
            obj.SetActive(false);
            coinPool.Enqueue(obj);
        }

        // Initialize Bomb pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bombPrefab, parentContainer);
            obj.SetActive(false);
            bombPool.Enqueue(obj);
        }
    }

    public GameObject GetTile()
    {
        if (tilePool.Count > 0)
        {
            GameObject tile = tilePool.Dequeue();
            tile.SetActive(true);
            return tile;
        }
        return Instantiate(tilePrefab, parentContainer);
    }

    public GameObject GetCoin()
    {
        if (coinPool.Count > 0)
        {
            GameObject coin = coinPool.Dequeue();
            coin.SetActive(true);
            return coin;
        }
        return Instantiate(coinPrefab, parentContainer);
    }

    public GameObject GetBomb()
    {
        if (bombPool.Count > 0)
        {
            GameObject bomb = bombPool.Dequeue();
            bomb.SetActive(true);
            return bomb;
        }
        return Instantiate(bombPrefab, parentContainer);
    }

    public void ReturnTile(GameObject tile)
    {
        tile.SetActive(false);
        tilePool.Enqueue(tile);
    }

    public void ReturnCoin(GameObject coin)
    {
        coin.SetActive(false);
        coinPool.Enqueue(coin);
    }

    public void ReturnBomb(GameObject bomb)
    {
        bomb.SetActive(false);
        bombPool.Enqueue(bomb);
    }

    private void SpawnTile()
    {
        GameObject tile = GetTile();
        tile.transform.position = nextTilePosition;
        nextTilePosition.z += tileLength;
        activeTiles.Add(tile);

        // Spawn objects on tile
        TileController tileController = tile.GetComponent<TileController>();
        if (tileController != null)
        {
            tileController.SpawnObjects();
        }
    }

    public void RecycleTile(GameObject tile)
    {
        // Return all children objects (coins and bombs) to pools
        TileController tileController = tile.GetComponent<TileController>();
        if (tileController != null)
        {
            tileController.DeactivateAllObjects();
        }

        ReturnTile(tile);
        activeTiles.Remove(tile);
        SpawnTile();
    }
}