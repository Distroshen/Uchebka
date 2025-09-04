using System.Collections.Generic;
using UnityEngine;

public class TileObjectPool : MonoBehaviour
{
    [Header("Настройки пула")]
    [SerializeField] private GameObject _tilePrefab; // Префаб тайла
    [SerializeField] private int _initialPoolSize = 15; // Начальный размер пула
    [SerializeField] private Transform _startPoint; // Стартовая позиция

    [Header("Списки объектов")]
    public List<GameObject> activeTiles = new List<GameObject>(); // Активные тайлы
    public List<GameObject> inactiveTiles = new List<GameObject>(); // Неактивные тайлы

    private void Start()
    {
        InitializePool();
    }

    // Инициализация пула объектов
    private void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewTile();
        }
    }

    // Создание нового тайла
    private void CreateNewTile()
    {
        GameObject tile = Instantiate(_tilePrefab, transform);

        // Устанавливаем начальную позицию
        Vector3 position = _startPoint.position;
        position.z += 5.15f;
        tile.transform.position = position;

        // Добавляем обработчик столкновений
        TilePoolHandler handler = tile.GetComponent<TilePoolHandler>();
        if (handler == null)
        {
            handler = tile.AddComponent<TilePoolHandler>();
        }
        handler.Initialize(this);

        // Деактивируем и добавляем в пул неактивных
        tile.SetActive(false);
        inactiveTiles.Add(tile);
    }

    // Получение тайла из пула
    public GameObject GetTile(Vector3 position)
    {
        // Если нет неактивных тайлов, создаем новый
        if (inactiveTiles.Count == 0)
        {
            CreateNewTile();
        }

        // Берем первый неактивный тайл
        GameObject tile = inactiveTiles[0];
        inactiveTiles.RemoveAt(0);

        // Устанавливаем позицию
        tile.transform.position = position;

        // Очищаем сгенерированные объекты перед активацией
        Tile tileComponent = tile.GetComponent<Tile>();
        if (tileComponent != null)
        {
            tileComponent.ClearGeneratedObjects();
        }

        // Активируем и добавляем в список активных
        tile.SetActive(true);
        activeTiles.Add(tile);

        return tile;
    }

    // Возврат тайла в пул
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

        // Очищаем сгенерированные объекты
        Tile tileComponent = tile.GetComponent<Tile>();
        if (tileComponent != null)
        {
            tileComponent.ClearGeneratedObjects();
        }

        // Перемещаем тайл перед самой дальней платформой
        RepositionTile(tile);
    }

    // Перемещение тайла перед самой дальней активной платформой
    private void RepositionTile(GameObject tile)
    {
        float farthestZ = GetFarthestTilePosition();
        Vector3 newPosition = tile.transform.position;
        newPosition.z = farthestZ + 10f; // Добавляем 10 единиц по Z
        tile.transform.position = newPosition;
    }

    // Получение позиции самой дальней активной платформы
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

// Вспомогательный класс для обработки столкновений тайлов
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