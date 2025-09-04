using System.Collections.Generic;
using UnityEngine;

public class TileObjectPool : MonoBehaviour
{
    [Header("��������� ����")]
    [SerializeField] private GameObject _tilePrefab; // ������ �����
    [SerializeField] private int _initialPoolSize = 15; // ��������� ������ ����
    [SerializeField] private Transform _startPoint; // ��������� �������

    [Header("������ ��������")]
    public List<GameObject> activeTiles = new List<GameObject>(); // �������� �����
    public List<GameObject> inactiveTiles = new List<GameObject>(); // ���������� �����

    private void Start()
    {
        InitializePool();
    }

    // ������������� ���� ��������
    private void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewTile();
        }
    }

    // �������� ������ �����
    private void CreateNewTile()
    {
        GameObject tile = Instantiate(_tilePrefab, transform);

        // ������������� ��������� �������
        Vector3 position = _startPoint.position;
        position.z += 5.15f;
        tile.transform.position = position;

        // ��������� ���������� ������������
        TilePoolHandler handler = tile.GetComponent<TilePoolHandler>();
        if (handler == null)
        {
            handler = tile.AddComponent<TilePoolHandler>();
        }
        handler.Initialize(this);

        // ������������ � ��������� � ��� ����������
        tile.SetActive(false);
        inactiveTiles.Add(tile);
    }

    // ��������� ����� �� ����
    public GameObject GetTile(Vector3 position)
    {
        // ���� ��� ���������� ������, ������� �����
        if (inactiveTiles.Count == 0)
        {
            CreateNewTile();
        }

        // ����� ������ ���������� ����
        GameObject tile = inactiveTiles[0];
        inactiveTiles.RemoveAt(0);

        // ������������� �������
        tile.transform.position = position;

        // ������� ��������������� ������� ����� ����������
        Tile tileComponent = tile.GetComponent<Tile>();
        if (tileComponent != null)
        {
            tileComponent.ClearGeneratedObjects();
        }

        // ���������� � ��������� � ������ ��������
        tile.SetActive(true);
        activeTiles.Add(tile);

        return tile;
    }

    // ������� ����� � ���
    public void ReturnTile(GameObject tile)
    {
        if (activeTiles.Contains(tile))
        {
            activeTiles.Remove(tile);
        }

        tile.SetActive(false);

        if (!inactiveTiles.Contains(tile))
        {
            inactiveTiles.Add(tile);
        }

        // ������� ��������������� �������
        Tile tileComponent = tile.GetComponent<Tile>();
        if (tileComponent != null)
        {
            tileComponent.ClearGeneratedObjects();
        }

        // ���������� ���� ����� ����� ������� ����������
        RepositionTile(tile);
    }

    // ����������� ����� ����� ����� ������� �������� ����������
    private void RepositionTile(GameObject tile)
    {
        float farthestZ = GetFarthestTilePosition();
        Vector3 newPosition = tile.transform.position;
        newPosition.z = farthestZ + 10f; // ��������� 10 ������ �� Z
        tile.transform.position = newPosition;
    }

    // ��������� ������� ����� ������� �������� ���������
    private float GetFarthestTilePosition()
    {
        if (activeTiles.Count == 0)
        {
            return _startPoint.position.z + 5.15f;
        }

        float maxZ = activeTiles[0].transform.position.z;
        foreach (GameObject tile in activeTiles)
        {
            if (tile.transform.position.z > maxZ)
            {
                maxZ = tile.transform.position.z;
            }
        }

        return maxZ;
    }
}

// ��������������� ����� ��� ��������� ������������ ������
public class TilePoolHandler : MonoBehaviour
{
    private TileObjectPool _pool;

    public void Initialize(TileObjectPool pool)
    {
        _pool = pool;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dest"))
        {
            _pool.ReturnTile(gameObject);
        }
    }
}